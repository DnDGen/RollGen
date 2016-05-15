namespace RollGen.Domain.PartialRolls
{
    internal interface PartialRollFactory
    {
        PartialRoll Build(int quantity);
    }
}
