namespace DnDGen.RollGen
{
    internal class RollPrototype
    {
        public int Quantity { get; set; }
        public int Die { get; set; }
        public int Multiplier { get; set; } = 1;

        public string Build()
        {
            if (Multiplier > 1)
                return $"({Quantity}d{Die}-{Quantity})*{Multiplier}";

            return $"{Quantity}d{Die}";
        }

        public override string ToString()
        {
            return Build();
        }

        public bool IsValid => Quantity > 0
            && Quantity <= Limits.Quantity
            && Die > 0
            && Die <= Limits.Die;
    }
}
