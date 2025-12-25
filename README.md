# GameFrameworks.StatSystem

This project provides basic functionality to make generic stat container

# Table of content
- [Features](#features)
- [How to use](#how-to-use)
- [Initial setup](#initial-setup)
- [Container creation](#container-creation)
- [Stat value handlers](#stat-value-handlers)
- [Adding and removing stats](#adding-and-removing-stats)
- [Accessing stat values](#accessing-stat-values)
- [Tracking stat changes](#tracking-stat-changes)
- [Stat modifiers](#stat-modifiers)
- [Pass processors](#pass-processors)
- [Stat value postprocessors](#stat-value-postprocessors)
- [Running simulations and getting diffs](#running-simulations-and-getting-diffs)

## Features
 - Supports any numeric type that implements `INumber<T>`
 - Customisable pass system to handle stat modifiers any way you want
 - Customisable stat value postprocessors
 - Simulate any operations on stat container and get back stat value diffs


## How to use

### Initial setup: 
You will need to create a new class that will implement `IStat` interface 
```
public class StatDefinition : IStat 
{
    public string Name { get; }
}
```

### Container creation:

Now that you have a new class to define stats, you can create container to manage stats and their values
```
using GameFrameworks.StatSystem.PassProcessors;

IStatContainer<StatDefinition, float> statContainer = new StatContainer<StatDefinition, float>()
{
    // this line creates a new pass processor for the container
    // for now you might just leave it empty
    PassProcessor = PassProcessorBuilder<float>.Create().Build()
};
```

### Stat value handlers

This library provides 2 classes to work with:
 1. `StatValue<TStatDefinition, TNumber>` - handles primary stats by simply providing base value
 2. `CalculatedStatValue<TStatDefinition, TNumber>` - handles derived stats by passing in expression to calculate base value

You can implement any other stat value handler as long as it implements `IStatValue<TStatDefinition, TNumber>` interface

### Adding and removing stats

To add a stat, simply call
```
statContainer.AddStat(stat, value);
```

To remove stat, `RemoveStat` method is called

### Accessing stat values

A few methods can be used to access stats:
```
// let's assume you already have the stat reference
IStat statReference;

// first method
if (statContainer.TryGetStatValue(statReference, out var statValue))
{
    var value = statValue.Value;
}

// second method
var statValue = statContainer[statReference];
if (statValue is not null)
{
    var value = statValue.Value;
}
```

### Tracking stat changes

To track specific stat changes, one must write following code:
```
statContainer.SubscribeToStatChange(
    statReference,
    OnStatValueChanged
);

// some example method with generic values, make sure to use correct types of your container
private void OnStatValueChanged(IStatValue<TStatDefinition, TNumber> statValue, TNumber oldValue) { }
```

When you no longer need to track of anything, or deleting tracking object, make sure to unsubscribe to prevent memory leaks
```
statContainer.UnsubscribeFromStatChange(statReference, OnStatValueChanged);
```

### Stat modifiers

Stat modifiers can be added to any stat via following method call:
```
statContainer.AddStatModifier(statReference, statModifier, source);
```

`source` parameter is a reference to any object, that can be linked to group certain modifiers internally and remove them via said source if needed

Modifiers can be removed with `RemoveStatModifier(stat, modifier, source)`, `BatchRemoveStatModifiersFromSource(source)`, `RemoveAllModifiers(stat)` methods

### Pass processors
To handle stat modifiers, you must use `IStatModifierPassProcessor<TNumber>`.
To create pass processor builder one must use following code:
```
var builder = PassProcessorBuilder<float>.Create();
```
Then you can add multiple processor passes before building it
```
builder.AddPass(IStatModifierPassProcessor<TNumber> passProcessor)
```

Pass processors have 2 main functions to do:
1. Filter modifiers
2. Handle modifier calculations

You will need to implement your own pass processors, here's an example of a flat value modifier adder:
```
// let's assume that we already have modifier class created
public class FlatValueModifier : IStatModifier<float> 
{
    public float GetModifier() => 5;
}

public class FlatValueModifierPassProcessor : IStatModifierPassProcessor<float> 
{
    public IStatModifierFilter<float> Filter { get; } = StatModifierFilterChain.WithCondition<float>(mod => mod is FlatValueModifier);

    public TNumber ProcessPass(TNumber initialValue, IStatModifier<TNumber>[] modifiers)
    {
        return initialValue + modifiers.Sum(x => x.GetModifier());
    }
}
```

You might've noticed `IStatModifierFilter<float>` in the code above. It is used to filter what modifiers and applied to the current pass.

### Stat value postprocessors

When creating stats, one might want to limit stat values in some way or cut away bad value changes

`IStatValuePostProcessor<TNumber>` is used to achieve such functionality

```
var postProcessor = PostProcessorChain.WithMinValue<float>(1f).WithMaxValue<float>(100f);

var newStatValue = postProcessor.ProcessValue(statInitialValue, 120f);

Console.WriteLine(newStatValue); // will print out 100f, since that was limited by postprocessor
```

### Running simulations and getting diffs

```
// enter simulation mode

statContainer.StartUpdate();

// simulate an operation
var statDifference = statContainer.RunSimulations(container => container.AddStatModifier(statReference, modifier));

// confirm all updates and post them to stat container
statContainer.ConfirmUpdate();

// cancel updates and discard changes
statContainer.CancelUpdate();
```