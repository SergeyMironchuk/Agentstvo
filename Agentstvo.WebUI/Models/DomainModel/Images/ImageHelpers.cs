﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Agentstvo.WebUI.Models.DomainModel.Images
{
    public static class ImageHelpers
    {
        public static Bitmap CombineBitmap(IEnumerable<string> files)
        {
            //read all images into memory
            List<Bitmap> images = new List<Bitmap>();
            Bitmap finalImage = null;

            try
            {
                int width = 0;
                int height = 0;

                foreach (string filename in files)
                {
                    // create a Bitmap from the file and add it to the list
                    Bitmap bitmap = new Bitmap(filename);

                    // update the size of the final bitmap
                    width += bitmap.Width;
                    height = bitmap.Height > height ? bitmap.Height : height;

                    images.Add(bitmap);
                }

                // create a bitmap to hold the combined image
                finalImage = new Bitmap(width, height);

                // get a graphics object from the image so we can draw on it
                using (Graphics g = Graphics.FromImage(finalImage))
                {
                    // set background color
                    g.Clear(Color.Transparent);

                    // go through each image and draw it on th final image
                    foreach (Bitmap image in images)
                    {
                        g.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height));
                    }
                }

                var destRect = new Rectangle(0, finalImage.Height / 3, finalImage.Width, finalImage.Height / 3);
                return finalImage.AddBluredRect(destRect);
            }
            catch (Exception)
            {
                if (finalImage != null) finalImage.Dispose();
                throw;
            }
            finally
            {
                // clean up memory
                foreach (Bitmap image in images)
                {
                    image.Dispose();
                }
            }
        }

        public static Bitmap AddBluredRect(this Bitmap fromImage, decimal bottomPercent)
        {
            var destRect = new Rectangle(0, fromImage.Height - (int)(fromImage.Height * bottomPercent / 100), fromImage.Width, (int)(fromImage.Height * bottomPercent / 100));
            return fromImage.AddBluredRect(destRect);
        }


        public static Bitmap AddBluredRect(this Bitmap fromImage, Rectangle destRect)
        {
            // Создаем полосу
            Bitmap strip = new Bitmap(destRect.Width, destRect.Height);
            Graphics graphics = Graphics.FromImage(strip);
            graphics.FillRectangle(new SolidBrush(Color.FromArgb(102, 102, 102)), 0, 0, destRect.Width, destRect.Height);

            // Размываем
            var subImage = fromImage.Clone(destRect, PixelFormat.Undefined);
            graphics = Graphics.FromImage(fromImage);
            graphics.DrawImage(subImage.ImageBlurFilter(BlurType.MotionBlur9x9), destRect);

            // Накладываем полупрозрачную полосу
            var colorMatrix = new ColorMatrix { Matrix33 = (float)0.8 };
            var imageAttributes = new ImageAttributes();
            imageAttributes.SetColorMatrix(
                colorMatrix,
                ColorMatrixFlag.Default,
                ColorAdjustType.Bitmap);
            graphics.DrawImage(strip, destRect, 0, 0, strip.Width, strip.Height, GraphicsUnit.Pixel, imageAttributes);

            return fromImage;
        }

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap ResizeImage(this Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
    }
}