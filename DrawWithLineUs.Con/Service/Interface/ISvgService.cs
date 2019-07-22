using System.Collections.Generic;
using DrawWithLineUs.Model;

namespace DrawWithLineUs.Service
{
    public interface ISvgService
    {
        List<CoordinateStructure> ExtractCoordinates(List<string> listPathNodes);
        List<string> ExtractPaths(string PathToSourceSVG);
    }
}