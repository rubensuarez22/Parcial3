using System;
using System.Windows.Forms;

namespace PLAYGROUND
{
    public partial class MyForm : Form
    {
        Scene scene;
        Renderer renderer;
        Canvas canvas;

        public MyForm()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            canvas = new Canvas(PCT_CANVAS);
            renderer = new Renderer(canvas);
            scene = new Scene();

            ObjLoader loader = new ObjLoader();
            Mesh model = loader.Load("Cube.obj"); // Asegúrate de especificar la ruta correcta del archivo
            scene.AddModel(model);
        }

        private void MyForm_SizeChanged(object sender, EventArgs e)
        {
            Init();
        }

        private void BTN_Rotate_Click(object sender, EventArgs e)
        {
            // Obtén la referencia al Mesh que deseas rotar
            Mesh modelToRotate = scene.Models[0]; // Asume que solo hay un modelo en la escena

            // Rota el modelo en los ejes X, Y y Z
            modelToRotate.Transform.Rotate(1, 0, 0); // Ajusta los valores de rotación según sea necesario

            // Fuerza el renderizado actualizado
            renderer.RenderScene(scene);
        }


        private void TIMER_Tick(object sender, EventArgs e)
        {
            /*Mesh modelToRotate = scene.Models[0]; // Asume que solo hay un modelo en la escena

            // Rota el modelo en los ejes X, Y y Z
            modelToRotate.Transform.Rotate(1, 0, 0);
            renderer.RenderScene(scene);*/
        }

        private void PCT_CANVAS_MouseMove(object sender, MouseEventArgs e)
        {
            LBL_STATUS.Text = e.Location.ToString() + canvas.bmp.Size;
        }
    }
}