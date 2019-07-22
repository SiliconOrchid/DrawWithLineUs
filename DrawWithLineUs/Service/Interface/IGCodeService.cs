using System.Collections.Generic;
using DrawWithLineUs.Model;

namespace DrawWithLineUs.Service
{
    public interface IGCodeService
    {
        List<string> GenerateGCode(List<CoordinateStructure> listCoordinateStructures);
    }
}