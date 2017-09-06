using System.Numerics;
using ImageSharp;
using TinyRenderer.Utils;

namespace TinyRenderer.Shaders
{
    public sealed class GouraudShader : IShader
    {
        private Vector3 _varyingIntensity;


        public Matrix4x4 Transform { get; set; }
        public Vector3 Light { get; set; }


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

            var color = Rgba32.White;

            pixelColor = color.Multiply(intensity);
            return true;
        }
    }
}