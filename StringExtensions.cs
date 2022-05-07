using System.Globalization;

namespace PictureRenderer.Optimizely
{
    public static class StringExtensions
    {
        public static (double x, double y) ParseFocalPoint(this string focalPointString)
        {
            if (focalPointString == default)
                return default;
            
            (double x, double y) focalPoint = default;
            var focalValues = focalPointString.Split('|');

            if (focalValues.Length != 2)
                return focalPoint;

            if (!(double.TryParse(focalValues[0], NumberStyles.Any, CultureInfo.InvariantCulture, out focalPoint.x) &&
                  double.TryParse(focalValues[1], NumberStyles.Any, CultureInfo.InvariantCulture, out focalPoint.y)))
                focalPoint = default;

            return focalPoint;
        }
    }
}
