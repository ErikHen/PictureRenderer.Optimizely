using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureRenderer.Optimizely
{
    public abstract class RteProfileBase
    {
        /// <summary>
        /// Maximum image width.
        /// </summary>
        public int MaxImageWidth { get; set; }

        public int Quality { get; set; }

        protected RteProfileBase()
        {
            MaxImageWidth = 1024;
            Quality = 80;
        }
    }
}
