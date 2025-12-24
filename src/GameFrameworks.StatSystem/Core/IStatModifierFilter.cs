using System.Numerics;

namespace GameFrameworks.StatSystem.Core;

/// <summary>
///     Interface for filtering <see cref="IStatModifier{TNumber}"/> when all modifiers are passed to <see cref="IStatModifierPassProcessor{TNumber}"/>
/// </summary>
/// <typeparam name="TNumber"></typeparam>
public interface IStatModifierFilter<TNumber> : ICreateCopy<IStatModifierFilter<TNumber>>
    where TNumber : INumber<TNumber>
{
    /// <summary>
    ///     Whether this modifier will be applied in current pass
    /// </summary>
    /// <param name="modifier">Stat modifier instance</param>
    /// <returns><see langword="true"/> if modifier would be used, <see langword="false"/> otherwise</returns>
    public bool Matches(IStatModifier<TNumber> modifier);
}
