using System.Numerics;
using SixLabors.ImageSharp;
using TinyRenderer.Utils;

namespace TinyRenderer.Shaders
{
    public sealed class NormalMapShader : IShader
    {
        private readonly Vector2[] _varyingUV = new Vector2[3];


        public Matrix4x4 Transform { get; set; }
        public Matrix4x4 MIT { get; set; }
        public Vector3 Light { get; set; }

        public Image<Rgba32> Texture { get; set; }
        public Image<Rgba32> NormalMap { get; set; }


        #region Implementation of IShader

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

            var normal = NormalMap.GetNormal(uv);
            normal = MIT.Multiply(normal).Normalize();

            var intensity = normal.Dot(Light);

            var color = Texture.GetColor(uv);

            pixelColor = color.Multiply(intensity);
            return true;
        }

        #endregion
    }
}