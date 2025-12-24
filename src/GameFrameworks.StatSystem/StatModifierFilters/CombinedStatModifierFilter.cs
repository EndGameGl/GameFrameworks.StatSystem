using System.Numerics;
using GameFrameworks.StatSystem.Core;

namespace GameFrameworks.StatSystem.StatModifierFilters;

internal readonly struct CombinedStatModifierFilter<TNumber> : IStatModifierFilter<TNumber>
    where TNumber : INumber<TNumber>
{
    private readonly IStatModifierFilter<TNumber> _first;
    private readonly IStatModifierFilter<TNumber> _second;

    public CombinedStatModifierFilter(
        IStatModifierFilter<TNumber> first,
        IStatModifierFilter<TNumber> second
    )
    {
        _first = first;
        _second = second;
    }

    public bool Matches(IStatModifier<TNumber> modifier)
    {
        return _first.Matches(modifier) && _second.Matches(modifier);
    }

    public IStatModifierFilter<TNumber> CreateCopy()
    {
        return new CombinedStatModifierFilter<TNumber>(_first.CreateCopy(), _second.CreateCopy());
    }
}
