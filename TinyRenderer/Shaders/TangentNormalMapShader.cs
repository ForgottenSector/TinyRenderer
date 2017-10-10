using System.Numerics;
using SixLabors.ImageSharp;
using TinyRenderer.Utils;

namespace TinyRenderer.Shaders
{
    public sealed class TangentNormalMapShader : IShader
    {
        private readonly Vector2[] _varyingUV = new Vector2[3];
        private readonly Vector3[] _varyingNormal = new Vector3[3];
        private readonly Vector3[] _varyingTrinagle = new Vector3[3];


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

            var normal = vertexInfo.Normal;
            normal = MIT.Multiply(normal);
            _varyingNormal[vertexIndex] = normal;

            var vertex = vertexInfo.Vertex.ToVector4();
            vertex = Transform.Multiply(vertex);

            _varyingTrinagle[vertexIndex] = vertex.ToVector3();

            return vertex;
        }


        public bool Fragment((int x, int y, float depth) fragment, Vector3 barycentric, out Rgba32 pixelColor)
        {
            var uv = _varyingUV[0] * barycentric.X +
                     _varyingUV[1] * barycentric.Y +
                     _varyingUV[2] * barycentric.Z;

            var normal = (_varyingNormal[0] * barycentric.X +
                          _varyingNormal[1] * barycentric.Y +
                          _varyingNormal[2] * barycentric.Z).Normalize();

            // https://learnopengl.com/#!Advanced-Lighting/Normal-Mapping
            var edge1 = _varyingTrinagle[1] - _varyingTrinagle[0];
            var edge2 = _varyingTrinagle[2] - _varyingTrinagle[0];

            var deltaUV1 = _varyingUV[1] - _varyingUV[0];
            var deltaUV2 = _varyingUV[2] - _varyingUV[0];

            var f = 1 / (deltaUV1.X * deltaUV2.Y - deltaUV1.Y * deltaUV2.X);
            var tangent = (f * (deltaUV2.Y * edge1 - deltaUV1.Y * edge2)).Normalize();
            var bitangent = (f * (deltaUV1.X * edge2 - deltaUV2.X * edge1)).Normalize();

            var TBN = Matrix4x4Extensions.CreateFromColumns(
                tangent,
                bitangent,
                normal);

            normal = NormalMap.GetNormal(uv);
            normal = TBN.Multiply(normal).Normalize();

            var diffuse = normal.Dot(Light);

            var color = Texture.GetColor(uv);

            pixelColor = color.Multiply(diffuse);
            return true;
        }

        #endregion
    }
}