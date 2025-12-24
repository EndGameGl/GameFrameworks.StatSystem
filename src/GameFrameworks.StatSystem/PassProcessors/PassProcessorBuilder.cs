using System.Numerics;
using GameFrameworks.StatSystem.Core;

namespace GameFrameworks.StatSystem.PassProcessors;

public class PassProcessorBuilder<TNumber>
    where TNumber : INumber<TNumber>
{
    private readonly List<IStatModifierPassProcessor<TNumber>> _passes = [];

    public PassProcessorBuilder<TNumber> AddPass(IStatModifierPassProcessor<TNumber> passProcessor)
    {
        _passes.Add(passProcessor);
        return this;
    }

    public IStatModifierPassProcessor<TNumber> Build()
    {
        return new PassProcessorCollection<TNumber>([.. _passes]);
    }

    public static PassProcessorBuilder<TNumber> Create() => new();
}
