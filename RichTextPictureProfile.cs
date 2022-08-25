using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureRenderer.Optimizely
{
    public class RichTextPictureProfile
    {
        /// <summary>
        /// Maximum image width.
        /// </summary>
        public int MaxImageWidth { get; set; }

        public int Quality { get; set; }

        /// <summary>
        /// The image formats that should be offered as webp versions.
        /// PictureRenderer.ImageFormat.Jpeg is added by default.
        /// </summary>
        public string[] CreateWebpForFormat { get; set; }

        public RichTextPictureProfile()
        {
            MaxImageWidth = 1024;
            Quality = 80;
            CreateWebpForFormat = new[] {ImageFormat.Jpeg};
        }
    }
}
