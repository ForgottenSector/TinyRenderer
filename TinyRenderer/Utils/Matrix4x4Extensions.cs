﻿using System.Numerics;
using System.Runtime.CompilerServices;

namespace TinyRenderer.Utils
{
    public static class Matrix4x4Extensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix4x4 CreateFromColumns(Vector3 row1, Vector3 row2, Vector3 row3) =>
            new Matrix4x4
            {
                M11 = row1.X,
                M21 = row1.Y,
                M31 = row1.Z,

                M12 = row2.X,
                M22 = row2.Y,
                M32 = row2.Z,

                M13 = row3.X,
                M23 = row3.Y,
                M33 = row3.Z,

                M44 = 1
            };


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix4x4 Transpose(this Matrix4x4 matrix) => Matrix4x4.Transpose(matrix);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix4x4 Invert(this Matrix4x4 matrix)
        {
            Matrix4x4.Invert(matrix, out matrix);
            return matrix;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 Multiply(this Matrix4x4 matrix, Vector4 vector) => new Vector4(
            matrix.M11 * vector.X + matrix.M12 * vector.Y + matrix.M13 * vector.Z + matrix.M14 * vector.W,
            matrix.M21 * vector.X + matrix.M22 * vector.Y + matrix.M23 * vector.Z + matrix.M24 * vector.W,
            matrix.M31 * vector.X + matrix.M32 * vector.Y + matrix.M33 * vector.Z + matrix.M34 * vector.W,
            matrix.M41 * vector.X + matrix.M42 * vector.Y + matrix.M43 * vector.Z + matrix.M44 * vector.W);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Multiply(this Matrix4x4 matrix, Vector3 vector)
        {
            var w = matrix.M41 * vector.X + matrix.M42 * vector.Y + matrix.M43 * vector.Z + matrix.M44;

            return new Vector3(
                       matrix.M11 * vector.X + matrix.M12 * vector.Y + matrix.M13 * vector.Z + matrix.M14,
                       matrix.M21 * vector.X + matrix.M22 * vector.Y + matrix.M23 * vector.Z + matrix.M24,
                       matrix.M31 * vector.X + matrix.M32 * vector.Y + matrix.M33 * vector.Z + matrix.M34
                   ) / w;
        }
    }
}