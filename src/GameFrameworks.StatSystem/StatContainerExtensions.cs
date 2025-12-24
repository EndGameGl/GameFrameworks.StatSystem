using System.Linq.Expressions;
using System.Numerics;
using GameFrameworks.StatSystem.Core;
using GameFrameworks.StatSystem.StatValues;

namespace GameFrameworks.StatSystem;

public static class StatContainerExtensions
{
    public static void AddPrimaryStat<TStatDefinition, TNumber>(
        this IStatContainer<TStatDefinition, TNumber> system,
        TStatDefinition stat,
        TNumber baseValue,
        IStatValuePostProcessor<TNumber>? postProcessor = null
    )
        where TStatDefinition : IStat
        where TNumber : INumber<TNumber>
    {
        system.AddStat(stat, new StatValue<TStatDefinition, TNumber>(baseValue, postProcessor));
    }

    public static void AddDerivedStat<TStatDefinition, TNumber>(
        this IStatContainer<TStatDefinition, TNumber> system,
        TStatDefinition stat,
        Expression<Func<IStatContainer<TStatDefinition, TNumber>, TNumber>> valueFactory,
        IStatValuePostProcessor<TNumber>? postProcessor = null
    )
        where TStatDefinition : IStat
        where TNumber : INumber<TNumber>
    {
        system.AddStat(
            stat,
            new CalculatedStatValue<TStatDefinition, TNumber>(valueFactory, postProcessor)
        );
    }
}
