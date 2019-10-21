using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using DRD.Domain;

namespace DRD.Core
{
    public class ImageService
    {
        public int UploadFoto(JsonDataImage dataImage)
        {
            var path = System.Web.Hosting.HostingEnvironment.MapPath(dataImage.TargetFolder) + dataImage.FileName;

            Image x = (Bitmap)((new ImageConverter()).ConvertFrom(dataImage.Data));
            x.Save(path);

            return 0;
        }

        public int UploadFoto(JsonDataImage dataImage, string oldFile)
        {
            var mappath = System.Web.Hosting.HostingEnvironment.MapPath(dataImage.TargetFolder);

            if (File.Exists(mappath + oldFile))
            {
                File.Delete(mappath + oldFile);
            }

            var path = mappath + dataImage.FileName;
            Image x = (Bitmap)((new ImageConverter()).ConvertFrom(dataImage.Data));
            x.Save(path);

            return 0;
        }
    }
}
