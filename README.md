# RollGen

Rolls a set of dice, as determined by the D20 die system.

[![Build Status](https://dev.azure.com/dndgen/DnDGen/_apis/build/status/DnDGen.RollGen?branchName=master)](https://dev.azure.com/dndgen/DnDGen/_build/latest?definitionId=1&branchName=master)

## Syntax

### Use

To use RollGen, simple adhere to the verbose, fluid syntax:

```C#
var standardRoll = dice.Roll(4).d6().AsSum();
var customRoll = dice.Roll(92).d(66).AsSum();
var expressionRoll = dice.Roll("5+3d4*2").AsSum();

var chainedRolls = dice.Roll().d2().d(5).Keeping(1).d6().AsSum(); //1d2d5k1d6 Evaluated left to right -> ((1d2)d5k1)d6

var individualRolls = dice.Roll(4).d6().AsIndividualRolls();
var parsedRolls = dice.Roll("5+3d4*2").AsIndividualRolls(); //NOTE: This will only return 1 roll, the same as AsSum()

var keptRolls = dice.Roll(4).d6().Keeping(3).AsIndividualRolls(); //Returns the highest 3 rolls
var expressionKeptRolls = dice.Roll("3d4k2").AsSum(); //Returns the sum of 2 highest rolls

var averageRoll = dice.Roll(4).d6().AsPotentialAverage(); //Returns the average roll for the expression.  For here, it will return 14.
var expressionAverageRoll = dice.Roll("5+3d4*3").AsPotentialAverage(); //5+7.5*3, returning 27.5 

var minRoll = dice.Roll(4).d6().AsPotentialMinimum(); //Returns the minimum roll for the expression.  For here, it will return 4.
var expressionMinRoll = dice.Roll("5+3d4*3").AsPotentialMinimum(); //5+3*3, returning 14

var maxRoll = dice.Roll(4).d6().AsPotentialMaximum(); //Returns the maximum roll for the expression.  For here, it will return 24.
var expressionMaxRoll = dice.Roll("5+3d4*3").AsPotentialMaximum(); //5+12*3, returning 41 

var success = dice.Roll().Percentile().AsTrueOrFalse(); //Returns true if high (51-100), false if low (1-50)
var customPercentageSuccess = dice.Roll().Percentile().AsTrueOrFalse(.9); //Returns true if > 90, false if <= 90
var customRollSuccess = dice.Roll().Percentile().AsTrueOrFalse(90); //Returns true if >= 90, false if < 90
var expressionSuccess = dice.Roll("5+3d4*2").AsTrueOrFalse(); //Returns true if high, false if low
var explicitExpressionSuccess = dice.Roll("2d6 >= 1d12").AsTrueOrFalse(); //Evalutes boolean expression after rolling

var containsRoll = dice.ContainsRoll("This contains a roll of 4d6k3 for rolling stats"); //will return true here
var summedSentence = dice.ReplaceRollsWithSumExpression("This contains a roll of 4d6k3 for rolling stats"); //returns "This contains a roll of (5 + 3 + 2) for rolling stats"
var rolledSentence = dice.ReplaceExpressionWithTotal("This contains a roll of 4d6k3 for rolling stats"); //returns "This contains a roll of 10 for rolling stats"
var rolledComplexSentenceMin = dice.ReplaceWrappedExpressions<double>("Fireball does {min(4d6,10) + 0.5} damage"); //returns "Fireball does 7.5 damage"
var rolledComplexSentenceMax = dice.ReplaceWrappedExpressions<double>("Fireball does {max(4d6,10) + 0.5} damage"); //returns "Fireball does 15.5 damage"

var optimizedRoll = RollHelper.GetRollWithMostEvenDistribution(4, 9); //returns "1d6+3", which is the most evenly-distributed roll possible, whether optimizing for dice or distribution
var optimizedRollWithMultipleDice = RollHelper.GetRollWithMostEvenDistribution(1, 9); //returns "1d8+1d2-1", because it more evenly-distributed than "4d3-3"
var optimizedRollWithFewestDice = RollHelper.GetRollWithFewestDice(1, 9); //returns "4d3-3", because it uses only 1 kind of dice compared to "1d8+1d2-1"

var explodedRolls = dice.Roll(4).d6().Explode().AsIndividualRolls(); //If a 6 is rolled, then an additional roll is performed.  I.E., 3 + 6 + 5 + 4 + 1
var expressionExplodedRolls = dice.Roll("3d4!").AsSum(); //Return the sum of the rolls, including bonus rolls from explosion on rolls of 4
var expressionExplodedKeptRolls = dice.Roll("3d4!k2").AsSum(); //Returns the sum of 2 highest rolls, including bonus rolls from explosion on rolls of 4
var expressionExplodedMultipleRolls = dice.Roll("3d4!e3").AsSum(); //Return the sum of the rolls, including bonus rolls from explosion on rolls of 3 or 4
var expressionExplodedMultipleKeptRolls = dice.Roll("3d4e1e2k2").AsSum(); //Returns the sum of 2 highest rolls, including bonus rolls from explosion on rolls of 1 or 2

var transformedRolls = dice.Roll(3).d6().Transforming(1).AsIndividualRolls(); //If a 1 is rolled, we will count it as a 6.  I.E., 3 + 1 + 6 = 3 + 6 + 6
var transformedMultipleRolls = dice.Roll("3d6t1t5").AsSum(); //Return the sum of the rolls, including 1's and 5's transformed to 6's
var transformedExplodedKeptRolls = dice.Roll("3d6!t1k2").AsSum(); //Returns the sum of 2 highest rolls, including bonus rolls from explosion and transforming 1's to 6's.
var transformedCustomRolls = dice.Roll("3d6t1:2").AsSum(); //Return the sum of the rolls, transforming 1's into 2's.

//Returns a qualitative description of the roll, based on percentages
var description = dice.Describe("3d6", 9); //Returns "Bad"
description = dice.Describe("3d6", 13); //Returns "Good"
description = dice.Describe("3d6", 12, "Bad", "Average", "Good"); //Returns "Average"
description = dice.Describe("3d6", 6, "Bad", "Average", "Good"); //Returns "Bad"
description = dice.Describe("3d6", 16, "Bad", "Average", "Good"); //Returns "Good"

//Returns whether the roll is valid
var valid = dice.IsValid("3d6+1"); //Returns TRUE
valid = dice.IsValid("(3)d(6)+1"); //Returns TRUE
valid = dice.IsValid("(1d2)d(3d4)+5d6"); //Returns TRUE
valid = dice.IsValid("10000d10000"); //Returns TRUE
valid = dice.IsValid("(100d100d100)d(100d100d100)"); //Returns TRUE

valid = dice.IsValid("(0)d(6)+1"); //Returns FALSE, because quantity is too low
valid = dice.IsValid("(3.1)d(6)+1"); //Returns FALSE, because decimals are not allowed for dice operations
valid = dice.IsValid("0d6+1"); //Returns FALSE, because quantity is too low
valid = dice.IsValid("10001d10000"); //Returns FALSE, because quantity is too high
valid = dice.IsValid("(100d100d100+1)d(100d100d100)"); //Returns FALSE, because quantity could potentially be above the 10,000 limit

valid = dice.IsValid("(3)d(-6)+1"); //Returns FALSE, because die is too low
valid = dice.IsValid("(3)d(6.1)+1"); //Returns FALSE, because decimals are not allowed for dice operations
valid = dice.IsValid("3d0+1"); //Returns FALSE, because die is too low
valid = dice.IsValid("10000d10001"); //Returns FALSE, because die is too high
valid = dice.IsValid("(100d100d100)d(100d100d100+1)"); //Returns FALSE, because die could potentially be above the 10,000 limit

valid = dice.IsValid("4d6k3"); //Returns TRUE
valid = dice.IsValid("(4)d(6)k(3)"); //Returns TRUE
valid = dice.IsValid("(4)d(6)k(-1)"); //Returns FALSE, because keep value is too low
valid = dice.IsValid("(4)d(6)k(3.1)"); //Returns FALSE, because decimals are not allowed for dice operations
valid = dice.IsValid("4d6k10000"); //Returns TRUE
valid = dice.IsValid("4d6k10001"); //Returns FALSE, because keep value is too high
valid = dice.IsValid("4d6k(100d100d100+1)"); //Returns FALSE, because keep value could potentially be above the 10,000 limit

valid = dice.IsValid("2d3!"); //Returns TRUE
valid = dice.IsValid("2d3!e2"); //Returns TRUE
valid = dice.IsValid("2d3!e2e1"); //Returns FALSE, because it explodes on all values

valid = dice.IsValid("3d6t1"); //Returns TRUE
valid = dice.IsValid("3d6t1t2"); //Returns TRUE
valid = dice.IsValid("3d6t7"); //Returns TRUE
valid = dice.IsValid("3d6t0"); //Returns FALSE, because transform target is too low
valid = dice.IsValid("3d6t6:0"); //Returns TRUE
valid = dice.IsValid("3d6t10001"); //Returns FALSE, because transform target is too high
valid = dice.IsValid("3d6t6:10001"); //Returns TRUE

valid = dice.IsValid("avg(1d12, 2d6, 3d4, 4d3, 6d2)"); //Returns TRUE, because this is a valid Albatross function
valid = dice.IsValid("bad(1d12, 2d6, 3d4, 4d3, 6d2)"); //Returns FALSE, because "bad" is not a valid Albatross function

valid = dice.IsValid("this is not a roll"); //Returns FALSE
valid = dice.IsValid("this contains 3d6, but is not a roll"); //Returns FALSE
valid = dice.IsValid("9266+90210-42*600/1337%1336+96d(783d82%45+922-2022/337)-min(max(avg(1d2, 3d4, 5d6)))"); //Returns TRUE

```

Important things to note:

1. When retrieving individual rolls from an expression, only the sum of the expression is returned.  This is because determining what an "individual roll" is within a complex expression is not certain.  As an example, what would individual rolls be for `1d2+3d4`?
2. Mathematical expressions supported are `+`, `-`, `*`, `/`, and `%`, as well as die rolls as demonstrated above.  Spaces are allowed in the expression strings.  Paranthetical expressions are also allowed.
3. For replacement methods on `Dice`, there is an option to do "lenient" replacements (optional boolean).  The difference:
    a. "1d2 ghouls and 2d4 zombies" strict -> "1 ghouls and 5 zombies"
    b. "1d2 ghouls and 2d4 zombies" lenient -> "1 ghouls an3 zombies" (it reads "and 2d4" as "an(d 2d4)")

#### Order of Operations

In regards to the operators one can apply to a roll (Keeping, Exploding, Transforming), this is the order of operations:

1. Explode
2. Transform
3. Keep

One can specify these commands in any order, as they will be evaluated in their order of operation.  For example, all of these rolls will parse the same: `4d3!t2k1`, `4d3!k1t2`, `4d3t2!k1`, `4d3t2k1!`, `4d3k1!t2`, `4d3k1t2!` - all will be evaluated as `4d3`, exploding on a `3`, then transforming `2` into `3`, then keeping the highest roll.

Beyond this, order of operations is respected as outlined by the Albatross documentation: https://rushuiguan.github.io/expression/articles/operations.html#the-precedence-of-infix-operations

The documentation also outlines supported functions (such as `min` and `max`) that can be used: https://rushuiguan.github.io/expression/articles/operations.html

### Getting `Dice` Objects

You can obtain dice from the domain project. Because the dice are very complex and are decorated in various ways, there is not a (recommended) way to build these objects manually. Please use the ModuleLoader for Ninject.

```C#
var kernel = new StandardKernel();
var rollGenModuleLoader = new RollGenModuleLoader();

rollGenModuleLoader.LoadModules(kernel);
```

Your particular syntax for how the Ninject injection should work will depend on your project (class library, web site, etc.).  Each of the examples below work independently.  You should use the one that is appropriate for your project.

```C#
[Inject] //Ninject property
public Dice MyDice { get; set; }

public MyClass()
{ }

public int Roll()
{
    return MyDice.Roll(4).d6().Keeping(3).AsSum();
}
```

```C#
private myDice;

public MyClass(Dice dice) //This works if you are calling Ninject to build MyClass
{
    myDice = dice;
}

public int Roll()
{
    return myDice.Roll(4).d6().Keeping(3).AsSum();
}
```

```C#
public MyClass()
{ }

public int Roll()
{
    var myDice = DiceFactory.Create(); //Located in RollGen.IoC
    return myDice.Roll(4).d6().Keeping(3).AsSum();
}
```

### Installing RollGen

The project is on [Nuget](https://www.nuget.org/packages/DnDGen.RollGen). Install via the NuGet Package Manager.

    PM > Install-Package DnDGen.RollGen

