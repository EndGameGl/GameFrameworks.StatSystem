using System.Numerics;
using GameFrameworks.StatSystem.Core;

namespace GameFrameworks.StatSystem.SimulationRunner;

#if DEBUG
[System.Diagnostics.DebuggerDisplay("[{Stat}] {ValueBefore} -> {ValueAfter}")]
#endif
internal class StatValueDiff<TStatDefinition, TNumber> : IStatValueDiff<TStatDefinition, TNumber>
    where TStatDefinition : IStat
    where TNumber : INumber<TNumber>
{
    public required TStatDefinition Stat { get; init; }

    public required TNumber? ValueBefore { get; init; }

    public required TNumber? ValueAfter { get; init; }
}
