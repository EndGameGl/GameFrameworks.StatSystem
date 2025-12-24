using System.Numerics;
using GameFrameworks.StatSystem.Core;

namespace GameFrameworks.StatSystem.StatModifierFilters;

internal class DelegateStatModifierFilter<TNumber> : IStatModifierFilter<TNumber>
    where TNumber : INumber<TNumber>
{
    private readonly Func<IStatModifier<TNumber>, bool> _condition;

    public DelegateStatModifierFilter(Func<IStatModifier<TNumber>, bool> condition)
    {
        _condition = condition;
    }

    public bool Matches(IStatModifier<TNumber> modifier)
    {
        return _condition.Invoke(modifier);
    }

    public IStatModifierFilter<TNumber> CreateCopy()
    {
        return new DelegateStatModifierFilter<TNumber>(_condition);
    }
}
