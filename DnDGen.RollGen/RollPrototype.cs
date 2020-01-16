namespace DnDGen.RollGen
{
    internal class RollPrototype
    {
        public int Quantity { get; set; }
        public int Die { get; set; }

        public string Build()
        {
            return $"{Quantity}d{Die}";
        }

        public override string ToString()
        {
            return Build();
        }

        public int Range => Die * Quantity - Quantity + 1;
    }
}
