using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.IO;

using DRD.Models;
using DRD.Models.View;
using DRD.Models.Custom;
using DRD.Models.API;
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
        public WorkflowItem GetById(long id)
        {
            using (var db = new ServiceContext())
            {
                var result =
                    (from workflow in db.Workflows
                     where workflow.Id == id
                     select new WorkflowItem
                     {
                         Id = workflow.Id,
                         Name = workflow.Name,
                         Description = workflow.Description,
                         UserEmail = workflow.UserEmail,
                         IsActive = workflow.IsActive,
                         IsTemplate = workflow.IsTemplate,
                         Type = workflow.Type,
                         IsUsed = (workflow.TotalUsed > 0),
                     }).FirstOrDefault();

                if (result != null)
                {
                    var nodes = db.WorkflowNodes.Where(wfnode => wfnode.WorkflowId == result.Id).ToList();
                    if (nodes.Count() > 0)
                    {
                        int dmid = 0;
                        result.WorkflowNodes = new List<WorkflowNodeItem>();
                        foreach (WorkflowNode workflowNode in nodes)
                        {
                            WorkflowNodeItem WorkflowNodeItem = new WorkflowNodeItem();
                            WorkflowNodeItem.Id = workflowNode.Id;
                            WorkflowNodeItem.element = getSymbolsFromCsvById(workflowNode.SymbolCode).ElementName + "-" + dmid;
                            WorkflowNodeItem.symbolCode = getSymbolsFromCsvById(workflowNode.SymbolCode).Code;
                            WorkflowNodeItem.userId = workflowNode.UserId;
                            WorkflowNodeItem.caption = workflowNode.Caption;
                            WorkflowNodeItem.info = workflowNode.Info;
                            WorkflowNodeItem.Operator = workflowNode.Operator;
                            WorkflowNodeItem.value = workflowNode.Value;
                            WorkflowNodeItem.textColor = workflowNode.TextColor;
                            WorkflowNodeItem.backColor = workflowNode.BackColor;
                            WorkflowNodeItem.posLeft = workflowNode.PosLeft;
                            WorkflowNodeItem.posTop = workflowNode.PosTop;
                            WorkflowNodeItem.width = workflowNode.Width;
                            WorkflowNodeItem.height = workflowNode.Height;
                            User user = db.Users.Where(dbuser => dbuser.Id == workflowNode.UserId).FirstOrDefault();
                            if (user != null) {
                                WorkflowNodeItem.user = new WorkflowNodeUser
                                {
                                    id = user.Id,
                                    name = user.Name,
                                    email = user.Email,
                                    imageProfile = user.ImageProfile,
                                };
                            }
                            else
                            {
                                WorkflowNodeItem.user = null;
                            }
                            result.WorkflowNodes.Add(WorkflowNodeItem);
                            dmid++;
                        }
                    }

                    var links = db.WorkflowNodeLinks.Where(wfNdLink => wfNdLink.WorkflowNodes.WorkflowId == result.Id).ToList();
                    System.Diagnostics.Debug.WriteLine("[[JUMLAH WFNODELINKS]] " + links.Count());
                    if (links.Count() > 0)
                    {
                        result.WorkflowNodeLinks = new List<WorkflowNodeLinkItem>();

                        foreach (WorkflowNodeLink wfnl in links)
                        {
                            WorkflowNodeLinkItem jwfnl = new WorkflowNodeLinkItem();
                            jwfnl.NodeId = wfnl.WorkflowNodeId;
                            jwfnl.NodeToId = wfnl.WorkflowNodeToId;
                            jwfnl.elementFrom = result.WorkflowNodes.FirstOrDefault(wfNode => wfNode.Id == wfnl.WorkflowNodeId).element;
                            jwfnl.elementTo = result.WorkflowNodes.FirstOrDefault(wfNode => wfNode.Id == wfnl.WorkflowNodeToId).element;
                            jwfnl.symbolCode = getSymbolsFromCsvById(wfnl.SymbolCode).Code;
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
        public ListWorkflowItem FindWorkflows(long creatorId, string topCriteria, int page, int pageSize)
        {
            Expression<Func<WorkflowItem, bool>> criteriaUsed = WorkflowItem => true;
            return FindWorkflows(creatorId, topCriteria, page, pageSize, null, criteriaUsed);
        }

        public ListWorkflowItem FindWorkflows(long creatorId, string topCriteria, int page, int pageSize, Expression<Func<WorkflowItem, string>> order)
        {
            Expression<Func<WorkflowItem, bool>> criteriaUsed = WorkflowItem => true;
            return FindWorkflows(creatorId, topCriteria, page, pageSize, order, criteriaUsed);
        }

        public ListWorkflowItem FindWorkflows(long creatorId, string topCriteria, int page, int pageSize, Expression<Func<WorkflowItem, string>> order, Expression<Func<WorkflowItem ,bool>> criteria)
        {
            int skip = pageSize * (page - 1);
            Expression<Func<WorkflowItem, string>> ordering = WorkflowItem => "IsTemplate desc, Name";

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
                     where workflow.CreatorId == creatorId && (topCriteria.Equals("") || tops.All(x => (workflow.Name + " " + workflow.Description).Contains(x)))
                     select new WorkflowItem
                     {
                         Id = workflow.Id,
                         Name = workflow.Name,
                         Description = workflow.Description,
                         IsActive = workflow.IsActive,
                         IsTemplate = workflow.IsTemplate,
                         Type = workflow.Type,
                         UserEmail = workflow.UserEmail,
                         DateCreated = workflow.DateCreated,
                         DateUpdated = workflow.DateUpdated,
                     }).Where(criteria).OrderBy(ordering).Skip(skip).Take(pageSize).ToList();
                ListWorkflowItem returnValue = new ListWorkflowItem();
                if (result != null)
                {
                    int CounterItem = 0;
                    MenuService menuService = new MenuService();
                    foreach (WorkflowItem workflow in result)
                    {
                        CounterItem+=1;
                        workflow.Key = menuService.EncryptData(workflow.Id);
                        returnValue.Items.Add(workflow);
                    }
                    returnValue.Count = CounterItem;
                }
                return returnValue;
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
                    product = new Workflow();

                product.Name = WorkflowItem.Name;
                product.Description = WorkflowItem.Description;
                product.IsActive = WorkflowItem.IsActive;
                product.IsTemplate = WorkflowItem.IsTemplate;
                product.Type = WorkflowItem.Type;
                product.CreatorId = WorkflowItem.CreatorId;
                product.UserEmail = WorkflowItem.UserEmail;
                if (WorkflowItem.Id == 0)
                {
                    product.DateCreated = DateTime.Now;
                    db.Workflows.Add(product);
                }
                else
                    product.DateUpdated = DateTime.Now;

                var result = db.SaveChanges();

                var isused = false;
                if (WorkflowItem.Id != 0 && product.TotalUsed > 0)
                    isused = true;

                if (!isused) // skip when workflow is used by transaction
                {
                    // delete existing node
                    if (WorkflowItem.Id != 0)
                    {
                        var oldNodeLinks = db.WorkflowNodeLinks.Where(workflow => workflow.WorkflowNodes.WorkflowId == WorkflowItem.Id || workflow.WorkflowNodeTos.WorkflowId == WorkflowItem.Id).ToList();
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
                            node.UserId = (jnode.userId == 0 ? null : jnode.userId);
                            node.SymbolCode = getSymbolsFromCsvByCode(jnode.symbolCode).Id;
                            //node.SymbolCode = getSymbolsFromCsvByCode(jnode.symbolCode).Id;

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
                    if (WorkflowItem.WorkflowNodeLinks != null)
                    {
                        foreach (WorkflowNodeLinkItem jnodelink in WorkflowItem.WorkflowNodeLinks)
                        {
                            var nodelink = new WorkflowNodeLink();
                            nodelink.WorkflowNodeId = WorkflowItem.WorkflowNodes.FirstOrDefault(workflow => workflow.element.Equals(jnodelink.elementFrom)).Id;
                            var wfnod = db.WorkflowNodes.FirstOrDefault(c => c.Id == nodelink.WorkflowNodeId);
                            nodelink.WorkflowNodes = wfnod;
                            nodelink.WorkflowNodeToId = WorkflowItem.WorkflowNodes.FirstOrDefault(workflow => workflow.element.Equals(jnodelink.elementTo)).Id;
                            var to = db.WorkflowNodes.FirstOrDefault(c => c.Id == nodelink.WorkflowNodeToId);
                            nodelink.WorkflowNodeTos = to;
                            nodelink.Caption = jnodelink.caption;
                            nodelink.SymbolCode = getSymbolsFromCsvByCode(jnodelink.symbolCode).Id;
                            /*db.Symbols.FirstOrDefault(workflow => workflow.Code.Equals(jnodelink.symbolCode)).Id;*/
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
        public static Symbol getSymbolsFromCsvByCode(string code)
        {
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
    }
}
