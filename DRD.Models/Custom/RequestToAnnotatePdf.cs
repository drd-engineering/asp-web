using DRD.Models.API;
using System;
using System.Collections.Generic;

namespace DRD.Models.Custom
{
    public class RequestToAnnotatePdf
    {
        public string pdffile;
        public ICollection<AnnotationDetail> annotations;

        public RequestToAnnotatePdf(string pdfstring, ICollection<DocumentAnnotationsInboxData> annotations)
        {
            this.pdffile = pdfstring;
            this.annotations = new List<AnnotationDetail>();
            foreach (DocumentAnnotationsInboxData item in annotations)
            {
                this.annotations.Add(new AnnotationDetail(item));
            }
        }
    }

    public class AnnotationDetail
    {
        public string anType;
        public int page;
        public double x;
        public double y;
        public double width;
        public double height;
        public string text;
        public double[,] points;
        public IdentityDetails identification;
        
        public AnnotationDetail(DocumentAnnotationsInboxData item)
        {
            if (item.ElementTypeId == (int)Constant.EnumElementTypeId.PEN)
                this.AddPenAnnotation(item.Text, item.Opacity);
            else if (item.ElementTypeId == (int)Constant.EnumElementTypeId.TEXT)
                this.AddTextAnnotation(item);
            else if (item.ElementTypeId == (int)Constant.EnumElementTypeId.SIGNATURE ||
                item.ElementTypeId == (int)Constant.EnumElementTypeId.INITIAL ||
                item.ElementTypeId == (int)Constant.EnumElementTypeId.PRIVATESTAMP)
                this.AddIdentityAnnotation(item);

            this.page = item.Page;
            // GET bounding Box
            // convert from px to pt
            this.x = item.LeftPosition.Value * 0.75;
            this.y = item.TopPosition.Value * 0.75;
            if (item.WidthPosition.HasValue)
                this.width = item.WidthPosition.Value * 0.75;
            else
                this.width = 0;
            if (item.HeightPosition.HasValue)
                this.height = item.HeightPosition.Value * 0.75;
            else
                this.height = 16;
        }

        private double DoubleFromString(string target)
        {
            double valueParsed;
            if (target.Contains("."))
            {
                string[] part = target.Split('.');
                int decimalLength = part[1].Length;
                double part1 = Convert.ToDouble(part[0]);
                double part2 = Convert.ToDouble(part[1]);
                valueParsed = part1 + (part2 / (10 * decimalLength));
            }
            else
                valueParsed = Convert.ToDouble(target);
            return valueParsed;
        }
        private void AddPenAnnotation(string SvgPath, double Opacity)
        {
            this.MakePenPoints(SvgPath);
            if (Opacity.Equals(1))
                this.CheckTypePenLineAnnotation();
            else
                this.CheckTypePenHighlightAnnotation();
        }
        private void MakePenPoints(string SvgPath)
        {
            // Get data about line coordinates
            string[] separator = { "M", " ", "L" };
            string[] coordinatesString = SvgPath.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            // There is an error parsing if use Convert.ToDouble and double.TryParse, so we use a method to convert it
            this.points = new double[coordinatesString.Length/2, 2];
            for (var i = 0; i < coordinatesString.Length; i += 2)
            {
                var x = this.DoubleFromString(coordinatesString[i]) * 0.75;
                var y = this.DoubleFromString(coordinatesString[i + 1]) * 0.75;
                this.points[i/2, 0] = x;
                this.points[i/2, 1] = y;
            }
        }
        private void CheckTypePenLineAnnotation()
        {
            if (this.points.GetLength(0) > 2)
                this.anType = "polyline";
            else
                this.anType = "line";
        }
        private void CheckTypePenHighlightAnnotation()
        {
            if (this.points.GetLength(0) > 2)
                this.anType = "highlighterPolyline";
            else
                this.anType = "highlighter";
        }
        private void AddIdentityAnnotation(DocumentAnnotationsInboxData item)
        {
            this.anType = "identity";
            this.identification = new IdentityDetails
            {
                encryptedUserId = item.Element.EncryptedUserId,
                imageFileName = item.AssignedAnnotationImageFileName,
                code = item.AssignedAnnotationCode,
                name = item.Element.Name,
            };
        }
        private void AddTextAnnotation(DocumentAnnotationsInboxData item)
        {
            this.anType = "text";
            this.text = item.Text;
        }
    }

    public class IdentityDetails
    {
        public string encryptedUserId;
        public string imageFileName;
        public string name;
        public string code;
    }
}
