using System;
using System.Runtime.InteropServices;

namespace CSharp_TinyRender
{
    public static class ModelExtern
    {
        private const string DLL = "TGADLL";

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr model_create(string fileName);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int nverts(IntPtr model);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int nfaces(IntPtr model);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr vert(IntPtr model, int i);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr face(IntPtr model, int idx,ref int count);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Vec3f
    {
        public float x;
        public float y;
        public float z;
    }

    public class Model
    {
        public static Model Create(string fileName)
        {
            return new Model(ModelExtern.model_create(fileName));
        }
        
        private readonly IntPtr _ptr;

        public Model(IntPtr ptr)
        {
            _ptr = ptr;
        }

        public int nverts()
        {
            return ModelExtern.nverts(_ptr);
        }


        public int nfaces()
        {
            return ModelExtern.nfaces(_ptr);
        }


        public Vec3f vert(int i)
        {
            return Marshal.PtrToStructure<Vec3f>(ModelExtern.vert(_ptr, i));
        }


        public int[] face(int idx)
        {
            int count = 0;
            IntPtr arr= ModelExtern.face(_ptr, idx, ref count);
            int[] arrayRes = new int[count];
            Marshal.Copy(arr, arrayRes, 0, count);
            return arrayRes;
        }
    }
}