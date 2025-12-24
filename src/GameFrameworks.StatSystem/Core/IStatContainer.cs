using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace GameFrameworks.StatSystem.Core;

/// <summary>
///     Delegate for handling <see cref="IStatContainer{TStatDefinition, TNumber}.SubscribeToStatChange(TStatDefinition, GameFrameworks.StatSystem.Core.StatChangeHandler{TStatDefinition, TNumber})"/> events
/// </summary>
/// <typeparam name="TStatDefinition"><see cref="IStat"/> implementation used with <see cref="IStatContainer{TStatDefinition, TNumber}"/></typeparam>
/// <typeparam name="TNumber">Data type used for calculations with current <see cref="IStatContainer{TStatDefinition, TNumber}"/></typeparam>
/// <param name="statValue"><see cref="IStatValueDiff{TStatDefinition, TNumber}"/> that was changed</param>
/// <param name="oldValue">Old value of stat before change</param>
public delegate void StatChangeHandler<TStatDefinition, TNumber>(
    IStatValue<TStatDefinition, TNumber> statValue,
    TNumber oldValue
)
    where TStatDefinition : IStat
    where TNumber : INumber<TNumber>;

/// <summary>
///     Interface for a container to handle stat storage, modification and updates
/// </summary>
/// <typeparam name="TStatDefinition">A type that implements <see cref="IStat"/> interface</typeparam>
/// <typeparam name="TNumber">Numeric type that this system could operate with</typeparam>
public interface IStatContainer<TStatDefinition, TNumber>
    : ICreateCopy<IStatContainer<TStatDefinition, TNumber>>
    where TStatDefinition : IStat
    where TNumber : INumber<TNumber>
{
    /// <summary>
    ///     Pass processor that will be used to determine order and application of all <see cref="IStatModifier{TNumber}"/> applied to stats in this container
    /// </summary>
    IStatModifierPassProcessor<TNumber> PassProcessor { get; }

    /// <summary>
    ///     Attempts to get <see cref="IStatValue{TStatDefinition, TNumber}"/> based on passed <see cref="IStat"/> parameter
    /// </summary>
    /// <param name="stat">Stat to get value of</param>
    /// <param name="value"></param>
    /// <returns><see langword="true"/>, if stat value was found, <see langword="false"/> otherwise</returns>
    bool TryGetStatValue(
        TStatDefinition stat,
        [NotNullWhen(true)] out IStatValue<TStatDefinition, TNumber>? value
    );

    /// <summary>
    ///     Attempts to get <see cref="IStatValue{TStatDefinition, TNumber}"/> based on passed <see cref="IStat"/> parameter
    /// </summary>
    /// <param name="stat">Stat to get value of</param>
    /// <returns><see langword="null"/> if nothing was found, <see cref="IStatValue{TStatDefinition, TNumber}"/> otherwise</returns>
    IStatValue<TStatDefinition, TNumber>? this[TStatDefinition stat] { get; }

    /// <summary>
    ///     Initial initialization to perform operations and hook up logic after everything was added
    /// </summary>
    void Initialize();

    /// <summary>
    ///     Adds new stat to container
    /// </summary>
    /// <param name="stat">Stat to add</param>
    /// <param name="value">Stat value handler</param>
    void AddStat(TStatDefinition stat, IStatValue<TStatDefinition, TNumber> value);

    /// <summary>
    ///     Removes specified stat from container
    /// </summary>
    /// <param name="stat">Stat to be removed</param>
    void RemoveStat(TStatDefinition stat);

    /// <summary>
    ///     Adds new modifier to specified stat
    /// </summary>
    /// <param name="stat">Stat reference</param>
    /// <param name="modifier">Stat modifier instance</param>
    /// <param name="source">Stat modifier source, can be used to link multiple modifiers to single source</param>
    void AddStatModifier(
        TStatDefinition stat,
        IStatModifier<TNumber> modifier,
        object? source = null
    );

    /// <summary>
    ///     Removes stat modifier from specified source
    /// </summary>
    /// <param name="stat">Stat reference</param>
    /// <param name="modifier">Stat modifier instance</param>
    /// <param name="source">Stat modifier source, can be used to link multiple modifiers to single source</param>
    void RemoveStatModifier(
        TStatDefinition stat,
        IStatModifier<TNumber> modifier,
        object? source = null
    );

    /// <summary>
    ///     Removes all stat modifiers linked to specific source
    /// </summary>
    /// <param name="source">Source of stat modifiers</param>
    void BatchRemoveStatModifiersFromSource(object source);

    /// <summary>
    ///     Removes all stat modifiers from a specific stat
    /// </summary>
    /// <param name="stat"></param>
    void RemoveAllModifiers(TStatDefinition stat);

    /// <summary>
    ///     Adds a new handler to listen for specific stat changes
    /// </summary>
    /// <param name="stat"><see cref="IStat"/> to listen changes for</param>
    /// <param name="handler">Handler method for stat value change</param>
    void SubscribeToStatChange(
        TStatDefinition stat,
        StatChangeHandler<TStatDefinition, TNumber> handler
    );

    /// <summary>
    ///     Removes specific handler that listens for specific stat changes
    /// </summary>
    /// <param name="stat"><see cref="IStat"/> those changes were listened</param>
    /// <param name="handler">Handler method for stat value change</param>
    void UnsubscribeFromStatChange(
        TStatDefinition stat,
        StatChangeHandler<TStatDefinition, TNumber> handler
    );

    /// <summary>
    ///     Iterate internal stat storage
    /// </summary>
    /// <param name="action"></param>
    void ForEachStat(Action<TStatDefinition, IStatValue<TStatDefinition, TNumber>> action);

    #region change tracking and simulations

    /// <summary>
    ///     Enter simulation mode for this specific container, creates a new copy of current <see cref="IStatContainer{TStatDefinition, TNumber}"/> to perform operations on
    /// <para />
    ///     Useful to test what will change if certain stat changes are made
    /// </summary>
    public void StartUpdate();

    /// <summary>
    ///     Perform arbitrary simulations and return an array of all stat value changes
    /// <para />
    ///     Multiple operations will be stored in a list and applied sequentially when confirmed
    /// </summary>
    /// <param name="simulateAction">Action to perform</param>
    /// <returns></returns>
    public IStatValueDiff<TStatDefinition, TNumber>[] RunSimulations(
        Action<IStatContainer<TStatDefinition, TNumber>> simulateAction
    );

    /// <summary>
    ///     Confirm application of all performed simulations to current <see cref="IStatContainer{TStatDefinition, TNumber}"/> and exits simulation mode
    /// </summary>
    public void ConfirmUpdate();

    /// <summary>
    ///     Discards all operations made to <see cref="IStatContainer{TStatDefinition, TNumber}"/> and exists simulation mode
    /// </summary>
    public void CancelUpdate();
    #endregion
}
