namespace RollGen
{
    public interface PartialRoll
    {
        int d2();
        int d3();
        int d4();
        int d6();
        int d8();
        int d10();
        int d12();
        int d20();
        int Percentile();
        int d(int die);
    }
}