using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using GameFrameworks.StatSystem.Internal;
using GameFrameworks.StatSystem.Core;

namespace GameFrameworks.StatSystem;

public class StatContainer<TStatDefinition, TNumber> : IStatContainer<TStatDefinition, TNumber>
    where TStatDefinition : IStat
    where TNumber : INumber<TNumber>
{
    private readonly StatSystemSimulationRunner<TStatDefinition, TNumber> _simulationRunner;

    private ModifierSourceTracker<TStatDefinition, TNumber> _sourceTracker;

    private readonly Dictionary<
        TStatDefinition,
        List<StatChangeHandler<TStatDefinition, TNumber>>
    > _statChangeSubscriptions;

    private readonly Dictionary<TStatDefinition, IStatValue<TStatDefinition, TNumber>> _stats;

    public required IStatModifierPassProcessor<TNumber> PassProcessor { get; init; }

    public IStatValue<TStatDefinition, TNumber>? this[TStatDefinition stat]
    {
        get
        {
            TryGetStatValue(stat, out var value);
            return value;
        }
    }

    public StatContainer()
    {
        _stats = [];
        _statChangeSubscriptions = [];
        _simulationRunner = new StatSystemSimulationRunner<TStatDefinition, TNumber>(this);
        _sourceTracker = new ModifierSourceTracker<TStatDefinition, TNumber>();
    }

    public void Initialize()
    {
        foreach (var (_, statValue) in _stats)
        {
            statValue.Initialize();
        }
    }

    public void AddStat(TStatDefinition stat, IStatValue<TStatDefinition, TNumber> value)
    {
        value.Stat = stat;
        value.System = this;
        value.OnStatValueChanged += OnStatValueChangedInternal;
        _stats.Add(stat, value);
    }

    private void OnStatValueChangedInternal(
        IStatValue<TStatDefinition, TNumber> statValue,
        TNumber oldValue
    )
    {
        if (!_statChangeSubscriptions.TryGetValue(statValue.Stat, out var handlers))
        {
            return;
        }

        for (int i = handlers.Count - 1; i >= 0; i--)
        {
            handlers[i].Invoke(statValue, oldValue);
        }
    }

    public void RemoveStat(TStatDefinition stat)
    {
        if (_stats.Remove(stat, out var statValue))
        {
            statValue.OnStatValueChanged -= OnStatValueChangedInternal;
        }
    }

    public void AddStatModifier(
        TStatDefinition stat,
        IStatModifier<TNumber> modifier,
        object? source = null
    )
    {
        if (!TryGetStatValue(stat, out var value))
        {
            return;
        }

        if (source is not null)
        {
            _sourceTracker.TrackStatModifierSource(stat, modifier, source);
        }

        value.AddModifier(modifier);
    }

    public void RemoveStatModifier(
        TStatDefinition stat,
        IStatModifier<TNumber> modifier,
        object? source = null
    )
    {
        if (source is not null)
        {
            _sourceTracker.RemoveTrackedModifier(stat, modifier, source);
        }

        if (!TryGetStatValue(stat, out var value))
        {
            return;
        }

        value.RemoveModifier(modifier);
    }

    public void BatchRemoveStatModifiersFromSource(object source)
    {
        _sourceTracker.RemoveTrackingForSource(
            source,
            (stat, modifier) =>
            {
                if (!TryGetStatValue(stat, out var statValue))
                {
                    return;
                }

                statValue.RemoveModifier(modifier);
            }
        );
    }

    public void RemoveAllModifiers(TStatDefinition stat)
    {
        _sourceTracker.RemoveAllModifiersForStat(stat);

        if (!TryGetStatValue(stat, out var value))
        {
            return;
        }

        value.RemoveAllModifiers();
    }

    public void SubscribeToStatChange(
        TStatDefinition stat,
        StatChangeHandler<TStatDefinition, TNumber> handler
    )
    {
        if (_statChangeSubscriptions.TryGetValue(stat, out var handlers))
        {
            handlers.Add(handler);
        }

        handlers = [handler];
        _statChangeSubscriptions[stat] = handlers;
    }

    public void UnsubscribeFromStatChange(
        TStatDefinition stat,
        StatChangeHandler<TStatDefinition, TNumber> handler
    )
    {
        if (!_statChangeSubscriptions.TryGetValue(stat, out var handlers))
        {
            return;
        }

        handlers.Remove(handler);
    }

    public bool TryGetStatValue(
        TStatDefinition stat,
        [NotNullWhen(true)] out IStatValue<TStatDefinition, TNumber>? value
    )
    {
        return _stats.TryGetValue(stat, out value);
    }

    public void ForEachStat(Action<TStatDefinition, IStatValue<TStatDefinition, TNumber>> action)
    {
        foreach (var (stat, statValue) in _stats)
        {
            action.Invoke(stat, statValue);
        }
    }

    public void StartUpdate()
    {
        _simulationRunner.StartUpdate();
    }

    public IStatValueDiff<TStatDefinition, TNumber>[] RunSimulations(
        Action<IStatContainer<TStatDefinition, TNumber>> simulateAction
    )
    {
        return _simulationRunner.SimulateUpdateAction(simulateAction);
    }

    public void ConfirmUpdate()
    {
        _simulationRunner.ConfirmUpdate();
    }

    public void CancelUpdate()
    {
        _simulationRunner.CancelUpdate();
    }

    public IStatContainer<TStatDefinition, TNumber> CreateCopy()
    {
        var copySystem = new StatContainer<TStatDefinition, TNumber>()
        {
            PassProcessor = this.PassProcessor.CreateCopy(),
        };

        foreach (var (stat, statValue) in _stats)
        {
            var statValueCopy = statValue.CreateCopy();

            copySystem.AddStat(stat, statValueCopy);

            foreach (var statValueModifier in statValue.Modifiers)
            {
                _sourceTracker.TryGetSourceForStatModifier(stat, statValueModifier, out var source);
                copySystem.AddStatModifier(stat, statValueModifier.CreateCopy(), source);
            }
        }

        copySystem.Initialize();

        return copySystem;
    }
}
