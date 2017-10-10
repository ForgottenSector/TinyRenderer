using System.Numerics;
using SixLabors.ImageSharp;
using TinyRenderer.Utils;

namespace TinyRenderer.Shaders
{
    public sealed class FlatShader : IShader
    {
        private float _varyingIntensity;


        public Matrix4x4 Transform { get; set; }
        public Vector3 Light { get; set; }


        #region Implementation of IShader

        public Vector4 Vertex(TriangleInfo triangle, int vertexIndex)
        {
            var a = triangle.Vertex0.Vertex;
            var b = triangle.Vertex1.Vertex;
            var c = triangle.Vertex2.Vertex;

            var normal = (c - a).Cross(b - a).Normalize();
            _varyingIntensity = normal.Dot(Light);

            var vertex = triangle[vertexIndex].Vertex.ToVector4();
            return Transform.Multiply(vertex);
        }


        public bool Fragment((int x, int y, float depth) fragment, Vector3 barycentric, out Rgba32 pixelColor)
        {
            if (_varyingIntensity <= 0)
            {
                pixelColor = Rgba32.Black;
                return false;
            }

            var color = Rgba32.White;

            pixelColor = color.Multiply(_varyingIntensity);
            return true;
        }

        #endregion
    }
}