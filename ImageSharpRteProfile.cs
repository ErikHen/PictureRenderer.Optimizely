using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureRenderer.Optimizely
{
    public class ImageSharpRteProfile : RteProfileBase
    {
        public string[] CreateWebpForFormat { get; set; }

        public ImageSharpRteProfile() : base()
        {
            CreateWebpForFormat = new[] { ImageFormat.Jpeg };
        }
    }
}
