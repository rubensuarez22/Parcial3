
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

        public void Swap()
        { }

        public void SortByY()
        { }

        public void Interpolate()
        {}

        public void RenderTriangle()
        { }

        public void DrawLine()
        { }


        private PointF PerspectiveTransform(Vertex vertex, float cameraZ, float focalLength)
        {
            // Ajustar la profundidad relativa
            float z = vertex.Z - cameraZ;
            if (z == 0) z = 0.001f; // Evitar la división por cero

            // Proyección en perspectiva
            float xProjected = (vertex.X * focalLength / z) + canvas.Width / 2;
            float yProjected = (vertex.Y * focalLength / z) + canvas.Height / 2;

            return new PointF(xProjected, canvas.Height - yProjected); // Ajuste para la coordenada Y
        }



        public void RenderScene(Scene scene)
        {
            canvas.FastClear(); // Limpia el canvas

            float cameraZ = -5.0f; // Asumiendo que este es un buen valor para tu escena
            float focalLength = 200; // También ajusta este valor como sea necesario

            foreach (Mesh mesh in scene.Models)
            {
                mesh.CalculateCenter();
                for (int i = 0; i < mesh.Indexes.Count; i += 3)
                {
                    Vertex v1 = mesh.Vertices[mesh.Indexes[i]];
                    Vertex v2 = mesh.Vertices[mesh.Indexes[i + 1]];
                    Vertex v3 = mesh.Vertices[mesh.Indexes[i + 2]];

                    v1 = Rotaciones.Rot(mesh.Transform.RotationX, v1, 'X');
                    v2 = Rotaciones.Rot(mesh.Transform.RotationX, v2, 'X');
                    v3 = Rotaciones.Rot(mesh.Transform.RotationX, v3, 'X');

                    v1 = Rotaciones.Rot(mesh.Transform.RotationY, v1, 'Y');
                    v2 = Rotaciones.Rot(mesh.Transform.RotationY, v2, 'Y');
                    v3 = Rotaciones.Rot(mesh.Transform.RotationY, v3, 'Y');

                    v1 = Rotaciones.Rot(mesh.Transform.RotationZ, v1, 'Z');
                    v2 = Rotaciones.Rot(mesh.Transform.RotationZ, v2, 'Z');
                    v3 = Rotaciones.Rot(mesh.Transform.RotationZ, v3, 'Z');

                    PointF p1 = PerspectiveTransform(v1, cameraZ, focalLength);
                    PointF p2 = PerspectiveTransform(v2, cameraZ, focalLength);
                    PointF p3 = PerspectiveTransform(v3, cameraZ, focalLength);

                    RenderTriangle(p1, p2, p3);
                }
            }

            canvas.Refresh(); // Actualiza el PictureBox para mostrar el nuevo contenido
        }


   


        private void DrawLine(Vertex v1, Vertex v2)
        {
            int x1 = (int)v1.X;
            int y1 = (int)v1.Y;
            int x2 = (int)v2.X;
            int y2 = (int)v2.Y;

            int dx = Math.Abs(x2 - x1), sx = x1 < x2 ? 1 : -1;
            int dy = -Math.Abs(y2 - y1), sy = y1 < y2 ? 1 : -1;
            int err = dx + dy, e2;

            while (true)
            {
                canvas.SetPixel(x1, y1, Color.White);
                if (x1 == x2 && y1 == y2) break;
                e2 = 2 * err;
                if (e2 >= dy) { err += dy; x1 += sx; }
                if (e2 <= dx) { err += dx; y1 += sy; }
            }
        }

        public void RenderTriangle(PointF p1, PointF p2, PointF p3)
        {
            // Crear instancias de Vertex con los puntos proyectados
            Vertex v1 = new Vertex { X = p1.X, Y = p1.Y, Z = 0 };
            Vertex v2 = new Vertex { X = p2.X, Y = p2.Y, Z = 0 };
            Vertex v3 = new Vertex { X = p3.X, Y = p3.Y, Z = 0 };

            // Dibujar líneas para formar un triángulo
            DrawLine(v1, v2);
            DrawLine(v2, v3);
            DrawLine(v3, v1);
        }
    }
}
