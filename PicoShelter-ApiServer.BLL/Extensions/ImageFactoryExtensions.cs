using ImageProcessor;
using ImageProcessor.Imaging;
using System;
using System.Drawing;

namespace PicoShelter_ApiServer.BLL.Extensions
{
    public static class ImageFactoryExtensions
    {
        public static ImageFactory CropToThumbnail(this ImageFactory imageFactory, int maxSide)
        {
            var size = imageFactory.Image.Size;
            var minSide = Math.Min(size.Height, size.Height);
            var destSize = new Size(maxSide, maxSide);
            if (minSide < maxSide)
            {
                destSize = new(minSide, minSide);
            }
            imageFactory.Resize(new ResizeLayer(destSize, ResizeMode.Crop));

            return imageFactory;
        }
    }
}
