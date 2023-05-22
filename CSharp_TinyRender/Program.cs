using System;

namespace CSharp_TinyRender
{
    internal class Program
    {
        static TgaColor white = new TgaColor(255, 255, 255, 255);
        static TgaColor red = new TgaColor(255, 0, 0, 255);
        static TgaColor green = new TgaColor(0, 255, 0, 255);

        const int width = 200;
        const int height = 200;

        public static void Main(string[] args)
        {
            var random = new Random();
            var model = Model.Create("obj/african_head.obj");
            var image = TgaImage.Create(width, height, (int)Format.RGB);
            for (int i = 0; i < model.nfaces(); i++)
            {
                int[] face = model.face(i);
                Vec2i[] screen_coords = new Vec2i[3];
                for (int j = 0; j < 3; j++)
                {
                    Vec3f world_coords = model.vert(face[j]);
                    screen_coords[j] = new Vec2i((int)((world_coords.x + 1f) * width / 2f),
                        (int)((world_coords.y + 1f) * height / 2f));
                }

                Triangle(screen_coords, image,
                    new TgaColor(Convert.ToByte(random.Next() % 255), Convert.ToByte(random.Next() % 255),
                        Convert.ToByte(random.Next() % 255), 255));
            }

            image.flip_vertically(); // i want to have the origin at the left bottom corner of the image
            image.write_tga_file("Triangle3.tga");
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

        //优化一下
        static void Triangle3(Vec2i t0, Vec2i t1, Vec2i t2, TgaImage image, TgaColor color)
        {
            if (t0.y == t1.y && t0.y == t2.y) return; // I dont care about degenerate triangles 
            // sort the vertices, t0, t1, t2 lower−to−upper (bubblesort yay!) 
            if (t0.y > t1.y) (t0, t1) = (t1, t0);
            if (t0.y > t2.y) (t0, t2) = (t2, t0);
            if (t1.y > t2.y) (t1, t2) = (t2, t1);

            int total_height = t2.y - t0.y;
            for (int i = 0; i < total_height; i++)
            {
                bool second_half = i > t1.y - t0.y || t1.y == t0.y;
                int segment_height = second_half ? t2.y - t1.y : t1.y - t0.y;
                float alpha = (float)i / total_height;
                float beta = (float)(i - (second_half ? t1.y - t0.y : 0)) /
                             segment_height; // be careful: with above conditions no division by zero here 
                Vec2i A = t0 + (t2 - t0) * alpha;
                Vec2i B = second_half ? t1 + (t2 - t1) * beta : t0 + (t1 - t0) * beta;
                if (A.x > B.x) (A, B) = (B, A);
                for (int j = A.x; j <= B.x; j++)
                {
                    image.set(j, t0.y + i, color); // attention, due to int casts t0.y+i != A.y 
                }
            }
        }


        /*
         * 重心坐标 barycentric coordinates： P = (1-u-v)A + uB + vC
         * 其中 u v （1-u-v） 都>=0 就代表点P在三角形ABC中
         * 推导可得：（u,v,1） =  (ABx,ACx,PAx) 叉乘  (ABy,ACy,PAy)
         *
         * 推导：
         * 1. 展开公式 P=A + u(B-A) + v(C-A)
         * 2. 换成向量的概念 0 = PA + uAB + vAC
         * 3. xy拆开 0 = PAx + uABx + vACx  0 = PAy + uABy + vACy
         * 4. 转换成矩阵：
         *                        PAx
         *             1,u,v  X   ABx  = 0
         *                        ACx
         *    y值同上
         *    相当于(1,u,v) 垂直 (PAx,ABx,ACx) 和 (PAy,ABy,ACy)
         *    也就是说 (1,u,v) =  (PAx,ABx,ACx) 叉乘 (PAy,ABy,ACy)
         *
         *    最后判断(1-u-v) u v 都>=0即可 
         */

        static Vec3f Barycentric(Vec2i[] pts, Vec2i P)
        {
            Vec3f xForward = new Vec3f(pts[2][0] - pts[0][0], pts[1][0] - pts[0][0], pts[0][0] - P[0]);
            Vec3f yForward = new Vec3f(pts[2][1] - pts[0][1], pts[1][1] - pts[0][1], pts[0][1] - P[1]);
            Vec3f u = xForward ^ yForward;
            /* `pts` and `P` has integer value as coordinates
               so `abs(u[2])` < 1 means `u[2]` is 0, that means
               triangle is degenerate, in this case return something with negative coordinates */
            if (Math.Abs(u.z) < 1) return new Vec3f(-1, 1, 1);
            return new Vec3f(1 - (u.x + u.y) / u.z, u.y / u.z, u.x / u.z);
        }

        static void Triangle(Vec2i[] pts, TgaImage image, TgaColor color)
        {
            //算出box盒
            Vec2i bboxmin = new Vec2i(image.get_width() - 1, image.get_height() - 1);
            Vec2i bboxmax = new Vec2i(0, 0);
            Vec2i clamp = new Vec2i(image.get_width() - 1, image.get_height() - 1);
            for (int i = 0; i < 3; i++)
            {
                bboxmin.x = Math.Max(0, Math.Min(bboxmin.x, pts[i].x));
                bboxmin.y = Math.Max(0, Math.Min(bboxmin.y, pts[i].y));

                bboxmax.x = Math.Min(clamp.x, Math.Max(bboxmax.x, pts[i].x));
                bboxmax.y = Math.Min(clamp.y, Math.Max(bboxmax.y, pts[i].y));
            }

            //遍历box盒中所有像素，判断是否再三角形内
            Vec2i P;
            for (P.x = bboxmin.x; P.x <= bboxmax.x; P.x++)
            {
                for (P.y = bboxmin.y; P.y <= bboxmax.y; P.y++)
                {
                    Vec3f bc_screen = Barycentric(pts, P);
                    if (bc_screen.x < 0 || bc_screen.y < 0 || bc_screen.z < 0) continue;
                    image.set(P.x, P.y, color);
                }
            }
        }
    }
}