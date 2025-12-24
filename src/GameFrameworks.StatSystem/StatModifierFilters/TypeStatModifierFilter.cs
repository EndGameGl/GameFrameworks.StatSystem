using System.Numerics;
using GameFrameworks.StatSystem.Core;

namespace GameFrameworks.StatSystem.StatModifierFilters;

internal class TypeStatModifierFilter<TNumber> : IStatModifierFilter<TNumber>
    where TNumber : INumber<TNumber>
{
    private readonly Type _filteredType;

    public TypeStatModifierFilter(Type type)
    {
        _filteredType = type;
    }

    public bool Matches(IStatModifier<TNumber> modifier)
    {
        return modifier.GetType().IsAssignableTo(_filteredType);
    }

    public IStatModifierFilter<TNumber> CreateCopy()
    {
        return new TypeStatModifierFilter<TNumber>(_filteredType);
    }
}
