﻿namespace DRD.Models.API
{
    public class Register
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public long? CompanyId { get; set; }
    }
}
