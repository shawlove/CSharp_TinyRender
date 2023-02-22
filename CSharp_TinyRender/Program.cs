using System;

namespace CSharp_TinyRender
{
    internal class Program
    {
        static TgaColor white = new TgaColor(255, 255, 255, 255);
        static TgaColor red = new TgaColor(255, 0, 0, 255);
        static TgaColor green = new TgaColor(0, 255, 0, 255);

        const int width = 800;
        const int height = 800;

        public struct Vec2i
        {
            public int x;
            public int y;

            public Vec2i(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

            public static Vec2i operator +(Vec2i l, Vec2i r)
            {
                return new Vec2i() { x = l.x + r.x, y = l.y + r.y };
            }

            public static Vec2i operator -(Vec2i l, Vec2i r)
            {
                return new Vec2i() { x = l.x - r.x, y = l.y - r.y };
            }

            public static Vec2i operator *(Vec2i l, float r)
            {
                return new Vec2i() { x = (int)(l.x * r), y = (int)(l.y * r) };
            }
        }

        public static void Main(string[] args)
        {
            var model = Model.Create("obj/african_head.obj");
            var image = TgaImage.Create(width, height, (int)Format.RGB);
            Vec2i[] t0 = { new Vec2i(10, 70), new Vec2i(50, 160), new Vec2i(70, 80) };
            Vec2i[] t1 = { new Vec2i(180, 50), new Vec2i(150, 1), new Vec2i(70, 180) };
            Vec2i[] t2 = { new Vec2i(180, 150), new Vec2i(120, 160), new Vec2i(130, 180) };
            Triangle2(t0[0], t0[1], t0[2], image, red);
            Triangle2(t1[0], t1[1], t1[2], image, white);
            Triangle2(t2[0], t2[1], t2[2], image, green);
            image.flip_vertically(); // i want to have the origin at the left bottom corner of the image
            image.write_tga_file("Triangle2.tga");
        }

        //按y排序，分为t0、t1、t2. 按y轴向上画t0-t2 t0-t1两条线,通过高度比映射出x值。到t1的时候就截断了三角形。
        static void Triangle1(Vec2i t0, Vec2i t1, Vec2i t2, TgaImage image, TgaColor color)
        {
            // sort the vertices, t0, t1, t2 lower−to−upper (bubblesort yay!) 
            if (t0.y > t1.y) (t0, t1) = (t1, t0);
            if (t0.y > t2.y) (t0, t2) = (t2, t0);
            if (t1.y > t2.y) (t1, t2) = (t2, t1);

            int total_height = t2.y - t0.y;
            for (int y = t0.y; y <= t1.y; y++)
            {
                int segment_height = t1.y - t0.y + 1;
                float alpha = (float)(y - t0.y) / total_height;
                float beta = (float)(y - t0.y) / segment_height; // be careful with divisions by zero 
                Vec2i A = t0 + (t2 - t0) * alpha;
                Vec2i B = t0 + (t1 - t0) * beta;

                image.set(A.x, y, red);
                image.set(B.x, y, green);
            }
        }

        //y值相同，把A、B之间的x轴填满即可。且画上下两个部分
        static void Triangle2(Vec2i t0, Vec2i t1, Vec2i t2, TgaImage image, TgaColor color)
        {
            // sort the vertices, t0, t1, t2 lower−to−upper (bubblesort yay!) 
            if (t0.y > t1.y) (t0, t1) = (t1, t0);
            if (t0.y > t2.y) (t0, t2) = (t2, t0);
            if (t1.y > t2.y) (t1, t2) = (t2, t1);

            int total_height = t2.y - t0.y;
            for (int y = t0.y; y <= t1.y; y++)
            {
                int segment_height = t1.y - t0.y + 1;
                float alpha = (float)(y - t0.y) / total_height;
                float beta = (float)(y - t0.y) / segment_height; // be careful with divisions by zero 
                Vec2i A = t0 + (t2 - t0) * alpha;
                Vec2i B = t0 + (t1 - t0) * beta;

                if (A.x > B.x) (A, B) = (B, A);
                for (int j = A.x; j <= B.x; j++)
                {
                    image.set(j, y, color); // attention, due to int casts t0.y+i != A.y 
                }
            }

            for (int y = t1.y; y <= t2.y; y++)
            {
                int segment_height = t2.y - t1.y + 1;
                float alpha = (float)(y - t0.y) / total_height;
                float beta = (float)(y - t1.y) / segment_height; // be careful with divisions by zero 
                Vec2i A = t0 + (t2 - t0) * alpha;
                Vec2i B = t1 + (t2 - t1) * beta;
                if (A.x > B.x) (A, B) = (B, A);
                for (int j = A.x; j <= B.x; j++)
                {
                    image.set(j, y, color); // attention, due to int casts t0.y+i != A.y 
                }
            }
        }
    }
}