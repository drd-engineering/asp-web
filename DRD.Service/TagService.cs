using DRD.Models;
using DRD.Service.Context;
using System.Collections.Generic;
using System.Linq;

namespace DRD.Service
{
    public class TagService
    {
        public ICollection<string> GetTagsAsString(long rotationId)
        {
            using var db = new Connection();
            var result = (from tag in db.Tags
                            join tagitem in db.TagItems on tag.Id equals tagitem.TagId
                            where tagitem.RotationId == rotationId
                            select tag.Name).ToList();
            return result;
        }
    }
}