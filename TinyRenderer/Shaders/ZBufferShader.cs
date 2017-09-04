using System.Numerics;
using ImageSharp;
using TinyRenderer.Utils;

namespace TinyRenderer.Shaders
{
    public sealed class ZBufferShader : IShader
    {
        public Matrix4x4 Transform { get; set; }


        public Vector4 Vertex(TriangleInfo triangle, int vertexIndex)
        {
            var vertex = triangle[vertexIndex].Vertex.ToVector4();

            return Transform.Multiply(vertex);
        }


        public bool Fragment((int x, int y, float depth) fragment, Vector3 barycentric, out Rgba32 pixelColor)
        {
            var color = Rgba32.White;

            pixelColor = color.Multiply(fragment.depth);
            return true;
        }
    }
}