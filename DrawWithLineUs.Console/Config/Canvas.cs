using System.Drawing;
using DrawWithLineUs.Console.Model;


namespace DrawWithLineUs.Console.Config
{
    public static class Canvas
    {
        // line-us drawable area as defined :  https://github.com/Line-us/Line-us-Programming/blob/master/Documentation/LineUsDrawingArea.pdf

        public static BoundingBox DrawableArea { get; private set; }
            = new BoundingBox
            {
                BottomLeft = new Point(700, -1000),
                TopRight = new Point(1800, 1000)
            };

    }
}
