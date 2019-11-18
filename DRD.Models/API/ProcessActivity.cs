using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DRD.Models;

namespace DRD.Models.API
{
    public class ProcessActivity
    {
        public long RotationNodeId { get; set; }
        public string Remark { get; set; }
        public string Value { get; set; }
        public IEnumerable<RotationNodeDoc> RotationNodeDocs { get; set; }
        public IEnumerable<RotationNodeUpDoc> RotationNodeUpDocs { get; set; }
    }
}
