using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class JsonProcessActivity
    {
        public long RotationNodeId { get; set; }
        public string Remark { get; set; }
        public string Value { get; set; }
        public IEnumerable<DtoRotationNodeDoc> RotationNodeDocs { get; set; }
        public IEnumerable<DtoRotationNodeUpDoc> RotationNodeUpDocs { get; set; }
    }
}
