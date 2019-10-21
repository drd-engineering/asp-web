using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class JsonDashboardLink
    {
        public string Inbox { get; set; }
        public string InProgress { get; set; }
        public string Completed { get; set; }
        public string Contact { get; set; }
        public string Document { get; set; }
        public string DrDrive { get; set; }
    }
}
