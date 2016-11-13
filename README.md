# RollGen

Rolls a set of dice, as determined by the D20 die system.

[![Build Status](https://travis-ci.org/DnDGen/RollGen.svg?branch=master)](https://travis-ci.org/DnDGen/RollGen)

## Syntax

### Use

To use RollGen, simple adhere to the verbose, fluid syntax:

```C#
var standardRoll = dice.Roll(4).d6().AsSum();
var customRoll = dice.Roll(92).d(66).AsSum();
var parsedRoll = dice.Roll("5+3d4*2").AsSum();

var chainedRolls = dice.Roll().d2().d(5).Keeping(1).d6().AsSum(); //Evaluated left to right

var individualRolls = dice.Roll(4).d6().AsIndividualRolls();
var parsedRolls = dice.Roll("5+3d4*2").AsIndividualRolls(); //NOTE: This will only return 1 roll, the same as AsSum()

var keptRolls = dice.Roll(4).d6().Keeping(3).AsSum();

var averageRoll = dice.Roll(4).d6().AsPotentialAverage();
```

### Getting `Dice` Objects

You can obtain dice from the domain project. Because the dice are very complex and are decorated in various ways, there is not a (recommended) way to build these objects manually. Please use the ModuleLoader for Ninject.

```C#
var kernel = new StandardKernel();
var rollGenModuleLoader = new RollGenModuleLoader();

rollGenModuleLoader.LoadModules(kernel);
```

Your particular syntax for how the Ninject injection should work will depend on your project (class library, web site, etc.)

### Installing RollGen

The project is on [Nuget](https://www.nuget.org/packages/DnDGen.RollGen). Install via the NuGet Package Manager.

    PM > Install-Package DnDGen.RollGen

#### There's RollGen and RollGen.Domain - which do I install?

That depends on your project.  If you are making a library that will only **reference** RollGen, but does not expressly implement it (such as the TreasureGen project), then you only need the RollGen package.  If you actually want to run and implement the dice (such as on the DnDGenSite or in the tests for TreasureGen), then you need RollGen.Domain, which will install RollGen as a dependency.
