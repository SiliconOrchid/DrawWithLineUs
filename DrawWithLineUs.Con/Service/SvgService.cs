
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Xml;

using DrawWithLineUs.Con.Model;

namespace DrawWithLineUs.Con.Service
{
    public static class SvgService
    {

        public static List<string> ExtractPaths(string PathToSourceSVG)
        {
            List<string> listPathNodes = new List<string>();

            // used because of https://stackoverflow.com/questions/13854068/dtd-prohibited-in-xml-document-exception
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ProhibitDtd = false;

            //Console.WriteLine($"Reading XML(SVG)...");
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


                // ###   the following routine for extracting paths from the SVG is borked. ###
                // ###   it currently assumes a fixed pattern, which is not the case        ###
                // ###   https://developer.mozilla.org/en-US/docs/Web/SVG/Tutorial/Paths    ###

                //TODO:   Write an extractor that works!

                // get offset coordinates
                // don't iterate word-by-word, increment by 6 (the "c" instruction are an X/Y set of two beziers, with the 3rd pair being the end coordinate
                // start loop at index 2, as the first pairs will be the base coordinates
                for (int i = 2; i < listWords.Length - 6; i = i + 6)
                {
                    string bezierControl1X = Regex.Replace(listWords[i], "[^0-9.+-]", "");
                    string bezierControl1Y = Regex.Replace(listWords[i + 1], "[^0-9.+-]", "");
                    string bezierControl2X = Regex.Replace(listWords[i + 2], "[^0-9.+-]", "");
                    string bezierControl2Y = Regex.Replace(listWords[i + 3], "[^0-9.+-]", "");
                    string coordX = Regex.Replace(listWords[i + 4], "[^0-9.+-]", "");
                    string coordY = Regex.Replace(listWords[i + 5], "[^0-9.+-]", "");

                    int offsetX = int.Parse(coordX);
                    int offsetY = int.Parse(coordY);

                    //coordinateStructure.ListPoints.Add(new Point(coordBaseX + offsetX, coordBaseY + offsetY));
                    Point previousPoint = coordinateStructure.ListPoints[coordinateStructure.ListPoints.Count - 1];
                    coordinateStructure.ListPoints.Add(new Point(previousPoint.X + offsetX, previousPoint.Y + offsetY));
                }

                listCoordinateStructures.Add(coordinateStructure);
            }

            return listCoordinateStructures;
        }
    }
}
