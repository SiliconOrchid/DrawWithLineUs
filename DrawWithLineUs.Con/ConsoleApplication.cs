
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using DrawWithLineUs.Con.Config;
using DrawWithLineUs.Con.Model;
using DrawWithLineUs.Con.Service;

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
            GetCoordinatesFromSVG();
            ApplyGeometry();
            GenerateGCode();
            Draw();
        }


        private void GenerateGCode()
        {
            _listGCodes = _gCodeService.GenerateGCode(_listCoordinateStructures);
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

        private void GetCoordinatesFromSVG()
        {
            List<string> listPathNodes = _svgService.ExtractPaths(ProgramConfig.PathToSourceSVG);
            _listCoordinateStructures = _svgService.ExtractCoordinates(listPathNodes);
        }


        private void Draw()
        {
            if (ProgramConfig.TestMode)
            {
                foreach (var gcode in _listGCodes)
                {
                    Console.WriteLine(gcode);
                }
            }
            else
            {
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
