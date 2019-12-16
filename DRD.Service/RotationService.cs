using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DRD.Models;
using DRD.Models.Custom;
using DRD.Models.API;
using DRD.Models.View.Rotation;
using DRD.Models.API.List;
using DRD.Service.Context;
using System.Linq.Expressions;

namespace DRD.Service
{
    public class RotationService
    {
        private readonly string _connString;
        private string _appZoneAccess;
        private Constant constant = new Constant();

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
        public RotationIndex GetRotationById(long id)
        {
            using (var db = new ServiceContext())
            {
                var result =
                    (from rotation in db.Rotations
                     where rotation.Id == id
                     select new RotationIndex
                     {
                         Id = rotation.Id,
                         Subject = rotation.Subject,
                         Remark = rotation.Remark,
                         WorkflowId = rotation.WorkflowId,
                         Workflow = new WorkflowItem
                         {
                             Id = rotation.Workflow.Id,
                             Name = rotation.Workflow.Name,
                         },
                         Status = rotation.Status,
                         UserId = rotation.UserId, // filled when using personal plan
                         RotationUsers = (from x in rotation.RotationUsers
                                          select new RotationUserItem
                                          {
                                              UserId = x.UserId,
                                              WorkflowNodeId = x.WorkflowNodeId,
                                              ActivityName = x.WorkflowNode.Caption,
                                              MemberNumber = (x.UserId == null ? (long?) null : x.User.Id),
                                              MemberName = (x.UserId == null ? "Undefined" : x.User.Name),
                                              MemberEmail = (x.UserId == null ? "" : x.User.Email),
                                              MemberPicture = (x.UserId == null ? "icon_user.png" : x.User.ImageProfile),
                                              FlagPermission = x.FlagPermission,
                                              //FlagAction = x.FlagAction,
                                              //CxDownload = x.CxDownload,
                                              //CxPrint = x.CxPrint,
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
                         UserId = rotation.UserId,
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

        public IEnumerable<Rotation> GetByUserId(long userId)
        {
            using (var db = new ServiceContext())
            {
                int[] finishedStatus = { (int)Constant.RotationStatus.Completed, (int)Constant.RotationStatus.Canceled, (int)Constant.RotationStatus.Declined};
                var rotnodes = db.RotationNodes.Where(rotation => rotation.UserId == userId && !finishedStatus.Contains(rotation.Rotation.Status)).ToList();

                long[] Ids = (from rotation in rotnodes select rotation.Id).ToArray();
                if (Ids.Length > 0)
                    Ids = Ids.Distinct().ToArray();
                var rots = db.Rotations.Where(rotation => (Ids.Contains(rotation.Id) || rotation.UserId == userId) && !finishedStatus.Contains(rotation.Status)).ToList();

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

        public IEnumerable<Rotation> GetByUserId(long userId, string status)
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
                var rots = db.Rotations.Where(rotation => (Ids.Contains(rotation.Id) || rotation.UserId == userId) && statuses.Contains(rotation.Status)).ToList();

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

        public IEnumerable<Rotation> GetNodeByUserId(long userId, string status)
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

        public IEnumerable<RotationData> GetNodeByUserId(long userId, string status, string topCriteria, int page, int pageSize)
        {
            int skip = pageSize * (page - 1);
            Expression<Func<RotationData, string>> ordering = WorkflowData => "Status, DateCreated desc";

            // top criteria
            string[] tops = new string[] { };
            if (!string.IsNullOrEmpty(topCriteria))
                tops = topCriteria.Split(' ');
            else
                topCriteria = "";

            using (var db = new ServiceContext())
            {
                var result =
                    (from rotationNode in db.RotationNodes
                     where rotationNode.Rotation.Status.Equals("01") &&
                            rotationNode.UserId == userId && rotationNode.Status.Equals(status) &&
                            (topCriteria == null || tops.All(RotationUser => (rotationNode.Rotation.Subject).Contains(RotationUser)))
                     select new RotationData
                     {
                         Id = rotationNode.Rotation.Id,
                         RotationNodeId = rotationNode.Id,
                         Subject = rotationNode.Rotation.Subject,
                         Status = rotationNode.Status,
                         WorkflowId = rotationNode.Rotation.Workflow.Id,
                         WorkflowName = rotationNode.Rotation.Workflow.Name,
                         ActivityName = rotationNode.WorkflowNode.Caption,
                         UserId = rotationNode.UserId,
                         StatusDescription = constant.getRotationStatusName(rotationNode.Status),
                         CreatedAt = rotationNode.CreatedAt,
                         UpdatedAt = rotationNode.UpdatedAt,
                         DateStarted = rotationNode.Rotation.DateUpdated,
                     }).OrderBy(ordering).Skip(skip).Take(pageSize).ToList();

                if (result != null)
                {
                    MenuService menuService = new MenuService();
                    foreach (RotationData rotationItem in result)
                    {
                        rotationItem.Key = menuService.EncryptData(rotationItem.Id);
                    }
                }

                return result;

            }
        }

        public IEnumerable<Rotation> GetInboxByUserId(long userId)
        {
            using (var db = new ServiceContext())
            {
                var rotnodes = db.RotationNodes.Where(rotation => rotation.UserId == userId && rotation.Status.Equals((int)Constant.RotationStatus.Open)).ToList();

                long[] Ids = (from rotation in rotnodes select rotation.Id).ToArray();
                if (Ids.Length > 0)
                    Ids = Ids.Distinct().ToArray();
                var rots = db.Rotations.Where(rotation => Ids.Contains(rotation.Id) && rotation.Status.Equals((int)Constant.RotationStatus.In_Progress)).ToList();

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
                         UserId = rotationNode.User.Id,
                         DateCreated = rotationNode.CreatedAt,
                         DateUpdated = rotationNode.UpdatedAt,
                         DateStarted = rotationNode.Rotation.DateUpdated,
                         RotationNodeId = id,
                         DefWorkflowNodeId = rotationNode.WorkflowNode.Id,
                         FlagAction = 0,
                         DecissionInfo = "",
                         StatusDescription = constant.getRotationStatusName(rotationNode.Status),
                         Workflow = new Workflow
                         {
                             Id = rotationNode.Rotation.Workflow.Id,
                             Name = rotationNode.Rotation.Workflow.Name,
                         },

                     }).FirstOrDefault();

                WorkflowDeepService workflowDeepService = new WorkflowDeepService();
                result = workflowDeepService.assignNodes(db, result, result.UserId.Value, new DocumentService());

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

        public IEnumerable<RotationUserData> GetUsersWorkflow(long workflowId)
        {
            using (var db = new ServiceContext())
            {
                var result =
                    (from workflowNode in db.WorkflowNodes
                     where workflowNode.WorkflowId == workflowId && workflowNode.SymbolCode == 5
                     select new RotationUserData
                     {
                         ActivityName = workflowNode.Caption,
                         WorkflowNodeId = workflowNode.Id,
                         UserId = workflowNode.UserId.Value
                     }).ToList();
                foreach (RotationUserData item in result)
                {
                    if(item.UserId != 0)
                    {
                        User user = (from Userdb in db.Users where Userdb.Id == item.UserId select Userdb).FirstOrDefault();
                        if (user != null)
                        {
                            item.Email = user.Email;
                            item.Name = user.Name;
                            item.Picture = user.ImageProfile;
                        }
                    }
                }
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
        public ListRotationData FindRotations(long creatorId, string topCriteria, int page, int pageSize)
        {
            Expression<Func<RotationData, bool>> criteriaUsed = WorkflowData => true;
            return FindRotations(creatorId, topCriteria, page, pageSize, null, criteriaUsed);
        }
        public ListRotationData FindRotations(long creatorId, string topCriteria, int page, int pageSize, Expression<Func<RotationData, string>> order)
        {
            Expression<Func<RotationData, bool>> criteriaUsed = WorkflowData => true;
            return FindRotations(creatorId, topCriteria, page, pageSize, order, criteriaUsed);
        }
        public ListRotationData FindRotations(long creatorId, string topCriteria, int page, int pageSize, Expression<Func<RotationData, string>> order, Expression<Func<RotationData, bool>> criteria)
        {
            int skip = pageSize * (page - 1);
            Expression<Func<RotationData, string>> ordering = WorkflowData => "IsTemplate desc, Name";

            if (order != null)
                ordering = order;
            
            // top criteria
            string[] tops = new string[] { };
            if (!string.IsNullOrEmpty(topCriteria))
                tops = topCriteria.Split(' ');
            else
                topCriteria = "";

            using (var db = new ServiceContext())
            {
                if(db.Rotations != null)
                {
                    
                    var result =
                    (from rotation in db.Rotations
                     where rotation.CreatorId == creatorId && (topCriteria.Equals("") || tops.All(RotationUser => (rotation.Subject).Contains(RotationUser)))
                     select new RotationData
                     {
                         Id = rotation.Id,
                         Subject = rotation.Subject,
                         Status = rotation.Status,
                         WorkflowId = rotation.Workflow.Id,
                         WorkflowName = rotation.Workflow.Name,
                         UserId = rotation.UserId,
                         CreatedAt = rotation.DateCreated,
                         UpdatedAt = rotation.DateUpdated,
                         DateStarted = rotation.DateUpdated,
                     }).Where(criteria).OrderBy(ordering).Skip(skip).Take(pageSize).ToList();

                    foreach (RotationData resultItem in result){
                        resultItem.StatusDescription = constant.getRotationStatusName(resultItem.Status);
                    }

                    ListRotationData returnValue = new ListRotationData();
                    if (result != null)
                    {
                        int counterRotation = 0;
                        MenuService menuService = new MenuService();
                        foreach (RotationData rotationItem in result)
                        {
                            counterRotation += 1;
                            rotationItem.Key = menuService.EncryptData(rotationItem.Id);
                            returnValue.Items.Add(rotationItem);
                        }
                        returnValue.Count = counterRotation;
                    }
                    return returnValue;
                }
                return null;
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
        public IEnumerable<RotationData> GetLiteAll(long userId, string topCriteria, int page, int pageSize)
        {
            return GetLiteAll(userId, topCriteria, page, pageSize, null, null);
        }
        public IEnumerable<RotationData> GetLiteAll(long userId, string topCriteria, int page, int pageSize, Expression<Func<RotationData, string>> order)
        {
            return GetLiteAll(userId, topCriteria, page, pageSize, order, null);
        }
        public IEnumerable<RotationData> GetLiteAll(long userId, string topCriteria, int page, int pageSize, Expression<Func<RotationData, string>> order, Expression<Func<RotationData, bool>> criteria)
        {
            int skip = pageSize * (page - 1);
            Expression<Func<RotationData, string>> ordering = WorkflowData => "Status, DateCreated desc";

            if (order != null)
                ordering = order;

            // top criteria
            string[] tops = new string[] { };
            if (!string.IsNullOrEmpty(topCriteria))
                tops = topCriteria.Split(' ');
            else
                topCriteria = "";

            using (var db = new ServiceContext())
            {
                var rotnodes = db.RotationNodes.Where(rotation => rotation.User.Id == userId).ToList();
                long[] Ids = (from rotation in rotnodes select rotation.Id).ToArray();
                var result =
                    (from rotation in db.Rotations
                     where (Ids.Contains(rotation.Id) || rotation.UserId == userId) &&
                            (topCriteria == null || tops.All(RotationUser => (rotation.Subject).Contains(RotationUser)))
                     select new RotationData
                     {
                         Id = rotation.Id,
                         Subject = rotation.Subject,
                         Status = rotation.Status,
                         WorkflowId = rotation.Workflow.Id,
                         WorkflowName = rotation.Workflow.Name,
                         UserId = rotation.UserId,
                         StatusDescription = constant.getRotationStatusName(rotation.Status),
                         CreatedAt = rotation.DateCreated,
                         UpdatedAt = rotation.DateUpdated,
                         DateStarted = rotation.DateUpdated,
                     }).Where(criteria).OrderBy(ordering).Skip(skip).Take(pageSize).ToList();

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
                            (Ids.Contains(rotation.Id) || rotation.UserId == userId) &&
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
        public IEnumerable<RotationData> GetLiteStatusAll(long userId, string status, string topCriteria, int page, int pageSize)
        {
            return GetLiteStatusAll(userId, status, topCriteria, page, pageSize, null, null);
        }
        public IEnumerable<RotationData> GetLiteStatusAll(long userId, string status, string topCriteria, int page, int pageSize, Expression<Func<RotationData, string>> order)
        {
            return GetLiteStatusAll(userId, status, topCriteria, page, pageSize, order, null);
        }
        public IEnumerable<RotationData> GetLiteStatusAll(long userId, string status, string topCriteria, int page, int pageSize, Expression<Func<RotationData, string>> order, Expression<Func<RotationData, bool>> criteria)
        {
            int skip = pageSize * (page - 1);
            Expression<Func<RotationData, string>> ordering = WorkflowData => "Status, DateCreated desc";

            if (order != null)
                ordering = order;

            // top criteria
            string[] tops = new string[] { };
            if (!string.IsNullOrEmpty(topCriteria))
                tops = topCriteria.Split(' ');
            else
                topCriteria = "";

            var statuses = status.Split(',').Select(Int32.Parse).ToList();

            using (var db = new ServiceContext())
            {
                var result =
                    (from rotation in db.Rotations
                     where statuses.Contains(rotation.Status) &&
                            (rotation.UserId == userId || rotation.RotationNodes.Any(RotationUser => RotationUser.User.Id == userId)) &&
                            (topCriteria == null || tops.All(RotationUser => (rotation.Subject).Contains(RotationUser)))
                     select new RotationData
                     {
                         Id = rotation.Id,
                         Subject = rotation.Subject,
                         Status = rotation.Status,
                         WorkflowId = rotation.Workflow.Id,
                         WorkflowName = rotation.Workflow.Name,
                         UserId = rotation.UserId,
                         StatusDescription = constant.getRotationStatusName(rotation.Status),
                         CreatedAt = rotation.DateCreated,
                         UpdatedAt = rotation.DateCreated,
                         DateStarted = rotation.DateStarted,
                     }).Where(criteria).OrderBy(ordering).Skip(skip).Take(pageSize).ToList();

                if (result != null)
                {
                    MenuService menuService = new MenuService();
                    foreach (RotationData rotationItem in result)
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
                            (Ids.Contains(rotation.Id) || rotation.UserId == userId) &&
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
        public IEnumerable<RotationData> GetNodeLiteAll(long userId, string status, string topCriteria, int page, int pageSize, Expression<Func<RotationData, string>> order, Expression<Func<RotationData, bool>> criteria)
        {
            int skip = pageSize * (page - 1);
            Expression<Func<RotationData, string>> ordering = WorkflowData => "IsTemplate desc, Name";

            if (order != null)
                ordering = order;

            // top criteria
            string[] tops = new string[] { };
            if (!string.IsNullOrEmpty(topCriteria))
                tops = topCriteria.Split(' ');
            else
                topCriteria = "";

            var statuses = status.Split(',').Select(Int32.Parse).ToList();

            using (var db = new ServiceContext())
            {

                var result =
                    (from rotation in db.RotationNodes
                     where rotation.User.Id == userId && statuses.Contains(rotation.Status) &&
                            (topCriteria == null || tops.All(RotationUser => (rotation.Rotation.Subject).Contains(RotationUser)))
                     select new RotationData
                     {
                         Id = rotation.Id,
                         Subject = rotation.Rotation.Subject,
                         Status = rotation.Rotation.Status,
                         WorkflowId = rotation.Rotation.Workflow.Id,
                         WorkflowName = rotation.Rotation.Workflow.Name,
                         ActivityName = rotation.WorkflowNode.Caption,
                         StatusDescription = constant.getRotationStatusName(rotation.Status),
                         UserId = rotation.User.Id,
                         CreatedAt = rotation.CreatedAt,
                         UpdatedAt = rotation.UpdatedAt,
                         DateStarted = rotation.DateRead,
                     }).Where(criteria).OrderBy(ordering).Skip(skip).Take(pageSize).ToList();

                if (result != null)
                {
                    MenuService menuService = new MenuService();
                    foreach (RotationData rotationItem in result)
                    {
                        rotationItem.Key = menuService.EncryptData(rotationItem.Id);
                    }
                }

                return result;

            }
        }
        public IEnumerable<RotationData> GetNodeLiteAll(long userId, string status, string topCriteria, int page, int pageSize)
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
                     select new RotationData
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
        public long Save(RotationItem prod)
        {
            WorkflowDeepService workflowDeepService = new WorkflowDeepService();
            return workflowDeepService.Save(prod);
        }

        public int Start(long userId, long rotationId, long subscriptionId)
        {
            WorkflowDeepService workflowDeepService = new WorkflowDeepService();
            var returnItem = workflowDeepService.Start(userId, rotationId, subscriptionId);
            MemberService memberService = new MemberService();
            
            //MemberService userService = new MemberService();
            //foreach (JsonActivityResult act in ret)
            //{
            //    userService.sendEmailInbox(act);
            //}
            return returnItem.FirstOrDefault().ExitCode;
        }

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
