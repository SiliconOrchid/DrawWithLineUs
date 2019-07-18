using System;
using System.Collections.Generic;
using System.Drawing;

using DrawWithLineUs.Con.Config;
using DrawWithLineUs.Con.Model;

namespace DrawWithLineUs.Con.Service
{
    public class GeometryService : IGeometryService
    {
        /// <summary>
        /// Compares the height & width of image, against the dimensions of the Canvas' drawable-area, to determine
        /// a scaling factor that needs to be used.   Automatically selects the smaller scaling ratio from the two axis.
        /// </summary>
        /// <param name="sourceBoundingBox"></param>
        /// <returns></returns>
        public decimal DetermineScalingRatio(BoundingBox sourceBoundingBox)
        {
            decimal scalingRatio = 1;
            decimal ratioX = (decimal)Canvas.DrawableArea.GetBoxWidth() / (decimal)sourceBoundingBox.GetBoxWidth();
            decimal ratioY = (decimal)Canvas.DrawableArea.GetBoxHeight() / (decimal)sourceBoundingBox.GetBoxHeight();

            Console.WriteLine($"ratio X ({sourceBoundingBox.GetBoxWidth()} / {Canvas.DrawableArea.GetBoxWidth()}) {ratioX}");
            Console.WriteLine($"ratio Y ({sourceBoundingBox.GetBoxHeight()} / {Canvas.DrawableArea.GetBoxHeight()})  {ratioY}");


            if (ratioX < ratioY)
            {
                scalingRatio = ratioX;
                Console.WriteLine($"Using Ratio X ({ratioX})");
            }
            else
            {
                scalingRatio = ratioY;
                Console.WriteLine($"Using Ratio Y ({ratioY})");
            }

            return scalingRatio;
        }


        /// <summary>
        /// Determine the max boundaries of the coordinate points, extracted from the original image, by inspecting every possible point
        /// </summary>
        /// <param name="listCoordinateStructures"></param>
        /// <returns>Populated [BoundingBox] with coordinates of the image extremities</returns>
        public BoundingBox DetermineSourceBounds(List<CoordinateStructure> listCoordinateStructures)
        {
            Console.WriteLine($"Determining boundaries...");

            // box used to determine extremeties of source image - corners are deliberatly set with large opposing values, as these will be redefined later
            BoundingBox sourceBoundingBox = new BoundingBox
            {
                BottomLeft = new Point(10000, 10000),
                TopRight = new Point(-10000, -10000)
            };

            foreach (var sequence in listCoordinateStructures)
            {
                foreach (var step in sequence.ListPoints)
                {
                    if (step.X <= sourceBoundingBox.BottomLeft.X)
                    {
                        sourceBoundingBox.BottomLeft = new Point(step.X, sourceBoundingBox.BottomLeft.Y);
                    }

                    if (step.X >= sourceBoundingBox.TopRight.X)
                    {
                        sourceBoundingBox.TopRight = new Point(step.X, sourceBoundingBox.TopRight.Y);
                    }

                    if (step.Y <= sourceBoundingBox.BottomLeft.Y)
                    {
                        sourceBoundingBox.BottomLeft = new Point(sourceBoundingBox.BottomLeft.X, step.Y);
                    }

                    if (step.Y >= sourceBoundingBox.TopRight.Y)
                    {
                        sourceBoundingBox.TopRight = new Point(sourceBoundingBox.TopRight.X, step.Y);
                    }
                }
            }

            Console.WriteLine($"Determined that original image has boundaries of ({sourceBoundingBox.BottomLeft.X},{sourceBoundingBox.BottomLeft.Y}) - ({sourceBoundingBox.TopRight.X},{sourceBoundingBox.TopRight.Y})");
            Console.WriteLine($"Determined that original image has dimensions of (width: {sourceBoundingBox.GetBoxWidth()}, height: {sourceBoundingBox.GetBoxHeight()})");

            return sourceBoundingBox;

        }


        public void RescaleAndOffset(List<CoordinateStructure> listCoordinateStructures, decimal scalingRatio, int offsetX, int offsetY)
        {
            foreach (var coordinateStucture in listCoordinateStructures)
            {
                foreach (var step in coordinateStucture.ListPoints)
                {
                    PointF rescaledPoint = new PointF((float)step.X * (float)scalingRatio, (float)step.Y * (float)scalingRatio);
                    rescaledPoint.X = rescaledPoint.X + offsetX;
                    rescaledPoint.Y = rescaledPoint.Y + offsetY;
                    coordinateStucture.ListRescaledPoints.Add(rescaledPoint);
                }
            }
        }
    }
}
