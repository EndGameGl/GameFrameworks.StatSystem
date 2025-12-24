using System.Numerics;

namespace GameFrameworks.StatSystem.Core;

public delegate void StatValueChangedEventHandler<TStatDefinition, TNumber>(
    IStatValue<TStatDefinition, TNumber> statValue,
    TNumber oldValue
)
    where TStatDefinition : IStat
    where TNumber : INumber<TNumber>;

/// <summary>
///     Stat value handler, used to store and update an actual value of stats
/// </summary>
/// <typeparam name="TStatDefinition"></typeparam>
/// <typeparam name="TNumber"></typeparam>
public interface IStatValue<TStatDefinition, TNumber>
    : ICreateCopy<IStatValue<TStatDefinition, TNumber>>
    where TStatDefinition : IStat
    where TNumber : INumber<TNumber>
{
    /// <summary>
    ///     Fired when stat value was updated in any way, doesn't mean it actually changed, since actual stat value isn't checked after update
    /// </summary>
    event StatValueChangedEventHandler<TStatDefinition, TNumber>? OnStatValueChanged;

    /// <summary>
    ///     Reference to parent stat container
    /// </summary>
    IStatContainer<TStatDefinition, TNumber> System { get; set; }

    /// <summary>
    ///     Reference to parent stat instance
    /// </summary>
    TStatDefinition Stat { get; set; }

    /// <summary>
    ///     Current applied modifiers of this stat
    /// </summary>
    List<IStatModifier<TNumber>> Modifiers { get; }

    /// <summary>
    ///     Current stat value with all modifiers and postprocessors applied
    /// </summary>
    TNumber Value { get; }

    /// <summary>
    ///     Base stat value before all calculations applied
    /// </summary>
    TNumber BaseValue { get; }

    /// <summary>
    ///     Stat value postprocessor
    /// </summary>
    IStatValuePostProcessor<TNumber>? PostProcessor { get; }

    /// <summary>
    ///     Pass processor that can be used in this stat value calculatuins separately
    /// </summary>
    IStatModifierPassProcessor<TNumber>? PassProcessor { get; }

    /// <summary>
    ///     Updates <see cref="BaseValue"/> of this stat
    /// </summary>
    /// <param name="nextValue"></param>
    void UpdateBaseValue(TNumber nextValue);

    /// <summary>
    ///     Adds new modifier, mostly for internal use
    /// <para />
    ///     Prefer using <see cref="IStatContainer{TStatDefinition, TNumber}.AddStatModifier(TStatDefinition, IStatModifier{TNumber}, object?)"/>
    /// </summary>
    /// <param name="modifier">Modifier instance</param>
    void AddModifier(IStatModifier<TNumber> modifier);

    /// <summary>
    ///     Removes modifier, mostly for internal use
    /// <para />
    ///     Prefer using <see cref="IStatContainer{TStatDefinition, TNumber}.RemoveStatModifier(TStatDefinition, IStatModifier{TNumber}, object?)"/>
    /// </summary>
    /// <param name="modifier">Modifier instance</param>
    void RemoveModifier(IStatModifier<TNumber> modifier);

    /// <summary>
    ///     Removes all modifiers from current stat, mostly for internal use
    /// <para />
    ///     Prefer using <see cref="IStatContainer{TStatDefinition, TNumber}.RemoveAllModifiers(TStatDefinition)"/>
    /// </summary>
    void RemoveAllModifiers();

    /// <summary>
    ///     Runs when <see cref="IStatContainer{TStatDefinition, TNumber}.Initialize"/> is called
    /// </summary>
    void Initialize();
}
