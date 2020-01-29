﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DRD.Models.API;

namespace DRD.Models
{
    
    public class DocumentElementInboxData
    {

        public long Id { get; set; } // Id (Primary key)
        public int Page { get; set; } // Page
        public double? LeftPosition { get; set; } // LeftPos
        public double? TopPosition { get; set; } // TopPos
        public double? WidthPosition { get; set; } // WidthPos
        public double? HeightPosition { get; set; } // HeightPos
        public string Color { get; set; } // Color (length: 50)
        public string BackColor { get; set; } // BackColor (length: 50)
        public string Data { get; set; } // Data
        public string Data2 { get; set; } // Data2
        public int Rotation { get; set; } // Rotation
        public double ScaleX { get; set; } // ScaleX
        public double ScaleY { get; set; } // ScaleY
        public double TransitionX { get; set; } // TransX
        public double TransitionY { get; set; } // TransY
        public double StrokeWidth { get; set; } // StrokeWidth
        public double Opacity { get; set; } // Opacity
        public long? CreatorId { get; set; } // CreatorId
        public long? ElementId { get; set; } // AnnotateId
        public int Flag { get; set; } // Flag
        public string FlagCode { get; set; } // FlagCode (length: 20)
        public System.DateTime? FlagDate { get; set; } // FlagDate
        public string FlagImage { get; set; } // FlagImage (length: 100)
        public string UserId { get; set; } // UserId (length: 50)
        public System.DateTime CreatedAt { get; set; } // DateCreated
        public System.DateTime? UpdatedAt { get; set; } // DateUpdated
        public long DocumentId {set; get;}

        // Foreign keys
        public virtual ElementTypeInboxData ElementType { get; set; } // FK_DocumentAnnotate_AnnotateType
        [ForeignKey("DocumentId")]
        public virtual DocumentInboxData Document { get; set; } // FK_DocumentAnnotate_Document
        [ForeignKey("ElementId")]
        public Element Element { get; set; }

        public DocumentElementInboxData()
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
    }
}