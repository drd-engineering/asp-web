using System.Collections.Generic;

namespace DRD.Models.API
{
    public class ProcessActivity
    {
        public long RotationNodeId { get; set; }
        public string Value { get; set; }
        public IEnumerable<RotationNodeDoc> RotationNodeDocs { get; set; }
    }
}
