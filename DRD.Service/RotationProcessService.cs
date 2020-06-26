﻿using DRD.Models;
using DRD.Models.API;
using DRD.Models.View;
using DRD.Service.Context;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DRD.Service
{
    public class RotationProcessService
    {
        private readonly string _connString;
        private string _appZoneAccess;
        private SubscriptionService subscriptionService = new SubscriptionService();
        private SymbolService symbolService = new SymbolService();

        public RotationProcessService(string appZoneAccess, string connString)
        {
            _appZoneAccess = appZoneAccess;
            _connString = connString;
        }

        public RotationProcessService(string appZoneAccess)
        {
            _appZoneAccess = appZoneAccess;
            _connString = Constant.CONSTRING;
        }

        public RotationProcessService()
        {
            _connString = Constant.CONSTRING;
        }

        public int ProcessActivity(ProcessActivity parameter, Constant.EnumActivityAction enumActivityAction)
        {
            var ret = ProcessActivity(parameter, enumActivityAction, new DocumentService());
            return ret.FirstOrDefault().ExitCode;
        }

        public List<ActivityItem> ProcessActivity(ProcessActivity param, Constant.EnumActivityAction bit, IDocumentService docSvr)
        {
            List<ActivityItem> retvalues = new List<ActivityItem>();

            using (var db = new ServiceContext())
            {
                var strbit = bit.ToString();

                //get current rotation node
                RotationNode rtnode = db.RotationNodes.FirstOrDefault(c => c.Id == param.RotationNodeId);

                //set last node to in progress
                rtnode.Status = (int)Constant.RotationStatus.In_Progress;
                rtnode.UpdatedAt = DateTime.Now;
                rtnode.Rotation.StartedAt = DateTime.Now;
                InsertDoc(param.RotationNodeDocs, db, ref rtnode, docSvr);

                if (strbit.Equals("REVISI"))
                {
                    rtnode.Status = (int)Constant.RotationStatus.Revision;

                    var workflowNodeLink = db.WorkflowNodeLinks.Where(c => c.SourceId == rtnode.WorkflowNodeId).FirstOrDefault();
                    RotationNode rtnode2 = new RotationNode();

                    rtnode2.RotationId = rtnode.RotationId;
                    rtnode2.WorkflowNodeId = workflowNodeLink.FirstNodeId;
                    rtnode2.FirstNodeId = rtnode.FirstNodeId;
                    rtnode2.SenderRotationNodeId = rtnode.Id;
                    rtnode2.UserId = (long)workflowNodeLink.FirstNode.RotationUsers.FirstOrDefault(c => c.WorkflowNodeId == workflowNodeLink.FirstNodeId && c.RotationId == rtnode.RotationId).User.Id;
                    rtnode2.PreviousWorkflowNodeId = workflowNodeLink.SourceId;// tested OK
                    rtnode2.Status = (int)Constant.RotationStatus.Open;
                    rtnode2.CreatedAt = DateTime.Now;
                    db.RotationNodes.Add(rtnode2);
                    db.SaveChanges();
                    //TODO change how to get last id inserted
                    long lastProductId = db.RotationNodes.Where(item => item.RotationId == rtnode2.RotationId).Max(item => item.Id);
                    retvalues.Add(createActivityResult(rtnode2.UserId, rtnode.UserId, (int)Constant.RotationStatus.Revision, rtnode2.Rotation.Name, rtnode2.RotationId, lastProductId, strbit));
                }
                else if (strbit.Equals("REJECT"))
                {
                    var workflowNodeLink = db.WorkflowNodeLinks.Where(c => c.SourceId == rtnode.WorkflowNodeId).FirstOrDefault();
                    rtnode.Status = (int)Constant.RotationStatus.Declined;
                    UpdateStatus(db, rtnode.Rotation.Id, (int)Constant.RotationStatus.Open, (int)Constant.RotationStatus.Declined);

                    retvalues.Add(createActivityResult(rtnode.UserId, rtnode.UserId, (int)Constant.RotationStatus.Declined, rtnode.Rotation.Name, rtnode.RotationId, rtnode.Id, strbit));
                }
                else if (strbit.Equals("SUBMIT"))
                {
                    Symbol symbol = symbolService.getSymbol(strbit);
                    int symbolCode = symbol == null ? 0 : symbol.Id;
                    var wfnodes = db.WorkflowNodeLinks.Where(c => c.SourceId == rtnode.WorkflowNodeId && c.SymbolCode == symbolCode).ToList();
                    List<RotationNode> rotnodes = new List<RotationNode>();

                    foreach (WorkflowNodeLink workflowNodeLink in wfnodes)
                    {
                        var nodeto = workflowNodeLink.Target;

                        if (nodeto.SymbolCode == symbolService.getSymbolId("ACTIVITY"))
                        {
                            /*                            RotationNode rtnode2 = new RotationNode();*/
                            RotationNode rtnode2 = db.RotationNodes.Create<RotationNode>();

                            rtnode2.RotationId = rtnode.RotationId;
                            rtnode2.WorkflowNodeId = workflowNodeLink.TargetId;
                            rtnode2.FirstNodeId = rtnode.FirstNodeId;
                            rtnode2.SenderRotationNodeId = rtnode.Id;
                            rtnode2.UserId = (long)workflowNodeLink.Target.RotationUsers.FirstOrDefault(c => c.WorkflowNodeId == workflowNodeLink.TargetId && c.RotationId == rtnode.RotationId).User.Id;
                            rtnode2.PreviousWorkflowNodeId = workflowNodeLink.SourceId;// tested OK
                            rtnode2.Status = (int)Constant.RotationStatus.Open;
                            rtnode2.CreatedAt = DateTime.Now;
                            db.RotationNodes.Add(rtnode2);

                            db.SaveChanges();
                            //TODO change how to get last id inserted
                            long lastProductId = db.RotationNodes.Where(item => item.RotationId == rtnode2.RotationId).Max(item => item.Id);
                            retvalues.Add(createActivityResult(rtnode2.UserId, rtnode.UserId, (int)Constant.RotationStatus.In_Progress, rtnode2.Rotation.Name, rtnode2.RotationId, lastProductId, strbit));
                        }
                        else if (nodeto.SymbolCode == symbolService.getSymbolId("END"))
                        {
                            if (rtnode.Status.Equals((int)Constant.RotationStatus.Declined))
                                UpdateStatus(db, rtnode.Rotation.Id, (int)Constant.RotationStatus.Open, (int)Constant.RotationStatus.Declined);
                            else
                                UpdateStatus(db, rtnode.Rotation.Id, (int)Constant.RotationStatus.Open, (int)Constant.RotationStatus.Declined);
                            retvalues.Add(createActivityResult(rtnode.UserId, rtnode.UserId, 1, rtnode.Rotation.Name, rtnode.RotationId, rtnode.Id, "END"));
                        }
                    }
                }
                var result = db.SaveChanges();

                InboxService inboxService = new InboxService();
                foreach (ActivityItem act in retvalues)
                {
                    inboxService.GenerateNewInbox(act);
                }
                return retvalues;
            }
        }

        public ResponseStatus Start(long userId, long rotationId, long subscriptionId)
        {
            var returnItem = StartProcess(userId, rotationId, subscriptionId);
            InboxService inboxService = new InboxService();
            List<int> returnValue = new List<int>();

            if (returnItem != null)
            {
                foreach (ActivityItem act in returnItem)
                {
                    returnValue.Add(inboxService.CreateInbox(act));
                }

                return new ResponseStatus() {code = returnItem[0].ExitCode , status = returnItem[0].ExitStatus };
            }

            return new ResponseStatus() { code = -6 , status = "" };
        }

        // subscription Id is either userId or companyId
        public List<ActivityItem> StartProcess(long userId, long rotationId, long usageId)
        {
            List<ActivityItem> retvalues = new List<ActivityItem>();
            Constant.BusinessUsageStatus subscriptionStatus = subscriptionService.IsSubscriptionValid(userId, usageId);
            System.Diagnostics.Debug.WriteLine("LALALA: statbro " + subscriptionStatus);
            if (!subscriptionStatus.Equals(Constant.BusinessUsageStatus.OK))
            {
                retvalues.Add(createActivityResult((int)subscriptionStatus, subscriptionStatus.ToString()));
                return retvalues;
            }

            using (var db = new ServiceContext())
            {
                var rt = db.Rotations.FirstOrDefault(c => c.Id == rotationId);
                if (!rt.Status.Equals((int)Constant.RotationStatus.Open))
                {
                    retvalues.Add(createActivityResult((int)Constant.RotationStatus.ERROR_ROTATION_ALREADY_STARTED, Constant.RotationStatus.ERROR_ROTATION_ALREADY_STARTED.ToString()));
                    return retvalues; //Invalid rotation
                }

                //update rotation
                rt.Status = (int)Constant.RotationStatus.In_Progress;
                var companyUsage = db.BusinessUsages.FirstOrDefault(c => c.Id == usageId && c.IsActive);
                rt.CompanyId = companyUsage.CompanyId;
                rt.SubscriptionType = companyUsage.CompanyId != 0 ? (byte)Constant.SubscriptionType.BUSINESS : (byte)Constant.SubscriptionType.PERSONAL;
                rt.UpdatedAt = DateTime.Now;
                rt.StartedAt = DateTime.Now;

                // first node, node after start symbol
                var workflowNodeLinks = db.WorkflowNodeLinks.Where(c => c.Source.WorkflowId == rt.WorkflowId && c.Source.SymbolCode == 0).ToList();
                if (workflowNodeLinks == null)
                {
                    retvalues.Add(createActivityResult((int)Constant.RotationStatus.ERROR_WORKFLOW_START_NODE_NOT_FOUND, Constant.RotationStatus.ERROR_WORKFLOW_START_NODE_NOT_FOUND.ToString()));
                    return retvalues; //Invalid rotation
                }

                //check rotation started limit or add when limit passed
                var rotationStartedLimitStatus = subscriptionService.CheckOrAddSpecificUsage(Constant.BusinessPackageItem.Rotation_Started, rt.CompanyId.Value, additional:1, addAfterSubscriptionValid: true);
           

                if (!rotationStartedLimitStatus.Equals(Constant.BusinessUsageStatus.OK) )
                {
                    retvalues.Add(createActivityResult((int)rotationStartedLimitStatus, rotationStartedLimitStatus.ToString()));
                    return retvalues;
                }
            

                
                long rotId = rt.Id;

                // send to all activity under start node +
                foreach (WorkflowNodeLink workflowNodeLink in workflowNodeLinks)
                {
            
                    RotationNode rtnode = new RotationNode();
                    //rtnode.Rotation = rt;
                    rtnode.RotationId = rt.Id;
                    //rtnode.RotationId = rotId;
                    rtnode.WorkflowNodeId = workflowNodeLink.TargetId;
                    rtnode.WorkflowNode = workflowNodeLink.Target;
                    rtnode.FirstNodeId = workflowNodeLink.FirstNodeId;
                    //long user = db.RotationUsers.FirstOrDefault(c => c.WorkflowNodeId == workflowNodeLink.WorkflowNodeToId && c.RotationId == rt.Id).UserId.Value;
                    long userNodeId = GetUserId(workflowNodeLink.TargetId, rt.Id);
                    //rtnode.User = user;
                    rtnode.UserId = userNodeId;
                    rtnode.Status = (int)Constant.RotationStatus.Open;
                    rtnode.CreatedAt = DateTime.Now;
                    db.RotationNodes.Add(rtnode);
                    db.SaveChanges();
                    retvalues.Add(CreateActivityResult(rtnode.UserId, userId, 1, rt.Name, rtnode.Id, rotationId, exitStatus:Constant.RotationStatus.OK.ToString()));   
                }

                db.SaveChanges();
                return retvalues;
            }
        }

        private ActivityItem createActivityResult(long userId, long previousUserId, int exitCode, string rotationName, long rotationId, long rotationNodeId, string lastActivityStatus)
        {
            using (var db = new ServiceContext())
            {
                ActivityItem ret = CreateActivityResult(userId, previousUserId, exitCode, rotationName, rotationNodeId, rotationId);
                ret.LastActivityStatus = lastActivityStatus;
                return ret;
            }
        }
        private ActivityItem CreateActivityResult(long userId, long previousUserId, int exitCode, string rotationName, long rotationNodeId, long rotationId, string exitStatus = null)
        {
            using (var db = new ServiceContext())
            {
                System.Diagnostics.Debug.WriteLine("LALALA: "+rotationName+exitStatus);
               ActivityItem ret = new ActivityItem();
                ret.RotationId = rotationId;
                ret.ExitCode = exitCode;
                ret.ExitStatus = ret.ExitStatus == null ? Constant.RotationStatus.OK.ToString() : ret.ExitStatus;
                var mem = db.Users.FirstOrDefault(c => c.Id == userId);
                ret.UserId = userId;
                ret.UserName = mem.Name;
                ret.Email = mem.Email;

                var prev = db.Users.FirstOrDefault(c => c.Id == previousUserId);
                ret.PreviousUserId = previousUserId;
                ret.PreviousUserName = prev.Name;
                ret.PreviousEmail = prev.Email;

                ret.RotationName = rotationName;
                ret.RotationNodeId = rotationNodeId;
                return ret;
            }
        }
        private ActivityItem createActivityResult(int exitCode, string exitStatus = "")
        {
            ActivityItem ret = new ActivityItem();
            ret.ExitCode = exitCode;
            ret.ExitStatus = exitStatus;
            return ret;
        }

        private long GetUserId(long WorkflowNodeToId, long RotationId)
        {
            using (var db = new ServiceContext())
            {
                return db.RotationUsers.FirstOrDefault(c => c.WorkflowNodeId == WorkflowNodeToId && c.RotationId == RotationId).UserId.Value;
            }
        }

        private void InsertDoc(IEnumerable<RotationNodeDoc> docs, ServiceContext db, ref RotationNode rotationNode, IDocumentService docSvr)
        {
            if (docs != null )
            {
                foreach (RotationNodeDoc rnc in docs)
                {
                    RotationNodeDoc dx = new RotationNodeDoc();
                    dx.DocumentId = rnc.Document.Id;
                    dx.ActionStatus = rnc.ActionStatus;
                    dx.RotationNodeId = rotationNode.Id;
                    dx.RotationId = rotationNode.RotationId;
                    rotationNode.RotationNodeDocs.Add(dx);

                    // update flag action di master rotationNodeDoc member
                    var userId = rotationNode.UserId;
                    var rotationid = rotationNode.RotationId;
                    var docm = db.DocumentUsers.FirstOrDefault(c => c.DocumentId == rnc.Document.Id && c.UserId == userId);
                    var rotationUser = db.RotationUsers.FirstOrDefault(rtUsr => rtUsr.RotationId == rotationid && rtUsr.UserId == userId);
                    if (docm != null)
                    {
                        if (((rnc.ActionStatus & (int)Constant.EnumDocumentAction.REMOVE) == (int)Constant.EnumDocumentAction.REMOVE) ||
                            ((rnc.ActionStatus & (int)Constant.EnumDocumentAction.REVISI) == (int)Constant.EnumDocumentAction.REVISI))
                            docSvr.DocumentRemovedorRevisedFromRotation(rnc.DocumentId);
                        else if (docm.ActionStatus != rnc.ActionStatus) docSvr.DocumentUpdatedByRotation(rnc.DocumentId);
                        docm.ActionStatus |= rnc.ActionStatus;
                        // Also Document permission updating related to Rotation User that have permission
                        docm.ActionPermission |= rotationUser.ActionPermission;
                    }
                    else
                    {
                        DocumentUser docmem = new DocumentUser();
                        docmem.DocumentId = rnc.Document.Id;
                        docmem.UserId = userId;
                        if (((rnc.ActionStatus & (int)Constant.EnumDocumentAction.REMOVE) == (int)Constant.EnumDocumentAction.REMOVE) ||
                            ((rnc.ActionStatus & (int)Constant.EnumDocumentAction.REVISI) == (int)Constant.EnumDocumentAction.REVISI))
                            docSvr.DocumentRemovedorRevisedFromRotation(rnc.DocumentId);
                        else if (rnc.ActionStatus != 0) docSvr.DocumentUpdatedByRotation(rnc.DocumentId); docmem.ActionStatus |= rnc.ActionStatus; 
                        docmem.ActionPermission = 6; // default view, add annotate
                        // Also Document permission updating related to Rotation User that have permission
                        docmem.ActionPermission |= rotationUser.ActionPermission;
                        db.DocumentUsers.Add(docmem);
                    }

                    if (rnc.Document != null)
                    { // save annos first before set sign/initial/stamp
                        ICollection<DocumentElementInboxData> docElement = new List<DocumentElementInboxData>();
                        foreach (DocumentAnnotation x in rnc.Document.DocumentElements)
                        {
                            docElement.Add(new DocumentElementInboxData(x));
                        }
                        docElement = docSvr.SaveAnnos(rnc.Document.Id, userId, "", docElement);
                    }
                    if ((rnc.ActionStatus & (int)Constant.EnumDocumentAction.SIGN) == (int)Constant.EnumDocumentAction.SIGN)
                        docSvr.Signature((long)rnc.Document.Id, userId, rotationNode.Rotation.Id);
                    if ((rnc.ActionStatus & (int)Constant.EnumDocumentAction.PRIVATESTAMP) == (int)Constant.EnumDocumentAction.PRIVATESTAMP)
                        docSvr.Stamp((long)rnc.Document.Id, userId, rotationNode.Rotation.Id);
                }
                db.SaveChanges();
            }
        }

        private bool IsExistNode(RotationNode node)
        {
            using (var db = new ServiceContext())
            {
                int[] statuses = { (int)Constant.RotationStatus.Open,
                    (int)Constant.RotationStatus.In_Progress,
                    (int)Constant.RotationStatus.Pending,
                    (int)Constant.RotationStatus.Completed };

                var ndx = db.RotationNodes.FirstOrDefault(c =>
                                    c.Rotation.Id == node.Rotation.Id &&
                                    c.WorkflowNode.Id == node.WorkflowNode.Id &&
                                    c.PreviousWorkflowNodeId == node.PreviousWorkflowNodeId &&
                                    c.UserId == node.UserId &&
                                    (statuses).Contains(c.Status));

                return (ndx != null);
            }
        }

        private void UpdateStatus(ServiceContext db, long rotationId, int previousStatus, int status)
        {
            var rot = db.Rotations.FirstOrDefault(c => c.Id == rotationId);
            rot.Status = status;
            rot.UpdatedAt = DateTime.Now;
            foreach (RotationNode rotationNode in rot.RotationNodes)
            {
                if (rotationNode.Status.Equals(previousStatus))
                    rotationNode.Status = status;
            }
        }
    }
}