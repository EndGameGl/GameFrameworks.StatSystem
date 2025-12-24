using System.Numerics;
using GameFrameworks.StatSystem.Core;

namespace GameFrameworks.StatSystem.StatValuePostProcessors;

#if DEBUG
[System.Diagnostics.DebuggerDisplay("{_first} | {_second}")]
#endif
internal readonly struct CombinedPostProcessor<TNumber> : IStatValuePostProcessor<TNumber>
    where TNumber : INumber<TNumber>
{
    private readonly IStatValuePostProcessor<TNumber> _first;
    private readonly IStatValuePostProcessor<TNumber> _second;

    public CombinedPostProcessor(
        IStatValuePostProcessor<TNumber> first,
        IStatValuePostProcessor<TNumber> second
    )
    {
        _first = first;
        _second = second;
    }

    public TNumber ProcessValue(TNumber oldValue, TNumber newValue)
    {
        var firstPass = _first.ProcessValue(oldValue, newValue);
        return _second.ProcessValue(oldValue, firstPass);
    }

    public IStatValuePostProcessor<TNumber> CreateCopy()
    {
        return new CombinedPostProcessor<TNumber>(_first.CreateCopy(), _second.CreateCopy());
    }
}
