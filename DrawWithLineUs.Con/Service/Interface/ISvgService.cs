using System.Collections.Generic;
using DrawWithLineUs.Con.Model;

namespace DrawWithLineUs.Con.Service
{
    public interface ISvgService
    {
        List<CoordinateStructure> ExtractCoordinates(List<string> listPathNodes);
        List<string> ExtractPaths(string PathToSourceSVG);
    }
}