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

            foreach (var sequence in listCoordinateStructures)
            {
                SequenceStart(listGCodes, sequence);

                SequencePath(listGCodes, sequence);

                SequenceEnd(listGCodes, sequence);
            }

            return listGCodes;
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
            listGCodes.Add($"{GCodes.LinearInterpolation} x{startingPoint.X} y{startingPoint.Y} z{Pen.PenDownIndex}\n");
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
                // using "G01" for "linear interpolation", to draw line between points
                listGCodes.Add($"{GCodes.LinearInterpolation} x{(int)step.X} y{(int)step.Y} z{Pen.PenDownIndex}\n");
            }
        }



        /// <summary>
        /// Lift the pen at the last coordinate (ready to 'rapid reposition to start of next path')
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
