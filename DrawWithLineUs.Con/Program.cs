using System;
using System.Collections.Generic;
using System.Net.Sockets;

using DrawWithLineUs.Con.Model;
using DrawWithLineUs.Con.Service;
using Microsoft.Extensions.DependencyInjection;

namespace DrawWithLineUs.Con
{
    class Program
    {
        private static IServiceProvider _serviceProvider;


        const bool testMode = true;
        const string pathToSourceSVG = @"C:\Users\jimmc\Source\Repos\DrawWithLineUs\Resources\parrot.svg";
        const string lineusIP = "192.168.1.212";
        const int lineusport = 1337;


        static void Main(string[] args)
        {
            RegisterServices();

            var svgService = _serviceProvider.GetService<ISvgService>();
            var geometryService = _serviceProvider.GetService<IGeometryService>();
            var gCodeService = _serviceProvider.GetService<IGCodeService>();
            var communicationService = _serviceProvider.GetService<ICommunicationService>();


            List<string> listPathNodes = svgService.ExtractPaths(pathToSourceSVG);

            List<CoordinateStructure> listCoordinateStructures = svgService.ExtractCoordinates(listPathNodes);

            BoundingBox sourceBoundingBox = geometryService.DetermineSourceBounds(listCoordinateStructures);

            decimal scalingRatio = geometryService.DetermineScalingRatio(sourceBoundingBox);

            //TODO: calculate appropriate offset automatically (so as to center image in drawable area)
            int offsetX = 800;
            int offsetY = -500;

            geometryService.RescaleAndOffset(listCoordinateStructures, scalingRatio, offsetX, offsetY);

            List<string> listGCodes = gCodeService.GenerateGCode(listCoordinateStructures);

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
                communicationService.ConnectToLineUs(out client, out stream, lineusIP, lineusport);

                foreach (var gcode in listGCodes)
                {
                    communicationService.Transmit(stream, gcode);
                }

                client.Close();
            }


            DisposeServices();
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
