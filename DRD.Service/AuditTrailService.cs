
using DRD.Models;
using DRD.Models.API;
using DRD.Models.Custom;
using DRD.Models.View;
using DRD.Service.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DRD.Service
{
    public class AuditTrailService
    {
       public static bool RecordLog(long userId, string type, string message)
        {
            using var db = new Connection();
            var trails = new AuditTrail()
            {
                Activity = message,
                Type = type,
                UserId = userId
            };
            db.AuditTrails.Add(trails);
            return true;
        }
    }
}
