using System.Diagnostics.CodeAnalysis;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PictureRenderer.Optimizely
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public static class PictureHelper
    {
        public static HtmlString Picture(this IHtmlHelper helper, ContentReference imageReference, PictureProfile profile, LazyLoading lazyLoading)
        {
            return Picture(helper, imageReference, profile, string.Empty, lazyLoading);
        }

        [SuppressMessage("ReSharper", "UnusedParameter.Global")]
        public static HtmlString Picture(this IHtmlHelper helper, ContentReference imageReference, PictureProfile profile, string altText = "", LazyLoading lazyLoading = LazyLoading.Browser)
        {
            if (imageReference == null)
            {
                return new HtmlString(string.Empty);
            }

            (double x, double y) focalPoint = default;
            
            if (profile.GetDataFromImage)
            {
                var image = ServiceLocator.Current.GetInstance<IContentLoader>().Get<IContent>(imageReference);
                if (string.IsNullOrEmpty(altText) && image?.Property["AltText"]?.Value != null)
                {
                    altText = image.Property["AltText"].ToString();
                }
                
                if (image?.Property["ImageFocalPoint"]?.Value != null)
                {
                    var focalPointString = image.Property["ImageFocalPoint"].ToString();
                    
                    focalPoint = focalPointString.ParseFocalPoint();
                }
            }

            var imageUrl = new UrlBuilder(ServiceLocator.Current.GetInstance<UrlResolver>().GetUrl(imageReference));

            return new HtmlString(PictureRenderer.Picture.Render(imageUrl.ToString(), profile, altText, lazyLoading, focalPoint));
        }
    }
}
