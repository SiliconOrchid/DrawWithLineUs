using System.Collections.Generic;
using DrawWithLineUs.Con.Model;

namespace DrawWithLineUs.Con.Service
{
    public interface IGeometryService
    {
        decimal DetermineScalingRatio(BoundingBox sourceBoundingBox);
        BoundingBox DetermineSourceBounds(List<CoordinateStructure> listCoordinateStructures);
        void RescaleAndOffset(List<CoordinateStructure> listCoordinateStructures, decimal scalingRatio, int offsetX, int offsetY);
    }
}