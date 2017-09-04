using System.Numerics;
using ImageSharp;
using TinyRenderer.Utils;
using static System.Math;

namespace TinyRenderer.Shaders
{
    public sealed class OcclusionShader : IShader
    {
        private readonly Vector2[] _varyingUV = new Vector2[3];


        public Matrix4x4 Transform { get; set; }

        public Image<Rgba32> PassOcclusionMap { get; set; }

        public float[,] OcclusionBuffer { get; set; }


        public Vector4 Vertex(TriangleInfo triangle, int vertexIndex)
        {
            var vertexInfo = triangle[vertexIndex];

            var uv = vertexInfo.UV;
            _varyingUV[vertexIndex] = uv;

            var vertex = vertexInfo.Vertex.ToVector4();
            return Transform.Multiply(vertex);
        }

        public bool Fragment((int x, int y, float depth) fragment, Vector3 barycentric, out Rgba32 pixelColor)
        {
            var uv = _varyingUV[0] * barycentric.X +
                     _varyingUV[1] * barycentric.Y +
                     _varyingUV[2] * barycentric.Z;

            if (Abs(OcclusionBuffer[fragment.x, fragment.y] - fragment.depth) <= .01f)
                PassOcclusionMap.SetColor(uv, Rgba32.White);

            pixelColor = Rgba32.Black;
            return true;
        }
    }
}