
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Xml;
using DrawWithLineUs.Con.Enum;
using DrawWithLineUs.Con.Model;

namespace DrawWithLineUs.Con.Service
{
    public static class SvgService
    {

        // ### useful guide to SVG path syntax  https://developer.mozilla.org/en-US/docs/Web/SVG/Tutorial/Paths    ###


        public static List<string> ExtractPaths(string PathToSourceSVG)
        {
            List<string> listPathNodes = new List<string>();

            // DTD setting (used because of this issue: https://stackoverflow.com/questions/13854068/dtd-prohibited-in-xml-document-exception)
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ProhibitDtd = false;

            Console.WriteLine($"Reading XML(SVG)...");
            using (XmlReader reader = XmlReader.Create($"{PathToSourceSVG}", settings))
            {
                while (reader.Read())
                {
                    switch (reader.Name.ToString())
                    {
                        case "path":
                            listPathNodes.Add(reader.GetAttribute("d"));
                            break;
                    }
                }
            }

            return listPathNodes;
        }



        public static List<CoordinateStructure> ExtractCoordinates(List<string> listPathNodes)
        {
            // an array of arrays (which contain point coordinates, which are constructed from the list of offsets in the SVG
            List<CoordinateStructure> listCoordinateStructures = new List<CoordinateStructure>();

            Console.WriteLine($"Parsing nodes...");
            foreach (var pathNode in listPathNodes)
            {
                var coordinateStructure = new CoordinateStructure();

                var listWords = pathNode.Split(" ");

                // extract start coordinates
                int coordStartX = int.Parse(Regex.Replace(listWords[0], "[^0-9.+-]", ""));
                int coordStartY = int.Parse(Regex.Replace(listWords[1], "[^0-9.+-]", ""));
                coordinateStructure.ListPoints.Add(new Point(coordStartX, coordStartY));





                int i = 2; // don't start at very beginning, skip over the first pair of values as these are always the "starting position"
                SvgPathVariantEnum currentSvgPathVariantEnum = SvgPathVariantEnum.Unset;

                while (i < listWords.Length)
                {
                    //check to see if the "variant" has changed (is this a line or curve, or are we carrying on from the last previously set variant?)
                    currentSvgPathVariantEnum = SetCurrentPathVariant(listWords, i, currentSvgPathVariantEnum);

                    // depending on the "variant" extract the appropriate coordinates and add this to "coordinateStructure.ListPoints" collection. 
                    ExtractCoordinatesAddToList(coordinateStructure, listWords, i, currentSvgPathVariantEnum);


                    //depending on the current variant, increment the current index appropriately (either by 2 for a line, or 6 for a curve)
                    i = IncrementCurrentPathIndex(i, currentSvgPathVariantEnum);

                }

                listCoordinateStructures.Add(coordinateStructure);
            }

            return listCoordinateStructures;
        }

        private static void ExtractCoordinatesAddToList(CoordinateStructure coordinateStructure, string[] listWords, int i, SvgPathVariantEnum currentSvgPathVariantEnum)
        {
            string coordX;
            string coordY;
            switch (currentSvgPathVariantEnum)
            {
                case SvgPathVariantEnum.Line:
                    coordX = Regex.Replace(listWords[i], "[^0-9.+-]", "");
                    coordY = Regex.Replace(listWords[i + 1], "[^0-9.+-]", "");
                    break;
                case SvgPathVariantEnum.Curve:
                    // n.b. in an SVG curve, there are 3 pairs of coordinates (the first two are bezier control points), where the last pair is the coordinates where the curve ends (we want these coordinates!)
                    coordX = Regex.Replace(listWords[i + 4], "[^0-9.+-]", "");
                    coordY = Regex.Replace(listWords[i + 5], "[^0-9.+-]", "");
                    break;
                case SvgPathVariantEnum.MoveTo:
                    // we simply don't need to deal with a move-to command in this code (because we're not drawing proper curves)
                    return;
                    
                default:
                    throw new Exception("Encountered 'SvgPathVariantEnum.Unset'");
            }

            int offsetX = int.Parse(coordX);
            int offsetY = int.Parse(coordY);

            //coordinateStructure.ListPoints.Add(new Point(coordBaseX + offsetX, coordBaseY + offsetY));
            Point previousPoint = coordinateStructure.ListPoints[coordinateStructure.ListPoints.Count - 1];
            coordinateStructure.ListPoints.Add(new Point(previousPoint.X + offsetX, previousPoint.Y + offsetY));
        }

        private static SvgPathVariantEnum SetCurrentPathVariant(string[] listWords, int i, SvgPathVariantEnum currentSvgPathVariantEnum)
        {
            //check word for a path-variant (either "l" for line, or "c" for curve) that indicates a change in following data...
            string currentPathVariantChar = Regex.Replace(listWords[i], "[0-9.+-]", "");
            if (!String.IsNullOrWhiteSpace(currentPathVariantChar))  //only update the current "currentSvgPathVariantEnum" if we have encountered a new letter, otherwise keep going with the existing one
            {
                currentSvgPathVariantEnum = GetSvgPathVariantEnumFromString(currentPathVariantChar);
            }
            return currentSvgPathVariantEnum;
        }

        private static int IncrementCurrentPathIndex(int i, SvgPathVariantEnum currentSvgPathVariantEnum)
        {
            // in an SVG path, this code accounts for two "variants".
            // a "line variant" has coordinate values that come in just pairs, therefore we increment the index by 2
            // a "curve variant" has coordinate values that comprise of 3 pairs.  The first two pairs are "bezier control points", whilst the last is the coordinate of wherethe curve ends.  So we increment by 6.

            // push the "index" forward, depending on the current variant
            switch (currentSvgPathVariantEnum)
            {
                case SvgPathVariantEnum.Line:
                    i = i + 2;
                    break;
                case SvgPathVariantEnum.Curve:
                    i = i + 6;
                    break;
                case SvgPathVariantEnum.MoveTo:
                    i = i + 2;
                    break;

                default:
                    throw new Exception("Encountered 'SvgPathVariantEnum.Unset'");
            }
            return i;
        }
        /// <summary>
        /// Return an appropriate [SvgPathVariantEnum] from a character.
        /// </summary>
        /// <param name="variantCharacter"></param>
        /// <returns></returns>
        private static SvgPathVariantEnum GetSvgPathVariantEnumFromString(string variantCharacter)
        {
            switch (variantCharacter.ToLower())
            {
                case "l":
                    return SvgPathVariantEnum.Line;
                case "c":
                    return SvgPathVariantEnum.Curve;
                case "m":
                    return SvgPathVariantEnum.MoveTo;
            }

            return SvgPathVariantEnum.Unset;
        }
    }
}
