using System.Collections.Generic;
using NUnit.Framework;

using DrawWithLineUs.Model;
using DrawWithLineUs.Service;
using DrawWithLineUs.Enum;
using System;
using System.Drawing;

namespace DrawWithLineUs.Console.Test
{
    public class SvgServiceTests
    {

        ISvgService _svgService;

        readonly string _singlePathNode = "M2219 5983 c-13 -16 -12 -17 4 -4 9 7 17 15 17 17 0 8 -8 3 -21 -13z";
        readonly string[] _listOfWords = { "M2219", "5983", "c-13", "-16", "-12", "-17", "4", "-4", "9", "7", "17", "15", "17", "17", "0", "8", "-8", "3", "-21", "-13z" };


        [SetUp]
        public void Setup()
        {
            _svgService = new SvgService();
        }


        #region  IncrementCurrentPathIndex -------------------------------------


        [Test]
        public void IncrementCurrentPathIndex_ShouldReturn_ExpectedFromLine()
        {
            var result = _svgService.IncrementCurrentPathIndex(SvgPathVariantEnum.Line);
            Assert.AreEqual(result, 2);
        }

        [Test]
        public void IncrementCurrentPathIndex_ShouldReturn_ExpectedFromCurve()
        {
            var result = _svgService.IncrementCurrentPathIndex(SvgPathVariantEnum.Curve);
            Assert.AreEqual(result, 6);
        }

        [Test]
        public void IncrementCurrentPathIndex_ShouldReturn_ExpectedFromMoveTo()
        {
            var result = _svgService.IncrementCurrentPathIndex(SvgPathVariantEnum.MoveTo);
            Assert.AreEqual(result, 2);
        }

        #endregion

        #region  GetSvgPathVariantEnumFromString -------------------------------------

        [Test]
        public void GetSvgPathVariantEnumFromString_ShouldReturn_ExpectedStringL()
        {
            var result = _svgService.GetSvgPathVariantEnumFromString("L");
            Assert.AreEqual(result, SvgPathVariantEnum.Line);
        }

        [Test]
        public void GetSvgPathVariantEnumFromString_ShouldReturn_ExpectedStringC()
        {
            var result = _svgService.GetSvgPathVariantEnumFromString("C");
            Assert.AreEqual(result, SvgPathVariantEnum.Curve);
        }

        [Test]
        public void GetSvgPathVariantEnumFromString_Should_ThrowException()
        {
            try
            {
                var result = _svgService.GetSvgPathVariantEnumFromString("A");
                Assert.Fail();
            }
            catch
            {

            }
        }

        #endregion


        #region  GetCurrentPathVariant -------------------------------------

        [Test]
        public void GetCurrentPathVariant_ShouldReturn_ExpectedPathVariantEnum()
        {

            var result = _svgService.GetCurrentPathVariant(_listOfWords, 2);
            Assert.AreEqual(result, SvgPathVariantEnum.Curve);
        }

        [Test]
        public void GetCurrentPathVariant_ShouldNotReturn_CurveVariantEnum()
        {

            var result = _svgService.GetCurrentPathVariant(_listOfWords, 3); // this index contains no alphanbetic characters
            Assert.AreNotEqual(result, SvgPathVariantEnum.Curve);
        }


        [Test]
        public void GetCurrentPathVariant_ShouldNotReturn_UnsetEnum()
        {

            var result = _svgService.GetCurrentPathVariant(_listOfWords, 2);
            Assert.AreNotEqual(result, SvgPathVariantEnum.Unset);
        }

        #endregion

        #region  GetCurrentPathVariant -------------------------------------

        [Test]
        public void ExtractCoordinate_ShouldReturn_ExpectedCoordForLine()
        {
            //arrange
            // values derived from indexed positions in "_listWords"
            // { "M2219", "5983", "c-13", "-16", "-12", "-17", "4", "-4"

            int startingCoordX = 2219;
            int startingCoordY = 5983;
            var previousPoint = new Point(startingCoordX, startingCoordY);

            // where "-13" and "-16" are the coord pair, in a line description
            int expectedCoordX = startingCoordX + (-13);
            int expectedCoordY = startingCoordY + (-16);
            var expectedPoint = new Point(expectedCoordX, expectedCoordY);

            //act
            // n.b. starts at index postion 2 (first 2 are path starting, and not iterated upon)
            Point? result = _svgService.ExtractCoordinate(previousPoint, _listOfWords, 2, SvgPathVariantEnum.Line);

            //assert
            Assert.AreEqual(result.Value.X, expectedPoint.X);
            Assert.AreEqual(result.Value.Y, expectedPoint.Y);
        }


        [Test]
        public void ExtractCoordinate_ShouldReturn_ExpectedCoordForCurve()
        {
            //arrange
            // values derived from indexed positions in "_listWords"
            // { "M2219", "5983", "c-13", "-16", "-12", "-17", "4", "-4"

            int startingCoordX = 2219;
            int startingCoordY = 5983;
            var previousPoint = new Point(startingCoordX, startingCoordY);

            // where "4" and "-4" are the 3rd pair, in a curve description
            int expectedCoordX = startingCoordX + 4;
            int expectedCoordY = startingCoordY + (-4);
            var expectedPoint = new Point(expectedCoordX, expectedCoordY);

            //act
            // n.b. starts at index postion 2 (first 2 are path starting, and not iterated upon)
            Point? result = _svgService.ExtractCoordinate(previousPoint, _listOfWords, 2, SvgPathVariantEnum.Curve);

            //assert
            Assert.AreEqual(result.Value.X, expectedPoint.X);
            Assert.AreEqual(result.Value.Y, expectedPoint.Y);
        }

        #endregion


        #region  GetCurrentPathVariant -------------------------------------

        [Test]
        public void ExtractCoordinates_ShouldReturn_ExpectedList()
        {
            //arrange
            List<string> listPathNodesToTest = new List<string> { _singlePathNode };


            //act
            var result = _svgService.ExtractCoordinates(listPathNodesToTest);

            //assert

            var item = result[0].ListPoints[0].Y;

            // offset values are derived from each 3rd coordinate pair
            Assert.AreEqual(result[0].ListPoints[0].X, 2219);
            Assert.AreEqual(result[0].ListPoints[0].Y, 5983);
            Assert.AreEqual(result[0].ListPoints[1].X, 2219 + 4);
            Assert.AreEqual(result[0].ListPoints[1].Y, 5983 + (-4));
            Assert.AreEqual(result[0].ListPoints[2].X, 2219 + 4 + 17);
            Assert.AreEqual(result[0].ListPoints[2].Y, 5983 + (-4) + 17);

        }

        #endregion

    }
}
