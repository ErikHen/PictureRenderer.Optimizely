using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
        public static HtmlString Picture(this IHtmlHelper helper, ContentReference imageReference, PictureProfileBase profile, LazyLoading lazyLoading, string cssClass = "")
        {
            return Picture(helper, imageReference, profile, string.Empty, lazyLoading, cssClass);
        }

        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Public API")]
        public static HtmlString Picture(this IHtmlHelper helper, ContentReference imageReference, PictureProfileBase profile, string altText = "", LazyLoading lazyLoading = LazyLoading.Browser, string cssClass = "")
        {
            if (imageReference == null)
            {
                return new HtmlString(string.Empty);
            }

            var imageUrl = UrlResolver.Current.GetUrl(imageReference, null, new VirtualPathArguments {ContextMode = ContextMode.Default});

            (double x, double y) focalPoint = default;
            var image = ServiceLocator.Current.GetInstance<IContentLoader>().Get<IContent>(imageReference);
            if (string.IsNullOrEmpty(altText) && image?.Property["AltText"]?.Value != null)
            {
                altText = image.Property["AltText"].ToString();
            }
            
            if (image?.Property["ImageFocalPoint"]?.Value != null)
            {
                var focalPointString = image.Property["ImageFocalPoint"].ToString();
                focalPoint = focalPointString.ToImageFocalPoint();
            }

            return new HtmlString(PictureRenderer.Picture.Render(imageUrl.ToString(), profile, altText, lazyLoading, focalPoint, cssClass));
        }


        public static HtmlString Picture(this IHtmlHelper helper, ContentReference[] imageReference, PictureProfileBase profile, LazyLoading lazyLoading, string cssClass = "")
        {
            return Picture(helper, imageReference, profile, string.Empty, lazyLoading, cssClass);
        }

        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Public API")]
        public static HtmlString Picture(this IHtmlHelper helper, ContentReference[] imageReferences, PictureProfileBase profile, string altText = "", LazyLoading lazyLoading = LazyLoading.Browser, string cssClass = "")
        {
            if (imageReferences == null || imageReferences.All(ir => ir == null))
            {
                return new HtmlString(string.Empty);
            }

            var imageUrls = new List<string>();
            var focalPoints = new List<(double x, double y)>();
            foreach (var imageRef in imageReferences.Where(ir => ir != null))
            {
                var imageUrl = UrlResolver.Current.GetUrl(imageRef, null, new VirtualPathArguments { ContextMode = ContextMode.Default });
                imageUrls.Add(imageUrl);

                var image = ServiceLocator.Current.GetInstance<IContentLoader>().Get<IContent>(imageRef);

                //use the alt text from the first image that has any alt text defined.
                if (string.IsNullOrEmpty(altText) && image?.Property["AltText"]?.Value != null)
                {
                    altText = image.Property["AltText"].ToString();
                }

                if (image?.Property["ImageFocalPoint"]?.Value != null)
                {
                    var focalPointString = image.Property["ImageFocalPoint"].ToString();
                    focalPoints.Add(focalPointString.ToImageFocalPoint());
                }
                else
                {
                    //add empty focal point for current image
                    focalPoints.Add(default);
                }
            }

            return new HtmlString(PictureRenderer.Picture.Render(imageUrls.ToArray(), profile, altText, lazyLoading, focalPoints.ToArray(), cssClass));
        }
    }
}
