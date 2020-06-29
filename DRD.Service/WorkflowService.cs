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
        //helper
        private bool CheckIdExist(long id)
        {
            using var db = new ServiceContext();
            return db.Workflows.Any(i => i.Id == id);
        }

        //helper
        private static Symbol GetSymbolsFromCsvByCode(string code)
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


        //helper
        private static Symbol GetSymbolsFromCsvById(int id)
        {
            var root = System.Web.HttpContext.Current.Server.MapPath("~");
            var path = Path.Combine(root, @"Symbols.csv");
            Symbol values = File.ReadAllLines(path)
                                           .Select(v => Symbol.FromCsv(v))
                                           .Where(c => c.Id == id).FirstOrDefault();

            return values;
        }

        /// <summary>
        /// used for getting workflow from index
        /// </summary>
        /// <param name="id"></param
        /// <returns></returns>
        public WorkflowItem GetById(long id, long userId)
        {
            using var db = new ServiceContext();
            var result =
                (from workflow in db.Workflows
                where workflow.Id == id && workflow.CreatorId == userId && workflow.IsActive
                select new WorkflowItem
                {
                    Id = workflow.Id,
                    Name = workflow.Name,
                    Description = workflow.Description,
                    IsActive = workflow.IsActive,
                    IsTemplate = workflow.IsTemplate,
                    IsUsed = (workflow.TotalUsed > 0),
                }).FirstOrDefault();

            //check if null
            if (result == null) return null;
            
            var links = db.WorkflowNodeLinks.Where(wfNdLink => wfNdLink.Source.WorkflowId == result.Id).ToList();
            var nodes = db.WorkflowNodes.Where(wfnode => wfnode.WorkflowId == result.Id).ToList();
            
            //check if nodes or links not available 
            if (nodes.Count() == 0 || links.Count() == 0) return result;
            
            int dmid = 2;
            result.WorkflowNodes = new List<WorkflowNodeItem>();
            result.WorkflowNodeLinks = new List<WorkflowNodeLinkItem>();
            
            //node process
            foreach (WorkflowNode workflowNode in nodes)
            {
                WorkflowNodeItem WorkflowNodeItem = new WorkflowNodeItem
                {
                    Id = workflowNode.Id
                };
                var elementname = GetSymbolsFromCsvById(workflowNode.SymbolCode).ElementName;
                if (elementname == "start")
                    WorkflowNodeItem.element = elementname + "-" + 0;
                else if (elementname == "end")
                    WorkflowNodeItem.element = elementname + "-" + 1;
                else
                {
                    WorkflowNodeItem.element = GetSymbolsFromCsvById(workflowNode.SymbolCode).ElementName + "-" + dmid;
                    dmid++;
                }
                WorkflowNodeItem.symbolCode = GetSymbolsFromCsvById(workflowNode.SymbolCode).Code;
                WorkflowNodeItem.caption = workflowNode.Caption;
                WorkflowNodeItem.textColor = workflowNode.TextColor;
                WorkflowNodeItem.backColor = workflowNode.BackColor;
                WorkflowNodeItem.posLeft = workflowNode.PosLeft;
                WorkflowNodeItem.posTop = workflowNode.PosTop;
                WorkflowNodeItem.width = workflowNode.Width;
                WorkflowNodeItem.height = workflowNode.Height;
                result.WorkflowNodes.Add(WorkflowNodeItem);
            }
            
            //link process
            foreach (WorkflowNodeLink wfnl in links)
            {
                WorkflowNodeLinkItem jwfnl = new WorkflowNodeLinkItem
                {
                    NodeId = wfnl.SourceId,
                    NodeToId = wfnl.TargetId,
                    firstNodeId = wfnl.FirstNodeId,
                    elementFrom = result.WorkflowNodes.FirstOrDefault(wfNode => wfNode.Id == wfnl.SourceId).element,
                    elementTo = result.WorkflowNodes.FirstOrDefault(wfNode => wfNode.Id == wfnl.TargetId).element,
                    firstNode = result.WorkflowNodes.FirstOrDefault(wfNode => wfNode.Id == wfnl.FirstNodeId).element,
                    symbolCode = GetSymbolsFromCsvById(wfnl.SymbolCode).Code
                };
                result.WorkflowNodeLinks.Add(jwfnl);
            }
            
            
            return result;
        }

        /// <summary>
        /// GET All Workflow match with criteria
        /// </summary>
        /// <param name="creatorId"></param>
        /// <param name="criteria"></param>
        /// <param name="page"></param>
        /// <param name="totalItemPerPage"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public ICollection<WorkflowListItem> GetWorkflows(long creatorId, string criteria, int page, int totalItemPerPage, Expression<Func<WorkflowItem, string>> order = null, bool isActive=true)
        {
            int skip = totalItemPerPage * (page - 1);
            Expression<Func<WorkflowItem, string>> ordering = WorkflowItem => "CreatedAt desc, IsTemplate desc, Name";

            if (order != null) ordering = order;

            // several criterias
            string[] criterias = new string[] { };
            if (!string.IsNullOrEmpty(criteria))
                criterias = criteria.Split(' ');
            else
                criteria = "";

            //call db objects
            using var db = new ServiceContext();
            var result =
                (from workflow in db.Workflows
                where workflow.CreatorId == creatorId && workflow.IsActive == isActive && (criteria.Equals("") || criterias.All(x => (workflow.Name + " " + workflow.Description).ToLower().Contains(x.ToLower())))
                orderby workflow.CreatedAt descending, workflow.IsTemplate descending, workflow.Name
                select new WorkflowListItem
                {
                    Id = workflow.Id,
                    Name = workflow.Name
                }).Skip(skip).Take(totalItemPerPage).ToList();

            return result;
        }

        /// <summary>
        /// Count workflow that match params
        /// </summary>
        /// <param name="creatorId"></param>
        /// <param name="criteria"></param>
        /// <param name="isActive"></param>
        /// <returns></returns>
        public int CountWorkflows(long creatorId, string criteria, bool isActive = true)
        {
            // top criteria
            string[] criterias = new string[] { };
            if (!string.IsNullOrEmpty(criteria))
                criterias = criteria.Split(' ');
            else
                criteria = "";

            using var db = new ServiceContext();
            var result = db.Workflows.Count(workflow =>
                workflow.CreatorId == creatorId &&
                workflow.IsActive == isActive &&
                (criteria.Equals("") ||
                criterias.All(x =>
                (workflow.Name + " " + workflow.Description).ToLower().Contains(x.ToLower()))
                )
                );

            return result;
        }

        /// <summary>
        /// Save updated and new workflow
        /// </summary>
        /// <param name="WorkflowItem"></param>
        /// <returns></returns>
        public int Save(WorkflowItem WorkflowItem)
        {
            Workflow product;
            using var db = new ServiceContext();

            // validity subscription type
            var creator = db.Users.FirstOrDefault(user => user.Id == WorkflowItem.CreatorId);

            if (WorkflowItem.Id != 0)
                product = db.Workflows.FirstOrDefault(workflow => workflow.Id == WorkflowItem.Id);
            else
            {
                product = new Workflow();
                while (CheckIdExist(product.Id))
                {
                    product.Id = Utilities.RandomLongGenerator(minimumValue: Constant.MINIMUM_VALUE_ID, maximumValue: Constant.MAXIMUM_VALUE_ID);
                }
            }

            product.Name = WorkflowItem.Name;
            product.Description = WorkflowItem.Description;
            product.IsActive = WorkflowItem.IsActive;
            product.IsTemplate = WorkflowItem.IsTemplate;
            product.CreatorId = WorkflowItem.CreatorId;
            if (WorkflowItem.Id == 0)db.Workflows.Add(product);
            

            var result = db.SaveChanges();

            var isused = false;
            if (WorkflowItem.Id != 0 && product.TotalUsed > 0)
                isused = true;

            //changes in canvas can't be updated
            if (isused) return result;
            
            // delete existing node
            if (WorkflowItem.Id == 0)
            {
                var oldNodeLinks = db.WorkflowNodeLinks.Where(workflow => workflow.Source.WorkflowId == WorkflowItem.Id || workflow.Target.WorkflowId == WorkflowItem.Id).ToList();
                db.WorkflowNodeLinks.RemoveRange(oldNodeLinks);
                db.SaveChanges();

                var oldNodes = db.WorkflowNodes.Where(workflow => workflow.WorkflowId == WorkflowItem.Id).ToList();
                db.WorkflowNodes.RemoveRange(oldNodes);
                db.SaveChanges();
            }

            // check if workflow node or link null => automatically invalid
            if (WorkflowItem.WorkflowNodes == null || WorkflowItem.WorkflowNodeLinks == null) return result;
            
            // save node
            foreach (WorkflowNodeItem newNode in WorkflowItem.WorkflowNodes)
            {
                var node = new WorkflowNode
                {
                    WorkflowId = product.Id,
                    SymbolCode = GetSymbolsFromCsvByCode(newNode.symbolCode).Id,

                    Caption = newNode.caption,
                    PosLeft = newNode.posLeft,
                    PosTop = newNode.posTop,
                    Width = newNode.width,
                    Height = newNode.height,
                    TextColor = newNode.textColor,
                    BackColor = newNode.backColor
                };
                db.WorkflowNodes.Add(node);
                db.SaveChanges();
                newNode.Id = node.Id;
            }
            
            // save node link
            foreach (WorkflowNodeLinkItem newNodeLink in WorkflowItem.WorkflowNodeLinks)
            {
                var nodelink = new WorkflowNodeLink
                {
                    SourceId = WorkflowItem.WorkflowNodes.FirstOrDefault(workflow => workflow.element.Equals(newNodeLink.elementFrom)).Id,
                    TargetId = WorkflowItem.WorkflowNodes.FirstOrDefault(workflow => workflow.element.Equals(newNodeLink.elementTo)).Id,
                    SymbolCode = GetSymbolsFromCsvByCode(newNodeLink.symbolCode).Id,
                    FirstNodeId = WorkflowItem.WorkflowNodes.FirstOrDefault(workflow => workflow.element.Equals(newNodeLink.firstNode)).Id
                };

                var wfnod = db.WorkflowNodes.FirstOrDefault(c => c.Id == nodelink.SourceId);
                var to = db.WorkflowNodes.FirstOrDefault(c => c.Id == nodelink.TargetId);
                var first = db.WorkflowNodes.FirstOrDefault(c => c.Id == nodelink.FirstNodeId);

                nodelink.Source = wfnod;
                nodelink.Target = to;
                nodelink.FirstNode = first;
                        
                db.WorkflowNodeLinks.Add(nodelink);
                db.SaveChanges();
            }

            return result;
        }
        
        /// <summary>
        /// delete workflow based on workflow id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string Delete(long id)
        {
            using var db = new ServiceContext();
            var workflow = db.Workflows.Where(i => i.Id == id).FirstOrDefault();

            //check if workflow exist
            if (workflow == null) return Constant.WorkflowStatus.NOT_FOUND.ToString();

            //check if workflow is already being used
            if (workflow.TotalUsed > 0) return Constant.WorkflowStatus.USED_IN_ROTATION.ToString();

            workflow.IsActive = false;
            workflow.UpdatedAt = DateTime.Now;
            db.SaveChanges();
            return Constant.WorkflowStatus.OK.ToString();
        }
    }
}
