using System.Numerics;
using GameFrameworks.StatSystem;
using GameFrameworks.StatSystem.Core;
using GameFrameworks.StatSystem.StatValuePostProcessors;

namespace GameFrameworks.StatSystem;

public static class PostProcessorChain
{
    public static IStatValuePostProcessor<TNumber> Then<TNumber>(
        this IStatValuePostProcessor<TNumber> current,
        IStatValuePostProcessor<TNumber> next
    )
        where TNumber : INumber<TNumber>
    {
        return new CombinedPostProcessor<TNumber>(current, next);
    }

    public static IStatValuePostProcessor<TNumber> WithMinValue<TNumber>(TNumber value)
        where TNumber : INumber<TNumber>
    {
        return new MinValuePostProcessor<TNumber>(value);
    }

    public static IStatValuePostProcessor<TNumber> WithMinValue<TNumber>(
        this IStatValuePostProcessor<TNumber> current,
        TNumber value
    )
        where TNumber : INumber<TNumber>
    {
        return current.Then(WithMinValue(value));
    }

    public static IStatValuePostProcessor<TNumber> WithMaxValue<TNumber>(TNumber value)
        where TNumber : INumber<TNumber>
    {
        return new MaxValuePostProcessor<TNumber>(value);
    }

    public static IStatValuePostProcessor<TNumber> WithMaxValue<TNumber>(
        this IStatValuePostProcessor<TNumber> current,
        TNumber value
    )
        where TNumber : INumber<TNumber>
    {
        return current.Then(WithMaxValue(value));
    }
}
