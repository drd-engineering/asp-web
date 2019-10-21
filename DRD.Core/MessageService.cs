using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DRD.Domain;
using System.Based.Core.Entity;
using System.Based.Core;

namespace DRD.Core
{
    public class MessageService
    {
        public enum enumClient
        {
            OPERATOR, DRIVER, CUSTOMER, MEMBER
        }

        private readonly string _connString;

        public MessageService(string connString)
        {
            _connString = connString;
        }
        public MessageService()
        {
            _connString = ConfigConstant.CONSTRING;
        }

        public DtoMessage GetById(long Id)
        {
            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.Messages
                     where c.Id == Id
                     select new DtoMessage
                     {
                         Id = c.Id,
                         FromId = c.FromId,
                         ToId = c.ToId,
                         BroadcastMessageId = c.BroadcastMessageId,
                         TextMessage = (c.BroadcastMessageId != null ? c.BroadcastMessage.TextMessage : c.TextMessage),
                         MessageType = c.MessageType,
                         DateCreated = c.DateCreated,
                         DateOpened = c.DateOpened,
                         DateReplied = c.DateReplied,
                     }).FirstOrDefault();


                return result;
            }
        }
        public IEnumerable<DtoMessage> GetByFrom(long Id)
        {
            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.Messages
                     where c.FromId == Id
                     orderby c.DateCreated descending
                     select new DtoMessage
                     {
                         Id = c.Id,
                         FromId = c.FromId,
                         ToId = c.ToId,
                         BroadcastMessageId = c.BroadcastMessageId,
                         TextMessage = (c.BroadcastMessageId != null ? c.BroadcastMessage.TextMessage : c.TextMessage),
                         MessageType = c.MessageType,
                         DateCreated = c.DateCreated,
                         DateOpened = c.DateOpened,
                         DateReplied = c.DateReplied,
                     }).ToList();


                return result;
            }
        }

        public IEnumerable<DtoMessage> GetByTo(long Id)
        {
            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.Messages
                     where c.ToId == Id
                     orderby c.DateCreated descending
                     select new DtoMessage
                     {
                         Id = c.Id,
                         FromId = c.FromId,
                         ToId = c.ToId,
                         BroadcastMessageId = c.BroadcastMessageId,
                         TextMessage = (c.BroadcastMessageId != null ? c.BroadcastMessage.TextMessage : c.TextMessage),
                         MessageType = c.MessageType,
                         DateCreated = c.DateCreated,
                         DateOpened = c.DateOpened,
                         DateReplied = c.DateReplied,
                     }).ToList();


                return result;
            }
        }

        public IEnumerable<DtoMessage> GetByFromTo(long Id)
        {
            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.Messages
                     where (c.FromId == Id || c.ToId == Id)
                     orderby c.DateCreated descending
                     select new DtoMessage
                     {
                         Id = c.Id,
                         FromId = c.FromId,
                         ToId = c.ToId,
                         BroadcastMessageId = c.BroadcastMessageId,
                         TextMessage = (c.BroadcastMessageId != null ? c.BroadcastMessage.TextMessage : c.TextMessage),
                         MessageType = c.MessageType,
                         DateCreated = c.DateCreated,
                         DateOpened = c.DateOpened,
                         DateReplied = c.DateReplied,
                     }).ToList();


                return result;
            }
        }

        public IEnumerable<DtoMessage> GetByFromTo(long fromId, long toId)
        {
            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.Messages
                     where (c.FromId == fromId && c.ToId == toId)
                     orderby c.DateCreated descending
                     select new DtoMessage
                     {
                         Id = c.Id,
                         FromId = c.FromId,
                         ToId = c.ToId,
                         BroadcastMessageId = c.BroadcastMessageId,
                         TextMessage = (c.BroadcastMessageId != null ? c.BroadcastMessage.TextMessage : c.TextMessage),
                         MessageType = c.MessageType,
                         DateCreated = c.DateCreated,
                         DateOpened = c.DateOpened,
                         DateReplied = c.DateReplied,
                     }).ToList();


                return result;
            }
        }

        public DtoMessage Save(DtoMessage msg)
        {
            Message data = new Message();
            using (var db = new DrdContext(_connString))
            {
                if (msg.ToId == -100)
                {
                    ApplConfigService asvr = new ApplConfigService();
                    var val = asvr.GetValue("MEMBER_CARE_ID");
                    if (val != null)
                        msg.ToId = long.Parse(val);
                }

                data.FromId = msg.FromId;
                data.ToId = msg.ToId;
                data.TextMessage = msg.TextMessage;
                data.BroadcastMessageId = (msg.BroadcastMessageId == 0 ? null : msg.BroadcastMessageId);
                data.MessageType = msg.MessageType;
                data.DateCreated = DateTime.Now;
                data.DateOpened = null;
                data.DateReplied = null;

                db.Messages.Add(data);
                var result = db.SaveChanges();
                msg.Id = data.Id;
                msg.DateCreated = data.DateCreated;
                return msg;
            }
        }

        //public int UpdateDateOpened(long Id)
        //{
        //    using (var db = new DrdContext(_connString))
        //    {
        //        var entity = db.Messages.FirstOrDefault(c => c.Id == Id && c.DateOpened == null);
        //        if (entity == null) return 0;

        //        entity.DateOpened = DateTime.Now;

        //        var result = db.SaveChanges();
        //        return result;
        //    }
        //}

        //public int UpdateDateOpened(long yourId)
        //{
        //    using (var db = new DrdContext(_connString))
        //    {
        //        var result =
        //            from c in db.Messages
        //            where (c.FromId == yourId || c.ToId == yourId) && c.DateOpened == null
        //            select c;

        //        foreach (Message msg in result)
        //        {
        //            msg.DateOpened = DateTime.Now;
        //        }

        //        return db.SaveChanges();
        //    }
        //}

        public int UpdateDateOpened(long fromId, long toId)
        {
            using (var db = new DrdContext(_connString))
            {
                var result =
                    from c in db.Messages
                    where c.FromId == fromId && c.ToId == toId && c.DateOpened == null
                    select c;

                foreach (Message msg in result)
                {
                    msg.DateOpened = DateTime.Now;
                }

                return db.SaveChanges();
            }
        }

        public int UpdateDateReplied(long Id)
        {
            using (var db = new DrdContext(_connString))
            {
                var entity = db.Messages.FirstOrDefault(c => c.Id == Id && c.DateReplied == null);
                if (entity == null) return 0;

                entity.DateReplied = DateTime.Now;

                var result = db.SaveChanges();
                return result;
            }
        }

        public JsonMessageCount GetCount(long Id)
        {
            JsonMessageCount count = new JsonMessageCount();
            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.Messages
                         //where (c.FromId == Id && c.FromType == type)
                         //       || (c.ToId == Id && c.ToType == type)
                     where (c.ToId == Id)
                     select new JsonMessageCount
                     {
                         Count = 1,
                         //Unread = (c.DateOpened == null && c.FromId + "-" + c.FromType != Id + "-" + type) ? 1 : 0,
                         Unread = (c.DateOpened == null) ? 1 : 0,

                     }).ToList();

                count.Count = result.Select(c => c.Count).Sum();
                count.Unread = result.Select(c => c.Unread).Sum();
                return count;
            }
        }


        public IEnumerable<DtoMessageSum> GetSum(long Id, long maxId, string topCriteria, int page, int pageSize)
        {
            List<DtoMessageSum> sum = new List<DtoMessageSum>();
            var key = Id;

            int skip = pageSize * (page - 1);

            // top criteria
            string[] tops = new string[] { };
            if (!string.IsNullOrEmpty(topCriteria))
                tops = topCriteria.ToUpper().Split(' ');
            else
                topCriteria = null;

            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.Messages
                     where (c.FromId == Id || c.ToId == Id) && c.Id > maxId
                     orderby c.DateCreated descending
                     select new DtoMessageSum
                     {
                         SenderId = (c.FromId != key ? c.FromId : c.ToId),
                         ReceiverId = (c.FromId != key ? c.ToId : c.FromId),
                         Unread = (c.DateOpened == null && c.FromId != key) ? 1 : 0,
                         TheDate = c.DateCreated,
                         Id = c.Id,
                     }).ToList();

                sum =
                    (from c in result
                     group c by new { c.SenderId, c.ReceiverId } into g
                     select new DtoMessageSum
                     {
                         SenderId = g.Key.SenderId,
                         ReceiverId = g.Key.ReceiverId,
                         TheDate = g.Max(x => x.TheDate),
                         Id = g.Max(x => x.Id),
                         Unread = g.Sum(x => x.Unread),
                     }).ToList();

                var sum2 =
                    (from c in sum
                     join m in db.Members on c.SenderId equals m.Id
                     from a in db.MemberTypes
                     where (m.MemberType & a.BitValue) == a.BitValue && (topCriteria == null || tops.All(x => (m.Name.ToUpper()).Contains(x)))
                     orderby c.Id descending, c.Unread descending
                     select new DtoMessageSum
                     {
                         SenderFoto = m.ImageProfile,
                         SenderId = c.SenderId,
                         SenderName = m.Name,
                         ReceiverId = c.ReceiverId,
                         TheDate = c.TheDate,
                         Id = c.Id,
                         Unread = c.Unread,
                         SenderProfession = (m.MemberType >= 4096 ? a.Info : ""),
                         SenderType = m.MemberType,

                     }).Skip(skip).Take(pageSize).ToList();

                return sum2;
            }

        }

        public IEnumerable<DtoMessageSumDetail> GetSumDetail(long myId, long yourId)
        {
            using (var db = new DrdContext(_connString))
            {
                UpdateDateOpened(yourId, myId);

                var result =
                    (from c in db.Messages
                     where (c.FromId == myId && c.ToId == yourId)
                            || (c.FromId == yourId && c.ToId == myId)
                     orderby c.DateCreated descending
                     select new DtoMessageSumDetail
                     {
                         TextMessage = (c.BroadcastMessageId != null ? c.BroadcastMessage.TextMessage : c.TextMessage),
                         MessageType = c.MessageType,
                         DateMessage = c.DateCreated,
                         IsMe = (c.FromId == myId),
                     }).ToList();
                return result;
            }
        }

        public IEnumerable<DtoMessageSumDetail> GetSumDetail(long myId, long yourId, int page, int pageSize)
        {
            int skip = pageSize * (page - 1);

            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.Messages
                     where (c.FromId == myId && c.ToId == yourId)
                            || (c.FromId == yourId && c.ToId == myId)
                     orderby c.DateCreated descending
                     select new DtoMessageSumDetail
                     {
                         TextMessage = (c.BroadcastMessageId != null ? c.BroadcastMessage.TextMessage : c.TextMessage),
                         MessageType = c.MessageType,
                         DateMessage = c.DateCreated,
                         IsMe = (c.FromId == myId),
                     }).Skip(skip).Take(pageSize).ToList();

                return result;
            }
        }

        public IEnumerable<DtoMessage> GetCountDetail(long Id, string command)
        {
            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.Messages
                     where (c.FromId == Id || c.ToId == Id)
                     orderby c.DateCreated descending
                     select new DtoMessage
                     {
                         Id = c.Id,
                         FromId = c.FromId,
                         ToId = c.ToId,
                         TextMessage = (c.BroadcastMessageId != null ? c.BroadcastMessage.TextMessage : c.TextMessage),
                         MessageType = c.MessageType,
                         DateCreated = c.DateCreated,
                         DateOpened = c.DateOpened,
                         DateReplied = c.DateReplied,

                     }).ToList();

                return result;
            }
        }

        public IEnumerable<DtoMessageSumDetail> GetNewMessage(long myId, long yourId)
        {
            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.Messages
                     where (c.ToId == myId && c.FromId == yourId && c.DateOpened == null)
                     orderby c.DateCreated descending
                     select new DtoMessageSumDetail
                     {
                         TextMessage = (c.BroadcastMessageId != null ? c.BroadcastMessage.TextMessage : c.TextMessage),
                         MessageType = c.MessageType,
                         DateMessage = c.DateCreated,
                         IsMe = false,
                     }).ToList();

                var result2 =
                    (from c in db.Messages
                     where (c.ToId == myId && c.FromId == yourId && c.DateOpened == null)
                     select c);

                foreach (Message msg in result2)
                {
                    msg.DateOpened = DateTime.Now;
                }

                db.SaveChanges();

                return result;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="maxId"></param>
        /// <param name="topCriteria"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<DtoMessageSum> GetListContact(long Id, long maxId, string topCriteria, int page, int pageSize)
        {
            List<DtoMessageSum> sum = new List<DtoMessageSum>();
            var key = Id;

            int skip = pageSize * (page - 1);

            // top criteria
            string[] tops = new string[] { };
            if (!string.IsNullOrEmpty(topCriteria))
                tops = topCriteria.ToUpper().Split(' ');
            else
                topCriteria = null;

            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.Messages
                     where (c.FromId == Id || c.ToId == Id) && c.Id > maxId
                     orderby c.DateCreated descending
                     select new DtoMessageSum
                     {
                         SenderId = (c.FromId != key ? c.FromId : c.ToId),
                         ReceiverId = (c.FromId != key ? c.ToId : c.FromId),
                         Unread = (c.DateOpened == null && c.FromId != key) ? 1 : 0,
                         TheDate = c.DateCreated,
                         Id = c.Id,
                     }).ToList();

                sum =
                    (from c in result
                     group c by new { c.SenderId, c.ReceiverId } into g
                     select new DtoMessageSum
                     {
                         SenderId = g.Key.SenderId,
                         ReceiverId = g.Key.ReceiverId,
                         TheDate = g.Max(x => x.TheDate),
                         Id = g.Max(x => x.Id),
                         Unread = g.Sum(x => x.Unread),
                     }).ToList();

                var sum2 =
                    (from c in sum
                     join m in db.Members on c.SenderId equals m.Id
                     from a in db.MemberTypes
                     where (m.MemberType & a.BitValue) == a.BitValue && (topCriteria == null || tops.All(x => (m.Name.ToUpper()).Contains(x)))
                     orderby c.Id descending, c.Unread descending
                     select new DtoMessageSum
                     {
                         SenderFoto = m.ImageProfile,
                         SenderId = c.SenderId,
                         SenderName = m.Name,
                         ReceiverId = c.ReceiverId,
                         TheDate = c.TheDate,
                         Id = c.Id,
                         Unread = c.Unread,
                         SenderProfession = a.Info,
                         SenderType = m.MemberType,
                     }).Skip(skip).Take(pageSize).ToList();

                return sum2;
            }

        }

    }
}
