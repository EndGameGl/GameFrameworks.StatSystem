using System.Buffers;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using GameFrameworks.StatSystem.Core;

namespace GameFrameworks.StatSystem.Internal;

internal class ModifierSourceTracker<TStatDefinition, TNumber>
    where TStatDefinition : IStat
    where TNumber : INumber<TNumber>
{
    private readonly struct StatLinkWithModifier
    {
        public TStatDefinition Stat { get; init; }

        public IStatModifier<TNumber> Modifier { get; init; }
    }

    private Dictionary<object, List<StatLinkWithModifier>> _statModifierSources;

    public ModifierSourceTracker()
    {
        _statModifierSources = [];
    }

    public void TrackStatModifierSource(
        TStatDefinition stat,
        IStatModifier<TNumber> modifier,
        object source
    )
    {
        EnsureLinkCollectionExists(source, out var links);
        links.Add(new StatLinkWithModifier { Stat = stat, Modifier = modifier });
    }

    public void RemoveTrackedModifier(
        TStatDefinition stat,
        IStatModifier<TNumber> modifier,
        object source
    )
    {
        if (!_statModifierSources.TryGetValue(source, out var links))
        {
            return;
        }

        for (var i = links.Count - 1; i >= 0; i--)
        {
            var link = links[i];

            if (link.Stat.Equals(stat) && link.Modifier.Equals(modifier))
            {
                links.RemoveAt(i);
            }
        }

        if (links.Count == 0)
        {
            _statModifierSources.Remove(source);
        }
    }

    public void RemoveTrackingForSource(
        object source,
        Action<TStatDefinition, IStatModifier<TNumber>> onLinkRemoved
    )
    {
        if (!_statModifierSources.TryGetValue(source, out var links))
        {
            return;
        }

        foreach (var link in links)
        {
            onLinkRemoved(link.Stat, link.Modifier);
        }

        _statModifierSources.Remove(source);
    }

    public void RemoveAllModifiersForStat(TStatDefinition stat)
    {
        var sourcesToRemove = ArrayPool<object>.Shared.Rent(_statModifierSources.Count);
        try
        {
            var sourceRemovalCount = 0;
            foreach (var (source, links) in _statModifierSources)
            {
                for (var i = links.Count - 1; i >= 0; i--)
                {
                    var link = links[i];
                    if (link.Stat.Equals(stat))
                    {
                        links.RemoveAt(i);
                    }
                }

                if (links.Count == 0)
                {
                    sourcesToRemove[sourceRemovalCount] = source;
                    sourceRemovalCount++;
                }
            }

            for (int i = 0; i < sourceRemovalCount; i++)
            {
                _statModifierSources.Remove(sourcesToRemove[i]);
            }
        }
        finally
        {
            ArrayPool<object>.Shared.Return(sourcesToRemove, clearArray: true);
        }
    }

    private void EnsureLinkCollectionExists(object source, out List<StatLinkWithModifier> links)
    {
        if (!_statModifierSources.TryGetValue(source, out var affectedStats))
        {
            affectedStats = [];
            _statModifierSources[source] = affectedStats;
        }
        links = affectedStats;
    }

    public bool TryGetSourceForStatModifier(
        TStatDefinition stat,
        IStatModifier<TNumber> modifier,
        [NotNullWhen(true)] out object? source
    )
    {
        source = null;
        foreach (var (modSource, links) in _statModifierSources)
        {
            foreach (var link in links)
            {
                if (link.Stat.Equals(stat) && link.Modifier.Equals(modifier))
                {
                    source = modSource;
                    return true;
                }
            }
        }

        return false;
    }
}
