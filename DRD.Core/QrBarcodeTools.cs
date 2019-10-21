using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;
using System.Based.Core.Entity;
using DRD.Domain;
using System.Configuration;
using Geocoding.Google;
using Geocoding;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using ZXing;
using ZXing.QrCode;
using System.Drawing;
using System.IO;

namespace DRD.Core
{
    public class QrBarcodeTools
    {

        public sbyte[] Generate(string datatext)
        {
            //var QrcodeContent = context.AllAttributes["content"].Value.ToString();
            var alt = datatext;// context.AllAttributes["alt"].Value.ToString();
            var width = 250; // width of the Qr Code   
            var height = 250; // height of the Qr Code   
            var margin = 0;
            var qrCodeWriter = new ZXing.BarcodeWriterPixelData
            {
                Format = ZXing.BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions
                {
                    Height = height,
                    Width = width,
                    Margin = margin
                }
            };
            var pixelData = qrCodeWriter.Write(datatext);
            // creating a bitmap from the raw pixel data; if only black and white colors are used it makes no difference   
            // that the pixel data ist BGRA oriented and the bitmap is initialized with RGB   
            using (var bitmap = new System.Drawing.Bitmap(pixelData.Width, pixelData.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb))
            using (var ms = new MemoryStream())
            {
                var bitmapData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, pixelData.Width, pixelData.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                try
                {
                    // we assume that the row stride of the bitmap is aligned to 4 byte multiplied by the width of the image   
                    System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
                }
                finally
                {
                    bitmap.UnlockBits(bitmapData);
                }
                // save to stream as PNG   
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                sbyte[] datas=(sbyte[])(object)ms.ToArray();
                return datas;
                //output.TagName = "img";
                //output.Attributes.Clear();
                //output.Attributes.Add("width", width);
                //output.Attributes.Add("height", height);
                //output.Attributes.Add("alt", alt);
                //output.Attributes.Add("src", String.Format("data:image/png;base64,{0}", Convert.ToBase64String(ms.ToArray())));
            }
        }
    }
}
