using System.Numerics;
using ImageSharp;

namespace TinyRenderer
{
    public interface IShader
    {
        Vector4 Vertex(TriangleInfo triangle, int vertexIndex);
        bool Fragment((int x, int y, float depth) fragment, Vector3 barycentric, out Rgba32 pixelColor);
    }
}