using System.Numerics;
using GameFrameworks.StatSystem.Core;

namespace GameFrameworks.StatSystem.PassProcessors;

public class PassProcessorCollection<TNumber> : IStatModifierPassProcessor<TNumber>
    where TNumber : INumber<TNumber>
{
    private readonly IStatModifierPassProcessor<TNumber>[] _passProcessors;

    public IStatModifierFilter<TNumber> Filter { get; private set; }

    public PassProcessorCollection(IStatModifierPassProcessor<TNumber>[] passProcessors)
    {
        _passProcessors = passProcessors;

        if (passProcessors.Length == 0)
        {
            return;
        }

        if (_passProcessors.Length == 1)
        {
            Filter = _passProcessors[0].Filter;
        }

        var filter = _passProcessors[0].Filter;

        for (int i = 1; i < passProcessors.Length; i++)
        {
            var processor = _passProcessors[i];
            filter = filter.With(processor.Filter);
        }

        Filter = filter;
    }

    public TNumber ProcessPass(TNumber initialValue, IStatModifier<TNumber>[] modifiers)
    {
        var value = initialValue;

        for (int i = 0; i < _passProcessors.Length; i++)
        {
            var processor = _passProcessors[i];

            value = processor.ProcessPass(value, [.. modifiers.Where(processor.Filter.Matches)]);
        }

        return value;
    }

    public IStatModifierPassProcessor<TNumber> CreateCopy()
    {
        var copies = new IStatModifierPassProcessor<TNumber>[_passProcessors.Length];

        for (int i = 0; i < _passProcessors.Length; i++)
        {
            copies[i] = _passProcessors[i].CreateCopy();
        }

        return new PassProcessorCollection<TNumber>(copies);
    }
}
