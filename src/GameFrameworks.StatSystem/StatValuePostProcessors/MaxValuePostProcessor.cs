using System.Numerics;
using GameFrameworks.StatSystem.Core;

namespace GameFrameworks.StatSystem.StatValuePostProcessors;

#if DEBUG
[System.Diagnostics.DebuggerDisplay("Max value: {_maxValue}")]
#endif
public class MaxValuePostProcessor<TNumber> : IStatValuePostProcessor<TNumber>
    where TNumber : INumber<TNumber>
{
    private readonly TNumber _maxValue;

    public MaxValuePostProcessor(TNumber maxValue)
    {
        _maxValue = maxValue;
    }

    public TNumber ProcessValue(TNumber oldValue, TNumber newValue)
    {
        return TNumber.Min(newValue, _maxValue);
    }

    public IStatValuePostProcessor<TNumber> CreateCopy()
    {
        return new MaxValuePostProcessor<TNumber>(_maxValue);
    }
}
