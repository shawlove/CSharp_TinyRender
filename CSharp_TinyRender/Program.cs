namespace CSharp_TinyRender
{
    internal class Program
    {
        static TgaColor white = new TgaColor(255, 255, 255, 255);
        static TgaColor red = new TgaColor(255, 0, 0, 255);

        public static void Main(string[] args)
        {
            var image = TgaImageExtern.Tga_Create(100, 100, (int)Format.RGB);
            image.set(52, 41, red);
            image.flip_vertically(); // i want to have the origin at the left bottom corner of the image
            image.write_tga_file("output.tga");
        }
    }
}