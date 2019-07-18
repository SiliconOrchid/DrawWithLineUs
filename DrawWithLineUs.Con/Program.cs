using System;
using System.Collections.Generic;
using System.Net.Sockets;
using DrawWithLineUs.Con.Config;
using DrawWithLineUs.Con.Model;
using DrawWithLineUs.Con.Service;
using Microsoft.Extensions.DependencyInjection;

namespace DrawWithLineUs.Con
{
    class Program
    {
        private static IServiceProvider _serviceProvider;

        private static ISvgService _svgService;
        private static IGeometryService _geometryService;
        private static IGCodeService _gCodeService;
        private static ICommunicationService _communicationService;

        private static List<CoordinateStructure> _listCoordinateStructures;
        private static List<string> _listGCodes;





        static void Main(string[] args)
        {
            RegisterServices();
            SetServices();

            GetCoordinatesFromSVG();
            ApplyGeometry();
            GenerateGCode();
            Draw();

            DisposeServices();
        }

        private static void Draw()
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

        private static void GenerateGCode()
        {
            _listGCodes = _gCodeService.GenerateGCode(_listCoordinateStructures);
        }

        private static void ApplyGeometry()
        {
            BoundingBox sourceBoundingBox = _geometryService.DetermineSourceBounds(_listCoordinateStructures);

            decimal scalingRatio = _geometryService.DetermineScalingRatio(sourceBoundingBox);

            //TODO: calculate appropriate offset automatically (so as to center image in drawable area)
            int offsetX = 800;
            int offsetY = -500;

            _geometryService.RescaleAndOffset(_listCoordinateStructures, scalingRatio, offsetX, offsetY);
        }

        private static void GetCoordinatesFromSVG()
        {
            List<string> listPathNodes = _svgService.ExtractPaths(ProgramConfig.PathToSourceSVG);
            _listCoordinateStructures = _svgService.ExtractCoordinates(listPathNodes);
        }


        private static void RegisterServices()
        {
            var collection = new ServiceCollection();
            collection.AddScoped<ICommunicationService, CommunicationService>();
            collection.AddScoped<IGCodeService, GCodeService>();
            collection.AddScoped<IGeometryService, GeometryService>();
            collection.AddScoped<ISvgService, SvgService>();
            _serviceProvider = collection.BuildServiceProvider();
        }

        private static void SetServices()
        {
            _svgService = _serviceProvider.GetService<ISvgService>();
            _geometryService = _serviceProvider.GetService<IGeometryService>();
            _gCodeService = _serviceProvider.GetService<IGCodeService>();
            _communicationService = _serviceProvider.GetService<ICommunicationService>();
        }

        private static void DisposeServices()
        {
            if (_serviceProvider == null)
            {
                return;
            }
            if (_serviceProvider is IDisposable)
            {
                ((IDisposable)_serviceProvider).Dispose();
            }
        }
    }
}
