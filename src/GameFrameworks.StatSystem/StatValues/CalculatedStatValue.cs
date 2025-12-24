using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;
using GameFrameworks.StatSystem.Core;

namespace GameFrameworks.StatSystem.StatValues;

#if DEBUG
[System.Diagnostics.DebuggerDisplay(
    "Value: {Value} | Base: {BaseValue} | {Modifiers.Count} modifiers | Formula: {ValueFactory}"
)]
#endif
public class CalculatedStatValue<TStatDefinition, TNumber> : IStatValue<TStatDefinition, TNumber>
    where TStatDefinition : IStat
    where TNumber : INumber<TNumber>
{
    private Func<IStatContainer<TStatDefinition, TNumber>, TNumber> _compiledDelegate;
    private TNumber _baseValue;
    private TNumber _value;
    private bool _isDirty;

    public event StatValueChangedEventHandler<TStatDefinition, TNumber>? OnStatValueChanged;

    public Expression<Func<IStatContainer<TStatDefinition, TNumber>, TNumber>> ValueFactory { get; }
    public IStatContainer<TStatDefinition, TNumber> System { get; set; }
    public TStatDefinition Stat { get; set; }

    public TNumber Value => GetValueInternal();
    public TNumber BaseValue => _baseValue;

    public IStatValuePostProcessor<TNumber>? PostProcessor { get; }

    public IStatModifierPassProcessor<TNumber>? PassProcessor { get; init; }
    public List<IStatModifier<TNumber>> Modifiers { get; }

    public CalculatedStatValue(
        Expression<Func<IStatContainer<TStatDefinition, TNumber>, TNumber>> valueFactory,
        IStatValuePostProcessor<TNumber>? postProcessor = null
    )
    {
        Modifiers = [];
        ValueFactory = valueFactory;
        PostProcessor = postProcessor;
    }

    public void Initialize()
    {
        var extractor = new ExpressionStatExtractor();
        extractor.Visit(ValueFactory);
        foreach (var dependantStat in extractor.ExtractedStatDependencies)
        {
            System.SubscribeToStatChange(dependantStat, OnValueChanged);
        }
        _compiledDelegate = ValueFactory.Compile();
        _baseValue = _compiledDelegate(System);
        _value = _baseValue;
    }

    private void OnValueChanged(IStatValue<TStatDefinition, TNumber> value, TNumber oldValue)
    {
        var old = _value;
        _isDirty = true;

        OnStatValueChanged?.Invoke(this, old);
    }

    public void UpdateBaseValue(TNumber nextValue)
    {
        throw new ArgumentException("Can't set value of calculated stats");
    }

    private TNumber GetValueInternal()
    {
        if (_isDirty)
        {
            _baseValue = _compiledDelegate(System);

            var newValue = _baseValue;

            var passProcessor = PassProcessor ?? System.PassProcessor;

            if (passProcessor is not null)
            {
                newValue = passProcessor.ProcessPass(newValue, [.. Modifiers]);
            }

            if (PostProcessor is not null)
            {
                newValue = PostProcessor.ProcessValue(_baseValue, newValue);
            }

            _value = newValue;
            _isDirty = false;
        }

        return _value;
    }

    public IStatValue<TStatDefinition, TNumber> CreateCopy()
    {
        return new CalculatedStatValue<TStatDefinition, TNumber>(
            ValueFactory,
            PostProcessor?.CreateCopy()
        )
        {
            PassProcessor = PassProcessor?.CreateCopy(),
        };
    }

    public void AddModifier(IStatModifier<TNumber> modifier)
    {
        var oldValue = _value;
        Modifiers.Add(modifier);
        _isDirty = true;
        OnStatValueChanged?.Invoke(this, oldValue);
    }

    public void RemoveModifier(IStatModifier<TNumber> modifier)
    {
        var oldValue = _value;
        Modifiers.Remove(modifier);
        _isDirty = true;
        OnStatValueChanged?.Invoke(this, oldValue);
    }

    public void RemoveAllModifiers()
    {
        var oldValue = _value;
        Modifiers.Clear();
        _isDirty = true;
        OnStatValueChanged?.Invoke(this, oldValue);
    }

    private class ExpressionStatExtractor : ExpressionVisitor
    {
        public List<TStatDefinition> ExtractedStatDependencies { get; } = [];

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (!node.Type.IsGenericType)
            {
                return base.VisitMethodCall(node);
            }

            if (node.Object is null)
            {
                return base.VisitMethodCall(node);
            }

            if (
                node.Object.Type.GetGenericTypeDefinition() == typeof(IStatContainer<,>)
                && node.Method.Name == "get_Item"
            )
            {
                var statExpression = node.Arguments[0];
                var value = GetExpressionValue(statExpression);

                if (value is TStatDefinition statValue)
                {
                    ExtractedStatDependencies.Add(statValue);
                }
            }

            return base.VisitMethodCall(node);
        }

        private static object? GetExpressionValue(ConstantExpression expression)
        {
            return expression.Value;
        }

        private static object? GetExpressionValue(MemberExpression memberExpression)
        {
            var source = GetExpressionValue(memberExpression.Expression);

            if (memberExpression.Member is PropertyInfo propertyInfo)
            {
                return propertyInfo.GetValue(source);
            }

            if (memberExpression.Member is FieldInfo fieldInfo)
            {
                return fieldInfo.GetValue(source);
            }

            return null;
        }

        private static object? GetExpressionValue(MethodCallExpression methodCallExpression)
        {
            return methodCallExpression.Method.Invoke(
                GetExpressionValue(methodCallExpression.Object),
                [.. methodCallExpression.Arguments.Select(x => GetExpressionValue(x))]
            );
        }

        private static object? GetExpressionValue(InvocationExpression invocationExpression)
        {
            if (GetExpressionValue(invocationExpression.Expression) is Delegate delegateValue)
            {
                return delegateValue.Method.Invoke(
                    delegateValue.Target,
                    [.. invocationExpression.Arguments.Select(x => GetExpressionValue(x))]
                );
            }

            return null;
        }

        private static object? GetExpressionValue(Expression? expr)
        {
            return expr switch
            {
                null => null,
                ConstantExpression constantExpression => GetExpressionValue(constantExpression),
                MemberExpression memberExpression => GetExpressionValue(memberExpression),
                MethodCallExpression methodCallExpression => GetExpressionValue(
                    methodCallExpression
                ),
                InvocationExpression invocationExpression => GetExpressionValue(
                    invocationExpression
                ),
                _ => null,
            };
        }
    }
}
