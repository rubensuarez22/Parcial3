﻿
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

        private void Swap(ref Point p1, ref Point p2)
        {
            Point temp = p1;
            p1 = p2;
            p2 = temp;
        }

        public void SortByY(Triangle triangle)
        {
            List<Vertex> vertices = new List<Vertex> { triangle.A, triangle.B, triangle.C };
            vertices.Sort((v1, v2) => v1.Position.Y.CompareTo(v2.Position.Y));
            triangle.A = vertices[0];
            triangle.B = vertices[1];
            triangle.C = vertices[2];
        }


        public List<float> Interpolate(float start, float end, int steps)
        {
            List<float> results = new List<float>();
            float delta = (end - start) / steps;
            for (int i = 0; i <= steps; i++)
            {
                results.Add(start + delta * i);
            }
            return results;
        }


        public void DrawLine(Vertex v0, Vertex v1, Color color)
        {
            int steps = (int)Math.Max(Math.Abs(v1.X - v0.X), Math.Abs(v1.Y - v0.Y));
            List<float> xValues = Interpolate(v0.X, v1.X, steps);
            List<float> yValues = Interpolate(v0.Y, v1.Y, steps);

            for (int i = 0; i <= steps; i++)
            {
                int x = (int)Math.Round(xValues[i]);
                int y = (int)Math.Round(yValues[i]);
                canvas.SetPixel(x, y, CalculateShadedColorVertex(v0.Color, v1.Color, i / (float)steps));
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
            canvas.FastClear(); // Limpia el canvas

            float cameraZ = -5.0f; // Asumiendo que este es un buen valor para tu escena
            float focalLength = 200; // También ajusta este valor como sea necesario

            foreach (Mesh mesh in scene.Models)
            {
                // Aplica las rotaciones a cada vértice del mesh
                List<Vertex> rotatedVertices = new List<Vertex>();
                foreach (Vertex vertex in mesh.Vertices)
                {
                    Vertex rotated = vertex;
                    rotated = Rotaciones.Rot(mesh.Transform.RotationX, rotated, 'X');
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

                    RenderTriangle(p1, p2, p3);
                }
            }

            canvas.Refresh(); // Actualiza el PictureBox para mostrar el nuevo contenido
        }

    }
}
