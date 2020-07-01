using DRD.Models.API;
using System.ComponentModel.DataAnnotations.Schema;

namespace DRD.Models
{

    public class DocumentAnnotationsInboxData
    {
        public long Id { get; set; } // Id (Primary key)
        public int Page { get; set; } // Page
        public double? LeftPosition { get; set; } // LeftPosition
        public double? TopPosition { get; set; } // TopPosition
        public double? WidthPosition { get; set; } // WidthPosition
        public double? HeightPosition { get; set; } // HeightPosition
        public string Color { get; set; } // Color (length: 50)
        public string BackColor { get; set; } // BackColor (length: 50)
        public string Text { get; set; } // Data
        public string Unknown { get; set; } // Data2
        public int Rotation { get; set; } // Rotation
        public double ScaleX { get; set; } // ScaleX
        public double ScaleY { get; set; } // ScaleY
        public double TransitionX { get; set; } // TransitionX
        public double TransitionY { get; set; } // TransitionY
        public double StrokeWidth { get; set; } // StrokeWidth
        public double Opacity { get; set; } // Opacity
        public long? CreatorId { get; set; } // CreatorId
        public long? UserId { get; set; } // ElementId
        public bool IsDeleted { get; set; }
        public int Flag { get; set; } // Flag
        public string AssignedAnnotationCode { get; set; } // FlagCode (length: 20)
        public System.DateTime? AssignedAt { get; set; } // FlagDate
        public string AssignedAnnotationImageFileName { get; set; } // FlagImage (length: 100)
        public string EmailOfUserAssigned { get; set; } // UserId (length: 50)
        public System.DateTime? CreatedAt { get; set; } // DateCreated
        public System.DateTime? UpdatedAt { get; set; } // DateUpdated
        public long DocumentId { set; get; }
        public int ElementTypeId { set; get; }
        [ForeignKey("DocumentId")]
        public virtual DocumentInboxData Document { get; set; } // FK_DocumentAnnotate_Document
        public Element Element { get; set; }

        public DocumentAnnotationsInboxData()
        {
            Rotation = 0;
            ScaleX = 1;
            ScaleY = 1;
            TransitionX = 0;
            TransitionY = 0;
            StrokeWidth = 4;
            Opacity = 1;
            Flag = 0;
            // Annotate = new JsonAnnotate();
        }
        public DocumentAnnotationsInboxData(DocumentAnnotation item)
        {
            this.Id = item.Id;
            this.Page = item.Page;
            this.LeftPosition = item.LeftPosition;
            this.TopPosition = item.TopPosition;
            this.WidthPosition = item.WidthPosition;
            this.HeightPosition = item.HeightPosition;
            this.Color = item.Color;
            this.BackColor = item.BackColor;
            this.Text = item.Text;
            this.Unknown = item.Unknown;
            this.Rotation = item.Rotation;
            this.ScaleX = item.ScaleX;
            this.ScaleY = item.ScaleY;
            this.TransitionX = item.TransitionX;
            this.TransitionY = item.TransitionY;
            this.StrokeWidth = item.StrokeWidth;
            this.Opacity = item.Opacity;
            this.CreatorId = item.CreatorId;
            this.UserId = item.UserId;
            this.Flag = item.Flag;
            this.AssignedAnnotationCode = item.AssignedAnnotationCode;
            this.AssignedAt = item.AssignedAt;
            this.AssignedAnnotationImageFileName = item.AssignedAnnotationImageFileName;
            this.EmailOfUserAssigned = item.EmailOfUserAssigned;
            this.DocumentId = item.DocumentId;
            this.ElementTypeId = item.ElementTypeId;
            this.Element = item.Element;
        }
    }
}
