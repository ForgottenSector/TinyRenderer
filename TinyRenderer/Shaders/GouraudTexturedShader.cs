using System.Numerics;
using SixLabors.ImageSharp;
using TinyRenderer.Utils;

namespace TinyRenderer.Shaders
{
    public sealed class GouraudTexturedShader : IShader
    {
        private readonly Vector2[] _varyingUV = new Vector2[3];
        private Vector3 _varyingIntensity;


        public Matrix4x4 Transform { get; set; }
        public Vector3 Light { get; set; }

        public Image<Rgba32> Texture { get; set; }


        #region Implementation of IShader

        public Vector4 Vertex(TriangleInfo triangle, int vertexIndex)
        {
            var vertexInfo = triangle[vertexIndex];

            var uv = vertexInfo.UV;
            _varyingUV[vertexIndex] = uv;

            var normal = vertexInfo.Normal;
            var intensity = normal.Dot(Light);
            Vector3Extensions.Set(ref _varyingIntensity, vertexIndex) = intensity;

            var vertex = vertexInfo.Vertex.ToVector4();
            return Transform.Multiply(vertex);
        }


        public bool Fragment((int x, int y, float depth) fragment, Vector3 barycentric, out Rgba32 pixelColor)
        {
            var uv = _varyingUV[0] * barycentric.X +
                     _varyingUV[1] * barycentric.Y +
                     _varyingUV[2] * barycentric.Z;

            var intensity = _varyingIntensity.Dot(barycentric);

            var color = Texture.GetColor(uv);

            pixelColor = color.Multiply(intensity);
            return true;
        }

        #endregion
    }
}