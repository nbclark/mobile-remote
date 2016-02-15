using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Text;

namespace MobileSRC.MobileRemote
{
    internal static class Imaging
    {
        public static Bitmap RotateSquareImage(int rotationAngle, Image originalBitmap)
        {
            return RotateSquareImage(rotationAngle, (Bitmap)originalBitmap);
        }

        public static Bitmap RotateSquareImage(int rotationAngle, Bitmap originalBitmap)
        {
            // It should be faster to access values stored on the stack
            // compared to calling a method (in this case a property) to 
            // retrieve a value. That's why we store the width and height
            // of the bitmaps here so that when we're traversing the pixels
            // we won't have to call more methods than necessary.

            int newWidth = originalBitmap.Height;
            int newHeight = originalBitmap.Width;

            int originalWidth = originalBitmap.Width;
            int originalHeight = originalBitmap.Height;

            Bitmap rotatedBitmap = new Bitmap(newWidth, newHeight);

            // We're going to use the new width and height minus one a lot so lets 
            // pre-calculate that once to save some more time
            int newWidthMinusOne = newWidth - 1;
            int newHeightMinusOne = newHeight - 1;

            // To grab the raw bitmap data into a BitmapData object we need to
            // "lock" the data (bits that make up the image) into system memory.
            // We lock the source image as ReadOnly and the destination image
            // as WriteOnly and hope that the .NET Framework can perform some
            // sort of optimization based on this.
            // Note that this piece of code relies on the PixelFormat of the 
            // images to be 32 bpp (bits per pixel). We're not interested in 
            // the order of the components (red, green, blue and alpha) as 
            // we're going to copy the entire 32 bits as they are.
            BitmapData originalData = originalBitmap.LockBits(
                new Rectangle(0, 0, originalWidth, originalHeight),
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppRgb);
            BitmapData rotatedData = rotatedBitmap.LockBits(
                new Rectangle(0, 0, rotatedBitmap.Width, rotatedBitmap.Height),
                ImageLockMode.WriteOnly,
                PixelFormat.Format32bppRgb);

            // We're not allowed to use pointers in "safe" code so this
            // section has to be marked as "unsafe". Cool!
            unsafe
            {
                // Grab int pointers to the source image data and the 
                // destination image data. We can think of this pointer
                // as a reference to the first pixel on the first row of the 
                // image. It's actually a pointer to the piece of memory 
                // holding the int pixel data and we're going to treat it as
                // an array of one dimension later on to address the pixels.
                int* originalPointer = (int*)originalData.Scan0.ToPointer();
                int* rotatedPointer = (int*)rotatedData.Scan0.ToPointer();

                // There are nested for-loops in all of these case statements
                // and one might argue that it would have been neater and more
                // tidy to have the switch statement inside the a single nested
                // set of for loops, doing it this way saves us up to three int 
                // to int comparisons per pixel. 
                //
                switch (rotationAngle)
                {
                    case 90:
                        for (int y = 0; y < originalHeight; ++y)
                        {
                            int destinationX = newWidthMinusOne - y;
                            for (int x = 0; x < originalWidth; ++x)
                            {
                                int sourcePosition = (x + y * originalWidth);
                                int destinationY = x;
                                int destinationPosition =
                                        (destinationX + destinationY * newWidth);
                                rotatedPointer[destinationPosition] =
                                    originalPointer[sourcePosition];
                            }
                        }
                        break;
                    case 180:
                        for (int y = 0; y < originalHeight; ++y)
                        {
                            int destinationY = (newHeightMinusOne - y) * newWidth;
                            for (int x = 0; x < originalWidth; ++x)
                            {
                                int sourcePosition = (x + y * originalWidth);
                                int destinationX = newWidthMinusOne - x;
                                int destinationPosition = (destinationX + destinationY);
                                rotatedPointer[destinationPosition] =
                                    originalPointer[sourcePosition];
                            }
                        }
                        break;
                    case 270:
                        for (int y = 0; y < originalHeight; ++y)
                        {
                            int destinationX = y;
                            for (int x = 0; x < originalWidth; ++x)
                            {
                                int sourcePosition = (x + y * originalWidth);
                                int destinationY = newHeightMinusOne - x;
                                int destinationPosition =
                                    (destinationX + destinationY * newWidth);
                                rotatedPointer[destinationPosition] =
                                    originalPointer[sourcePosition];
                            }
                        }
                        break;
                }

                // We have to remember to unlock the bits when we're done.
                originalBitmap.UnlockBits(originalData);
                rotatedBitmap.UnlockBits(rotatedData);
            }
            return rotatedBitmap;
        }

        public static Bitmap RotateImage(int rotationAngle, Bitmap originalBitmap)
        {
            int width = originalBitmap.Width;
            int height = originalBitmap.Height;

            switch (rotationAngle)
            {
                case 90:
                case 270:
                    {
                        width = originalBitmap.Height;
                        height = originalBitmap.Width;
                    }
                    break;
            }

            Bitmap rotatedBitmap = new Bitmap(width, height);
            RotateImage(rotationAngle, originalBitmap, rotatedBitmap);

            return rotatedBitmap;
        }

        public static void RotateImage(int rotationAngle, Bitmap originalBitmap, Bitmap rotatedBitmap)
        {
            // It should be faster to access values stored on the stack
            // compared to calling a method (in this case a property) to 
            // retrieve a value. That's why we store the width and height
            // of the bitmaps here so that when we're traversing the pixels
            // we won't have to call more methods than necessary.

            int newWidth = rotatedBitmap.Width;
            int newHeight = rotatedBitmap.Height;

            int originalWidth = originalBitmap.Width;
            int originalHeight = originalBitmap.Height;

            // We're going to use the new width and height minus one a lot so lets 
            // pre-calculate that once to save some more time
            int newWidthMinusOne = newWidth - 1;
            int newHeightMinusOne = newHeight - 1;

            // To grab the raw bitmap data into a BitmapData object we need to
            // "lock" the data (bits that make up the image) into system memory.
            // We lock the source image as ReadOnly and the destination image
            // as WriteOnly and hope that the .NET Framework can perform some
            // sort of optimization based on this.
            // Note that this piece of code relies on the PixelFormat of the 
            // images to be 32 bpp (bits per pixel). We're not interested in 
            // the order of the components (red, green, blue and alpha) as 
            // we're going to copy the entire 32 bits as they are.
            BitmapData originalData = originalBitmap.LockBits(
                new Rectangle(0, 0, originalWidth, originalHeight),
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppRgb);
            BitmapData rotatedData = rotatedBitmap.LockBits(
                new Rectangle(0, 0, rotatedBitmap.Width, rotatedBitmap.Height),
                ImageLockMode.WriteOnly,
                PixelFormat.Format32bppRgb);

            // We're not allowed to use pointers in "safe" code so this
            // section has to be marked as "unsafe". Cool!
            unsafe
            {
                // Grab int pointers to the source image data and the 
                // destination image data. We can think of this pointer
                // as a reference to the first pixel on the first row of the 
                // image. It's actually a pointer to the piece of memory 
                // holding the int pixel data and we're going to treat it as
                // an array of one dimension later on to address the pixels.
                int* originalPointer = (int*)originalData.Scan0.ToPointer();
                int* rotatedPointer = (int*)rotatedData.Scan0.ToPointer();

                // There are nested for-loops in all of these case statements
                // and one might argue that it would have been neater and more
                // tidy to have the switch statement inside the a single nested
                // set of for loops, doing it this way saves us up to three int 
                // to int comparisons per pixel. 
                //
                switch (rotationAngle)
                {
                    case 90:
                        for (int y = 0; y < originalHeight; ++y)
                        {
                            int destinationX = newWidthMinusOne - y;
                            for (int x = 0; x < originalWidth; ++x)
                            {
                                int sourcePosition = (x + y * originalWidth);
                                int destinationY = x;
                                int destinationPosition =
                                        (destinationX + destinationY * newWidth);
                                rotatedPointer[destinationPosition] =
                                    originalPointer[sourcePosition];
                            }
                        }
                        break;
                    case 180:
                        for (int y = 0; y < originalHeight; ++y)
                        {
                            int destinationY = (newHeightMinusOne - y) * newWidth;
                            for (int x = 0; x < originalWidth; ++x)
                            {
                                int sourcePosition = (x + y * originalWidth);
                                int destinationX = newWidthMinusOne - x;
                                int destinationPosition = (destinationX + destinationY);
                                rotatedPointer[destinationPosition] =
                                    originalPointer[sourcePosition];
                            }
                        }
                        break;
                    case 270:
                        for (int y = 0; y < originalHeight; ++y)
                        {
                            int destinationX = y;
                            for (int x = 0; x < originalWidth; ++x)
                            {
                                int sourcePosition = (x + y * originalWidth);
                                int destinationY = newHeightMinusOne - x;
                                int destinationPosition =
                                    (destinationX + destinationY * newWidth);
                                rotatedPointer[destinationPosition] =
                                    originalPointer[sourcePosition];
                            }
                        }
                        break;
                }

                // We have to remember to unlock the bits when we're done.
                originalBitmap.UnlockBits(originalData);
                rotatedBitmap.UnlockBits(rotatedData);
            }
        }
    }
}
