using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using PictureRenderer.Profiles;
using PictureRenderer;

namespace PictureRenderer.Optimizely
{
    public static class PictureHelper
    {
        public static HtmlString Picture(this IHtmlHelper helper, ContentReference imageReference, PictureProfile profile, LazyLoading lazyLoading)
        {
            return Picture(helper, imageReference, profile, string.Empty, lazyLoading);
        }

        public static HtmlString Picture(this IHtmlHelper helper, ContentReference imageReference, PictureProfile profile, string altText = "", LazyLoading lazyLoading = LazyLoading.Browser)
        {
            if (imageReference == null)
            {
                return new HtmlString(string.Empty);
            }

            if (profile.GetDataFromImage)
            {
                var image = ServiceLocator.Current.GetInstance<IContentLoader>().Get<IContent>(imageReference);
                if (string.IsNullOrEmpty(altText) && image?.Property["AltText"]?.Value != null)
                {
                    altText = image.Property["AltText"].ToString();
                }
                //TODO: focal point   (double x, double y) focalPoint = default;
            }


            var imageUrl = new UrlBuilder(ServiceLocator.Current.GetInstance<UrlResolver>().GetUrl(imageReference));

            return new HtmlString(PictureRenderer.Picture.Render(imageUrl.ToString(), profile, altText, lazyLoading));
        }
    }
}
