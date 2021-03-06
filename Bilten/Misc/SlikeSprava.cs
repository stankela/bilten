using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Reflection;
using System.IO;
using Bilten.Domain;

namespace Bilten
{
    public static class SlikeSprava
    {
        private static Image[] slike;

        static SlikeSprava()
        {
            Sprava[] sprave = (Sprava[])Enum.GetValues(typeof(Sprava));
            int max = 0;
            foreach (Sprava s in sprave)
                max = Math.Max(max, (int)s);

            slike = new Image[max + 1];

            Assembly asm = Assembly.GetAssembly(typeof(Program));
            slike[(int)Sprava.Parter] = new Bitmap(asm.GetManifestResourceStream("Bilten.Images.Parter.bmp"));
            slike[(int)Sprava.Konj] = new Bitmap(asm.GetManifestResourceStream("Bilten.Images.Konj.bmp"));
            slike[(int)Sprava.Karike] = new Bitmap(asm.GetManifestResourceStream("Bilten.Images.Karike.bmp"));
            slike[(int)Sprava.Preskok] = new Bitmap(asm.GetManifestResourceStream("Bilten.Images.Preskok.bmp"));
            slike[(int)Sprava.Razboj] = new Bitmap(asm.GetManifestResourceStream("Bilten.Images.Razboj.bmp"));
            slike[(int)Sprava.Vratilo] = new Bitmap(asm.GetManifestResourceStream("Bilten.Images.Vratilo.bmp"));
            slike[(int)Sprava.DvovisinskiRazboj] = new Bitmap(asm.GetManifestResourceStream("Bilten.Images.DvovisinskiRazboj.bmp"));
            slike[(int)Sprava.Greda] = new Bitmap(asm.GetManifestResourceStream("Bilten.Images.Greda.bmp"));
            slike[(int)Sprava.PraznaSprava1] = new Bitmap(asm.GetManifestResourceStream("Bilten.Images.Pause1.jpg"));
            slike[(int)Sprava.PraznaSprava2] = new Bitmap(asm.GetManifestResourceStream("Bilten.Images.Pause2.jpg"));
            slike[(int)Sprava.PraznaSprava3] = new Bitmap(asm.GetManifestResourceStream("Bilten.Images.Pause3.jpg"));
            slike[(int)Sprava.PraznaSprava4] = new Bitmap(asm.GetManifestResourceStream("Bilten.Images.Pause4.jpg"));
            slike[(int)Sprava.PraznaSprava5] = new Bitmap(asm.GetManifestResourceStream("Bilten.Images.Pause5.jpg"));
            slike[(int)Sprava.PraznaSprava6] = new Bitmap(asm.GetManifestResourceStream("Bilten.Images.Pause6.jpg"));
        }

        public static Image getImage(Sprava sprava)
        {
            return slike[(int)sprava];
        }
    }
}
