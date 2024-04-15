using System.IO;
using System.Globalization;
using System.Collections.Generic;

namespace PLAYGROUND
{
    public class ObjLoader
    {
        public Mesh Load(string path)
        {
            Mesh mesh = new Mesh();
            mesh.Vertices = new List<Vertex>();
            mesh.Indexes = new List<int>();

            string[] lines = File.ReadAllLines(path);
            for (int lineIndex = 0; lineIndex < lines.Length; lineIndex++)
            {
                string line = lines[lineIndex];
                if (line.StartsWith("v "))
                {
                    var vertexData = line.Substring(2).Split(' ');
                    var vertex = new Vertex
                    {
                        X = float.Parse(vertexData[0], CultureInfo.InvariantCulture),
                        Y = float.Parse(vertexData[1], CultureInfo.InvariantCulture),
                        Z = float.Parse(vertexData[2], CultureInfo.InvariantCulture)
                    };
                    mesh.Vertices.Add(vertex);
                }
                else if (line.StartsWith("f "))
                {
                    var faceData = line.Substring(2).Split(' ');
                    if (faceData.Length > 3) // Si el polígono tiene más de 3 vértices, triangulamos
                    {
                        // Triangular el polígono (asumiendo que es convexo)
                        for (int i = 1; i < faceData.Length - 1; i++)
                        {
                            mesh.Indexes.Add(int.Parse(faceData[0].Split('/')[0]) - 1);
                            mesh.Indexes.Add(int.Parse(faceData[i].Split('/')[0]) - 1);
                            mesh.Indexes.Add(int.Parse(faceData[i + 1].Split('/')[0]) - 1);
                        }
                    }
                    else
                    {
                        // Agregar triángulo directamente
                        for (int i = 0; i < faceData.Length; i++)
                        {
                            mesh.Indexes.Add(int.Parse(faceData[i].Split('/')[0]) - 1);
                        }
                    }
                }
            }
            mesh.CalculateCenter();
            return mesh;
        }
    }
}
