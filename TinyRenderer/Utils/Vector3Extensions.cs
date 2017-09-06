using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace TinyRenderer.Utils
{
    public static class Vector3Extensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref float Set(ref Vector3 vector, int index)
        {
            switch (index)
            {
                case 0:
                    return ref vector.X;
                case 1:
                    return ref vector.Y;
                case 2:
                    return ref vector.Z;
                default:
                    throw new ArgumentOutOfRangeException(nameof(index));
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Normalize(this Vector3 vector) => Vector3.Normalize(vector);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Dot(this Vector3 vector1, Vector3 vector2) => Vector3.Dot(vector1, vector2);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Cross(this Vector3 vector1, Vector3 vector2) => Vector3.Cross(vector1, vector2);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 ToVector4(this Vector3 vector, float w = 1) => new Vector4(vector, w);
    }
}