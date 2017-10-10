using System;
using System.Numerics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace TinyRenderer.Utils
{
    public static class TextureUtils
    {
        public static Image<Rgba32> LoadTexture(string path)
        {
            var image = Image.Load(path);
            image.Mutate(context => context.Flip(FlipType.Vertical));
            return image;
        }


        public static Rgba32 GetColor(this Image<Rgba32> image, Vector2 uv) =>
            image[Convert.ToInt32(uv.X * image.Width), Convert.ToInt32(uv.Y * image.Height)];


        public static Vector3 GetNormal(this Image<Rgba32> image, Vector2 uv)
        {
            var color = image[Convert.ToInt32(uv.X * image.Width), Convert.ToInt32(uv.Y * image.Height)];

            return new Vector3(
                    color.R / 225f * 2 - 1,
                    color.G / 225f * 2 - 1,
                    color.B / 225f * 2 - 1)
                .Normalize();
        }


        public static float GetSpecular(this Image<Rgba32> image, Vector2 uv) =>
            image[Convert.ToInt32(uv.X * image.Width), Convert.ToInt32(uv.Y * image.Height)].R;


        public static void SetColor(this Image<Rgba32> image, Vector2 uv, Rgba32 color) =>
            image[Convert.ToInt32(uv.X * image.Width), Convert.ToInt32(uv.Y * image.Height)] = color;
    }
}