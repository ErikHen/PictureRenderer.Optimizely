using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Routing;
using PictureRenderer.Profiles;
using System;
using System.Text.RegularExpressions;

namespace PictureRenderer.Optimizely
{
    public static class XhtmlStringExtensions
    {
        /// <summary>
        /// Replaces img elements with a picture elements.
        /// </summary>
        public static XhtmlString RenderImageAsPicture(this XhtmlString xhtmlString, int maxWidth = 1024)
        {
            var ctxModeResolver = ServiceLocator.Current.GetInstance<EPiServer.Web.IContextModeResolver>();
            if (ctxModeResolver.CurrentMode == ContextMode.Edit)
            {
                return xhtmlString;
            }

            //todo: extend regex so that it doesn't match img element inside picture element (that would be a very rare edge case). https://www.regular-expressions.info/lookaround.html https://www.rexegg.com/regex-lookarounds.html
            var processedText = Regex.Replace(xhtmlString.ToInternalString(), "(<img.*?>)", m => GetPictureFromImg(m.Groups[1].Value, maxWidth));

            return new XhtmlString(processedText);
        }

        private static string GetPictureFromImg(string imgElement, int maxWidth)
        {
            var imgData = GetValuesFromImg(imgElement);

            int actualWidth;
            if (imgData.PercentageWidth > 0)
            {
                actualWidth = (int)Math.Round(maxWidth * imgData.PercentageWidth / 10, 0);
            }
            else
            {
                actualWidth = imgData.Width > maxWidth || imgData.Width == 0 ? maxWidth : imgData.Width;
            }
            var aspectRatio = imgData.Width > 0 && imgData.Height > 0 ? Math.Round((double)imgData.Width / imgData.Height, 3) : default;

            var tinyMcePictureProfile = new ImageSharpProfile()
            {
                SrcSetWidths = new[] { actualWidth },
                Sizes = new[] { $"{actualWidth}px" },
                AspectRatio = aspectRatio,
            };

            var imgUrl = UrlResolver.Current.GetUrl(imgData.Src);

            return Picture.Render(imgUrl, tinyMcePictureProfile, imgData.Alt, imgData.CssClass);
        }

        private static ImgData GetValuesFromImg(string imgElement)
        {
            var widthValue = Regex.Match(imgElement, "width=\"(.*?)\"").Groups[1].Value;
            var heightValue = Regex.Match(imgElement, "height=\"(.*?)\"").Groups[1].Value;
            _ = int.TryParse(widthValue, out var width);
            _ = int.TryParse(heightValue, out var height);
            double percentageWidth = default;
            if (widthValue.EndsWith('%'))
            {
                _ = double.TryParse(widthValue.TrimEnd('%'), out percentageWidth);
            }

            var imgData = new ImgData
            {
                Src = Regex.Match(imgElement, "src=\"(.*?)\"").Groups[1].Value,
                Alt = Regex.Match(imgElement, "alt=\"(.*?)\"").Groups[1].Value,
                CssClass = Regex.Match(imgElement, "class=\"(.*?)\"").Groups[1].Value,
                PercentageWidth = percentageWidth,
                Width = width,
                Height = height,
            };

            return imgData;
        }

        private struct ImgData
        {
            public string Src { get; init; }
            public string Alt { get; init; }
            public string CssClass { get; init; }
            public double PercentageWidth { get; init; }
            //public string PercentageHeight { get; set; }
            public int Width { get; init; }
            public int Height { get; init; }
        }
    }
}
