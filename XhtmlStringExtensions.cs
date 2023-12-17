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
        [Obsolete("Use ReplaceImgWithPicture instead.")]
        public static XhtmlString RenderImageAsPicture(this XhtmlString xhtmlString, RichTextPictureProfile profile = null)
        {
            if (profile == null)
            {
                profile = new RichTextPictureProfile();
            }
            return xhtmlString.ReplaceImgWithPicture(profile);
        }

        /// <summary>
        /// Replaces img elements with a picture elements.
        /// </summary>
        public static XhtmlString ReplaceImgWithPicture(this XhtmlString xhtmlString, RteProfileBase profile)
        {
            var ctxModeResolver = ServiceLocator.Current.GetInstance<EPiServer.Web.IContextModeResolver>();
            if (ctxModeResolver.CurrentMode == ContextMode.Edit)
            {
                return xhtmlString;
            }

            //todo: extend regex so that it doesn't match img element inside picture element (that would be a very rare edge case). https://www.regular-expressions.info/lookaround.html https://www.rexegg.com/regex-lookarounds.html
            var processedText = Regex.Replace(xhtmlString.ToInternalString(), "(<img.*?>)", m => GetPictureFromImg(m.Groups[1].Value, profile));

            return new XhtmlString(processedText);
        }

        private static string GetPictureFromImg(string imgElement, RteProfileBase richTextProfile)
        {
            var imgValues = GetValuesFromImg(imgElement);
            var calculatedWith = GetImageWidth(imgValues, richTextProfile.MaxImageWidth);

            PictureProfileBase tinyMcePictureProfile = null;
            if (richTextProfile is CloudflareRteProfile)
            {
                tinyMcePictureProfile = new CloudflareProfile()
                {
                    SrcSetWidths = new[] { calculatedWith },
                    Sizes = new[] { $"{calculatedWith}px" },
                    AspectRatio = CalculateAspectRatio(imgValues),
                    Quality = richTextProfile.Quality,
                    ImgWidthHeight = true
                };
            }
            if (richTextProfile is ImageSharpRteProfile imageSharpRteProfile)
            {
                tinyMcePictureProfile = new ImageSharpProfile
                {
                    SrcSetWidths = new[] { calculatedWith },
                    Sizes = new[] { $"{calculatedWith}px" },
                    AspectRatio = CalculateAspectRatio(imgValues),
                    CreateWebpForFormat = imageSharpRteProfile.CreateWebpForFormat,
                    Quality = richTextProfile.Quality,
                    ImgWidthHeight = true
                };
            }

            var imgUrl = UrlResolver.Current.GetUrl(imgValues.Src);
            var imgPercentageWidth = imgValues.PercentageWidth > 0 ? imgValues.PercentageWidth + "%" : string.Empty;

            return Picture.Render(imgUrl, tinyMcePictureProfile, imgValues.Alt, LazyLoading.Browser, default, imgValues.CssClass, imgPercentageWidth, style: imgValues.Style);
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
                Style = Regex.Match(imgElement, "style=\"(.*?)\"").Groups[1].Value,
                PercentageWidth = percentageWidth,
                Width = width,
                Height = height,
            };

            return imgData;
        }

        private static int GetImageWidth(ImgData imgValues, int maxWidth)
        {
            if (imgValues.PercentageWidth > 0)
            {
                return (int)Math.Round(maxWidth * imgValues.PercentageWidth / 100, 0);
            }

            return imgValues.Width > maxWidth || imgValues.Width == 0 ? maxWidth : imgValues.Width;
        }

        private static double CalculateAspectRatio(ImgData imgValues)
        {
            return imgValues.Width > 0 && imgValues.Height > 0 ? Math.Round((double)imgValues.Width / imgValues.Height, 3) : default;
        }

        private struct ImgData
        {
            public string Src { get; init; }
            public string Alt { get; init; }
            public string CssClass { get; init; }
            public double PercentageWidth { get; init; }
            public int Width { get; init; }
            public int Height { get; init; }
            public string Style { get; init; }
        }
    }
}
