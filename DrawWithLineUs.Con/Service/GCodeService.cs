using System;
using System.Collections.Generic;
using System.Drawing;

using DrawWithLineUs.Config;
using DrawWithLineUs.Model;

namespace DrawWithLineUs.Service
{
    public class GCodeService : IGCodeService
    {

        public List<string> GenerateGCode(List<CoordinateStructure> listCoordinateStructures)
        {
            Console.WriteLine($"Generating GCode");

            List<string> listGCodes = new List<string>();

            SequenceHome(listGCodes);

            foreach (var sequence in listCoordinateStructures)
            {
                SequenceStart(listGCodes, sequence);

                SequencePath(listGCodes, sequence);

                SequenceEnd(listGCodes, sequence);
            }

            return listGCodes;
        }



        /// <summary>
        /// Reposition pen to "home" and explicitly lift the pen as first step (1000,1000 is defined as home in documentation/diagram)
        /// using "G00" for rapid positioning
        /// </summary>
        /// <param name="listGCodes"></param>
        private static void SequenceHome(List<string> listGCodes)
        {
            listGCodes.Add($"{GCodes.RapidReposition} x1000 y1000 z{Pen.PenUpIndex}\n");
        }



        /// <summary>
        /// Move pen to starting point of new path (sequence)
        /// </summary>
        /// <param name="listGCodes"></param>
        /// <param name="sequence"></param>
        private static void SequenceStart(List<string> listGCodes, CoordinateStructure sequence)
        {
            Point startingPoint = new Point
            {
                X = (int)(sequence.ListRescaledPoints[0].X),
                Y = (int)(sequence.ListRescaledPoints[0].Y)
            };

            // reposition pen to start of sequence, explicitly lift the pen, so it doesn't draw a line during the repositioning [from previous step] 
            listGCodes.Add($"{GCodes.RapidReposition} x{startingPoint.X} y{startingPoint.Y} z{Pen.PenUpIndex}\n");

            // lower pen,  as we're about to start drawing...
            listGCodes.Add($"{GCodes.LinearInterpolation} x{startingPoint.X} y{startingPoint.Y} z{Pen.PenUpIndex}\n");
        }



        /// <summary>
        /// Move pen through each of the points in the path (sequence)
        /// </summary>
        /// <param name="listGCodes"></param>
        /// <param name="sequence"></param>
        private static void SequencePath(List<string> listGCodes, CoordinateStructure sequence)
        {
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
        }



        /// <summary>
        /// Lift the pen, re-using the last coordinates
        /// </summary>
        /// <param name="listGCodes"></param>
        /// <param name="sequence"></param>
        private static void SequenceEnd(List<string> listGCodes, CoordinateStructure sequence)
        {
            Point endPoint = new Point
            {
                X = (int)(sequence.ListRescaledPoints[sequence.ListRescaledPoints.Count - 1].X),
                Y = (int)(sequence.ListRescaledPoints[sequence.ListRescaledPoints.Count - 1].Y)
            };
            listGCodes.Add($"{GCodes.RapidReposition} x{endPoint.X} y{endPoint.Y} z{Pen.PenUpIndex}\n");
        }
    }
}
