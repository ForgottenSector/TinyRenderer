using System.Collections.Generic;

namespace TinyRenderer
{
    public sealed class Model
    {
        public List<TriangleInfo> Triangles { get; } = new List<TriangleInfo>();
    }
}