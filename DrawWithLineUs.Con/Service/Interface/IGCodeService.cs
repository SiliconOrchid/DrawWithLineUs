using System.Collections.Generic;
using DrawWithLineUs.Con.Model;

namespace DrawWithLineUs.Con.Service
{
    public interface IGCodeService
    {
        List<string> GenerateGCode(List<CoordinateStructure> listCoordinateStructures);
    }
}