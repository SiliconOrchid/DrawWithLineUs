using System.Drawing;

using DrawWithLineUs.Con.Model;


namespace DrawWithLineUs.Con.Config
{
    public static class Canvas
    {
        // line-us drawable area as defined :  https://github.com/Line-us/Line-us-Programming/blob/master/Documentation/LineUsDrawingArea.pdf

        public static BoundingBox DrawableArea { get; private set; }
            = new BoundingBox
            {
                BottomLeft = new Point(650, -1000),
                TopRight = new Point(1800, 1000)
            };

    }
}
