using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace TinyRenderer.Utils
{
    public static class MathUtils
    {
        private static readonly Random Random = new Random();


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Min(float value1, float value2, float value3)
        {
            var min = value1;

            if (value2 < min)
                min = value2;

            if (value3 < min)
                min = value3;

            return min;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Max(float value1, float value2, float value3)
        {
            var max = value1;

            if (value2 > max)
                max = value2;

            if (value3 > max)
                max = value3;

            return max;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ToVector3(this Vector4 vector, bool normalize = true) => normalize
            ? new Vector3(
                vector.X / vector.W,
                vector.Y / vector.W,
                vector.Z / vector.W)
            : new Vector3(
                vector.X,
                vector.Y,
                vector.Z);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ToBarycentric(this Vector2 p, Vector2 a, Vector2 b, Vector2 c)
        {
            var ab = b - a;
            var ac = c - a;
            var pa = a - p;

            var r = new Vector3(ab.X, ac.X, pa.X).Cross(new Vector3(ab.Y, ac.Y, pa.Y));

            if (Math.Abs(r.Z) < 1e-2f)
                return new Vector3(-1, 1, 1);

            r /= r.Z;
            return new Vector3(1 - r.X - r.Y, r.X, r.Y);
        }


        public static Matrix4x4 CreateViewPort(int x, int y, int width, int height)
        {
            const float depthMin = 0f;
            const float depthMax = 1f;

            return new Matrix4x4
            {
                // Сдвигаем в точку x,y
                M14 = x + width / 2f,
                M24 = y + height / 2f,
                M34 = (depthMax + depthMin) / 2f,

                // Маштабируем
                M11 = width / 2f,
                M22 = height / 2f,
                M33 = (depthMax - depthMin) / 2f,
                M44 = 1
            };
        }

        public static Matrix4x4 CreateProjection(float r)
        {
            // r = -1/c, c - расстояние камеры от центра по оси z
            var projection = Matrix4x4.Identity;
            projection.M43 = r;
            return projection;
        }

        public static Matrix4x4 CreateTranslation(Vector3 position)
        {
            var translation = Matrix4x4.Identity;

            translation.M14 = position.X;
            translation.M24 = position.Y;
            translation.M34 = position.Z;

            return translation;
        }

        public static Matrix4x4 CreateLookAt(Vector3 cameraPosition, Vector3 cameraTarget, Vector3 cameraUpVector)
        {
            var z = (cameraPosition - cameraTarget).Normalize();
            var x = cameraUpVector.Cross(z).Normalize();
            var y = z.Cross(x).Normalize();

            var modelView = Matrix4x4.Identity;

            modelView.M11 = x.X;
            modelView.M21 = y.X;
            modelView.M31 = z.X;

            modelView.M12 = x.Y;
            modelView.M22 = y.Y;
            modelView.M32 = z.Y;

            modelView.M13 = x.Z;
            modelView.M23 = y.Z;
            modelView.M33 = z.Z;

            var translation = CreateTranslation(-cameraTarget);

            return modelView * translation;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 RandomOnUnitSphere()
        {
            var u = Random.NextDouble();
            var v = Random.NextDouble();
            var theta = 2 * Math.PI * u;
            var phi = Math.Acos(2 * v - 1);

            return new Vector3
            (
                (float) (Math.Sin(phi) * Math.Cos(theta)),
                (float) (Math.Sin(phi) * Math.Sin(theta)),
                (float) Math.Cos(phi)
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 RandomUnitVector3() => new Vector3
        (
            (float) Random.NextDouble(),
            (float) Random.NextDouble(),
            (float) Random.NextDouble()
        );
    }
}