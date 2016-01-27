# RollGen

Rolls a set of dice, as determined by the D20 die system.

[![Build status](https://ci.appveyor.com/api/projects/status/027rvoq03xg3h1qj)](https://ci.appveyor.com/project/cidthecoatrack/rollgen)

## Syntax

### Use

To use RollGen, simple adhere to the verbose, fluid syntax:

```C#
var standardRoll = dice.Roll(3).d6();
var customRoll = dice.Roll(92).d(66);
var parsedRoll = dice.Roll("5+3d4*2");
```

### Getting `Dice` Objects

You can obtain a `Dice` object in one of 2 ways:

1. **Recommended** Use the Ninject Bootstrapper to inject the `Dice` object into your constructor

   ```C#
   var kernel = new StandardKernel();
   var rollGenModuleLoader = new RollGenModuleLoader();

   rollGenModuleLoader.LoadModules(kernel);
   ```

   Your particular syntax for how the Ninject injection should work will depend on your project (class library, web site, etc.)

2. Manually build the `Dice` object:

   ```C#
   var random = new Random();
   var dice = new RandomDice(random);
   ```

   **Important** If you are newing up dice objects multiple times, the seed for the `Random` class that you use will potentially be the same, and will not produce random results.  The Ninject bootstrapper has taken this into account and helps ensure random seeds for the `Random` object - which is why it is the recommended way to build your `Dice` object.

### Installing RollGen

The project is on [Nuget](https://www.nuget.org/packages/D20Dice). Install via the NuGet Package Manager.

    PM > Install-Package D20Dice

#### There's RollGen and RollGen.Bootstrap - which do I install?

That depends on your project.  If you are making a library that will only **reference** RollGen, but does not expressly implement it (such as the TreasureGen project), then you only need the RollGen package.  If you actually want to run and implement the dice (such as on the DnDGenSite or in the tests for TreasureGen), then you need RollGen.Bootstrap, which will install RollGen as a dependency.
