using System.Drawing;

using NUnit.Framework;

using DrawWithLineUs.Model;
using DrawWithLineUs.Service;
using System.Collections.Generic;

namespace Tests
{
    public class GeometryServiceTests
    {

        IGeometryService _geometryService;

        [SetUp]
        public void Setup()
        {

            _geometryService = new GeometryService();
        }

        [Test]
        public void DetermineScalingRatio_ShouldReturn_ExpectedRatio1to1()
        {
            // for this test, we set the size of the test box to be the same as the Drawable Canvas Size (as set in configuration "Canvas.cs")
            // we expect a 1:1 ratio
            BoundingBox testBoundingBox = new BoundingBox();
            testBoundingBox.BottomLeft = new Point(650, -1000);
            testBoundingBox.TopRight = new Point(1800, 1000);


            decimal scalingRatioResult = _geometryService.DetermineScalingRatio(testBoundingBox);

            Assert.AreEqual(scalingRatioResult, 1.00);
        }


        [Test]
        public void DetermineScalingRatio_ShouldReturn_ExpectedRatio50pcVertical()
        {
            // for this test, we set the size of the test box to have double the height of 
            //the Drawable Canvas Size (as set in configuration "Canvas.cs")
            // we expect a 50% scaling ratio (0.5)
            BoundingBox testBoundingBox = new BoundingBox();
            testBoundingBox.BottomLeft = new Point(650, -2000);
            testBoundingBox.TopRight = new Point(1800, 2000);


            decimal scalingRatioResult = _geometryService.DetermineScalingRatio(testBoundingBox);

            Assert.AreEqual(scalingRatioResult, 0.5);
        }

        [Test]
        public void DetermineScalingRatio_ShouldReturn_ExpectedRatio50pcHorizontal()
        {
            // same as above test, except we expect the result to derive from the horizontal axis
            // we expect a 50% scaling ratio (0.5)
            BoundingBox testBoundingBox = new BoundingBox();
            testBoundingBox.BottomLeft = new Point(650, -1000);
            testBoundingBox.TopRight = new Point(1800 + 1150, 1000);


            decimal scalingRatioResult = _geometryService.DetermineScalingRatio(testBoundingBox);

            Assert.AreEqual(scalingRatioResult, 0.5);
        }



        [Test]
        public void DetermineSourceBounds_ShouldReturn_ExpectedBoundingBox()
        {
            //we expect the result to be the compound spread of all extremities of the provided data

            CoordinateStructure coordinateStructure1 = new CoordinateStructure();
            coordinateStructure1.ListPoints = new List<Point> { new Point(50, 50), new Point(60, 60) };

            CoordinateStructure coordinateStructure2 = new CoordinateStructure();
            coordinateStructure2.ListPoints = new List<Point> { new Point(150, 150), new Point(160, 160) };

            List<CoordinateStructure> listCoordinateStructures = new List<CoordinateStructure> { coordinateStructure1, coordinateStructure2 };

            var result = _geometryService.DetermineSourceBounds(listCoordinateStructures);

            Assert.AreEqual(result.BottomLeft.X, 50);
            Assert.AreEqual(result.BottomLeft.Y, 50);
            Assert.AreEqual(result.TopRight.X, 160);
            Assert.AreEqual(result.TopRight.Y, 160);
        }
    }
}