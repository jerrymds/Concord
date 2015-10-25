﻿/*
Copyright (c) 2012 <a href="http://www.gutgames.com">James Craig</a>

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.*/

#region Usings
using System.Drawing;
using System.Drawing.Imaging;
using Company.Utilities.DataTypes.ExtensionMethods;
using Company.Utilities.Media.Image.ExtensionMethods;
#endregion

namespace Company.Utilities.Media.Image.Procedural
{
    /// <summary>
    /// Cellular texture helper
    /// </summary>
    public static class CellularTexture
    {
        #region Functions

        /// <summary>
        /// Generates a cellular texture image
        /// </summary>
        /// <param name="Width">Width</param>
        /// <param name="Height">Height</param>
        /// <param name="NumberOfPoints">Number of points</param>
        /// <param name="Seed">Random seed</param>
        /// <returns>Returns an image of a cellular texture</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "Body")]
        public static Bitmap Generate(int Width, int Height, int NumberOfPoints, int Seed)
        {
            float[,] DistanceBuffer = new float[Width, Height];
            float MinimumDistance = float.MaxValue;
            float MaxDistance = float.MinValue;
            CellularMap Map = new CellularMap(Seed, Width, Height, NumberOfPoints);
            MaxDistance = Map.MaxDistance;
            MinimumDistance = Map.MinDistance;
            DistanceBuffer = Map.Distances;
            Bitmap ReturnValue = new Bitmap(Width, Height);
            BitmapData ImageData = ReturnValue.LockImage();
            int ImagePixelSize = ImageData.GetPixelSize();
            for (int x = 0; x < Width; ++x)
            {
                for (int y = 0; y < Height; ++y)
                {
                    float Value = GetHeight(x, y, DistanceBuffer, MinimumDistance, MaxDistance);
                    Value *= 255;
                    int RGBValue = ((int)Value).Clamp(255, 0);
                    ImageData.SetPixel(x, y, Color.FromArgb(RGBValue, RGBValue, RGBValue), ImagePixelSize);
                }
            }
            ReturnValue.UnlockImage(ImageData);
            return ReturnValue;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "2#")]
        private static float GetHeight(float X, float Y, float[,] DistanceBuffer,
            float MinimumDistance, float MaxDistance)
        {
            return (DistanceBuffer[(int)X,(int)Y] - MinimumDistance) / (MaxDistance - MinimumDistance);
        }

        #endregion
    }
}