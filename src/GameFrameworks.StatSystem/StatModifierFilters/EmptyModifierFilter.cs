using System.Numerics;
using GameFrameworks.StatSystem.Core;

namespace GameFrameworks.StatSystem.StatModifierFilters;

internal class EmptyModifierFilter<TNumber> : IStatModifierFilter<TNumber>
    where TNumber : INumber<TNumber>
{
    public static EmptyModifierFilter<TNumber> Instance { get; } =
        new EmptyModifierFilter<TNumber>();

    public IStatModifierFilter<TNumber> CreateCopy()
    {
        return new EmptyModifierFilter<TNumber>();
    }

    public bool Matches(IStatModifier<TNumber> modifier)
    {
        return false;
    }
}
