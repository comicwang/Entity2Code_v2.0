using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace Utility
{
    public class ConfirmResource
    {
        public static void Copy()
        {
            string ini = Properties.Resource.CodeIni;
            string iniPath = Path.Combine(ConstCommon.RootPath, "Code.ini");
            if (!File.Exists(iniPath))
                FileOprateHelp.SaveTextFile(ini, iniPath);

            Bitmap img = Properties.Resource.img;
            string imgPath = Path.Combine(ConstCommon.RootPath, "img.jpg");
            if (!File.Exists(imgPath))
                img.Save(imgPath, System.Drawing.Imaging.ImageFormat.Jpeg);


        }
    }
}
