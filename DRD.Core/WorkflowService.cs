using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;
using System.Based.Core.Entity;
using DRD.Domain;
using System.Configuration;
using System.Based.Core;

namespace DRD.Core
{
    public class WorkflowService
    {
        private readonly string _connString;
        private string _appZoneAccess;

        public WorkflowService(string appZoneAccess, string connString)
        {
            _appZoneAccess = appZoneAccess;
            _connString = connString;
        }

        public WorkflowService(string appZoneAccess)
        {
            _appZoneAccess = appZoneAccess;
            _connString = ConfigConstant.CONSTRING;
        }
        public WorkflowService()
        {
            _connString = ConfigConstant.CONSTRING;
        }


        //
        // for edit data
        //
        public JsonWorkflow GetById(long id)
        {
            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.Workflows
                     where c.Id == id
                     select new JsonWorkflow
                     {
                         Id = c.Id,
                         Name = c.Name,
                         ProjectId = c.ProjectId,
                         Descr = c.Descr,
                         UserId = c.UserId,
                         IsActive = c.IsActive,
                         IsTemplate = c.IsTemplate,
                         WfType = c.WfType,
                         Project = new JsonWorkflowProject
                         {
                             Id = c.Project.Id,
                             Name = c.Project.Name,
                         },
                         IsUsed = (c.Rotations.Count() > 0),
                     }).FirstOrDefault();

                if (result != null)
                {
                    var nodes = db.WorkflowNodes.Where(c => c.WorkflowId == result.Id).ToList();
                    if (nodes.Count() > 0)
                    {
                        int dmid = 0;
                        result.WorkflowNodes = new List<JsonWorkflowNode>();
                        foreach (WorkflowNode wfn in nodes)
                        {
                            JsonWorkflowNode jwfn = new JsonWorkflowNode();
                            jwfn.Id = wfn.Id;
                            jwfn.element = wfn.Symbol.ElementName + "-" + dmid;
                            jwfn.symbolCode = wfn.Symbol.Code;
                            jwfn.memberId = wfn.MemberId;
                            jwfn.caption = wfn.Caption;
                            jwfn.info = wfn.Info;
                            jwfn.Operator = wfn.Operator;
                            jwfn.value = wfn.Value;
                            jwfn.textColor = wfn.TextColor;
                            jwfn.backColor = wfn.BackColor;
                            jwfn.posLeft = wfn.PosLeft;
                            jwfn.posTop = wfn.PosTop;
                            jwfn.width = wfn.Width;
                            jwfn.height = wfn.Height;
                            jwfn.member = (wfn.Member != null ? new JsonWorkflowNodeMember
                            {
                                number = wfn.Member.Number,
                                name = wfn.Member.Name,
                                email = wfn.Member.Email,
                                imageProfile = wfn.Member.ImageProfile,
                            } : null);

                            result.WorkflowNodes.Add(jwfn);
                            dmid++;
                        }
                    }

                    var links = db.WorkflowNodeLinks.Where(c => c.WorkflowNode_WorkflowNodeId.WorkflowId == result.Id).ToList();
                    if (links.Count() > 0)
                    {
                        result.WorkflowNodeLinks = new List<JsonWorkflowNodeLink>();

                        foreach (WorkflowNodeLink wfnl in links)
                        {
                            JsonWorkflowNodeLink jwfnl = new JsonWorkflowNodeLink();
                            jwfnl.NodeId = wfnl.WorkflowNodeId;
                            jwfnl.NodeToId = wfnl.WorkflowNodeToId;
                            jwfnl.elementFrom = result.WorkflowNodes.FirstOrDefault(c => c.Id == wfnl.WorkflowNodeId).element;
                            jwfnl.elementTo = result.WorkflowNodes.FirstOrDefault(c => c.Id == wfnl.WorkflowNodeToId).element;
                            jwfnl.symbolCode = wfnl.Symbol.Code;
                            jwfnl.caption = wfnl.Caption;
                            jwfnl.Operator = wfnl.Operator;
                            jwfnl.value = wfnl.Value;

                            result.WorkflowNodeLinks.Add(jwfnl);
                        }
                    }
                }
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
        public IEnumerable<DtoWorkflowLite> GetLiteAll(long creatorId, string topCriteria, int page, int pageSize)
        {
            return GetLiteAll(creatorId, topCriteria, page, pageSize, null, null);
        }

        public IEnumerable<DtoWorkflowLite> GetLiteAll(long creatorId, string topCriteria, int page, int pageSize, string order)
        {
            return GetLiteAll(creatorId, topCriteria, page, pageSize, order, null);
        }

        public IEnumerable<DtoWorkflowLite> GetLiteAll(long creatorId, string topCriteria, int page, int pageSize, string order, string criteria)
        {
            int skip = pageSize * (page - 1);
            string ordering = "IsTemplate desc, Name";

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
                    (from c in db.Workflows
                     where c.CreatorId == creatorId && (topCriteria == null || tops.All(x => (c.Name + " " + c.Descr).Contains(x)))
                     select new DtoWorkflowLite
                     {
                         Id = c.Id,
                         Name = c.Name,
                         ProjectId = c.ProjectId,
                         Descr = c.Descr,
                         IsActive = c.IsActive,
                         IsTemplate = c.IsTemplate,
                         WfType = c.WfType,
                         ProjectName = c.Project.Name,
                         UserId = c.UserId,
                         DateCreated = c.DateCreated,
                         DateUpdated = c.DateUpdated,
                     }).Where(criteria).OrderBy(ordering).Skip(skip).Take(pageSize).ToList();

                if (result != null)
                {
                    MenuService musvr = new MenuService();
                    foreach (DtoWorkflowLite dl in result)
                    {
                        dl.Key = musvr.EncryptData(dl.Id);
                    }
                }
                return result;

            }
        }

        public long GetLiteAllCount(long creatorId, string topCriteria)
        {
            return GetLiteAllCount(creatorId, topCriteria, null);
        }

        public long GetLiteAllCount(long creatorId, string topCriteria, string criteria)
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
                    (from c in db.Workflows
                     where c.CreatorId == creatorId && (topCriteria == null || tops.All(x => (c.Name + " " + c.Descr).Contains(x)))
                     select new DtoWorkflow
                     {
                         Id = c.Id,
                         WfType = c.WfType,
                     }).Where(criteria).Count();

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
        public IEnumerable<DtoWorkflowLite> GetPopupAll(long creatorId, string topCriteria, int page, int pageSize)
        {
            return GetPopupAll(creatorId, topCriteria, page, pageSize, null, null);
        }

        public IEnumerable<DtoWorkflowLite> GetPopupAll(long creatorId, string topCriteria, int page, int pageSize, string order)
        {
            return GetPopupAll(creatorId, topCriteria, page, pageSize, order, null);
        }

        public IEnumerable<DtoWorkflowLite> GetPopupAll(long creatorId, string topCriteria, int page, int pageSize, string order, string criteria)
        {
            int skip = pageSize * (page - 1);
            string ordering = "IsTemplate desc, Name";

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
                    (from c in db.Workflows
                     where (c.CreatorId == creatorId || (c.IsTemplate && c.IsActive)) && (topCriteria == null || tops.All(x => (c.Name + " " + c.Descr).Contains(x)))
                     select new DtoWorkflowLite
                     {
                         Id = c.Id,
                         Name = c.Name,
                         ProjectId = c.ProjectId,
                         Descr = c.Descr,
                         IsActive = c.IsActive,
                         IsTemplate = c.IsTemplate,
                         WfType = c.WfType,
                         ProjectName = c.Project.Name,
                         UserId = c.UserId,
                         DateCreated = c.DateCreated,
                         DateUpdated = c.DateUpdated,
                     }).Where(criteria).OrderBy(ordering).Skip(skip).Take(pageSize).ToList();

                if (result != null)
                {
                    MenuService musvr = new MenuService();
                    foreach (DtoWorkflowLite dl in result)
                    {
                        dl.Key = musvr.EncryptData(dl.Id);
                    }
                }
                return result;

            }
        }

        public long GetPopupAllCount(long creatorId, string topCriteria)
        {
            return GetPopupAllCount(creatorId, topCriteria, null);
        }

        public long GetPopupAllCount(long creatorId, string topCriteria, string criteria)
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
                    (from c in db.Workflows
                     where (c.CreatorId == creatorId || (c.IsTemplate && c.IsActive)) && (topCriteria == null || tops.All(x => (c.Name + " " + c.Descr).Contains(x)))
                     select new DtoWorkflow
                     {
                         Id = c.Id,
                     }).Where(criteria).Count();

                return result;

            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="prod"></param>
        /// <returns></returns>
        /// 

        public int Save(JsonWorkflow prod)
        {
            Workflow product;
            using (var db = new DrdContext(_connString))
            {
                // validity subscription type
                var mem = db.Members.FirstOrDefault(c => c.Id == prod.CreatorId);
                var plan = mem.MemberPlans.FirstOrDefault(c => c.IsDefault);
                if (plan == null && prod.WfType == 0)
                    return -2;

                var actCount = prod.WorkflowNodes.Count(c => c.symbolCode.Equals("ACTIVITY"));
                if (actCount > plan.FlowActivityCount + plan.FlowActivityCountAdd && prod.WfType == 0)
                    return -1; // out of quota 

                if (prod.Id != 0)
                    product = db.Workflows.FirstOrDefault(c => c.Id == prod.Id);
                else
                    product = new Workflow();

                product.Name = prod.Name;
                product.Descr = prod.Descr;
                product.ProjectId = prod.ProjectId;
                product.IsActive = prod.IsActive;
                product.IsTemplate = prod.IsTemplate;
                product.WfType = prod.WfType;
                product.CreatorId = prod.CreatorId;
                product.UserId = prod.UserId;
                if (prod.Id == 0)
                {
                    product.DateCreated = DateTime.Now;
                    db.Workflows.Add(product);
                }
                else
                    product.DateUpdated = DateTime.Now;

                var result = db.SaveChanges();

                var isused = false;
                if (prod.Id != 0 && product.Rotations.Count() > 0)
                    isused = true;

                if (!isused) // skip when workflow is used by transaction
                {
                    // delete existing node
                    if (prod.Id != 0)
                    {
                        var oldNodeLinks = db.WorkflowNodeLinks.Where(c => c.WorkflowNode_WorkflowNodeId.WorkflowId == prod.Id || c.WorkflowNode_WorkflowNodeToId.WorkflowId == prod.Id).ToList();
                        db.WorkflowNodeLinks.RemoveRange(oldNodeLinks);
                        db.SaveChanges();

                        var oldNodes = db.WorkflowNodes.Where(c => c.WorkflowId == prod.Id).ToList();
                        db.WorkflowNodes.RemoveRange(oldNodes);
                        db.SaveChanges();
                    }

                    // save node
                    if (prod.WorkflowNodes != null)
                    {
                        foreach (JsonWorkflowNode jnode in prod.WorkflowNodes)
                        {
                            var node = new WorkflowNode();
                            node.WorkflowId = product.Id;
                            node.MemberId = (jnode.memberId == 0 ? null : jnode.memberId);
                            node.SymbolId = db.Symbols.FirstOrDefault(c => c.Code.Equals(jnode.symbolCode)).Id;
                            node.Caption = jnode.caption;
                            node.Info = jnode.info;
                            node.Operator = jnode.Operator;
                            node.Value = jnode.value;
                            node.PosLeft = jnode.posLeft;
                            node.PosTop = jnode.posTop;
                            node.Width = jnode.width;
                            node.Height = jnode.height;
                            node.TextColor = jnode.textColor;
                            node.BackColor = jnode.backColor;
                            node.Flag = 0;
                            db.WorkflowNodes.Add(node);
                            db.SaveChanges();
                            jnode.Id = node.Id;
                        }
                    }
                    // save node link
                    if (prod.WorkflowNodeLinks != null)
                    {
                        foreach (JsonWorkflowNodeLink jnodelink in prod.WorkflowNodeLinks)
                        {
                            var nodelink = new WorkflowNodeLink();
                            nodelink.WorkflowNodeId = prod.WorkflowNodes.FirstOrDefault(c => c.element.Equals(jnodelink.elementFrom)).Id;
                            nodelink.WorkflowNodeToId = prod.WorkflowNodes.FirstOrDefault(c => c.element.Equals(jnodelink.elementTo)).Id;
                            nodelink.Caption = jnodelink.caption;
                            nodelink.SymbolId = db.Symbols.FirstOrDefault(c => c.Code.Equals(jnodelink.symbolCode)).Id;
                            nodelink.Operator = jnodelink.Operator;
                            nodelink.Value = jnodelink.value;
                            db.WorkflowNodeLinks.Add(nodelink);
                            db.SaveChanges();
                        }
                    }
                }


                return result;

            }

        }

    }
}
