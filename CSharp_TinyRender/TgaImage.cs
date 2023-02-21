using System;
using System.Runtime.InteropServices;

namespace CSharp_TinyRender
{
    public static class TgaImageExtern
    {
        private const string DLL = "TGADLL";

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr tga_create(int w, int h, int bpp);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool read_tga_file(IntPtr image, string filename);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool write_tga_file(IntPtr image, string filename, bool rle = true);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool flip_horizontally(IntPtr image);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool flip_vertically(IntPtr image);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool scale(IntPtr image, int w, int h);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr get(IntPtr image, int x, int y);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool set(IntPtr image, int x, int y, IntPtr c);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_width(IntPtr image);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_height(IntPtr image);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_bytespp(IntPtr image);

        //public static extern IntPtr buffer(IntPtr image);
        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void clear(IntPtr image);

        public static TgaImage Tga_Create(int w, int h, int bpp)
        {
            IntPtr tga = tga_create(w, h, bpp);
            return new TgaImage(tga);
        }
    }

    //bpp
    public enum Format
    {
        GRAYSCALE = 1,
        RGB = 3,
        RGBA = 4,
    };

    public class TgaImage
    {
        private readonly IntPtr _tgaImage;

        public TgaImage(IntPtr tga)
        {
            _tgaImage = tga;
        }

        public bool read_tga_file(string filename)
        {
            return TgaImageExtern.read_tga_file(_tgaImage, filename);
        }

        public bool write_tga_file(string filename, bool rle = true)
        {
            return TgaImageExtern.write_tga_file(_tgaImage, filename, rle);
        }

        public bool flip_horizontally()
        {
            return TgaImageExtern.flip_horizontally(_tgaImage);
        }

        public bool flip_vertically()
        {
            return TgaImageExtern.flip_vertically(_tgaImage);
        }

        public bool scale(int w, int h)
        {
            return TgaImageExtern.scale(_tgaImage, w, h);
        }

        public TgaColor get(int x, int y)
        {
            var ptr = TgaImageExtern.get(_tgaImage, x, y);
            return Marshal.PtrToStructure<TgaColor>(ptr);
        }

        public bool set(int x, int y, TgaColor c)
        {
            IntPtr pnt = Marshal.AllocHGlobal(Marshal.SizeOf(c));
            Marshal.StructureToPtr(c, pnt, false);
            return TgaImageExtern.set(_tgaImage, x, y, pnt);
        }

        public int get_width()
        {
            return TgaImageExtern.get_width(_tgaImage);
        }

        public int get_height()
        {
            return TgaImageExtern.get_height(_tgaImage);
        }

        public int get_bytespp()
        {
            return TgaImageExtern.get_bytespp(_tgaImage);
        }

//        unsigned char* buffer();
        public void clear()
        {
            TgaImageExtern.clear(_tgaImage);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TgaColor
    {
        public byte B;
        public byte G;
        public byte R;
        public byte A;

        public TgaColor(byte r,byte g,byte b,byte a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }
    }
}