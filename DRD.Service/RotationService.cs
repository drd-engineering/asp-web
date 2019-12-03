using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DRD.Models;
using DRD.Models.API;
using DRD.Models.View.List;
using DRD.Service.Context;

namespace DRD.Service
{
    public class RotationService
    {
        private readonly string _connString;
        private string _appZoneAccess;

        public RotationService(string appZoneAccess, string connString)
        {
            _appZoneAccess = appZoneAccess;
            _connString = connString;
        }
        public RotationService(string appZoneAccess)
        {
            _appZoneAccess = appZoneAccess;
            _connString = Constant.CONSTRING;
        }
        public RotationService()
        {
            _connString = Constant.CONSTRING;
        }

        //
        // for edit
        //
        public Rotation GetHeaderById(long id)
        {
            using (var db = new ServiceContext())
            {
                var result =
                    (from rotation in db.Rotations
                     where rotation.Id == id
                     select new Rotation
                     {
                         Id = rotation.Id,
                         Subject = rotation.Subject,
                         Remark = rotation.Remark,
                         Status = rotation.Status,
                         User = rotation.User, // filled when using personal plan
                         Member = rotation.Member, // filled when using business plan
                         DateUpdated = rotation.DateUpdated,
                         Workflow = new Workflow
                         {
                             Id = rotation.Workflow.Id,
                             Name = rotation.Workflow.Name,
                         },
                         RotationUsers =
                            (from RotationUser in rotation.RotationUsers
                             select new RotationUser
                             {
                                 Id = RotationUser.Id,
                                 WorkflowNodeId = RotationUser.WorkflowNodeId,
                                 ActivityName = RotationUser.WorkflowNode.Caption,
                                 MemberId = (RotationUser.User.Id),
                                 Name = (RotationUser.User.Name),
                                 Email = (RotationUser.User.Email),
                                 Picture = RotationUser.User.ImageProfile,
                                 FlagPermission = RotationUser.FlagPermission
                             }).ToList(),
                     }).FirstOrDefault();

                return result;
            }
        }

        public Rotation GetById(long id, long userId = 0)
        {
            using (var db = new ServiceContext())
            {
                var result =
                    (from rotation in db.Rotations
                     where rotation.Id == id
                     select new Rotation
                     {
                         Id = rotation.Id,
                         Subject = rotation.Subject,
                         Status = rotation.Status,
                         Remark = rotation.Remark,
                         User = rotation.User,
                         DateCreated = rotation.DateCreated,
                         DateUpdated = rotation.DateUpdated,
                         DateStarted = rotation.DateStarted,
                         Workflow = new Workflow
                         {
                             Id = rotation.Workflow.Id,
                             Name = rotation.Workflow.Name,
                         }
                     }).FirstOrDefault();
                WorkflowDeepService workflowDeepService = new WorkflowDeepService();
                result = workflowDeepService.assignNodes(db, result, userId, new DocumentService());
                return result;
            }
        }

        public IEnumerable<Rotation> GetByMemberId(long userId)
        {
            using (var db = new ServiceContext())
            {
                int[] finishedStatus = { (int)RotationItem.StatusName.Completed, (int)RotationItem.StatusName.Canceled, (int)RotationItem.StatusName.Declined};
                var rotnodes = db.RotationNodes.Where(rotation => rotation.UserId == userId && !finishedStatus.Contains(rotation.Rotation.Status)).ToList();

                long[] Ids = (from rotation in rotnodes select rotation.Id).ToArray();
                if (Ids.Length > 0)
                    Ids = Ids.Distinct().ToArray();
                var rots = db.Rotations.Where(rotation => (Ids.Contains(rotation.Id) || rotation.User.Id == userId) && !finishedStatus.Contains(rotation.Status)).ToList();

                List<Rotation> rotations = new List<Rotation>();
                foreach (Rotation rt in rots)
                {
                    var rotx = GetById(rt.Id);
                    if (rotx != null)
                        rotations.Add(rotx);
                }
                rotations = rotations.OrderByDescending(rotation => rotation.DateUpdated).ToList();
                return rotations;

            }
        }

        public IEnumerable<Rotation> GetByMemberId(long userId, string status)
        {
            using (var db = new ServiceContext())
            {
                //string[] statuses = Convert.ToInt32(status.Split(','));
                var statuses = status.Split(',').Select(Int32.Parse).ToList();

                var rotnodes = db.RotationNodes.Where(rotation => rotation.UserId == userId && statuses.Contains(rotation.Rotation.Status)).ToList();

                long[] Ids = (from rotation in rotnodes select rotation.Id).ToArray();
                if (Ids.Length > 0)
                    Ids = Ids.Distinct().ToArray();



                //var rots = db.Rotations.Where(rotation => (Ids.Contains(rotation.Id) || rotation.User.Id == userId) && rotation.Status.Equals(status)).ToList();
                var rots = db.Rotations.Where(rotation => (Ids.Contains(rotation.Id) || rotation.User.Id == userId) && statuses.Contains(rotation.Status)).ToList();

                List<Rotation> rotations = new List<Rotation>();

                foreach (Rotation rt in rots)
                {
                    var rotx = GetById(rt.Id);
                    if (rotx != null)
                        rotations.Add(rotx);
                }
                rotations = rotations.OrderByDescending(rotation => rotation.DateUpdated).ToList();
                return rotations;

            }
        }

        public IEnumerable<Rotation> GetNodeByMemberId(long userId, string status)
        {
            using (var db = new ServiceContext())
            {
                var rotnodes = db.RotationNodes.Where(rotation => rotation.UserId == userId && rotation.Status.Equals(status)).ToList();

                long[] Ids = (from rotation in rotnodes select rotation.Id).ToArray();
                if (Ids.Length > 0)
                    Ids = Ids.Distinct().ToArray();
                var rots = db.Rotations.Where(rotation => Ids.Contains(rotation.Id) && rotation.Status.Equals("01")).ToList();

                List<Rotation> rotations = new List<Rotation>();

                foreach (Rotation rt in rots)
                {
                    var rotx = GetById(rt.Id);
                    if (rotx != null)
                        rotations.Add(rotx);
                }
                rotations = rotations.OrderByDescending(rotation => rotation.DateUpdated).ToList();
                return rotations;

            }
        }

        public IEnumerable<RotationItem> GetNodeByMemberId(long userId, string status, string topCriteria, int page, int pageSize)
        {
            int skip = pageSize * (page - 1);
            string ordering = "Status, DateCreated desc";

            // top criteria
            string[] tops = new string[] { };
            if (!string.IsNullOrEmpty(topCriteria))
                tops = topCriteria.Split(' ');
            else
                topCriteria = null;

            using (var db = new ServiceContext())
            {
                var result =
                    (from rotationNode in db.RotationNodes
                     where rotationNode.Rotation.Status.Equals("01") &&
                            rotationNode.UserId == userId && rotationNode.Status.Equals(status) &&
                            (topCriteria == null || tops.All(RotationUser => (rotationNode.Rotation.Subject).Contains(RotationUser)))
                     select new RotationItem
                     {
                         Id = rotationNode.Rotation.Id,
                         RotationNodeId = rotationNode.Id,
                         Subject = rotationNode.Rotation.Subject,
                         Status = rotationNode.Status,
                         WorkflowId = rotationNode.Rotation.Workflow.Id,
                         WorkflowName = rotationNode.Rotation.Workflow.Name,
                         ActivityName = rotationNode.WorkflowNode.Caption,
                         UserId = rotationNode.UserId,
                         StatusDescription = RotationItem.getStatusName(rotationNode.Status),
                         DateCreated = rotationNode.DateCreated,
                         DateUpdated = rotationNode.DateUpdated,
                         DateStarted = rotationNode.Rotation.DateUpdated,
                     }).Skip(skip)
                       .Take(pageSize)
                       .ToList();

                if (result != null)
                {
                    MenuService menuService = new MenuService();
                    foreach (RotationItem rotationItem in result)
                    {
                        rotationItem.Key = menuService.EncryptData(rotationItem.Id);
                    }
                }

                return result;

            }
        }

        public IEnumerable<Rotation> GetInboxByMemberId(long userId)
        {
            using (var db = new ServiceContext())
            {
                var rotnodes = db.RotationNodes.Where(rotation => rotation.UserId == userId && rotation.Status.Equals((int)RotationItem.StatusName.Open)).ToList();

                long[] Ids = (from rotation in rotnodes select rotation.Id).ToArray();
                if (Ids.Length > 0)
                    Ids = Ids.Distinct().ToArray();
                var rots = db.Rotations.Where(rotation => Ids.Contains(rotation.Id) && rotation.Status.Equals((int)RotationItem.StatusName.In_Progress)).ToList();

                List<Rotation> rotations = new List<Rotation>();

                foreach (Rotation rt in rots)
                {
                    var rotx = GetById(rt.Id);
                    if (rotx != null)
                        rotations.Add(rotx);
                }
                rotations = rotations.OrderByDescending(rotation => rotation.DateUpdated).ToList();
                return rotations;

            }
        }

        public Rotation GetNodeById(long id)
        {
            using (var db = new ServiceContext())
            {
                var result =
                    (from rotationNode in db.RotationNodes
                     select new Rotation
                     {
                         Id = rotationNode.Rotation.Id,
                         Subject = rotationNode.Rotation.Subject,
                         Status = rotationNode.Status,
                         User = rotationNode.User,
                         Member = rotationNode.Member,
                         DateCreated = rotationNode.DateCreated,
                         DateUpdated = rotationNode.DateUpdated,
                         DateStarted = rotationNode.Rotation.DateUpdated,
                         RotationNodeId = id,
                         DefWorkflowNodeId = rotationNode.WorkflowNode.Id,
                         FlagAction = 0,
                         DecissionInfo = "",
                         StatusDescription = RotationItem.getStatusName(rotationNode.Status),
                         Workflow = new Workflow
                         {
                             Id = rotationNode.Rotation.Workflow.Id,
                             Name = rotationNode.Rotation.Workflow.Name,
                         },

                     }).FirstOrDefault();

                WorkflowDeepService workflowDeepService = new WorkflowDeepService();
                result = workflowDeepService.assignNodes(db, result, result.User.Id, new DocumentService());

                var workflowNodeLinks = db.WorkflowNodeLinks.Where(rotation => rotation.WorkflowNodeId == result.DefWorkflowNodeId).ToList();
                foreach (WorkflowNodeLink workflowNodeLink in workflowNodeLinks)
                {
                    if (workflowNodeLink.SymbolCode.Equals("SUBMIT"))
                    {
                        result.FlagAction |= (int)Constant.EnumActivityAction.SUBMIT;
                        if (workflowNodeLink.WorkflowNodeTos.SymbolCode.Equals("DECISION"))
                            result.DecissionInfo = "Value " + workflowNodeLink.WorkflowNodeTos.Operator + " " + workflowNodeLink.WorkflowNodeTos.Value;
                        else if (workflowNodeLink.WorkflowNodeTos.SymbolCode.Equals("CASE"))
                            result.DecissionInfo = "Expression: " + workflowNodeLink.WorkflowNodeTos.Value;
                    }
                    else if (workflowNodeLink.SymbolCode.Equals("REJECT"))
                        result.FlagAction |= (int)Constant.EnumActivityAction.REJECT;
                    else if (workflowNodeLink.SymbolCode.Equals("REVISI"))
                        result.FlagAction |= (int)Constant.EnumActivityAction.REVISI;
                    else if (workflowNodeLink.SymbolCode.Equals("ALTER"))
                        result.FlagAction |= (int)Constant.EnumActivityAction.ALTER;

                }


                return result;
            }
        }

        public IEnumerable<RotationUser> GetUsersWorkflow(long workflowId)
        {
            using (var db = new ServiceContext())
            {
                var result =
                    (from rotation in db.WorkflowNodes
                     where rotation.WorkflowId == workflowId && rotation.SymbolCode.Equals("ACTIVITY")
                     select new RotationUser
                     {
                         ActivityName = rotation.Caption,
                         WorkflowNodeId = rotation.Id,
                         User = rotation.User,
                         Email = (rotation.User.Id == null ? "" : rotation.User.Email),
                         Id = (rotation.User.Id == null ? -1 : rotation.User.Id),
                         Name = (rotation.User.Id == null ? "Undefined" : rotation.User.Name),
                         Picture = (rotation.User.Id == null ? "icon_user.png" : rotation.User.ImageProfile),
                     }).ToList();

                return result;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <parameter name="creatorId"></parameter>
        /// <parameter name="topCriteria"></parameter>
        /// <parameter name="page"></parameter>
        /// <parameter name="pageSize"></parameter>
        /// <returns></returns>
        public IEnumerable<RotationItem> GetList(long creatorId, string topCriteria, int page, int pageSize)
        {
            return GetList(creatorId, topCriteria, page, pageSize, null, null);
        }
        public IEnumerable<RotationItem> GetList(long creatorId, string topCriteria, int page, int pageSize, string order)
        {
            return GetList(creatorId, topCriteria, page, pageSize, order, null);
        }
        public IEnumerable<RotationItem> GetList(long creatorId, string topCriteria, int page, int pageSize, string order, string criteria)
        {
            int skip = pageSize * (page - 1);
            string ordering = "Status, DateCreated desc";

            if (!string.IsNullOrEmpty(order))
                ordering = order;

            if (string.IsNullOrEmpty(criteria))
                criteria = "1=1";

            // top criteria
            string[] tops = new string[] { };
            if (!string.IsNullOrEmpty(topCriteria))
                tops = topCriteria.Split(' ');
            else
                topCriteria = null;

            using (var db = new ServiceContext())
            {
                var result =
                    (from rotation in db.Rotations
                     where rotation.CreatorId == creatorId && (topCriteria == null || tops.All(RotationUser => (rotation.Subject).Contains(RotationUser)))
                     select new RotationItem
                     {
                         Id = rotation.Id,
                         Subject = rotation.Subject,
                         Status = rotation.Status,
                         WorkflowId = rotation.Workflow.Id,
                         WorkflowName = rotation.Workflow.Name,
                         UserId = rotation.User.Id,
                         StatusDescription = RotationItem.getStatusName(rotation.Status),
                         DateCreated = rotation.DateCreated,
                         DateUpdated = rotation.DateUpdated,
                         DateStarted = rotation.DateUpdated,
                     }).Skip(skip).Take(pageSize).ToList();

                if (result != null)
                {
                    MenuService menuService = new MenuService();
                    foreach (RotationItem rotationItem in result)
                    {
                        rotationItem.Key = menuService.EncryptData(rotationItem.Id);
                    }
                }

                return result;

            }
        }

        public long GetListCount(long creatorId, string topCriteria)
        {
            return GetListCount(creatorId, topCriteria, null);
        }
        public long GetListCount(long creatorId, string topCriteria, string criteria)
        {

            if (string.IsNullOrEmpty(criteria))
                criteria = "1=1";

            // top criteria
            string[] tops = new string[] { };
            if (!string.IsNullOrEmpty(topCriteria))
                tops = topCriteria.Split(' ');
            else
                topCriteria = null;

            using (var db = new ServiceContext())
            {
                var result =
                    (from rotation in db.Rotations
                     where rotation.CreatorId == creatorId && (topCriteria == null || tops.All(RotationUser => (rotation.Subject).Contains(RotationUser)))
                     select new Rotation
                     {
                         Id = rotation.Id,
                     }).Count();

                return result;

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <parameter name="userId"></parameter>
        /// <parameter name="topCriteria"></parameter>
        /// <parameter name="page"></parameter>
        /// <parameter name="pageSize"></parameter>
        /// <returns></returns>
        public IEnumerable<RotationItem> GetLiteAll(long userId, string topCriteria, int page, int pageSize)
        {
            return GetLiteAll(userId, topCriteria, page, pageSize, null, null);
        }
        public IEnumerable<RotationItem> GetLiteAll(long userId, string topCriteria, int page, int pageSize, string order)
        {
            return GetLiteAll(userId, topCriteria, page, pageSize, order, null);
        }
        public IEnumerable<RotationItem> GetLiteAll(long userId, string topCriteria, int page, int pageSize, string order, string criteria)
        {
            int skip = pageSize * (page - 1);
            string ordering = "Status, DateCreated desc";

            if (!string.IsNullOrEmpty(order))
                ordering = order;

            if (string.IsNullOrEmpty(criteria))
                criteria = "1=1";

            // top criteria
            string[] tops = new string[] { };
            if (!string.IsNullOrEmpty(topCriteria))
                tops = topCriteria.Split(' ');
            else
                topCriteria = null;

            using (var db = new ServiceContext())
            {
                var rotnodes = db.RotationNodes.Where(rotation => rotation.User.Id == userId).ToList();
                long[] Ids = (from rotation in rotnodes select rotation.Id).ToArray();
                var result =
                    (from rotation in db.Rotations
                     where (Ids.Contains(rotation.Id) || rotation.User.Id == userId) &&
                            (topCriteria == null || tops.All(RotationUser => (rotation.Subject).Contains(RotationUser)))
                     select new RotationItem
                     {
                         Id = rotation.Id,
                         Subject = rotation.Subject,
                         Status = rotation.Status,
                         WorkflowId = rotation.Workflow.Id,
                         WorkflowName = rotation.Workflow.Name,
                         UserId = rotation.User.Id,
                         StatusDescription = RotationItem.getStatusName(rotation.Status),
                         DateCreated = rotation.DateCreated,
                         DateUpdated = rotation.DateUpdated,
                         DateStarted = rotation.DateUpdated,
                     }).Skip(skip).Take(pageSize).ToList();

                return result;

            }
        }

        public long GetLiteAllCount(long userId, string topCriteria)
        {
            return GetLiteAllCount(userId, topCriteria, null);
        }
        public long GetLiteAllCount(long userId, string topCriteria, string criteria)
        {

            if (string.IsNullOrEmpty(criteria))
                criteria = "1=1";

            // top criteria
            string[] tops = new string[] { };
            if (!string.IsNullOrEmpty(topCriteria))
                tops = topCriteria.Split(' ');
            else
                topCriteria = null;

            using (var db = new ServiceContext())
            {
                var rotnodes = db.RotationNodes.Where(rotation => rotation.User.Id == userId).ToList();
                long[] Ids = (from rotation in rotnodes select rotation.Id).ToArray();

                var result =
                    (from rotation in db.Rotations
                     where //_appZoneAccess.Contains(rotation.AppZone) &&
                            (Ids.Contains(rotation.Id) || rotation.User.Id == userId) &&
                            (topCriteria == null || tops.All(RotationUser => (rotation.Subject).Contains(RotationUser)))
                     select new Rotation
                     {
                         Id = rotation.Id,
                     }).Count();

                return result;

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <parameter name="userId"></parameter>
        /// <parameter name="status"></parameter>
        /// <parameter name="topCriteria"></parameter>
        /// <parameter name="page"></parameter>
        /// <parameter name="pageSize"></parameter>
        /// <parameter name="order"></parameter>
        /// <parameter name="criteria"></parameter>
        /// <returns></returns>
        public IEnumerable<RotationItem> GetLiteStatusAll(long userId, string status, string topCriteria, int page, int pageSize)
        {
            return GetLiteStatusAll(userId, status, topCriteria, page, pageSize, null, null);
        }
        public IEnumerable<RotationItem> GetLiteStatusAll(long userId, string status, string topCriteria, int page, int pageSize, string order)
        {
            return GetLiteStatusAll(userId, status, topCriteria, page, pageSize, order, null);
        }
        public IEnumerable<RotationItem> GetLiteStatusAll(long userId, string status, string topCriteria, int page, int pageSize, string order, string criteria)
        {
            int skip = pageSize * (page - 1);
            string ordering = "Status, DateCreated desc";

            if (!string.IsNullOrEmpty(order))
                ordering = order;

            if (string.IsNullOrEmpty(criteria))
                criteria = "1=1";

            // top criteria
            string[] tops = new string[] { };
            if (!string.IsNullOrEmpty(topCriteria))
                tops = topCriteria.Split(' ');
            else
                topCriteria = null;

            var statuses = status.Split(',').Select(Int32.Parse).ToList();

            using (var db = new ServiceContext())
            {
                var result =
                    (from rotation in db.Rotations
                     where statuses.Contains(rotation.Status) &&
                            (rotation.User.Id == userId || rotation.RotationNodes.Any(RotationUser => RotationUser.User.Id == userId)) &&
                            (topCriteria == null || tops.All(RotationUser => (rotation.Subject).Contains(RotationUser)))
                     select new RotationItem
                     {
                         Id = rotation.Id,
                         Subject = rotation.Subject,
                         Status = rotation.Status,
                         WorkflowId = rotation.Workflow.Id,
                         WorkflowName = rotation.Workflow.Name,
                         UserId = rotation.User.Id,
                         StatusDescription = RotationItem.getStatusName(rotation.Status),
                         DateCreated = rotation.DateCreated,
                         DateUpdated = rotation.DateUpdated,
                         DateStarted = rotation.DateStarted,
                     }).Skip(skip).Take(pageSize).ToList();

                if (result != null)
                {
                    MenuService menuService = new MenuService();
                    foreach (RotationItem rotationItem in result)
                    {
                        rotationItem.Key = menuService.EncryptData(rotationItem.Id);
                    }
                }

                return result;

            }
        }

        public long GetLiteStatusAllCount(long userId, string status, string topCriteria)
        {
            return GetLiteStatusAllCount(userId, status, topCriteria, null);
        }
        public long GetLiteStatusAllCount(long userId, string status, string topCriteria, string criteria)
        {

            if (string.IsNullOrEmpty(criteria))
                criteria = "1=1";

            // top criteria
            string[] tops = new string[] { };
            if (!string.IsNullOrEmpty(topCriteria))
                tops = topCriteria.Split(' ');
            else
                topCriteria = null;

            using (var db = new ServiceContext())
            {
                var rotnodes = db.RotationNodes.Where(rotation => rotation.User.Id == userId).ToList();
                long[] Ids = (from rotation in rotnodes select rotation.Id).ToArray();
                 var statuses = status.Split(',').Select(Int32.Parse).ToList();
                var result =
                    (from rotation in db.Rotations
                     where statuses.Contains(rotation.Status) &&
                            (Ids.Contains(rotation.Id) || rotation.User.Id == userId) &&
                            (topCriteria == null || tops.All(RotationUser => (rotation.Subject).Contains(RotationUser)))
                     select new Rotation
                     {
                         Id = rotation.Id,
                     }).Count();

                return result;

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <parameter name="userId"></parameter>
        /// <parameter name="status"></parameter>
        /// <parameter name="topCriteria"></parameter>
        /// <parameter name="page"></parameter>
        /// <parameter name="pageSize"></parameter>
        /// <parameter name="order"></parameter>
        /// <parameter name="criteria"></parameter>
        /// <returns></returns>
        public IEnumerable<RotationItem> GetNodeLiteAll(long userId, string status, string topCriteria, int page, int pageSize, string order, string criteria)
        {
            int skip = pageSize * (page - 1);
            string ordering = "DateCreated desc";

            if (!string.IsNullOrEmpty(order))
                ordering = order;

            if (string.IsNullOrEmpty(criteria))
                criteria = "1=1";

            // top criteria
            string[] tops = new string[] { };
            if (!string.IsNullOrEmpty(topCriteria))
                tops = topCriteria.Split(' ');
            else
                topCriteria = null;

             var statuses = status.Split(',').Select(Int32.Parse).ToList();

            using (var db = new ServiceContext())
            {

                var result =
                    (from rotation in db.RotationNodes
                     where rotation.User.Id == userId && statuses.Contains(rotation.Status) &&
                            (topCriteria == null || tops.All(RotationUser => (rotation.Rotation.Subject).Contains(RotationUser)))
                     select new RotationItem
                     {
                         Id = rotation.Id,
                         Subject = rotation.Rotation.Subject,
                         Status = rotation.Rotation.Status,
                         WorkflowId = rotation.Rotation.Workflow.Id,
                         WorkflowName = rotation.Rotation.Workflow.Name,
                         ActivityName = rotation.WorkflowNode.Caption,
                         StatusDescription = RotationItem.getStatusName(rotation.Status),
                         UserId = rotation.User.Id,
                         DateCreated = rotation.DateCreated,
                         DateUpdated = rotation.DateUpdated,
                         DateStarted = rotation.DateRead,
                     }).Skip(skip).Take(pageSize).ToList();

                if (result != null)
                {
                    MenuService menuService = new MenuService();
                    foreach (RotationItem rotationItem in result)
                    {
                        rotationItem.Key = menuService.EncryptData(rotationItem.Id);
                    }
                }

                return result;

            }
        }
        public IEnumerable<RotationItem> GetNodeLiteAll(long userId, string status, string topCriteria, int page, int pageSize)
        {
            return GetNodeLiteAll(userId, status, topCriteria, page, pageSize, null, null);
        }

        public long GetNodeLiteAllCount(long userId, string status, string topCriteria, string criteria)
        {

            if (string.IsNullOrEmpty(criteria))
                criteria = "1=1";

            // top criteria
            string[] tops = new string[] { };
            if (!string.IsNullOrEmpty(topCriteria))
                tops = topCriteria.Split(' ');
            else
                topCriteria = null;

             var statuses = status.Split(',').Select(Int32.Parse).ToList();

            using (var db = new ServiceContext())
            {
                var result =
                    (from rotation in db.RotationNodes
                     where rotation.User.Id == userId && statuses.Contains(rotation.Status) &&
                            (topCriteria == null || tops.All(RotationUser => (rotation.Rotation.Subject).Contains(RotationUser)))
                     select new RotationItem
                     {
                         Id = rotation.Id,
                     }).Count();

                return result;

            }
        }
        public long GetNodeLiteAllCount(long userId, string status, string topCriteria)
        {
            return GetNodeLiteAllCount(userId, status, topCriteria, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <parameter name="prod"></parameter>
        /// <returns></returns>
        public long Save(Rotation prod)
        {
            WorkflowDeepService workflowDeepService = new WorkflowDeepService();
            return workflowDeepService.Save(prod);
        }

        //public int Start(long id)
        //{
            //WorkflowDeepService workflowDeepService = new WorkflowDeepService();
            //var ret = workflowDeepService.Start(id, new MemberDepositTrxService());

            //MemberService userService = new MemberService();
            //foreach (JsonActivityResult act in ret)
            //{
            //    userService.sendEmailInbox(act);
            //}

            //return ret.FirstOrDefault().ExitCode;
        //}

        //public int ProcessActivity(ProcessActivity parameter, Constant.EnumActivityAction enumActivityAction)
        //{
        //    WorkflowDeepService workflowDeepService = new WorkflowDeepService();
        //    var ret = workflowDeepService.ProcessActivity(parameter, enumActivityAction, new DocumentService(), new MemberDepositTrxService());
        //    UserService userService = new UserService();
        //    foreach (ActivityItem act in ret)
        //    {
        //        userService.sendEmailInbox(act);
        //    }
        //    return ret.FirstOrDefault().ExitCode;
        //}



    }

}
