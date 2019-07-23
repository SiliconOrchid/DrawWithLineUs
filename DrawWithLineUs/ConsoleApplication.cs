using System;
using System.Collections.Generic;
using System.Net.Sockets;

using DrawWithLineUs.Config;
using DrawWithLineUs.Model;
using DrawWithLineUs.Service;

namespace DrawWithLineUs.Con
{
    public class ConsoleApplication
    {
        private readonly ISvgService _svgService;
        private readonly IGeometryService _geometryService;
        private readonly IGCodeService _gCodeService;
        private readonly ICommunicationService _communicationService;

        private static List<CoordinateStructure> _listCoordinateStructures;
        private static List<string> _listGCodes;


        public ConsoleApplication(
            ISvgService svgService, 
            IGeometryService geometryService,
            IGCodeService gCodeService,
            ICommunicationService communicationService
            )
        {
            _svgService = svgService;
            _geometryService = geometryService;
            _gCodeService = gCodeService;
            _communicationService = communicationService;
        }

        // Application starting point
        public void Run()
        {
            GetCoordinatesFromSVG(); // extracts a list of coordinates from the source file
            ApplyGeometry(); // rescales the coordinates, to fit within the drawable area
            GenerateGCode(); // produces a list of G-Code from the list of coordinates
            Draw(); // Sends the G-Code to the Line-us
        }


        private void GetCoordinatesFromSVG()
        {
            List<string> listPathNodes = _svgService.ExtractPaths(ProgramConfig.PathToSourceSVG);
            _listCoordinateStructures = _svgService.ExtractCoordinates(listPathNodes);
        }


 

        private void ApplyGeometry()
        {
            BoundingBox sourceBoundingBox = _geometryService.DetermineSourceBounds(_listCoordinateStructures);

            decimal scalingRatio = _geometryService.DetermineScalingRatio(sourceBoundingBox);

            //TODO: calculate appropriate offset automatically (so as to center image in drawable area)
            int offsetX = 800;
            int offsetY = -500;

            _geometryService.RescaleAndOffset(_listCoordinateStructures, scalingRatio, offsetX, offsetY);
        }



        private void GenerateGCode()
        {
            _listGCodes = _gCodeService.GenerateGCode(_listCoordinateStructures);
        }



        private void Draw()
        {
            Console.WriteLine("++++++++++++");

            if (ProgramConfig.TestMode)
            {
                Console.WriteLine("Running in test mode - G-Code will be generated but not sent to Line-us.   You can change mode by editing [TestMode] in [ProgramConfig.cs]");
                Console.WriteLine("++++++++++++");


                foreach (var gcode in _listGCodes)
                {
                    Console.WriteLine(gcode);
                }
            }
            else
            {
                Console.WriteLine("Not running in test mode - G-Code will be transmitted to Line-us.   You can change mode by editing [TestMode] in [ProgramConfig.cs]");
                Console.WriteLine("If the Line-us is not drawing, check the IP address is set correctly in [ProgramConfig.cs]");
                Console.WriteLine("++++++++++++");


                TcpClient client;
                NetworkStream stream;
                _communicationService.ConnectToLineUs(out client, out stream, ProgramConfig.LineUsIP, ProgramConfig.LineUsport);

                foreach (var gcode in _listGCodes)
                {
                    _communicationService.Transmit(stream, gcode);
                }

                client.Close();
            }
        }
    }
}
