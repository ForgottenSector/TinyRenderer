using System.Numerics;
using ImageSharp;
using TinyRenderer.Utils;

namespace TinyRenderer.Shaders
{
    public sealed class FlatTexturedShader : IShader
    {
        private readonly Vector2[] _varyingUV = new Vector2[3];
        private float _varyingIntensity;


        public Matrix4x4 Transform { get; set; }
        public Vector3 Light { get; set; }

        public Image<Rgba32> Texture { get; set; }


        public Vector4 Vertex(TriangleInfo triangle, int vertexIndex)
        {
            var vertexInfo = triangle[vertexIndex];

            var uv = vertexInfo.UV;
            _varyingUV[vertexIndex] = uv;

            var a = triangle.Vertex0.Vertex;
            var b = triangle.Vertex1.Vertex;
            var c = triangle.Vertex2.Vertex;

            var normal = (c - a).Cross(b - a).Normalize();
            _varyingIntensity = normal.Dot(Light);

            var vertex = vertexInfo.Vertex;
            return Transform.Multiply(vertex.ToVector4());
        }

        public bool Fragment((int x, int y, float depth) fragment, Vector3 barycentric, out Rgba32 pixelColor)
        {
            if (_varyingIntensity <= 0)
            {
                pixelColor = Rgba32.Black;
                return false;
            }

            var uv = _varyingUV[0] * barycentric.X +
                     _varyingUV[1] * barycentric.Y +
                     _varyingUV[2] * barycentric.Z;

            var color = Texture.GetColor(uv);

            pixelColor = color.Multiply(_varyingIntensity);
            return true;
        }
    }
}