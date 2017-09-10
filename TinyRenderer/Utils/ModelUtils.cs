using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;

namespace TinyRenderer.Utils
{
    public static class ModelUtils
    {
        private const string VerticeTag = "v";
        private const string NormalTag = "vn";
        private const string UVTag = "vt";
        private const string TrinagleTag = "f";

        private static readonly char[] Separators =
        {
            '/', ' '
        };

        public static IReadOnlyCollection<TriangleInfo> LoadModel(string path)
        {
            var triangles = new List<TriangleInfo>();

            var vertices = new List<Vector3>();
            var normals = new List<Vector3>();
            var uv = new List<Vector2>();

            using (var stream = File.OpenRead(path))
            using (var reader = new StreamReader(stream))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var items = line.Split(Separators, StringSplitOptions.RemoveEmptyEntries);

                    switch (items.FirstOrDefault())
                    {
                        case VerticeTag:
                            vertices.Add(new Vector3(
                                Convert.ToSingle(items[1], CultureInfo.InvariantCulture.NumberFormat),
                                Convert.ToSingle(items[2], CultureInfo.InvariantCulture.NumberFormat),
                                Convert.ToSingle(items[3], CultureInfo.InvariantCulture.NumberFormat)));
                            break;
                        case NormalTag:
                            normals.Add(new Vector3(
                                Convert.ToSingle(items[1], CultureInfo.InvariantCulture.NumberFormat),
                                Convert.ToSingle(items[2], CultureInfo.InvariantCulture.NumberFormat),
                                Convert.ToSingle(items[3], CultureInfo.InvariantCulture.NumberFormat)));
                            break;
                        case UVTag:
                            uv.Add(new Vector2(
                                Convert.ToSingle(items[1], CultureInfo.InvariantCulture.NumberFormat),
                                Convert.ToSingle(items[2], CultureInfo.InvariantCulture.NumberFormat)));
                            break;
                        case TrinagleTag:
                            var triangle = new TriangleInfo(
                                new TriangleInfo.VertexInfo(
                                    vertices[Convert.ToInt32(items[1]) - 1],
                                    uv[Convert.ToInt32(items[2]) - 1],
                                    normals[Convert.ToInt32(items[3]) - 1]),
                                new TriangleInfo.VertexInfo(
                                    vertices[Convert.ToInt32(items[4]) - 1],
                                    uv[Convert.ToInt32(items[5]) - 1],
                                    normals[Convert.ToInt32(items[6]) - 1]),
                                new TriangleInfo.VertexInfo(
                                    vertices[Convert.ToInt32(items[7]) - 1],
                                    uv[Convert.ToInt32(items[8]) - 1],
                                    normals[Convert.ToInt32(items[9]) - 1])
                            );

                            triangles.Add(triangle);
                            break;
                    }
                }
            }

            return triangles;
        }
    }
}