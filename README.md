# RollGen

Rolls a set of dice, as determined by the D20 die system.

[![Build Status](https://travis-ci.org/DnDGen/RollGen.svg?branch=master)](https://travis-ci.org/DnDGen/RollGen)

## Syntax

### Use

To use RollGen, simple adhere to the verbose, fluid syntax:

```C#
var standardRoll = dice.Roll(3).d6();
var customRoll = dice.Roll(92).d(66);
var parsedRoll = dice.Roll("5+3d4*2");
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

The project is on [Nuget](https://www.nuget.org/packages/D20Dice). Install via the NuGet Package Manager.

    PM > Install-Package D20Dice

#### There's RollGen and RollGen.Domain - which do I install?

That depends on your project.  If you are making a library that will only **reference** RollGen, but does not expressly implement it (such as the TreasureGen project), then you only need the RollGen package.  If you actually want to run and implement the dice (such as on the DnDGenSite or in the tests for TreasureGen), then you need RollGen.Domain, which will install RollGen as a dependency.
