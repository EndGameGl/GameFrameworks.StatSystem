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
    public TStatDefinition Stat { get; init; }

    public TNumber? ValueBefore { get; init; }

    public TNumber? ValueAfter { get; init; }
}
