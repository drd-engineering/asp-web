using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DRD.Core;

namespace DRD.TaskScheduler
{
    class Program
    {
        static void Main(string[] args)
        {
            TaskSchedulerService rsvr = new TaskSchedulerService();
            Console.WriteLine("Task running...Auto altering: " + rsvr.ProcessAlterNode().ToString() + " records");
            Console.WriteLine("Task running...Auto expired payment: " + rsvr.ProcessExpiredPayment().ToString() + " records");
            Console.WriteLine("Task running...Auto renew member plan: " + rsvr.ProcessRenewMemberPlan().ToString() + " records");
            //Console.ReadKey();
        }
    }
}
