using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Routing;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using PictureRenderer.Profiles;

namespace PictureRenderer.Optimizely
{
    public static class PictureHelper
    {
        public static HtmlString Picture(this IHtmlHelper helper, ContentReference imageReference, PictureProfileBase profile, string altText = "", LazyLoading lazyLoading = LazyLoading.Browser, string cssClass = "")
        {
            return Picture(helper, imageReference, profile, new PictureAttributes { ImgAlt = altText, LazyLoading = lazyLoading, ImgClass = cssClass });
        }

        public static HtmlString Picture(this IHtmlHelper helper, ContentReference imageReference, PictureProfileBase profile, PictureAttributes attributes)
        {
            if (imageReference == null)
            {
                return new HtmlString(string.Empty);
            }


            var imageUrl = UrlResolver.Current.GetUrl(imageReference, null, new VirtualPathArguments { ContextMode = ContextMode.Default });
            var image = ServiceLocator.Current.GetInstance<IContentLoader>().Get<IContent>(imageReference);

            if (string.IsNullOrEmpty(attributes.ImgAlt) && image?.Property["AltText"]?.Value != null)
            {
                attributes.ImgAlt = image.Property["AltText"].ToString();
            }

            (double x, double y) focalPoint = default;
            if (image?.Property["ImageFocalPoint"]?.Value != null)
            {
                var focalPointString = image.Property["ImageFocalPoint"].ToString();
                focalPoint = focalPointString.ToImageFocalPoint();
            }

            return new HtmlString(PictureRenderer.Picture.Render(imageUrl, profile, attributes, focalPoint));
        }
    }
}
