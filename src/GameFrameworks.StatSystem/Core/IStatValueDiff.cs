using System.Numerics;

namespace GameFrameworks.StatSystem.Core;

/// <summary>
///     An object representing difference in stat after an operation simulation
/// </summary>
/// <typeparam name="TStatDefinition"></typeparam>
/// <typeparam name="TNumber"></typeparam>
public interface IStatValueDiff<TStatDefinition, TNumber>
    where TStatDefinition : IStat
    where TNumber : INumber<TNumber>
{
    /// <summary>
    ///     Stat reference
    /// </summary>
    TStatDefinition Stat { get; }

    /// <summary>
    ///     Value before simulation, can be <see langword="null"/>
    /// <para />
    ///     <see langword="null"/> value means that stat wasn't present before simulation
    /// </summary>
    TNumber? ValueBefore { get; }

    /// <summary>
    ///     Value after simulation, can be <see langword="null"/>
    /// <para />
    ///     <see langword="null"/> value means that stat wasn't present after simulation
    /// </summary>
    TNumber? ValueAfter { get; }
}
