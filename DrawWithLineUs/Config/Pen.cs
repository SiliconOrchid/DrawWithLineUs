namespace DrawWithLineUs.Config
{
    public static class Pen
    {
        //position of pen - it may be we need to globally adjust the amount of UP/DOWN.  Also we may be able to speed up drawing, by only lifting the pen the minimum amount necessary to clear the paper
        public const int PenUpIndex = 1000;
        public const int PenDownIndex = 1;
    }
}
