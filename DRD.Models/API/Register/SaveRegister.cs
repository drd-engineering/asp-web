﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Models.API.Register
{
    public class Register
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email{ get; set; }
        public long? CompanyId{get;set;}
    }
}
