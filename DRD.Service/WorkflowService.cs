using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DRD.Models;
using DRD.Models.Custom;
using DRD.Service.Context;

namespace DRD.Service
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
            _connString = Constant.CONSTRING;
        }
        public WorkflowService()
        {
            _connString = Constant.CONSTRING;
        }


        //
        // for edit data
        //
        public WorkflowData GetById(long id)
        {
            using (var db = new ServicesContext())
            {
                var result =
                    (from workflow in db.Workflows
                     where workflow.Id == id
                     select new WorkflowData
                     {
                         Id = workflow.Id,
                         Name = workflow.Name,
                         Description = workflow.Description,
                         UserId = workflow.UserId,
                         IsActive = workflow.IsActive,
                         IsTemplate = workflow.IsTemplate,
                         Type = workflow.Type,
                         IsUsed = (workflow.TotalUsed > 0),
                     }).FirstOrDefault();

                if (result != null)
                {
                    var nodes = db.WorkflowNodes.Where(workflow => workflow.WorkflowId == result.Id).ToList();
                    if (nodes.Count() > 0)
                    {
                        int dmid = 0;
                        result.WorkflowNodes = new List<WorkflowNodeData>();
                        foreach (WorkflowNode workflowNode in nodes)
                        {
                            WorkflowNodeData workflowNodeData = new WorkflowNodeData();
                            workflowNodeData.Id = workflowNode.Id;
                            workflowNodeData.element = workflowNode.Symbol.Name + "-" + dmid;
                            workflowNodeData.symbolCode = workflowNode.Symbol.Code;
                            workflowNodeData.memberId = workflowNode.MemberId;
                            workflowNodeData.caption = workflowNode.Caption;
                            workflowNodeData.info = workflowNode.Info;
                            workflowNodeData.Operator = workflowNode.Operator;
                            workflowNodeData.value = workflowNode.Value;
                            workflowNodeData.textColor = workflowNode.TextColor;
                            workflowNodeData.backColor = workflowNode.BackColor;
                            workflowNodeData.posLeft = workflowNode.PosLeft;
                            workflowNodeData.posTop = workflowNode.PosTop;
                            workflowNodeData.width = workflowNode.Width;
                            workflowNodeData.height = workflowNode.Height;
                            workflowNodeData.user = (workflowNode.User != null ? new WorkflowNodeUser
                            {
                                id = workflowNode.User.Id,
                                name = workflowNode.User.Name,
                                email = workflowNode.User.Email,
                                imageProfile = workflowNode.User.ImageProfile,
                            } : null);

                            result.WorkflowNodes.Add(workflowNodeData);
                            dmid++;
                        }
                    }

                    var links = db.WorkflowNodeLinks.Where(workflow => workflow.WorkflowNodes.WorkflowId == result.Id).ToList();
                    if (links.Count() > 0)
                    {
                        result.WorkflowNodeLinks = new List<WorkflowNodeLinkData>();

                        foreach (WorkflowNodeLink wfnl in links)
                        {
                            WorkflowNodeLinkData jwfnl = new WorkflowNodeLinkData();
                            jwfnl.NodeId = wfnl.WorkflowNodeId;
                            jwfnl.NodeToId = wfnl.WorkflowNodeToId;
                            jwfnl.elementFrom = result.WorkflowNodes.FirstOrDefault(workflow => workflow.Id == wfnl.WorkflowNodeId).element;
                            jwfnl.elementTo = result.WorkflowNodes.FirstOrDefault(workflow => workflow.Id == wfnl.WorkflowNodeToId).element;
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
        public IEnumerable<WorkflowData> GetLiteAll(long creatorId, string topCriteria, int page, int pageSize)
        {
            return GetLiteAll(creatorId, topCriteria, page, pageSize, null, null);
        }

        public IEnumerable<WorkflowData> GetLiteAll(long creatorId, string topCriteria, int page, int pageSize, Expression<Func<Workflow, string>> order)
        {
            return GetLiteAll(creatorId, topCriteria, page, pageSize, order, null);
        }

        public IEnumerable<WorkflowData> GetLiteAll(long creatorId, string topCriteria, int page, int pageSize, Expression<Func<Workflow, string>> order, Expression<System.Func<DRD.Models.Workflow ,bool>> criteria)
        {
            int skip = pageSize * (page - 1);
            Expression<Func<Workflow, string>> ordering = Workflow => Workflow.Name;

            if (order != null)
                ordering = order;

            // top criteria
            string[] tops = new string[] { };
            if (!string.IsNullOrEmpty(topCriteria))
                tops = topCriteria.Split(' ');
            else
                topCriteria = null;
                
            using (var db = new ServicesContext())
            {
                var result =
                    (from workflow in db.Workflows
                     where workflow.CreatorId == creatorId && (topCriteria == null || tops.All(x => (workflow.Name + " " + workflow.Description).Contains(x)))
                     select new WorkflowData
                     {
                         Id = workflow.Id,
                         Name = workflow.Name,
                         Description = workflow.Description,
                         IsActive = workflow.IsActive,
                         IsTemplate = workflow.IsTemplate,
                         Type = workflow.Type,
                         UserId = workflow.UserId,
                         DateCreated = workflow.DateCreated,
                         DateUpdated = workflow.DateUpdated,
                     }).Take(pageSize).ToList();

                if (result != null)
                {
                    MenuService menuService = new MenuService();
                    foreach (WorkflowData workflow in result)
                    {
                        workflow.Key = menuService.EncryptData(workflow.Id);
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

            using (var db = new ServicesContext())
            {
                var result =
                    (from workflow in db.Workflows
                     where workflow.CreatorId == creatorId && (topCriteria == null || tops.All(x => (workflow.Name + " " + workflow.Description).Contains(x)))
                     select new Workflow
                     {
                         Id = workflow.Id,
                         Type = workflow.Type,
                     }).Count();

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
        public IEnumerable<WorkflowData> GetPopupAll(long creatorId, string topCriteria, int page, int pageSize)
        {
            return GetPopupAll(creatorId, topCriteria, page, pageSize, null, null);
        }

        public IEnumerable<WorkflowData> GetPopupAll(long creatorId, string topCriteria, int page, int pageSize, string order)
        {
            return GetPopupAll(creatorId, topCriteria, page, pageSize, order, null);
        }

        public IEnumerable<WorkflowData> GetPopupAll(long creatorId, string topCriteria, int page, int pageSize, string order, string criteria)
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

            using (var db = new ServicesContext())
            {
                var result =
                    (from workflow in db.Workflows
                     where (workflow.CreatorId == creatorId || (workflow.IsTemplate && workflow.IsActive)) && (topCriteria == null || tops.All(x => (workflow.Name + " " + workflow.Description).Contains(x)))
                     select new WorkflowData
                     {
                         Id = workflow.Id,
                         Name = workflow.Name,
                         Description = workflow.Description,
                         IsActive = workflow.IsActive,
                         IsTemplate = workflow.IsTemplate,
                         Type = workflow.Type,
                         UserId = workflow.UserId,
                         DateCreated = workflow.DateCreated,
                         DateUpdated = workflow.DateUpdated,
                     }).Take(pageSize).ToList();

                if (result != null)
                {
                    MenuService menuService = new MenuService();
                    foreach (WorkflowData workflowData in result)
                    {
                        workflowData.Key = menuService.EncryptData(workflowData.Id);
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

            using (var db = new ServicesContext())
            {
                var result =
                    (from workflow in db.Workflows
                     where (workflow.CreatorId == creatorId || (workflow.IsTemplate && workflow.IsActive)) && (topCriteria == null || tops.All(x => (workflow.Name + " " + workflow.Description).Contains(x)))
                     select new Workflow
                     {
                         Id = workflow.Id,
                     }).Count();

                return result;

            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="workflowData"></param>
        /// <returns></returns>
        /// 

        public int Save(WorkflowData workflowData)
        {
            Workflow product;
            using (var db = new ServicesContext())
            {
                // validity subscription type
                var member = db.Members.FirstOrDefault(workflow => workflow.Id == workflowData.CreatorId);                
                var actCount = workflowData.WorkflowNodes.Count(workflow => workflow.symbolCode.Equals("ACTIVITY"));
            
                if (workflowData.Id != 0)
                    product = db.Workflows.FirstOrDefault(workflow => workflow.Id == workflowData.Id);
                else
                    product = new Workflow();

                product.Name = workflowData.Name;
                product.Description = workflowData.Description;
                product.IsActive = workflowData.IsActive;
                product.IsTemplate = workflowData.IsTemplate;
                product.Type = workflowData.Type;
                product.CreatorId = workflowData.CreatorId;
                product.UserId = workflowData.UserId;
                if (workflowData.Id == 0)
                {
                    product.DateCreated = DateTime.Now;
                    db.Workflows.Add(product);
                }
                else
                    product.DateUpdated = DateTime.Now;

                var result = db.SaveChanges();

                var isused = false;
                if (workflowData.Id != 0 && product.TotalUsed > 0)
                    isused = true;

                if (!isused) // skip when workflow is used by transaction
                {
                    // delete existing node
                    if (workflowData.Id != 0)
                    {
                        var oldNodeLinks = db.WorkflowNodeLinks.Where(workflow => workflow.WorkflowNodes.WorkflowId == workflowData.Id || workflow.WorkflowNodeTos.WorkflowId == workflowData.Id).ToList();
                        db.WorkflowNodeLinks.RemoveRange(oldNodeLinks);
                        db.SaveChanges();

                        var oldNodes = db.WorkflowNodes.Where(workflow => workflow.WorkflowId == workflowData.Id).ToList();
                        db.WorkflowNodes.RemoveRange(oldNodes);
                        db.SaveChanges();
                    }

                    // save node
                    if (workflowData.WorkflowNodes != null)
                    {
                        foreach (WorkflowNodeData jnode in workflowData.WorkflowNodes)
                        {
                            var node = new WorkflowNode();
                            node.WorkflowId = product.Id;
                            node.MemberId = (jnode.memberId == 0 ? null : jnode.memberId);
                            node.SymbolId = db.Symbols.FirstOrDefault(workflow => workflow.Code.Equals(jnode.symbolCode)).Id;
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
                    if (workflowData.WorkflowNodeLinks != null)
                    {
                        foreach (WorkflowNodeLinkData jnodelink in workflowData.WorkflowNodeLinks)
                        {
                            var nodelink = new WorkflowNodeLink();
                            nodelink.WorkflowNodeId = workflowData.WorkflowNodes.FirstOrDefault(workflow => workflow.element.Equals(jnodelink.elementFrom)).Id;
                            nodelink.WorkflowNodeToId = workflowData.WorkflowNodes.FirstOrDefault(workflow => workflow.element.Equals(jnodelink.elementTo)).Id;
                            nodelink.Caption = jnodelink.caption;
                            nodelink.SymbolId = db.Symbols.FirstOrDefault(workflow => workflow.Code.Equals(jnodelink.symbolCode)).Id;
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
