using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using ImageSharp;

namespace TinyRenderer.Utils
{
    public static class Rgba32Extensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ToVector3(this Rgba32 color) => new Vector3(
            (float) color.R / color.A,
            (float) color.G / color.A,
            (float) color.B / color.A);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rgba32 Add(this Rgba32 color, Rgba32 value)
        {
            color.R = (byte) Math.Clamp(color.R + value.R, 0, byte.MaxValue);
            color.G = (byte) Math.Clamp(color.G + value.G, 0, byte.MaxValue);
            color.B = (byte) Math.Clamp(color.B + value.B, 0, byte.MaxValue);

            return color;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rgba32 Multiply(this Rgba32 color, float value)
        {
            color.R = (byte) Math.Clamp(color.R * value, 0, byte.MaxValue);
            color.G = (byte) Math.Clamp(color.G * value, 0, byte.MaxValue);
            color.B = (byte) Math.Clamp(color.B * value, 0, byte.MaxValue);

            return color;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rgba32 Multiply(this Rgba32 color, Vector3 value)
        {
            color.R = (byte) Math.Clamp(color.R * value.X, 0, byte.MaxValue);
            color.G = (byte) Math.Clamp(color.G * value.Y, 0, byte.MaxValue);
            color.B = (byte) Math.Clamp(color.B * value.Z, 0, byte.MaxValue);

            return color;
        }
    }
}