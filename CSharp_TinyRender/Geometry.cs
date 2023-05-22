using System;
using System.Runtime.InteropServices;

namespace CSharp_TinyRender
{
    public struct Vec2i
    {
        public int x;
        public int y;

        public int this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return x;
                    case 1:
                        return y;
                }

                throw new Exception("越界勒");
            }
            set
            {
                switch (index)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                }
            }
        }

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

    [StructLayout(LayoutKind.Sequential)]
    public struct Vec3f
    {
        public float x;
        public float y;
        public float z;

        public Vec3f(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return x;
                    case 1:
                        return y;
                    case 2:
                        return z;
                }

                throw new Exception("越界勒");
            }
            set
            {
                switch (index)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    case 2:
                        z = value;
                        break;
                }
            }
        }

        public static Vec3f operator +(Vec3f l, Vec3f r)
        {
            return new Vec3f() { x = l.x + r.x, y = l.y + r.y, z = l.y + r.y };
        }

        public static Vec3f operator -(Vec3f l, Vec3f r)
        {
            return new Vec3f() { x = l.x - r.x, y = l.y - r.y, z = l.y - r.y };
        }

        public static float operator *(Vec3f l, Vec3f r)
        {
            return l.x * r.x + l.y * r.y + l.z * r.z;
        }

        public static Vec3f operator *(Vec3f l, float f)
        {
            return new Vec3f(l.x * f, l.y * f, l.z * f);
        }

        /// <summary>
        /// 叉乘 cross
        /// </summary>
        /// <returns></returns>
        public static Vec3f operator ^(Vec3f l, Vec3f v)
        {
            return new Vec3f(l.y * v.z - l.z * v.y, l.z * v.x - l.x * v.z, l.x * v.y - l.y * v.x);
        }
    }
}