using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DnDGen.RollGen.Tests.Integration")]
[assembly: InternalsVisibleTo("DnDGen.RollGen.Tests.Integration.IoC")]
[assembly: InternalsVisibleTo("DnDGen.RollGen.Tests.Unit")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace DnDGen.RollGen
{
    public interface Dice
    {
        PartialRoll Roll(int quantity = 1);
        PartialRoll Roll(string rollExpression);
        PartialRoll Roll(PartialRoll roll);
        /// <summary>Replaces dice rolls and other evaluatable expressions in a string that are wrapped by special strings.</summary>
        /// <typeparam name="T">Type to return Evaluated expressions in</typeparam>
        /// <param name="str">Contains expressions to replace and other text.</param>
        /// <param name="openexpr">Comes before expressions to replace. Mustn't be empty.</param>
        /// <param name="closeexpr">Ends expression to replace. Mustn't be empty.</param>
        /// <param name="openexprescape">Prefix openexpr with this to have the replacer ignore it. If null, there's no escape.</param>
        string ReplaceWrappedExpressions<T>(string str, string openexpr = "{", string closeexpr = "}", char? openexprescape = '\\');
        string ReplaceExpressionWithTotal(string expression, bool lenient = false);
        string ReplaceRollsWithSumExpression(string expression, bool lenient = false);
        bool ContainsRoll(string expression, bool lenient = false);
    }
}
