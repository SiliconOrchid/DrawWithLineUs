using System.Drawing;

namespace DrawWithLineUs.Con.Model
{
    public class BoundingBox
    {
        public Point BottomLeft { get; set; } = new Point(0, 0);
        public Point TopRight { get; set; } = new Point(0, 0);

        public int GetBoxWidth()
        {
            //TODO : Null handling
            return TopRight.X - BottomLeft.X;
        }

        public int GetBoxHeight()
        {
            //TODO : Null handling
            return TopRight.Y - BottomLeft.Y;
        }
    }
}
