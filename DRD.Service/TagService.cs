using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity.Infrastructure;
using System.Linq.Expressions;

using DRD.Models;
using DRD.Models.API;
using DRD.Models.Custom;
using DRD.Service.Context;
using DRD.Models.View;

namespace DRD.Service
{
    public class TagService
    {
        public ICollection<Tag> GetTags(long rotationId)
        {
            using (var db = new ServiceContext())
            {
                var result = (from tag in db.Tags
                              join tagitem in db.TagItems on tag.Id equals tagitem.TagId
                              where tagitem.RotationId == rotationId
                              select tag).ToList();
                return result;
            }
        }
    }
}