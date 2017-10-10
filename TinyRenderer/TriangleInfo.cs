using System;
using System.Numerics;

namespace TinyRenderer
{
    public sealed class TriangleInfo
    {
        public readonly VertexInfo Vertex0;
        public readonly VertexInfo Vertex1;
        public readonly VertexInfo Vertex2;


        public TriangleInfo(VertexInfo vertex0, VertexInfo vertex1, VertexInfo vertex2)
        {
            Vertex0 = vertex0;
            Vertex1 = vertex1;
            Vertex2 = vertex2;
        }


        public VertexInfo this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return Vertex0;
                    case 1:
                        return Vertex1;
                    case 2:
                        return Vertex2;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(index));
                }
            }
        }

        public sealed class VertexInfo
        {
            public readonly Vector3 Vertex;
            public readonly Vector2 UV;
            public readonly Vector3 Normal;


            public VertexInfo(Vector3 vertex, Vector2 uv, Vector3 normal)
            {
                Vertex = vertex;
                UV = uv;
                Normal = normal;
            }
        }
    }
}