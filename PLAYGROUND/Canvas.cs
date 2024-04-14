using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PLAYGROUND
{
    public class Canvas
    {
        static PictureBox pctCanvas;

        Size size;
        public Bitmap bmp;
        public float Width { get; private set; }
        public float Height { get; private set; }
        
        byte[] bits;
        Graphics g;
        int pixelFormatSize, stride;
        Rectangle rect;

        public Canvas(PictureBox pctCanvas)
        {
            Canvas.pctCanvas = pctCanvas;
            this.size = pctCanvas.Size;
            Init(size.Width, size.Height);
            pctCanvas.Image = bmp;
        }

        private void Init(int width, int height)
        {
            PixelFormat format;
            GCHandle handle;
            IntPtr bitPtr;
            int padding;

            format              = PixelFormat.Format32bppArgb;
            Width               = width;
            Height              = height;
            pixelFormatSize     = Image.GetPixelFormatSize(format) / 8; // 8 bits = 1 byte
            stride              = width * pixelFormatSize; // total pixels (width) times ARGB (4)
            padding             = (stride % 4); // PADD = move every pixel in bytes
            stride             += padding == 0 ? 0 : 4 - padding; // 4 byte multiple Alpha, Red, Green, Blue
            bits                = new byte[stride * height]; // total pixels (width) times ARGB (4) times Height
            handle              = GCHandle.Alloc(bits, GCHandleType.Pinned); // TO LOCK THE MEMORY
            bitPtr              = Marshal.UnsafeAddrOfPinnedArrayElement(bits, 0);
            bmp                 = new Bitmap(width, height, stride, format, bitPtr);
            g                   = Graphics.FromImage(bmp); // Para hacer pruebas regulares}
            rect                = new Rectangle(0, 0, bmp.Width, bmp.Height);
        }

        public void FastClear()
        {
            int div = 16;

            Parallel.For(0, bits.Length / div, i => // unrolling 
            {
                bits[(i * div) + 0] = 0;
                bits[(i * div) + 1] = 0;
                bits[(i * div) + 2] = 0;
                bits[(i * div) + 3] = 0;

                bits[(i * div) + 4] = 0;
                bits[(i * div) + 5] = 0;
                bits[(i * div) + 6] = 0;
                bits[(i * div) + 7] = 0;

                bits[(i * div) + 8] = 0;
                bits[(i * div) + 9] = 0;
                bits[(i * div) + 10] = 0;
                bits[(i * div) + 11] = 0;

                bits[(i * div) + 12] = 0;
                bits[(i * div) + 13] = 0;
                bits[(i * div) + 14] = 0;
                bits[(i * div) + 15] = 0;
            });
        }

        public void SetPixel(int x, int y, Color color)
        {
            if (x < 0 || x >= this.Width || y < 0 || y >= this.Height) return;
            int index = y * stride + x * 4;
            bits[index] = color.B;
            bits[index + 1] = color.G;
            bits[index + 2] = color.R;
            bits[index + 3] = 255;
        }

        public void Refresh()
        {
            pctCanvas.Invalidate();
        }
          
    }
}
