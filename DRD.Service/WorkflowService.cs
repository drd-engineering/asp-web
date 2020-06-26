using DRD.Models;
using DRD.Models.Custom;
using DRD.Models.View;
using DRD.Service.Context;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;

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
        public WorkflowItem GetById(long id)
        {
            using (var db = new ServiceContext())
            {
                var result =
                    (from workflow in db.Workflows
                     where workflow.Id == id && workflow.IsActive
                     select new WorkflowItem
                     {
                         Id = workflow.Id,
                         Name = workflow.Name,
                         Description = workflow.Description,
                         IsActive = workflow.IsActive,
                         IsTemplate = workflow.IsTemplate,
                         IsUsed = (workflow.TotalUsed > 0),
                     }).FirstOrDefault();

                if (result != null)
                {
                    var nodes = db.WorkflowNodes.Where(wfnode => wfnode.WorkflowId == result.Id).ToList();
                    if (nodes.Count() > 0)
                    {
                        int dmid = 2;
                        result.WorkflowNodes = new List<WorkflowNodeItem>();
                        foreach (WorkflowNode workflowNode in nodes)
                        {
                            WorkflowNodeItem WorkflowNodeItem = new WorkflowNodeItem();
                            WorkflowNodeItem.Id = workflowNode.Id;
                            var elementname = getSymbolsFromCsvById(workflowNode.SymbolCode).ElementName;
                            if (elementname == "start")
                                WorkflowNodeItem.element = elementname + "-" + 0;
                            else if (elementname == "end")
                                WorkflowNodeItem.element = elementname + "-" + 1;
                            else
                            {
                                WorkflowNodeItem.element = getSymbolsFromCsvById(workflowNode.SymbolCode).ElementName + "-" + dmid;
                                dmid++;
                            }
                            WorkflowNodeItem.symbolCode = getSymbolsFromCsvById(workflowNode.SymbolCode).Code;
                            WorkflowNodeItem.caption = workflowNode.Caption;
                            WorkflowNodeItem.textColor = workflowNode.TextColor;
                            WorkflowNodeItem.backColor = workflowNode.BackColor;
                            WorkflowNodeItem.posLeft = workflowNode.PosLeft;
                            WorkflowNodeItem.posTop = workflowNode.PosTop;
                            WorkflowNodeItem.width = workflowNode.Width;
                            WorkflowNodeItem.height = workflowNode.Height;
                            result.WorkflowNodes.Add(WorkflowNodeItem);
                        }
                    }

                    var links = db.WorkflowNodeLinks.Where(wfNdLink => wfNdLink.Source.WorkflowId == result.Id).ToList();
                    if (links.Count() > 0)
                    {
                        result.WorkflowNodeLinks = new List<WorkflowNodeLinkItem>();

                        foreach (WorkflowNodeLink wfnl in links)
                        {
                            WorkflowNodeLinkItem jwfnl = new WorkflowNodeLinkItem();
                            jwfnl.NodeId = wfnl.SourceId;
                            jwfnl.NodeToId = wfnl.TargetId;
                            jwfnl.firstNodeId = wfnl.FirstNodeId;
                            jwfnl.elementFrom = result.WorkflowNodes.FirstOrDefault(wfNode => wfNode.Id == wfnl.SourceId).element;
                            jwfnl.elementTo = result.WorkflowNodes.FirstOrDefault(wfNode => wfNode.Id == wfnl.TargetId).element;
                            jwfnl.firstNode = result.WorkflowNodes.FirstOrDefault(wfNode => wfNode.Id == wfnl.FirstNodeId).element;
                            jwfnl.symbolCode = getSymbolsFromCsvById(wfnl.SymbolCode).Code;
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
        /// <param name="order"></param>
        /// <returns></returns>
        public ICollection<WorkflowItem> FindWorkflows(long creatorId, string topCriteria, int page, int pageSize, Expression<Func<WorkflowItem, string>> order)
        {
            Expression<Func<WorkflowItem, bool>> criteriaUsed = WorkflowItem => true;
            return FindWorkflows(creatorId, topCriteria, page, pageSize, order, criteriaUsed);
        }

        public ICollection<WorkflowItem> FindWorkflows(long creatorId, string topCriteria, int page, int pageSize, Expression<Func<WorkflowItem, string>> order, Expression<Func<WorkflowItem, bool>> criteria,bool isActive=true)
        {
            int skip = pageSize * (page - 1);
            Expression<Func<WorkflowItem, string>> ordering = WorkflowItem => "DateCreated desc, IsTemplate desc, Name";

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
                var result =
                    (from workflow in db.Workflows
                     where workflow.CreatorId == creatorId && workflow.IsActive==isActive && (topCriteria.Equals("") || tops.All(x => (workflow.Name + " " + workflow.Description).ToLower().Contains(x.ToLower())))
                     orderby workflow.CreatedAt descending, workflow.IsTemplate descending, workflow.Name
                     select new WorkflowItem
                     {
                         Id = workflow.Id,
                         Name = workflow.Name,
                         Description = workflow.Description,
                         IsActive = workflow.IsActive,
                         IsTemplate = workflow.IsTemplate,
                         CreatedAt = workflow.CreatedAt,
                         UpdatedAt = workflow.UpdatedAt,
                     }).Where(criteria).Skip(skip).Take(pageSize).ToList();
                if (result != null)
                {
                    MenuService menuService = new MenuService();
                    for (var i = 0; i < result.Count(); i++)
                    {
                        var item = result.ElementAt(i);
                        item.Key = menuService.EncryptData(item.Id);
                        result[i] = item;
                    }
                }
                return result;
            }
        }
        public int FindWorkflowsCountAll(long creatorId, string topCriteria, bool isActive = true)
        {
            // top criteria
            string[] tops = new string[] { };
            if (!string.IsNullOrEmpty(topCriteria))
                tops = topCriteria.Split(' ');
            else
                topCriteria = "";

            using (var db = new ServiceContext())
            {
                var result =
                    (from workflow in db.Workflows
                     where workflow.CreatorId == creatorId && workflow.IsActive == isActive && (topCriteria.Equals("") || tops.All(x => (workflow.Name + " " + workflow.Description).ToLower().Contains(x.ToLower())))
                     orderby workflow.CreatedAt descending, workflow.IsTemplate descending, workflow.Name
                     select new WorkflowItem
                     {
                         Id = workflow.Id,
                         Name = workflow.Name,
                         Description = workflow.Description,
                         IsActive = workflow.IsActive,
                         IsTemplate = workflow.IsTemplate,
                         CreatedAt = workflow.CreatedAt,
                         UpdatedAt = workflow.UpdatedAt,
                     }).Count();
                return result;
            }
        }

        private bool checkIdExist(long id)
        {
            using (var db = new ServiceContext())
            {

                var count = db.Workflows.Where(i => i.Id == id).FirstOrDefault();

                return count != null;

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="WorkflowItem"></param>
        /// <returns></returns>
        /// 
        public int Save(WorkflowItem WorkflowItem)
        {
            Workflow product;
            using (var db = new ServiceContext())
            {
                // validity subscription type
                var creator = db.Users.FirstOrDefault(user => user.Id == WorkflowItem.CreatorId);
                var actCount = WorkflowItem.WorkflowNodes.Count(workflow => workflow.symbolCode.Equals("ACTIVITY"));

                if (WorkflowItem.Id != 0)
                    product = db.Workflows.FirstOrDefault(workflow => workflow.Id == WorkflowItem.Id);
                else
                {
                    product = new Workflow();
                    while (checkIdExist(product.Id))
                    {
                        product.Id = Utilities.RandomLongGenerator(minimumValue: Constant.MINIMUM_VALUE_ID, maximumValue: Constant.MAXIMUM_VALUE_ID);
                    }
                }

                product.Name = WorkflowItem.Name;
                product.Description = WorkflowItem.Description;
                product.IsActive = WorkflowItem.IsActive;
                product.IsTemplate = WorkflowItem.IsTemplate;
                product.CreatorId = WorkflowItem.CreatorId;
                if (WorkflowItem.Id == 0)
                {
                    db.Workflows.Add(product);
                }

                var result = db.SaveChanges();

                var isused = false;
                if (WorkflowItem.Id != 0 && product.TotalUsed > 0)
                    isused = true;

                if (!isused) // skip when workflow is used by transaction
                {
                    // delete existing node
                    if (WorkflowItem.Id != 0)
                    {
                        var oldNodeLinks = db.WorkflowNodeLinks.Where(workflow => workflow.Source.WorkflowId == WorkflowItem.Id || workflow.Target.WorkflowId == WorkflowItem.Id).ToList();
                        db.WorkflowNodeLinks.RemoveRange(oldNodeLinks);
                        db.SaveChanges();

                        var oldNodes = db.WorkflowNodes.Where(workflow => workflow.WorkflowId == WorkflowItem.Id).ToList();
                        db.WorkflowNodes.RemoveRange(oldNodes);
                        db.SaveChanges();
                    }

                    // save node
                    if (WorkflowItem.WorkflowNodes != null)
                    {
                        foreach (WorkflowNodeItem jnode in WorkflowItem.WorkflowNodes)
                        {
                            var node = new WorkflowNode();
                            node.WorkflowId = product.Id;
                            node.SymbolCode = getSymbolsFromCsvByCode(jnode.symbolCode).Id;
                            //node.SymbolCode = getSymbolsFromCsvByCode(jnode.symbolCode).Id;

                            node.Caption = jnode.caption;
                            node.PosLeft = jnode.posLeft;
                            node.PosTop = jnode.posTop;
                            node.Width = jnode.width;
                            node.Height = jnode.height;
                            node.TextColor = jnode.textColor;
                            node.BackColor = jnode.backColor;
                            db.WorkflowNodes.Add(node);
                            db.SaveChanges();
                            jnode.Id = node.Id;
                        }
                    }
                    // save node link
                    if (WorkflowItem.WorkflowNodeLinks != null)
                    {
                        foreach (WorkflowNodeLinkItem jnodelink in WorkflowItem.WorkflowNodeLinks)
                        {
                            var nodelink = new WorkflowNodeLink();
                            nodelink.SourceId = WorkflowItem.WorkflowNodes.FirstOrDefault(workflow => workflow.element.Equals(jnodelink.elementFrom)).Id;
                            var wfnod = db.WorkflowNodes.FirstOrDefault(c => c.Id == nodelink.SourceId);
                            nodelink.Source = wfnod;
                            nodelink.TargetId = WorkflowItem.WorkflowNodes.FirstOrDefault(workflow => workflow.element.Equals(jnodelink.elementTo)).Id;
                            var to = db.WorkflowNodes.FirstOrDefault(c => c.Id == nodelink.TargetId);
                            nodelink.Target = to;
                            nodelink.SymbolCode = getSymbolsFromCsvByCode(jnodelink.symbolCode).Id;
                            nodelink.FirstNodeId = WorkflowItem.WorkflowNodes.FirstOrDefault(workflow => workflow.element.Equals(jnodelink.firstNode)).Id;
                            var first = db.WorkflowNodes.FirstOrDefault(c => c.Id == nodelink.FirstNodeId);
                            nodelink.FirstNode = first;
                            db.WorkflowNodeLinks.Add(nodelink);
                            db.SaveChanges();
                        }
                    }
                }
                return result;
            }
        }
        public static Symbol getSymbolsFromCsvByCode(string code)
        {
            if (code.Equals("UPLOAD"))
            {
                code = "ACTIVITY";
            }
            var root = System.Web.HttpContext.Current.Server.MapPath("~");
            var path = Path.Combine(root, @"Symbols.csv");
            Symbol values = File.ReadAllLines(path)
                                           .Select(v => Symbol.FromCsv(v))
                                           .Where(c => c.Code.Equals(code)).FirstOrDefault();

            return values;
        }
        public static Symbol getSymbolsFromCsvById(int id)
        {
            var root = System.Web.HttpContext.Current.Server.MapPath("~");
            var path = Path.Combine(root, @"Symbols.csv");
            Symbol values = File.ReadAllLines(path)
                                           .Select(v => Symbol.FromCsv(v))
                                           .Where(c => c.Id == id).FirstOrDefault();

            return values;
        }

        public string Delete(long id)
        {
            using (var db = new ServiceContext())
            {
                var workflow = db.Workflows.Where(i => i.Id == id).FirstOrDefault();

                //check if workflow exist
                if (workflow == null) return Constant.WorkflowStatus.NOT_FOUND.ToString();

                //check if workflow is already being used
                var rotationNode = db.Rotations.Where(c => c.WorkflowId == id).FirstOrDefault();
                if (rotationNode != null) return Constant.WorkflowStatus.USED_IN_ROTATION.ToString();

                workflow.IsActive = false;
                workflow.UpdatedAt = DateTime.Now;
                db.SaveChanges();
                return Constant.WorkflowStatus.OK.ToString();
            }
        }
    }
}
