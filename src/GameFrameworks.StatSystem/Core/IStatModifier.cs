using System.Numerics;

namespace GameFrameworks.StatSystem.Core;

/// <summary>
///     Modifier that can be applied to a stat using <see cref="IStatContainer{TStatDefinition, TNumber}.AddStatModifier(TStatDefinition, GameFrameworks.StatSystem.Core.IStatModifier{TNumber}, object?)"/> method
/// </summary>
/// <typeparam name="TNumber">Numeric type used in stat container</typeparam>
public interface IStatModifier<TNumber> : ICreateCopy<IStatModifier<TNumber>>
    where TNumber : INumber<TNumber>
{
    /// <summary>
    ///     Gets modifier value
    /// <para/>
    ///     Can be used inside of <see cref="IStatModifierPassProcessor{TNumber}.ProcessPass(TNumber, IStatModifier{TNumber}[])"/> to calculate updated stat value
    /// </summary>
    /// <returns></returns>
    TNumber GetModifier();
}
