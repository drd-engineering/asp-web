namespace DRD.Models.API
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
        public DocumentAnnotationsInboxData(DocumentAnnotation documentAnnotationDb)
        {
            this.Id = documentAnnotationDb.Id;
            this.Page = documentAnnotationDb.Page;
            this.LeftPosition = documentAnnotationDb.LeftPosition;
            this.TopPosition = documentAnnotationDb.TopPosition;
            this.WidthPosition = documentAnnotationDb.WidthPosition;
            this.HeightPosition = documentAnnotationDb.HeightPosition;
            this.Color = documentAnnotationDb.Color;
            this.BackColor = documentAnnotationDb.BackColor;
            this.Text = documentAnnotationDb.Text;
            this.Unknown = documentAnnotationDb.Unknown;
            this.Rotation = documentAnnotationDb.Rotation;
            this.ScaleX = documentAnnotationDb.ScaleX;
            this.ScaleY = documentAnnotationDb.ScaleY;
            this.TransitionX = documentAnnotationDb.TransitionX;
            this.TransitionY = documentAnnotationDb.TransitionY;
            this.StrokeWidth = documentAnnotationDb.StrokeWidth;
            this.Opacity = documentAnnotationDb.Opacity;
            this.CreatorId = documentAnnotationDb.CreatorId;
            this.UserId = documentAnnotationDb.UserId;
            this.Flag = documentAnnotationDb.Flag;
            this.AssignedAnnotationCode = documentAnnotationDb.AssignedAnnotationCode;
            this.AssignedAt = documentAnnotationDb.AssignedAt;
            this.AssignedAnnotationImageFileName = documentAnnotationDb.AssignedAnnotationImageFileName;
            this.EmailOfUserAssigned = documentAnnotationDb.EmailOfUserAssigned;
            this.DocumentId = documentAnnotationDb.DocumentId;
            this.ElementTypeId = documentAnnotationDb.ElementTypeId;
            this.Element = documentAnnotationDb.Element;
            this.CreatedAt = documentAnnotationDb.CreatedAt;
            this.UpdatedAt = documentAnnotationDb.UpdatedAt;
        }
    }
}
