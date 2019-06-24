using System;
using System.Collections.Generic;
using System.Drawing;

using DrawWithLineUs.Con.Config;
using DrawWithLineUs.Con.Model;

namespace DrawWithLineUs.Con.Service
{
    public static class GCodeService
    {

        public static List<string> GenerateGCode(List<CoordinateStructure> listCoordinateStructures)
        {
            Console.WriteLine($"Generating GCode");

            List<string> listGCodes = new List<string>();

            // reposition pen to "home" and explicitly lift the pen as first step (1000,1000 is defined as home in diagram)
            // using "G00" for rapid positioning
            listGCodes.Add($"{GCodes.RapidReposition} x1000 y1000 z{Pen.PenUpIndex}\n");

            foreach (var sequence in listCoordinateStructures)
            {
                // ------------- starting point of sequence -------------
                Point startingPoint = new Point
                {
                    X = (int)(sequence.ListRescaledPoints[0].X),
                    Y = (int)(sequence.ListRescaledPoints[0].Y)
                };


                // reposition pen to start of sequence, explicitly lift the pen, so it doesn't draw a line during the repositioning [from previous step] 
                listGCodes.Add($"{GCodes.RapidReposition} x{startingPoint.X} y{startingPoint.Y} z{Pen.PenUpIndex}\n");

                // lower pen,  as we're about to start drawing...
                listGCodes.Add($"{GCodes.LinearInterpolation} x{startingPoint.X} y{startingPoint.Y} z{Pen.PenUpIndex}\n");


                // ------------- path of sequence -------------
                foreach (var step in sequence.ListRescaledPoints)
                {
                    Point stepPoint = new Point
                    {
                        X = (int)(step.X),
                        Y = (int)(step.Y)
                    };

                    // using "G01" for "linear interpolation", to draw line between points
                    listGCodes.Add($"{GCodes.LinearInterpolation} x{stepPoint.X} y{stepPoint.Y} z{Pen.PenDownIndex}\n");
                }


                // ------------- end of sequence -------------
                // finally, lift the pen, re-using the last coordinates

                Point endPoint = new Point
                {
                    X = (int)(sequence.ListRescaledPoints[sequence.ListRescaledPoints.Count - 1].X),
                    Y = (int)(sequence.ListRescaledPoints[sequence.ListRescaledPoints.Count - 1].Y)
                };
                listGCodes.Add($"{GCodes.RapidReposition} x{endPoint.X} y{endPoint.Y} z{Pen.PenUpIndex}\n");

            }

            return listGCodes;
        }
    }
}
