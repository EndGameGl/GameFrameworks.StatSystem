using System.Numerics;
using GameFrameworks.StatSystem;
using GameFrameworks.StatSystem.Core;
using GameFrameworks.StatSystem.StatModifierFilters;

namespace GameFrameworks.StatSystem
{
    public static class StatModifierFilterChain
    {
        public static IStatModifierFilter<TNumber> With<TNumber>(
            this IStatModifierFilter<TNumber> current,
            IStatModifierFilter<TNumber> next
        )
            where TNumber : INumber<TNumber>
        {
            return new CombinedStatModifierFilter<TNumber>(current, next);
        }

        public static IStatModifierFilter<TNumber> WithType<TNumber, TType>(
            this IStatModifierFilter<TNumber> current
        )
            where TNumber : INumber<TNumber>
            where TType : IStatModifier<TNumber>
        {
            return current.With(new TypeStatModifierFilter<TNumber>(typeof(TType)));
        }

        public static IStatModifierFilter<TNumber> WithType<TNumber, TType>()
            where TNumber : INumber<TNumber>
            where TType : IStatModifier<TNumber>
        {
            return new TypeStatModifierFilter<TNumber>(typeof(TType));
        }

        public static IStatModifierFilter<TNumber> WithCondition<TNumber>(
            this IStatModifierFilter<TNumber> current,
            Func<IStatModifier<TNumber>, bool> condition
        )
            where TNumber : INumber<TNumber>
        {
            return current.With(new DelegateStatModifierFilter<TNumber>(condition));
        }

        public static IStatModifierFilter<TNumber> WithCondition<TNumber>(
            Func<IStatModifier<TNumber>, bool> condition
        )
            where TNumber : INumber<TNumber>
        {
            return new DelegateStatModifierFilter<TNumber>(condition);
        }
    }
}
