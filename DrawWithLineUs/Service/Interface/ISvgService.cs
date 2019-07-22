using System.Collections.Generic;
using System.Drawing;
using DrawWithLineUs.Enum;
using DrawWithLineUs.Model;

namespace DrawWithLineUs.Service
{
    public interface ISvgService
    {
        List<CoordinateStructure> ExtractCoordinates(List<string> listPathNodes);

        List<string> ExtractPaths(string PathToSourceSVG);

        Point? ExtractCoordinate(Point previousPoint, string[] listWords, int i, SvgPathVariantEnum currentSvgPathVariantEnum);

        SvgPathVariantEnum GetCurrentPathVariant(string[] listWords, int i);

        int IncrementCurrentPathIndex(SvgPathVariantEnum currentSvgPathVariantEnum);

        SvgPathVariantEnum GetSvgPathVariantEnumFromString(string variantCharacter);

    }
}