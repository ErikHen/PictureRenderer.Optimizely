using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PictureRenderer.Profiles;

namespace PictureRenderer.Optimizely
{
    public class PictureProfile : ImageSharpProfile
    {
        /// <summary>
        /// Set to false for a potential slight performance gain. But it will not be possible to get alt text or focal point from image. 
        /// </summary>
        public bool GetDataFromImage { get; set; }

        /// <summary>
        /// Set a static height in order to render the height attribute in the img tag without setting a static aspect ratio. Useful for wide images on small screens, to utilize cropping in imagesharp.  
        /// </summary>
        public int? StaticHeight { get; set; }

        public PictureProfile()
        {
            GetDataFromImage = true;
        }
    }
}
