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
        public WorkflowData GetById(long id)
        {
            using (var db = new ServiceContext())
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
                            workflowNodeData.element = getSymbolsFromCsvById(workflowNode.SymbolCode).Name + "-" + dmid;
                            workflowNodeData.symbolCode = getSymbolsFromCsvById(workflowNode.SymbolCode).Code;
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
                            User user = db.Users.Where(dbuser => dbuser.Id == workflowNode.MemberId).FirstOrDefault();
                            if (user != null) {
                                workflowNodeData.user = new WorkflowNodeUser
                                {
                                    id = user.Id,
                                    name = user.Name,
                                    email = user.Email,
                                    imageProfile = user.ImageProfile,
                                };
                            }
                            else
                            {
                                workflowNodeData.user = null;
                            }
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
        public ListWorkflowData FindWorkflows(long creatorId, string topCriteria, int page, int pageSize)
        {
            Expression<Func<WorkflowData, bool>> criteriaUsed = WorkflowData => true;
            return FindWorkflows(creatorId, topCriteria, page, pageSize, null, criteriaUsed);
        }

        public ListWorkflowData FindWorkflows(long creatorId, string topCriteria, int page, int pageSize, Expression<Func<WorkflowData, string>> order)
        {
            Expression<Func<WorkflowData, bool>> criteriaUsed = WorkflowData => true;
            return FindWorkflows(creatorId, topCriteria, page, pageSize, order, criteriaUsed);
        }

        public ListWorkflowData FindWorkflows(long creatorId, string topCriteria, int page, int pageSize, Expression<Func<WorkflowData, string>> order, Expression<Func<WorkflowData ,bool>> criteria)
        {
            int skip = pageSize * (page - 1);
            Expression<Func<WorkflowData, string>> ordering = WorkflowData => "IsTemplate desc, Name";

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
                     }).Where(criteria).OrderBy(ordering).Skip(skip).Take(pageSize).ToList();
                ListWorkflowData returnValue = new ListWorkflowData();
                if (result != null)
                {
                    int CounterItem = 0;
                    MenuService menuService = new MenuService();
                    foreach (WorkflowData workflow in result)
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
        /// <param name="workflowData"></param>
        /// <returns></returns>
        /// 
        public int Save(WorkflowData workflowData)
        {
            Workflow product;
            using (var db = new ServiceContext())
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
                    if (workflowData.WorkflowNodeLinks != null)
                    {
                        foreach (WorkflowNodeLinkData jnodelink in workflowData.WorkflowNodeLinks)
                        {
                            var nodelink = new WorkflowNodeLink();
                            nodelink.WorkflowNodeId = workflowData.WorkflowNodes.FirstOrDefault(workflow => workflow.element.Equals(jnodelink.elementFrom)).Id;
                            nodelink.WorkflowNodeToId = workflowData.WorkflowNodes.FirstOrDefault(workflow => workflow.element.Equals(jnodelink.elementTo)).Id;
                            nodelink.Caption = jnodelink.caption;
                           // nodelink.SymbolCode = db.Symbols.FirstOrDefault(workflow => workflow.Code.Equals(jnodelink.symbolCode)).Id;
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
