using System;
using System.Collections.Generic;
using System.Numerics;
using SixLabors.ImageSharp;
using TinyRenderer.Shaders;
using TinyRenderer.Utils;

namespace TinyRenderer
{
    public static class TinyGL
    {
        public static Image<Rgba32> Render(this Image<Rgba32> image, IEnumerable<TriangleInfo> model, IShader shader,
            float[,] zBuffer)
        {
            foreach (var triangle in model)
                image.TriangleRasterize(
                    shader.Vertex(triangle, 0),
                    shader.Vertex(triangle, 1),
                    shader.Vertex(triangle, 2),
                    shader, zBuffer);

            return image;
        }


        public static Image<Rgba32> TriangleRasterize(this Image<Rgba32> image,
            Vector4 a, Vector4 b, Vector4 c, IShader shader, float[,] zBuffer)
        {
            var a2 = new Vector2(a.X, a.Y) / a.W;
            var b2 = new Vector2(b.X, b.Y) / b.W;
            var c2 = new Vector2(c.X, c.Y) / c.W;

            var minX = Math.Max(0, (int) MathUtils.Min(a2.X, b2.X, c2.X));
            var minY = Math.Max(0, (int) MathUtils.Min(a2.Y, b2.Y, c2.Y));

            var maxX = Math.Min(image.Width - 1, (int) MathUtils.Max(a2.X, b2.X, c2.X));
            var maxY = Math.Min(image.Height - 1, (int) MathUtils.Max(a2.Y, b2.Y, c2.Y));

            for (var x = minX; x <= maxX; x++)
            for (var y = minY; y <= maxY; y++)
            {
                var barycentricScreen = new Vector2(x, y).ToBarycentric(a2, b2, c2);

                if (barycentricScreen.X < 0 || barycentricScreen.Y < 0 || barycentricScreen.Z < 0)
                    continue;

                var barycentricGlobal = new Vector3(
                    barycentricScreen.X / a.W,
                    barycentricScreen.Y / b.W,
                    barycentricScreen.Z / c.W);

                barycentricGlobal /= barycentricGlobal.X + barycentricGlobal.Y + barycentricGlobal.Z;

                var depth = a.Z * barycentricGlobal.X +
                            b.Z * barycentricGlobal.Y +
                            c.Z * barycentricGlobal.Z;

                if (zBuffer[x, y] >= depth)
                    continue;

                zBuffer[x, y] = depth;

                if (!shader.Fragment((x, y, depth), barycentricGlobal, out var color))
                    continue;

                image[x, y] = color;
            }

            return image;
        }


        public static Image<Rgba32> RenderZBuffer(this Image<Rgba32> image, float[,] zBuffer,
            int screenWidth, int screenHeight)
        {
            for (var x = 0; x < screenWidth; x++)
            for (var y = 0; y < screenHeight; y++)
                image[x, y] = new Rgba32(Vector3.One * zBuffer[x, y]);

            return image;
        }


        public static Image<Rgba32> GenerateOccolusionMap(this IReadOnlyCollection<TriangleInfo> model,
            Matrix4x4 viewPort, int imageWidth,
            int imageHeight, int passNumber)
        {
            var screen = new Image<Rgba32>(imageWidth, imageHeight);
            screen.Mutate(context => context.Fill(Rgba32.Black));

            var resultOcclusionMap = new Image<Rgba32>(imageWidth, imageHeight);
            resultOcclusionMap.Mutate(context => context.Fill(Rgba32.Black));

            var passOcclusionMap = new Image<Rgba32>(imageWidth, imageHeight);
            passOcclusionMap.Mutate(context => context.Fill(Rgba32.Black));

            var occlusionBuffer = new float[imageWidth, imageHeight];
            var zBuffer = new float[imageWidth, imageHeight];

            var zBufferShader = new ZBufferShader();
            var occlusionShader = new OcclusionShader
            {
                PassOcclusionMap = passOcclusionMap,
                OcclusionBuffer = occlusionBuffer
            };

            var cameraTarget = new Vector3(0, 0, 0);

            for (var pass = 1; pass <= passNumber; pass++)
            {
                Array.Clear(occlusionBuffer, 0, imageWidth * imageHeight);
                Array.Clear(zBuffer, 0, imageWidth * imageHeight);
                passOcclusionMap.Mutate(context => context.Fill(Rgba32.Black));

                var cameraPosition = MathUtils.RandomOnUnitSphere();
                cameraPosition.Y = Math.Abs(cameraPosition.Y);
                var cameraUp = MathUtils.RandomUnitVector3();

                var modelView = MathUtils.CreateLookAt(cameraPosition, cameraTarget, cameraUp);
                var transform = viewPort * modelView;


                zBufferShader.Transform = transform;
                screen.Render(model, zBufferShader, occlusionBuffer);

                occlusionShader.Transform = transform;
                screen.Render(model, occlusionShader, zBuffer);

                for (var i = 0; i < imageWidth; i++)
                for (var j = 0; j < imageHeight; j++)
                {
                    var previous = resultOcclusionMap[i, j].R;
                    var current = passOcclusionMap[i, j].R;

                    var color = (byte) ((float) (previous * (pass - 1) + current) / pass + .5f);
                    resultOcclusionMap[i, j] = new Rgba32(color, color, color);
                }
            }

            return resultOcclusionMap;
        }


        public static float MaxElevationAngle(float[,] zBuffer, Vector2 position, Vector2 direction,
            int screenWidth, int screenHeight)
        {
            var maxElevationAngle = 0f;
            for (var t = 0; t < 1000; t += 1)
            {
                var current = position + direction * t;

                if (current.X >= screenWidth || current.Y >= screenHeight || current.X < 0 || current.Y < 0)
                    return maxElevationAngle;

                var distance = (position - current).Length();

                if (distance < 1)
                    continue;

                var elevation = (zBuffer[(int) current.X, (int) current.Y] -
                                 zBuffer[(int) position.X, (int) position.Y]) * 225;

                maxElevationAngle = (float) Math.Max(maxElevationAngle, Math.Atan(elevation / distance));
            }

            return maxElevationAngle;
        }


        public static Image<Rgba32> RenderAmbientOccolusion(this Image<Rgba32> image, float[,] zBuffer, int screenWidth,
            int screenHeight)
        {
            const int raysNumber = 8;
            const double rayAngle = 2 * Math.PI / raysNumber;

            image.Mutate(context => context.Fill(Rgba32.Black));

            for (var x = 0; x < screenWidth; x++)
            for (var y = 0; y < screenHeight; y++)
            {
                if (zBuffer[x, y] <= 0f)
                    continue;

                var total = .0;

                for (var i = 0; i < raysNumber; i++)
                {
                    var angle = i * rayAngle;

                    total += Math.PI / 2 - MaxElevationAngle(zBuffer, new Vector2(x, y),
                                 new Vector2((float) Math.Cos(angle), (float) Math.Sin(angle)),
                                 screenWidth, screenHeight);
                }

                total /= Math.PI / 2 * 8;

                var color = (byte) (total * 255);
                image[x, y] = new Rgba32(color, color, color);
            }

            return image;
        }
    }
}