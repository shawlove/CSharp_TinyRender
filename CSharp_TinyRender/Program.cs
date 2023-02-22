using System;

namespace CSharp_TinyRender
{
    internal class Program
    {
        static TgaColor white = new TgaColor(255, 255, 255, 255);
        static TgaColor red = new TgaColor(255, 0, 0, 255);

        const int width = 800;
        const int height = 800;

        public static void Main(string[] args)
        {
            // var image = TgaImage.Create(100, 100, (int)Format.RGB);
            // Line5(13, 20, 80, 40, image, white);
            // Line5(20, 13, 40, 80, image, red);
            // Line5(80, 40, 13, 20, image, red);
            // image.flip_vertically(); // i want to have the origin at the left bottom corner of the image
            // image.write_tga_file("attempt5.tga");

            var model = Model.Create("obj/african_head.obj");
            var image = TgaImage.Create(width, height, (int)Format.RGB);
            for (int i = 0; i < model.nfaces(); i++)
            {
                int[] face = model.face(i);
                for (int j = 0; j < 3; j++)
                {
                    Vec3f v0 = model.vert(face[j]);
                    Vec3f v1 = model.vert(face[(j + 1) % 3]);
                    int x0 = (int)((v0.x + 1) * width / 2f);
                    int y0 = (int)((v0.y + 1) * height / 2f);
                    int x1 = (int)((v1.x + 1) * width / 2f);
                    int y1 = (int)((v1.y + 1) * height / 2f);
                    Line5(x0, y0, x1, y1, image, white);
                }
            }
            
            image.flip_vertically(); // i want to have the origin at the left bottom corner of the image
            image.write_tga_file("Wireframe.tga");
        }

        //First attempt 取决于0.01
        public static void Line1(int x0, int y0, int x1, int y1, TgaImage image, TgaColor color)
        {
            for (float t = 0; t < 1; t += 0.01f)
            {
                int x = x0 + (int)((x1 - x0) * t);
                int y = y0 + (int)((y1 - y0) * t);
                image.set(x, y, color);
            }
        }

        //Second attempt 没有排序，坐标顺序不同结果不同 且y差值比x大，则y轴会有洞
        public static void Line2(int x0, int y0, int x1, int y1, TgaImage image, TgaColor color)
        {
            for (int i = x0; i <= x1; i++)
            {
                float t = (float)(i - x0) / (x1 - x0);
                int y = (int)((y1 - y0) * t);
                image.set(i, y, color);
            }
        }

        //Third attempt 解决了line2的问题
        public static void Line3(int x0, int y0, int x1, int y1, TgaImage image, TgaColor color)
        {
            bool steep = false;
            if (Math.Abs(x0 - x1) < Math.Abs(y0 - y1))
            {
                // if the line is steep, we transpose the image 
                (x0, y0) = (y0, x0);
                (x1, y1) = (y1, x1);
                steep = true;
            }

            if (x0 > x1)
            {
                // make it left−to−right 
                (x0, x1) = (x1, x0);
                (y0, y1) = (y1, y0);
            }

            for (int x = x0; x <= x1; x++)
            {
                float t = (x - x0) / (float)(x1 - x0);
                int y = (int)(y0 * (1 - t) + y1 * t);
                if (steep)
                {
                    image.set(y, x, color); // if transposed, de−transpose 
                }
                else
                {
                    image.set(x, y, color);
                }
            }

            // void Swap(ref int x, ref int y)
            // {
            //     (x, y) = (y, x);
            // }
        }

        //Final attempt 算法上的优化
        static void Line5(int x0, int y0, int x1, int y1, TgaImage image, TgaColor color)
        {
            bool steep = false;
            if (Math.Abs(x0 - x1) < Math.Abs(y0 - y1))
            {
                // if the line is steep, we transpose the image 
                (x0, y0) = (y0, x0);
                (x1, y1) = (y1, x1);
                steep = true;
            }

            if (x0 > x1)
            {
                // make it left−to−right 
                (x0, x1) = (x1, x0);
                (y0, y1) = (y1, y0);
            }

            int dx = x1 - x0;
            int dy = y1 - y0;
            int derror2 = Math.Abs(dy) * 2;
            int error2 = 0;
            int y = y0;
            for (int x = x0; x <= x1; x++)
            {
                if (steep)
                {
                    image.set(y, x, color);
                }
                else
                {
                    image.set(x, y, color);
                }

                error2 += derror2;
                if (error2 > dx)
                {
                    y += (y1 > y0 ? 1 : -1);
                    error2 -= dx * 2;
                }
            }
        }
    }
}