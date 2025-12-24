using System.Numerics;
using GameFrameworks.StatSystem.Core;

namespace GameFrameworks.StatSystem.StatValues;

#if DEBUG
[System.Diagnostics.DebuggerDisplay(
    "Value: {Value} | Base: {BaseValue} | {Modifiers.Count} modifiers"
)]
#endif
public class StatValue<TStatDefinition, TNumber> : IStatValue<TStatDefinition, TNumber>
    where TStatDefinition : IStat
    where TNumber : INumber<TNumber>
{
    private bool _isDirty = true;
    private TNumber _value;

    public event StatValueChangedEventHandler<TStatDefinition, TNumber>? OnStatValueChanged;

    public TNumber Value => GetValueInternal();
    public TNumber BaseValue { get; private set; }

    public IStatContainer<TStatDefinition, TNumber> System { get; set; }

    public TStatDefinition Stat { get; set; }

    public IStatValuePostProcessor<TNumber>? PostProcessor { get; }
    public IStatModifierPassProcessor<TNumber>? PassProcessor { get; init; }
    public List<IStatModifier<TNumber>> Modifiers { get; } = [];

    public StatValue(TNumber baseValue, IStatValuePostProcessor<TNumber>? postProcessor = null)
    {
        PostProcessor = postProcessor;
        BaseValue = baseValue;
    }

    public void UpdateBaseValue(TNumber nextValue)
    {
        var oldValue = _value;
        BaseValue = nextValue;
        _isDirty = true;
        OnStatValueChanged?.Invoke(this, oldValue);
    }

    public void Initialize()
    {
        return;
    }

    private TNumber GetValueInternal()
    {
        if (_isDirty)
        {
            var newValue = BaseValue;

            var passProcessor = PassProcessor ?? System.PassProcessor;

            if (passProcessor is not null)
            {
                newValue = passProcessor.ProcessPass(newValue, [.. Modifiers]);
            }

            if (PostProcessor is not null)
            {
                newValue = PostProcessor.ProcessValue(BaseValue, newValue);
            }

            _value = newValue;
            _isDirty = false;
        }

        return _value;
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

    public IStatValue<TStatDefinition, TNumber> CreateCopy()
    {
        return new StatValue<TStatDefinition, TNumber>(BaseValue, PostProcessor?.CreateCopy())
        {
            PassProcessor = PassProcessor?.CreateCopy(),
        };
    }
}
