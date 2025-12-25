using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using GameFrameworks.StatSystem.Core;
using GameFrameworks.StatSystem.SimulationRunner;

namespace GameFrameworks.StatSystem;

internal class StatSystemSimulationRunner<TStatDefinition, TNumber>
    where TStatDefinition : IStat
    where TNumber : INumber<TNumber>
{
    private bool _isSimulating = false;

    private readonly IStatContainer<TStatDefinition, TNumber> _sourceStatSystem;

    private IStatContainer<TStatDefinition, TNumber>? _testCopy;

    private readonly List<Action<IStatContainer<TStatDefinition, TNumber>>> _performedActions;

    public StatSystemSimulationRunner(IStatContainer<TStatDefinition, TNumber> statSystem)
    {
        _sourceStatSystem = statSystem;
        _performedActions = [];
    }

    internal void StartUpdate()
    {
        _isSimulating = true;
        _testCopy = _sourceStatSystem.CreateCopy();
    }

    internal IStatValueDiff<TStatDefinition, TNumber>[] SimulateUpdateAction(
        Action<IStatContainer<TStatDefinition, TNumber>> simulateAction
    )
    {
        AssertSimulating();
        AssertCopyExists(_testCopy);

        simulateAction(_testCopy);

        _performedActions.Add(simulateAction);

        Dictionary<IStat, IStatValueDiff<TStatDefinition, TNumber>> diffs = [];

        _testCopy.ForEachStat(
            (stat, value) =>
            {
                var sourceStatValue = _sourceStatSystem[stat];

                if (sourceStatValue is null)
                {
                    diffs[stat] = new StatValueDiff<TStatDefinition, TNumber>()
                    {
                        Stat = stat,
                        ValueBefore = default,
                        ValueAfter = value.Value,
                    };
                }
                else if (sourceStatValue.Value != value.Value)
                {
                    diffs[stat] = new StatValueDiff<TStatDefinition, TNumber>()
                    {
                        Stat = stat,
                        ValueBefore = sourceStatValue.Value,
                        ValueAfter = value.Value,
                    };
                }
            }
        );

        _sourceStatSystem.ForEachStat(
            (stat, value) =>
            {
                var newStatValue = _testCopy[stat];

                if (newStatValue is null)
                {
                    diffs[stat] = new StatValueDiff<TStatDefinition, TNumber>()
                    {
                        Stat = stat,
                        ValueBefore = value.Value,
                        ValueAfter = default,
                    };
                }
            }
        );

        return [.. diffs.Values];
    }

    internal void ConfirmUpdate()
    {
        AssertSimulating();
        foreach (var action in _performedActions)
        {
            action.Invoke(_sourceStatSystem);
        }
        Cleanup();
    }

    internal void CancelUpdate()
    {
        AssertSimulating();
        Cleanup();
    }

    private void Cleanup()
    {
        _performedActions.Clear();
        _isSimulating = false;
        _testCopy = null;
    }

    private void AssertSimulating()
    {
        if (!_isSimulating)
        {
            throw new InvalidOperationException("Can't perform this operation when not simulating");
        }
    }

    private static void AssertCopyExists(
        [NotNull] IStatContainer<TStatDefinition, TNumber>? container
    )
    {
        if (container is null)
        {
            throw new Exception(
                "Copy container was null when executing operation, ensure that StartUpdate() method was called"
            );
        }
    }
}
