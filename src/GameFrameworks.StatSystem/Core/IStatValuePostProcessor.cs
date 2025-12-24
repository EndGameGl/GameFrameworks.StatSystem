using System.Numerics;

namespace GameFrameworks.StatSystem.Core;

/// <summary>
///     Stat postprocessor that runs after any stat updates
/// <para />
///     Can be used to limit stat values in certain bounds, or round value
/// </summary>
/// <typeparam name="TNumber"></typeparam>
public interface IStatValuePostProcessor<TNumber> : ICreateCopy<IStatValuePostProcessor<TNumber>>
    where TNumber : INumber<TNumber>
{
    /// <summary>
    ///     Perform an operation with the stat value
    /// </summary>
    /// <param name="oldValue">Value before stat change</param>
    /// <param name="newValue">Current value after stat change</param>
    /// <returns>New value after processing</returns>
    TNumber ProcessValue(TNumber oldValue, TNumber newValue);
}
