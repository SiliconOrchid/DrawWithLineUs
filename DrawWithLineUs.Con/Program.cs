using System;
using System.Collections.Generic;
using System.Net.Sockets;

using DrawWithLineUs.Con.Model;
using DrawWithLineUs.Con.Service;

namespace DrawWithLineUs.Con
{
    class Program
    {

        const bool testMode = false;
        const string pathToSourceSVG = @"C:\Users\jimmc\Source\Repos\DrawWithLineUs\Resources\parrot.svg";
        const string lineusIP = "192.168.1.212";
        const int lineusport = 1337;



        static void Main(string[] args)
        {

            List<string> listPathNodes = SvgService.ExtractPaths(pathToSourceSVG);

            List<CoordinateStructure> listCoordinateStructures = SvgService.ExtractCoordinates(listPathNodes);

            BoundingBox sourceBoundingBox = GeometryService.DetermineSourceBounds(listCoordinateStructures);

            decimal scalingRatio = GeometryService.DetermineScalingRatio(sourceBoundingBox);

            //TODO: calculate appropriate offset automatically (so as to center image in drawable area)
            int offsetX = 800;
            int offsetY = -500;

            GeometryService.RescaleAndOffset(listCoordinateStructures, scalingRatio, offsetX, offsetY);

            List<string> listGCodes = GCodeService.GenerateGCode(listCoordinateStructures);

            if (testMode)
            {
                foreach (var gcode in listGCodes)
                {
                    Console.WriteLine(gcode);
                }
            }
            else
            {
                TcpClient client;
                NetworkStream stream;
                CommunicationService.ConnectToLineUs(out client, out stream, lineusIP, lineusport);

                foreach (var gcode in listGCodes)
                {
                    CommunicationService.Transmit(stream, gcode);
                }

                client.Close();
            }

        }
    }
}
