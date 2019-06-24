using System.Collections.Generic;
using System.Drawing;

namespace DrawWithLineUs.Console.Model
{
    public class CoordinateStructure
    {
        public List<Point> ListPoints { get; set; }
        public List<PointF> ListRescaledPoints { get; set; } //using PointF for better precision, having been scaled

        public CoordinateStructure()
        {
            ListPoints = new List<Point>();
            ListRescaledPoints = new List<PointF>();
        }
    }
}
