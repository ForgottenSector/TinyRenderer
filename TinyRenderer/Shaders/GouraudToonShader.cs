using System.Numerics;
using SixLabors.ImageSharp;
using TinyRenderer.Utils;

namespace TinyRenderer.Shaders
{
    public sealed class GouraudToonShader : IShader
    {
        private Vector3 _varyingIntensity;


        public Matrix4x4 Transform { get; set; }
        public Vector3 Light { get; set; }


        #region Implementation of IShader

        public Vector4 Vertex(TriangleInfo triangle, int vertexIndex)
        {
            var vertexInfo = triangle[vertexIndex];

            var normal = vertexInfo.Normal;
            var intensity = normal.Dot(Light);
            Vector3Extensions.Set(ref _varyingIntensity, vertexIndex) = intensity;

            var vertex = vertexInfo.Vertex.ToVector4();
            return Transform.Multiply(vertex);
        }


        public bool Fragment((int x, int y, float depth) fragment, Vector3 barycentric, out Rgba32 pixelColor)
        {
            var intensity = _varyingIntensity.Dot(barycentric);

            if (intensity > .85f)
                intensity = 1;
            else if (intensity > .60f)
                intensity = .80f;
            else if (intensity > .45f)
                intensity = .60f;
            else if (intensity > .30f)
                intensity = .45f;
            else if (intensity > .15f)
                intensity = .30f;
            else
                intensity = 0;

            var color = Rgba32.Orange;

            pixelColor = color.Multiply(intensity);
            return true;
        }

        #endregion
    }
}