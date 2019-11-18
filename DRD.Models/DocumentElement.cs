using System;
using System.ComponentModel.DataAnnotations.Schema;
using DRD.Models.API;

namespace DRD.Models
{
    [Table("DocumentElements", Schema = "public")]
    public class DocumentElement
    {
        public long Id { get; set; } // Id (Primary key)
        public int Page { get; set; } // Page
        public double? LeftPos { get; set; } // LeftPos
        public double? TopPos { get; set; } // TopPos
        public double? WidthPos { get; set; } // WidthPos
        public double? HeightPos { get; set; } // HeightPos
        public string Color { get; set; } // Color (length: 50)
        public string BackColor { get; set; } // BackColor (length: 50)
        public string Data { get; set; } // Data
        public string Data2 { get; set; } // Data2
        public int Rotation { get; set; } // Rotation
        public double ScaleX { get; set; } // ScaleX
        public double ScaleY { get; set; } // ScaleY
        public double TransX { get; set; } // TransX
        public double TransY { get; set; } // TransY
        public double StrokeWidth { get; set; } // StrokeWidth
        public double Opacity { get; set; } // Opacity
        public long? CreatorId { get; set; } // CreatorId
        public long? ElementId { get; set; } // AnnotateId
        public int Flag { get; set; } // Flag
        public string FlagCode { get; set; } // FlagCode (length: 20)
        public System.DateTime? FlagDate { get; set; } // FlagDate
        public string FlagImage { get; set; } // FlagImage (length: 100)
        public string UserId { get; set; } // UserId (length: 50)
        public System.DateTime DateCreated { get; set; } // DateCreated
        public System.DateTime? DateUpdated { get; set; } // DateUpdated

        public Element Element { get; set; }

        // Foreign keys
        public virtual ElementType ElementType { get; set; } // FK_DocumentAnnotate_AnnotateType
        
        public virtual Document Document { get; set; } // FK_DocumentAnnotate_Document

        public DocumentElement()
        {
            Rotation = 0;
            ScaleX = 1;
            ScaleY = 1;
            TransX = 0;
            TransY = 0;
            StrokeWidth = 4;
            Opacity = 1;
            Flag = 0;
           // Annotate = new JsonAnnotate();
        }
    }
}
