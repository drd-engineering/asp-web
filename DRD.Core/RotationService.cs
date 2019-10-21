using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;
using System.Based.Core.Entity;
using DRD.Domain;
using System.Configuration;
using Newtonsoft.Json;
using System.Based.Core;

namespace DRD.Core
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
            _connString = ConfigConstant.CONSTRING;
        }
        public RotationService()
        {
            _connString = ConfigConstant.CONSTRING;
        }

        //
        // for edit
        //
        public DtoRotation GetHeaderById(long id)
        {
            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.Rotations
                     where c.Id == id
                     select new DtoRotation
                     {
                         Id = c.Id,
                         Subject = c.Subject,
                         Remark = c.Remark,
                         WorkflowId = c.WorkflowId,
                         Status = c.Status,
                         MemberId = c.MemberId,
                         DateStatus = c.DateStatus,
                         Workflow = new DtoWorkflow
                         {
                             Id = c.Workflow.Id,
                             Name = c.Workflow.Name,
                         },
                         RotationMembers =
                            (from x in c.RotationMembers
                             select new DtoRotationMember
                             {
                                 MemberId = x.MemberId,
                                 WorkflowNodeId = x.WorkflowNodeId,
                                 ActivityName = x.WorkflowNode.Caption,
                                 MemberNumber = (x.MemberId == null ? "" : x.Member.Number),
                                 MemberName = (x.MemberId == null ? "Undefined" : x.Member.Name),
                                 MemberEmail = (x.MemberId == null ? "" : x.Member.Email),
                                 MemberPicture = (x.MemberId == null ? "icon_user.png" : x.Member.ImageProfile),
                                 FlagPermission = x.FlagPermission,
                                 //FlagAction = x.FlagAction,
                                 //CxDownload = x.CxDownload,
                                 //CxPrint = x.CxPrint,
                             }).ToList(),
                     }).FirstOrDefault();

                return result;
            }
        }

        public DtoRotation GetById(long id, long memberId = 0)
        {
            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.Rotations
                     where c.Id == id
                     select new DtoRotation
                     {
                         Id = c.Id,
                         Subject = c.Subject,
                         Status = c.Status,
                         Remark = c.Remark,
                         WorkflowId = c.WorkflowId,
                         MemberId = c.MemberId,
                         DateCreated = c.DateCreated,
                         DateUpdated = c.DateUpdated,
                         DateStatus = c.DateStatus,
                         Workflow = new DtoWorkflow
                         {
                             Id = c.Workflow.Id,
                             Name = c.Workflow.Name,
                         }
                     }).FirstOrDefault();
                WfeService wfeSvr = new WfeService();
                result = wfeSvr.assignNodes(db, result, memberId, new DocumentService());
                return result;
            }
        }

        public IEnumerable<DtoRotation> GetByMemberId(long memberId)
        {
            using (var db = new DrdContext(_connString))
            {
                var rotnodes = db.RotationNodes.Where(c => c.MemberId == memberId && !("90,98,99").Contains(c.Rotation.Status)).ToList();

                long[] Ids = (from c in rotnodes select c.RotationId).ToArray();
                if (Ids.Length > 0)
                    Ids = Ids.Distinct().ToArray();
                var rots = db.Rotations.Where(c => (Ids.Contains(c.Id) || c.MemberId == memberId) && !("90,98,99").Contains(c.Status)).ToList();

                List<DtoRotation> rotations = new List<DtoRotation>();
                foreach (Rotation rt in rots)
                {
                    var rotx = GetById(rt.Id);
                    if (rotx != null)
                        rotations.Add(rotx);
                }
                rotations = rotations.OrderByDescending(c => c.DateStatus).ToList();
                return rotations;

            }
        }

        public IEnumerable<DtoRotation> GetByMemberId(long memberId, string status)
        {
            using (var db = new DrdContext(_connString))
            {
                string[] sts = status.Split(',');

                var rotnodes = db.RotationNodes.Where(c => c.MemberId == memberId && sts.Contains(c.Rotation.Status)).ToList();

                long[] Ids = (from c in rotnodes select c.RotationId).ToArray();
                if (Ids.Length > 0)
                    Ids = Ids.Distinct().ToArray();



                //var rots = db.Rotations.Where(c => (Ids.Contains(c.Id) || c.MemberId == memberId) && c.Status.Equals(status)).ToList();
                var rots = db.Rotations.Where(c => (Ids.Contains(c.Id) || c.MemberId == memberId) && sts.Contains(c.Status)).ToList();

                List<DtoRotation> rotations = new List<DtoRotation>();

                foreach (Rotation rt in rots)
                {
                    var rotx = GetById(rt.Id);
                    if (rotx != null)
                        rotations.Add(rotx);
                }
                rotations = rotations.OrderByDescending(c => c.DateStatus).ToList();
                return rotations;

            }
        }

        public IEnumerable<DtoRotation> GetNodeByMemberId(long memberId, string status)
        {
            using (var db = new DrdContext(_connString))
            {
                var rotnodes = db.RotationNodes.Where(c => c.MemberId == memberId && c.Status.Equals(status)).ToList();

                long[] Ids = (from c in rotnodes select c.RotationId).ToArray();
                if (Ids.Length > 0)
                    Ids = Ids.Distinct().ToArray();
                var rots = db.Rotations.Where(c => Ids.Contains(c.Id) && c.Status.Equals("01")).ToList();

                List<DtoRotation> rotations = new List<DtoRotation>();

                foreach (Rotation rt in rots)
                {
                    var rotx = GetById(rt.Id);
                    if (rotx != null)
                        rotations.Add(rotx);
                }
                rotations = rotations.OrderByDescending(c => c.DateStatus).ToList();
                return rotations;

            }
        }

        public IEnumerable<DtoRotationLite> GetNodeByMemberId(long memberId, string status, string topCriteria, int page, int pageSize)
        {
            int skip = pageSize * (page - 1);
            string ordering = "Status, DateCreated desc";

            // top criteria
            string[] tops = new string[] { };
            if (!string.IsNullOrEmpty(topCriteria))
                tops = topCriteria.Split(' ');
            else
                topCriteria = null;

            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.RotationNodes
                     join s in db.StatusCodes on c.Status equals s.Code
                     where c.Rotation.Status.Equals("01") &&
                            c.MemberId == memberId && c.Status.Equals(status) &&
                            (topCriteria == null || tops.All(x => (c.Rotation.Subject).Contains(x)))
                     select new DtoRotationLite
                     {
                         Id = c.Rotation.Id,
                         RotationNodeId = c.Id,
                         Subject = c.Rotation.Subject,
                         Status = c.Status,
                         WorkflowId = c.Rotation.WorkflowId,
                         WorkflowName = c.Rotation.Workflow.Name,
                         ActivityName = c.WorkflowNode.Caption,
                         MemberId = c.MemberId,
                         StatusDescr = s.Descr,
                         DateCreated = c.DateCreated,
                         DateUpdated = c.DateUpdated,
                         DateStatus = c.Rotation.DateStatus,
                     }).OrderBy(ordering).Skip(skip).Take(pageSize).ToList();

                if (result != null)
                {
                    MenuService musvr = new MenuService();
                    foreach (DtoRotationLite dl in result)
                    {
                        dl.Key = musvr.EncryptData(dl.Id);
                    }
                }

                return result;

            }
        }

        public IEnumerable<DtoRotation> GetInboxByMemberId(long memberId)
        {
            using (var db = new DrdContext(_connString))
            {
                var rotnodes = db.RotationNodes.Where(c => c.MemberId == memberId && c.Status.Equals("00")).ToList();

                long[] Ids = (from c in rotnodes select c.RotationId).ToArray();
                if (Ids.Length > 0)
                    Ids = Ids.Distinct().ToArray();
                var rots = db.Rotations.Where(c => Ids.Contains(c.Id) && c.Status.Equals("01")).ToList();

                List<DtoRotation> rotations = new List<DtoRotation>();

                foreach (Rotation rt in rots)
                {
                    var rotx = GetById(rt.Id);
                    if (rotx != null)
                        rotations.Add(rotx);
                }
                rotations = rotations.OrderByDescending(c => c.DateStatus).ToList();
                return rotations;

            }
        }

        public DtoRotation GetNodeById(long id)
        {
            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.RotationNodes
                     join s in db.StatusCodes on c.Rotation.Status equals s.Code
                     where c.Id == id
                     select new DtoRotation
                     {
                         Id = c.Rotation.Id,
                         Subject = c.Rotation.Subject,
                         Status = c.Status,
                         WorkflowId = c.Rotation.WorkflowId,
                         MemberId = c.MemberId,
                         DateCreated = c.DateCreated,
                         DateUpdated = c.DateUpdated,
                         DateStatus = c.Rotation.DateStatus,
                         RotationNodeId = id,
                         DefWorkflowNodeId = c.WorkflowNodeId,
                         FlagAction = 0,
                         DecissionInfo = "",
                         StatusDescr = s.Descr,
                         Workflow = new DtoWorkflow
                         {
                             Id = c.Rotation.Workflow.Id,
                             Name = c.Rotation.Workflow.Name,
                         },

                     }).FirstOrDefault();

                WfeService wfeSvr = new WfeService();
                result = wfeSvr.assignNodes(db, result, result.MemberId, new DocumentService());

                var wfnls = db.WorkflowNodeLinks.Where(c => c.WorkflowNodeId == result.DefWorkflowNodeId).ToList();
                foreach (WorkflowNodeLink wfnl in wfnls)
                {
                    if (wfnl.Symbol.Code.Equals("SUBMIT"))
                    {
                        result.FlagAction |= (int)ConfigConstant.EnumActivityAction.SUBMIT;
                        if (wfnl.WorkflowNode_WorkflowNodeToId.Symbol.Code.Equals("DECISION"))
                            result.DecissionInfo = "Value " + wfnl.WorkflowNode_WorkflowNodeToId.Operator + " " + wfnl.WorkflowNode_WorkflowNodeToId.Value;
                        else if (wfnl.WorkflowNode_WorkflowNodeToId.Symbol.Code.Equals("CASE"))
                            result.DecissionInfo = "Expression: " + wfnl.WorkflowNode_WorkflowNodeToId.Value;
                    }
                    else if (wfnl.Symbol.Code.Equals("REJECT"))
                        result.FlagAction |= (int)ConfigConstant.EnumActivityAction.REJECT;
                    else if (wfnl.Symbol.Code.Equals("REVISI"))
                        result.FlagAction |= (int)ConfigConstant.EnumActivityAction.REVISI;
                    else if (wfnl.Symbol.Code.Equals("ALTER"))
                        result.FlagAction |= (int)ConfigConstant.EnumActivityAction.ALTER;

                }


                return result;
            }
        }

        public IEnumerable<DtoRotationMember> GetUsersWorkflow(long workflowId)
        {
            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.WorkflowNodes
                     where c.WorkflowId == workflowId && c.Symbol.Code.Equals("ACTIVITY")
                     select new DtoRotationMember
                     {
                         ActivityName = c.Caption,
                         WorkflowNodeId = c.Id,
                         MemberId = c.MemberId,
                         MemberEmail = (c.MemberId == null ? "" : c.Member.Email),
                         MemberNumber = (c.MemberId == null ? "" : c.Member.Number),
                         MemberName = (c.MemberId == null ? "Undefined" : c.Member.Name),
                         MemberPicture = (c.MemberId == null ? "icon_user.png" : c.Member.ImageProfile),
                     }).ToList();

                return result;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="creatorId"></param>
        /// <param name="topCriteria"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<DtoRotationLite> GetList(long creatorId, string topCriteria, int page, int pageSize)
        {
            return GetList(creatorId, topCriteria, page, pageSize, null, null);
        }
        public IEnumerable<DtoRotationLite> GetList(long creatorId, string topCriteria, int page, int pageSize, string order)
        {
            return GetList(creatorId, topCriteria, page, pageSize, order, null);
        }
        public IEnumerable<DtoRotationLite> GetList(long creatorId, string topCriteria, int page, int pageSize, string order, string criteria)
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

            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.Rotations
                     join s in db.StatusCodes on c.Status equals s.Code
                     where c.CreatorId == creatorId && (topCriteria == null || tops.All(x => (c.Subject).Contains(x)))
                     select new DtoRotationLite
                     {
                         Id = c.Id,
                         Subject = c.Subject,
                         Status = c.Status,
                         WorkflowId = c.WorkflowId,
                         WorkflowName = c.Workflow.Name,
                         MemberId = c.MemberId,
                         StatusDescr = s.Descr,
                         DateCreated = c.DateCreated,
                         DateUpdated = c.DateUpdated,
                         DateStatus = c.DateStatus,
                     }).Where(criteria).OrderBy(ordering).Skip(skip).Take(pageSize).ToList();

                if (result != null)
                {
                    MenuService musvr = new MenuService();
                    foreach (DtoRotationLite dl in result)
                    {
                        dl.Key = musvr.EncryptData(dl.Id);
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

            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.Rotations
                     where c.CreatorId == creatorId && (topCriteria == null || tops.All(x => (c.Subject).Contains(x)))
                     select new DtoRotation
                     {
                         Id = c.Id,
                     }).Where(criteria).Count();

                return result;

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="topCriteria"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<DtoRotationLite> GetLiteAll(long memberId, string topCriteria, int page, int pageSize)
        {
            return GetLiteAll(memberId, topCriteria, page, pageSize, null, null);
        }
        public IEnumerable<DtoRotationLite> GetLiteAll(long memberId, string topCriteria, int page, int pageSize, string order)
        {
            return GetLiteAll(memberId, topCriteria, page, pageSize, order, null);
        }
        public IEnumerable<DtoRotationLite> GetLiteAll(long memberId, string topCriteria, int page, int pageSize, string order, string criteria)
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

            using (var db = new DrdContext(_connString))
            {
                var rotnodes = db.RotationNodes.Where(c => c.MemberId == memberId).ToList();
                long[] Ids = (from c in rotnodes select c.RotationId).ToArray();
                var result =
                    (from c in db.Rotations
                     join s in db.StatusCodes on c.Status equals s.Code
                     where //_appZoneAccess.Contains(c.AppZone) &&
                            (Ids.Contains(c.Id) || c.MemberId == memberId) &&
                            (topCriteria == null || tops.All(x => (c.Subject).Contains(x)))
                     select new DtoRotationLite
                     {
                         Id = c.Id,
                         Subject = c.Subject,
                         Status = c.Status,
                         WorkflowId = c.WorkflowId,
                         WorkflowName = c.Workflow.Name,
                         MemberId = c.MemberId,
                         StatusDescr = s.Descr,
                         DateCreated = c.DateCreated,
                         DateUpdated = c.DateUpdated,
                         DateStatus = c.DateStatus,
                     }).Where(criteria).OrderBy(ordering).Skip(skip).Take(pageSize).ToList();

                return result;

            }
        }

        public long GetLiteAllCount(long memberId, string topCriteria)
        {
            return GetLiteAllCount(memberId, topCriteria, null);
        }
        public long GetLiteAllCount(long memberId, string topCriteria, string criteria)
        {

            if (string.IsNullOrEmpty(criteria))
                criteria = "1=1";

            // top criteria
            string[] tops = new string[] { };
            if (!string.IsNullOrEmpty(topCriteria))
                tops = topCriteria.Split(' ');
            else
                topCriteria = null;

            using (var db = new DrdContext(_connString))
            {
                var rotnodes = db.RotationNodes.Where(c => c.MemberId == memberId).ToList();
                long[] Ids = (from c in rotnodes select c.RotationId).ToArray();

                var result =
                    (from c in db.Rotations
                     where //_appZoneAccess.Contains(c.AppZone) &&
                            (Ids.Contains(c.Id) || c.MemberId == memberId) &&
                            (topCriteria == null || tops.All(x => (c.Subject).Contains(x)))
                     select new DtoRotation
                     {
                         Id = c.Id,
                     }).Where(criteria).Count();

                return result;

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="status"></param>
        /// <param name="topCriteria"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="order"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public IEnumerable<DtoRotationLite> GetLiteStatusAll(long memberId, string status, string topCriteria, int page, int pageSize)
        {
            return GetLiteStatusAll(memberId, status, topCriteria, page, pageSize, null, null);
        }
        public IEnumerable<DtoRotationLite> GetLiteStatusAll(long memberId, string status, string topCriteria, int page, int pageSize, string order)
        {
            return GetLiteStatusAll(memberId, status, topCriteria, page, pageSize, order, null);
        }
        public IEnumerable<DtoRotationLite> GetLiteStatusAll(long memberId, string status, string topCriteria, int page, int pageSize, string order, string criteria)
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

            string[] sts = status.Split(',');

            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.Rotations
                     join s in db.StatusCodes on c.Status equals s.Code
                     where sts.Contains(c.Status) &&
                            (c.MemberId == memberId || c.RotationNodes.Any(x => x.MemberId == memberId)) &&
                            (topCriteria == null || tops.All(x => (c.Subject).Contains(x)))
                     select new DtoRotationLite
                     {
                         Id = c.Id,
                         Subject = c.Subject,
                         Status = c.Status,
                         WorkflowId = c.WorkflowId,
                         WorkflowName = c.Workflow.Name,
                         MemberId = c.MemberId,
                         StatusDescr = s.Descr,
                         DateCreated = c.DateCreated,
                         DateUpdated = c.DateUpdated,
                         DateStatus = c.DateStatus,
                     }).Where(criteria).OrderBy(ordering).Skip(skip).Take(pageSize).ToList();

                if (result != null)
                {
                    MenuService musvr = new MenuService();
                    foreach (DtoRotationLite dl in result)
                    {
                        dl.Key = musvr.EncryptData(dl.Id);
                    }
                }

                return result;

            }
        }

        public long GetLiteStatusAllCount(long memberId, string status, string topCriteria)
        {
            return GetLiteStatusAllCount(memberId, status, topCriteria, null);
        }
        public long GetLiteStatusAllCount(long memberId, string status, string topCriteria, string criteria)
        {

            if (string.IsNullOrEmpty(criteria))
                criteria = "1=1";

            // top criteria
            string[] tops = new string[] { };
            if (!string.IsNullOrEmpty(topCriteria))
                tops = topCriteria.Split(' ');
            else
                topCriteria = null;

            using (var db = new DrdContext(_connString))
            {
                var rotnodes = db.RotationNodes.Where(c => c.MemberId == memberId).ToList();
                long[] Ids = (from c in rotnodes select c.RotationId).ToArray();
                string[] sts = status.Split(',');
                var result =
                    (from c in db.Rotations
                     where sts.Contains(c.Status) &&
                            (Ids.Contains(c.Id) || c.MemberId == memberId) &&
                            (topCriteria == null || tops.All(x => (c.Subject).Contains(x)))
                     select new DtoRotation
                     {
                         Id = c.Id,
                     }).Where(criteria).Count();

                return result;

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="status"></param>
        /// <param name="topCriteria"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="order"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public IEnumerable<DtoRotationLite> GetNodeLiteAll(long memberId, string status, string topCriteria, int page, int pageSize, string order, string criteria)
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

            string[] sts = status.Split(',');

            using (var db = new DrdContext(_connString))
            {

                var result =
                    (from c in db.RotationNodes
                     join s in db.StatusCodes on c.Rotation.Status equals s.Code
                     where c.MemberId == memberId && sts.Contains(c.Status) &&
                            (topCriteria == null || tops.All(x => (c.Rotation.Subject).Contains(x)))
                     select new DtoRotationLite
                     {
                         Id = c.Id,
                         Subject = c.Rotation.Subject,
                         Status = c.Rotation.Status,
                         WorkflowId = c.Rotation.WorkflowId,
                         WorkflowName = c.Rotation.Workflow.Name,
                         ActivityName = c.WorkflowNode.Caption,
                         StatusDescr = s.Descr,
                         MemberId = c.MemberId,
                         DateCreated = c.DateCreated,
                         DateUpdated = c.DateUpdated,
                         DateStatus = c.Rotation.DateStatus,
                     }).Where(criteria).OrderBy(ordering).Skip(skip).Take(pageSize).ToList();

                if (result != null)
                {
                    MenuService musvr = new MenuService();
                    foreach (DtoRotationLite dl in result)
                    {
                        dl.Key = musvr.EncryptData(dl.Id);
                    }
                }

                return result;

            }
        }
        public IEnumerable<DtoRotationLite> GetNodeLiteAll(long memberId, string status, string topCriteria, int page, int pageSize)
        {
            return GetNodeLiteAll(memberId, status, topCriteria, page, pageSize, null, null);
        }

        public long GetNodeLiteAllCount(long memberId, string status, string topCriteria, string criteria)
        {

            if (string.IsNullOrEmpty(criteria))
                criteria = "1=1";

            // top criteria
            string[] tops = new string[] { };
            if (!string.IsNullOrEmpty(topCriteria))
                tops = topCriteria.Split(' ');
            else
                topCriteria = null;

            string[] sts = status.Split(',');

            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.RotationNodes
                     join s in db.StatusCodes on c.Rotation.Status equals s.Code
                     where c.MemberId == memberId && sts.Contains(c.Status) &&
                            (topCriteria == null || tops.All(x => (c.Rotation.Subject).Contains(x)))
                     select new DtoRotationLite
                     {
                         Id = c.Id,
                     }).Where(criteria).Count();

                return result;

            }
        }
        public long GetNodeLiteAllCount(long memberId, string status, string topCriteria)
        {
            return GetNodeLiteAllCount(memberId, status, topCriteria, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prod"></param>
        /// <returns></returns>
        public long Save(DtoRotation prod)
        {
            WfeService wfesvr = new WfeService();
            return wfesvr.Save(prod);
        }

        public int Start(long id)
        {
            WfeService wfesvr = new WfeService();
            var ret = wfesvr.Start(id, new MemberDepositTrxService());

            MemberService svr = new MemberService();
            foreach (JsonActivityResult act in ret)
            {
                svr.sendEmailInbox(act);
            }

            return ret.FirstOrDefault().ExitCode;
        }

        public int ProcessActivity(JsonProcessActivity param, System.Based.Core.ConfigConstant.EnumActivityAction bit)
        {
            WfeService wfesvr = new WfeService();
            var ret = wfesvr.ProcessActivity(param, bit, new DocumentService(), new MemberDepositTrxService());
            MemberService svr = new MemberService();
            foreach (JsonActivityResult act in ret)
            {
                svr.sendEmailInbox(act);
            }
            return ret.FirstOrDefault().ExitCode;
        }

        

    }
}
