using System.Numerics;

namespace GameFrameworks.StatSystem.Core;

/// <summary>
///     Interface for combining and applying stat modifiers to stat values
/// </summary>
/// <typeparam name="TNumber">Numeric value type of current stat container</typeparam>
public interface IStatModifierPassProcessor<TNumber>
    : ICreateCopy<IStatModifierPassProcessor<TNumber>>
    where TNumber : INumber<TNumber>
{
    /// <summary>
    ///     Filter that helps narrow down what stats are used in this pass
    /// </summary>
    IStatModifierFilter<TNumber> Filter { get; }

    /// <summary>
    ///     Perform some arbitrary code to apply passed in modifiers to initial value
    /// </summary>
    /// <param name="initialValue">Initial stat value before this pass</param>
    /// <param name="modifiers">Set of modifiers that could be used in this pass</param>
    /// <returns>New value of the stat after calculations</returns>
    TNumber ProcessPass(TNumber initialValue, IStatModifier<TNumber>[] modifiers);
}
