﻿using System.Drawing;

namespace MotionRecognition
{
    public static class ImageCreator
    {
        public static Bitmap CreateNeuralImageFromDoubleArray(ref double[] arr, int size, bool Is3DImage = false)
        {
            int width = size;
            int height = (Is3DImage) ? size * 2 : size;

            Bitmap g = new Bitmap(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    g.SetPixel(x, y, (y < size) ? Color.Red : Color.White); // Use different background color for both halves of the image

                    if (arr[x + y * width] != 0) // y * width equates to the offset within the array
                    {
                        g.SetPixel(x, y, (y >= size) ? Color.Blue : Color.Black);
                    }
                }
            }
            return g;
        }

        public static void WriteBitmapToFS(Bitmap bitmap, string filename = "Image.bmp")
        {
            bitmap.Save(filename, System.Drawing.Imaging.ImageFormat.Bmp);
        }
    }
}
