
using System.Drawing;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PLAYGROUND
{
    public class Renderer
    {
        Canvas canvas;

        public Renderer(Canvas canvas)
        {

            this.canvas = canvas;
        }

        private void Swap(ref Vertex v1, ref Vertex v2)
        {
            Vertex temp = v1;
            v1 = v2;
            v2 = temp;
        }

        public void SortByY(Triangle triangle)
        {
            List<Vertex> vertices = new List<Vertex> { triangle.A, triangle.B, triangle.C };
            vertices.Sort((v1, v2) => v1.Position.Y.CompareTo(v2.Position.Y));
            triangle.A = vertices[0];
            triangle.B = vertices[1];
            triangle.C = vertices[2];
        }


        private List<float> Interpolate(float start, float end, int steps)
        {
            List<float> values = new List<float>();

            if (steps <= 0)
            {
                values.Add(start);
                return values;
            }

            float stepSize = (end - start) / steps;
            float value = start;

            for (int i = 0; i <= steps; i++)
            {
                values.Add(value);
                value += stepSize;
            }

            return values;
        }

        public void DrawLine(Vertex v0, Vertex v1, Color color)
        {
            int steps = Math.Max(Math.Abs((int)(v1.X - v0.X)), Math.Abs((int)(v1.Y - v0.Y)));
            List<float> xValues = Interpolate(v0.X, v1.X, steps);
            List<float> yValues = Interpolate(v0.Y, v1.Y, steps);

            for (int i = 0; i <= steps; i++)
            {
                canvas.SetPixel((int)Math.Round(xValues[i]), (int)Math.Round(yValues[i]), color);
            }
        }

        private Color CalculateShadedColorVertex(Color start, Color end, float ratio)
        {
            int r = Clamp((int)(start.R + ratio * (end.R - start.R)), 0, 255);
            int g = Clamp((int)(start.G + ratio * (end.G - start.G)), 0, 255);
            int b = Clamp((int)(start.B + ratio * (end.B - start.B)), 0, 255);
            return Color.FromArgb(r, g, b);
        }

        public static int Clamp(int value, int min, int max)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }

        public void DrawFilledTriangle(PointF p0, PointF p1, PointF p2)
        {
            List<PointF> points = new List<PointF> { p0, p1, p2 };
            points.Sort((a, b) => a.Y.CompareTo(b.Y));

            p0 = points[0];
            p1 = points[1];
            p2 = points[2];

            int y0 = (int)p0.Y;
            int y1 = (int)p1.Y;
            int y2 = (int)p2.Y;

            List<float> x01 = Interpolate(p0.X, p1.X, y1 - y0);
            List<float> x02 = Interpolate(p0.X, p2.X, y2 - y0);
            List<float> x12 = Interpolate(p1.X, p2.X, y2 - y1);

            for (int y = y0; y <= y2; y++)
            {
                int xStart = y < y1 ? (int)x01[y - y0] : (int)x12[y - y1];
                int xEnd = (int)x02[y - y0];

                if (xStart > xEnd) // Asegúrate de que xStart es siempre menor o igual a xEnd
                {
                    int temp = xStart;
                    xStart = xEnd;
                    xEnd = temp;
                }

                for (int x = xStart; x <= xEnd; x++)
                {
                    canvas.SetPixel(x, y, Color.Blue);
                }
            }
        }



        public void RenderTriangle(PointF p1, PointF p2, PointF p3)
        {
            Vertex v1 = new Vertex { X = p1.X, Y = p1.Y, Z = 0, Color = Color.White };
            Vertex v2 = new Vertex { X = p2.X, Y = p2.Y, Z = 0, Color = Color.White };
            Vertex v3 = new Vertex { X = p3.X, Y = p3.Y, Z = 0, Color = Color.White };

            Triangle triangle = new Triangle(v1, v2, v3);
            SortByY(triangle);
            DrawLine(triangle.A, triangle.B, Color.White);
            DrawLine(triangle.B, triangle.C, Color.White);
            DrawLine(triangle.C, triangle.A, Color.White);
        }

        public PointF PerspectiveTransform(Vertex vertex, float cameraZ, float focalLength)
        {
            float z = vertex.Z - cameraZ;
            if (z == 0) z = 0.001f; // Prevent division by zero
            return new PointF(
                (vertex.X * focalLength / z) + canvas.Width / 2,
                (vertex.Y * focalLength / z) + canvas.Height / 2
            );
        }

        public void RenderScene(Scene scene)
        {
            canvas.FastClear(); // Clears the canvas

            float cameraZ = -5.0f;
            float focalLength = 200;

            for (int j = 0; j < scene.Models.Count; j++)
            {
                Mesh mesh = scene.Models[j];
                List<Vertex> rotatedVertices = new List<Vertex>();

                for (int i = 0; i < mesh.Vertices.Count; i++)
                {
                    Vertex vertex = mesh.Vertices[i];
                    Vertex rotated = Rotaciones.Rot(mesh.Transform.RotationX, vertex, 'X');
                    rotated = Rotaciones.Rot(mesh.Transform.RotationY, rotated, 'Y');
                    rotated = Rotaciones.Rot(mesh.Transform.RotationZ, rotated, 'Z');
                    rotatedVertices.Add(rotated);
                }

                for (int i = 0; i < mesh.Indexes.Count; i += 3)
                {
                    Vertex v1 = rotatedVertices[mesh.Indexes[i]];
                    Vertex v2 = rotatedVertices[mesh.Indexes[i + 1]];
                    Vertex v3 = rotatedVertices[mesh.Indexes[i + 2]];

                    PointF p1 = PerspectiveTransform(v1, cameraZ, focalLength);
                    PointF p2 = PerspectiveTransform(v2, cameraZ, focalLength);
                    PointF p3 = PerspectiveTransform(v3, cameraZ, focalLength);
                    DrawFilledTriangle(p1, p2, p3);
                    RenderTriangle(p1, p2, p3);
                    
                }
            }

            canvas.Refresh(); // Refreshes the PictureBox to display the new content
        }



    }
}
