using System.Numerics;
using GameFrameworks.StatSystem.Core;

namespace GameFrameworks.StatSystem.StatValuePostProcessors;

#if DEBUG
[System.Diagnostics.DebuggerDisplay("Min value: {_minValue}")]
#endif
public class MinValuePostProcessor<TNumber> : IStatValuePostProcessor<TNumber>
    where TNumber : INumber<TNumber>
{
    private readonly TNumber _minValue;

    public MinValuePostProcessor(TNumber value)
    {
        _minValue = value;
    }

    public TNumber ProcessValue(TNumber oldValue, TNumber newValue)
    {
        return TNumber.Max(oldValue, newValue);
    }

    public IStatValuePostProcessor<TNumber> CreateCopy()
    {
        return new MinValuePostProcessor<TNumber>(_minValue);
    }
}
