namespace DrawWithLineUs.Config
{
    // line-us GCODE spec https://github.com/Line-us/Line-us-Programming/blob/master/Documentation/GCodeSpec.pdf

    public static class GCodes
    {
        public const string RapidReposition = "g00";
        public const string LinearInterpolation = "g01";
        public const string Home = "g28";
    }
}
