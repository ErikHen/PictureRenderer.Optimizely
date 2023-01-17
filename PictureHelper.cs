using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PictureRenderer.Optimizely
{
    public static class PictureHelper
    {
        public static HtmlString Picture(this IHtmlHelper helper, ContentReference imageReference, PictureProfile profile, LazyLoading lazyLoading, string cssClass = "")
        {
            return Picture(helper, imageReference, profile, string.Empty, lazyLoading, cssClass);
        }

        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Public API")]
        public static HtmlString Picture(this IHtmlHelper helper, ContentReference imageReference, PictureProfile profile, string altText = "", LazyLoading lazyLoading = LazyLoading.Browser, string cssClass = "")
        {
            if (imageReference == null)
            {
                return new HtmlString(string.Empty);
            }

            var imageUrl = new UrlBuilder(ServiceLocator.Current.GetInstance<UrlResolver>().GetUrl(imageReference));
           
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
                    
                    focalPoint = focalPointString.ToImageFocalPoint();
                }
            }

            return new HtmlString(PictureRenderer.Picture.Render(imageUrl.ToString(), profile, altText, lazyLoading, focalPoint, cssClass));
        }


        public static HtmlString Picture(this IHtmlHelper helper, ContentReference[] imageReference, PictureProfile profile, LazyLoading lazyLoading, string cssClass = "")
        {
            return Picture(helper, imageReference, profile, string.Empty, lazyLoading, cssClass);
        }

        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Public API")]
        public static HtmlString Picture(this IHtmlHelper helper, ContentReference[] imageReferences, PictureProfile profile, string altText = "", LazyLoading lazyLoading = LazyLoading.Browser, string cssClass = "")
        {
            if (imageReferences == null || imageReferences.All(ir => ir == null))
            {
                return new HtmlString(string.Empty);
            }

            var imageUrls = new List<string>();
            var focalPoints = new List<(double x, double y)>();
            foreach (var imageRef in imageReferences.Where(ir => ir != null))
            {
                imageUrls.Add((new UrlBuilder(ServiceLocator.Current.GetInstance<UrlResolver>().GetUrl(imageRef))).ToString());

                if (profile.GetDataFromImage)
                {
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
            }

            return new HtmlString(PictureRenderer.Picture.Render(imageUrls.ToArray(), profile, altText, lazyLoading, focalPoints.ToArray(), cssClass));
        }
    }
}
