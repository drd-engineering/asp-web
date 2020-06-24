using DRD.Models;
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
                rtnode.Value = param.Value;
                rtnode.UpdatedAt = DateTime.Now;
                rtnode.Rotation.DateStarted = DateTime.Now;
                InsertDoc(param.RotationNodeDocs, db, ref rtnode, docSvr);
                InsertUpDoc(param.RotationNodeUpDocs, ref rtnode);

                // insert remark to table
                if (!string.IsNullOrEmpty(param.Remark))
                {
                    RotationNodeRemark rtnoderemark = new RotationNodeRemark();
                    rtnoderemark.Remark = param.Remark;
                    rtnoderemark.RotationNodeId = param.RotationNodeId;
                    db.RotationNodeRemarks.Add(rtnoderemark);
                }
                if (strbit.Equals("REVISI"))
                {
                    rtnode.Status = (int)Constant.RotationStatus.Revision;

                    var workflowNodeLink = db.WorkflowNodeLinks.Where(c => c.WorkflowNodeId == rtnode.WorkflowNodeId).FirstOrDefault();
                    RotationNode rtnode2 = new RotationNode();

                    rtnode2.RotationId = rtnode.RotationId;
                    rtnode2.WorkflowNodeId = workflowNodeLink.FirstNodeId;
                    rtnode2.FirstNodeId = rtnode.FirstNodeId;
                    rtnode2.SenderRotationNodeId = rtnode.Id;
                    rtnode2.UserId = (long)workflowNodeLink.FirstNode.RotationUsers.FirstOrDefault(c => c.WorkflowNodeId == workflowNodeLink.FirstNodeId && c.RotationId == rtnode.RotationId).User.Id;
                    rtnode2.PrevWorkflowNodeId = workflowNodeLink.WorkflowNodeId;// tested OK
                    rtnode2.Status = (int)Constant.RotationStatus.Open;
                    rtnode2.Value = rtnode.Value;
                    rtnode2.CreatedAt = DateTime.Now;
                    db.RotationNodes.Add(rtnode2);
                    db.SaveChanges();
                    //TODO change how to get last id inserted
                    long lastProductId = db.RotationNodes.Where(item => item.RotationId == rtnode2.RotationId).Max(item => item.Id);
                    retvalues.Add(createActivityResult(rtnode2.UserId, rtnode.UserId, (int)Constant.RotationStatus.Revision, rtnode2.Rotation.Subject, rtnode2.RotationId, lastProductId, strbit));
                }
                else if (strbit.Equals("REJECT"))
                {
                    var workflowNodeLink = db.WorkflowNodeLinks.Where(c => c.WorkflowNodeId == rtnode.WorkflowNodeId).FirstOrDefault();
                    rtnode.Status = (int)Constant.RotationStatus.Declined;
                    UpdateStatus(db, rtnode.Rotation.Id, (int)Constant.RotationStatus.Open, (int)Constant.RotationStatus.Declined);

                    retvalues.Add(createActivityResult(rtnode.UserId, rtnode.UserId, (int)Constant.RotationStatus.Declined, rtnode.Rotation.Subject, rtnode.RotationId, rtnode.Id, strbit));
                }
                else if (strbit.Equals("SUBMIT"))
                {
                    Symbol symbol = symbolService.getSymbol(strbit);
                    int symbolCode = symbol == null ? 0 : symbol.Id;
                    var wfnodes = db.WorkflowNodeLinks.Where(c => c.WorkflowNodeId == rtnode.WorkflowNodeId && c.SymbolCode == symbolCode).ToList();
                    List<RotationNode> rotnodes = new List<RotationNode>();

                    foreach (WorkflowNodeLink workflowNodeLink in wfnodes)
                    {
                        var nodeto = workflowNodeLink.WorkflowNodeTo;

                        if (nodeto.SymbolCode == symbolService.getSymbolId("ACTIVITY"))
                        {
                            /*                            RotationNode rtnode2 = new RotationNode();*/
                            RotationNode rtnode2 = db.RotationNodes.Create<RotationNode>();

                            rtnode2.RotationId = rtnode.RotationId;
                            rtnode2.WorkflowNodeId = workflowNodeLink.WorkflowNodeToId;
                            rtnode2.FirstNodeId = rtnode.FirstNodeId;
                            rtnode2.SenderRotationNodeId = rtnode.Id;
                            rtnode2.UserId = (long)workflowNodeLink.WorkflowNodeTo.RotationUsers.FirstOrDefault(c => c.WorkflowNodeId == workflowNodeLink.WorkflowNodeToId && c.RotationId == rtnode.RotationId).User.Id;
                            rtnode2.PrevWorkflowNodeId = workflowNodeLink.WorkflowNodeId;// tested OK
                            rtnode2.Status = (int)Constant.RotationStatus.Open;
                            rtnode2.Value = rtnode.Value;
                            rtnode2.CreatedAt = DateTime.Now;
                            db.RotationNodes.Add(rtnode2);

                            db.SaveChanges();
                            //TODO change how to get last id inserted
                            long lastProductId = db.RotationNodes.Where(item => item.RotationId == rtnode2.RotationId).Max(item => item.Id);
                            retvalues.Add(createActivityResult(rtnode2.UserId, rtnode.UserId, (int)Constant.RotationStatus.In_Progress, rtnode2.Rotation.Subject, rtnode2.RotationId, lastProductId, strbit));
                        }
                        else if (nodeto.SymbolCode == symbolService.getSymbolId("PARALLEL"))
                        {
                            bool ispending = false;
                            foreach (WorkflowNodeLink workflowNodeLinkx in workflowNodeLink.WorkflowNodeTo.WorkflowNodeLinkTos)
                            {
                                var rotnode = db.RotationNodes.FirstOrDefault(c => c.WorkflowNode.Id == workflowNodeLinkx.WorkflowNodeId);
                                if (rotnode.WorkflowNode.Id != rtnode.WorkflowNode.Id)
                                {
                                    //var isready02 = false;
                                    //RotationNode retnode02 = null;
                                    if (rotnode.Status.Equals(Constant.RotationStatus.Revision))
                                    {
                                        // checking apakah 05 sudah menjadi 02 di id lain
                                        var retnode02 = db.RotationNodes.FirstOrDefault(c =>
                                                         c.Rotation.Id == rotnode.Rotation.Id &&
                                                         c.UserId == rotnode.UserId &&
                                                         c.PrevWorkflowNodeId == rotnode.PrevWorkflowNodeId &&
                                                         c.WorkflowNode.Id == workflowNodeLinkx.WorkflowNodeId &&
                                                         c.Status.Equals(Constant.RotationStatus.Pending));
                                        //isready02 = (retnode02 != null);
                                        if (retnode02 != null) rotnode = retnode02;
                                    }
                                    int[] statuses = { (int)Constant.RotationStatus.Open, (int)Constant.RotationStatus.Revision };
                                    if ((statuses).Contains(rotnode.Status))
                                    {
                                        ispending = true;
                                        break;
                                    }
                                    else if (rotnode.Status.Equals((int)Constant.RotationStatus.Pending))
                                    {
                                        rotnodes.Add(rotnode);
                                    }
                                }
                            }
                            if (ispending)
                            {
                                rtnode.Status = (int)Constant.RotationStatus.Pending;
                                continue;
                            }
                            else
                            {
                                // ubah semua status jadi 01, yang pending (02) menjadi in progress
                                foreach (RotationNode rotn in rotnodes)
                                {
                                    rotn.Status = (int)Constant.RotationStatus.In_Progress;
                                }

                                foreach (WorkflowNodeLink lnk in nodeto.WorkflowNodeLinkTos)
                                {
                                    RotationNode rtnode2 = new RotationNode();

                                    rtnode2.Rotation.Id = rtnode.Rotation.Id;
                                    rtnode2.WorkflowNode.Id = lnk.WorkflowNodeToId;
                                    rtnode2.FirstNodeId = rtnode.FirstNodeId;
                                    rtnode2.SenderRotationNodeId = rtnode.Id;
                                    //rtnode2.UserId = (long)lnk.WorkflowNodeTo.UserId;
                                    rtnode2.UserId = (long)lnk.WorkflowNodeTo.RotationUsers.FirstOrDefault(c => c.WorkflowNodeId == lnk.WorkflowNodeToId && c.Rotation.Id == rtnode.Rotation.Id).User.Id;
                                    rtnode2.PrevWorkflowNodeId = workflowNodeLink.WorkflowNodeToId; // tested OK
                                    rtnode2.Status = (int)Constant.RotationStatus.Open;
                                    rtnode2.Value = rtnode.Value;
                                    rtnode2.CreatedAt = DateTime.Now;
                                    db.RotationNodes.Add(rtnode2);

                                    db.SaveChanges();
                                    long lastProductId = db.RotationNodes.Where(item => item.RotationId == rtnode2.RotationId).Max(item => item.Id);
                                    retvalues.Add(createActivityResult(rtnode2.UserId, rtnode.UserId, (int)Constant.RotationStatus.In_Progress, rtnode2.Rotation.Subject, rtnode2.RotationId, lastProductId, strbit));
                                }
                            }
                        }
                        else if (nodeto.SymbolCode == symbolService.getSymbolId("DECISION"))
                        {
                            WorkflowNodeLink nodeToNext;
                            //if (decissionValue(rtnode.Value, nodeto.Value, nodeto.Operator))
                            //    nodeToNext = nodeto.WorkflowNodeLinks.FirstOrDefault(c => c.SymbolCode.Equals("YES"));
                            //else
                            nodeToNext = nodeto.WorkflowNodeLinks.FirstOrDefault(c => c.SymbolCode.Equals("NO"));

                            RotationNode rtnode2 = new RotationNode();

                            rtnode2.Rotation.Id = rtnode.Rotation.Id;
                            rtnode2.WorkflowNode.Id = nodeToNext.WorkflowNodeToId;
                            rtnode2.FirstNodeId = rtnode.FirstNodeId;
                            rtnode2.SenderRotationNodeId = rtnode.Id;
                            //rtnode2.UserId = (long)nodeToNext.WorkflowNodeTo.UserId;
                            rtnode2.UserId = (long)nodeToNext.WorkflowNodeTo.RotationUsers.FirstOrDefault(c => c.WorkflowNodeId == nodeToNext.WorkflowNodeToId && c.Rotation.Id == rtnode.Rotation.Id).User.Id;
                            rtnode2.PrevWorkflowNodeId = workflowNodeLink.WorkflowNodeId;//tested OK
                            rtnode2.Status = (int)Constant.RotationStatus.Open;
                            rtnode2.Value = rtnode.Value;
                            rtnode2.CreatedAt = DateTime.Now;
                            db.RotationNodes.Add(rtnode2);
                            db.SaveChanges();

                            long lastProductId = db.RotationNodes.Where(item => item.RotationId == rtnode2.RotationId).Max(item => item.Id);
                            retvalues.Add(createActivityResult(rtnode2.UserId, rtnode.UserId, 1, rtnode2.Rotation.Subject, rtnode2.RotationId, lastProductId, strbit));
                        }
                        else if (nodeto.SymbolCode == symbolService.getSymbolId("TRANSFER"))
                        {
                            WorkflowNodeLink nodeToNext;
                            nodeToNext = nodeto.WorkflowNodeLinks.FirstOrDefault();

                            RotationNode rtnode2 = new RotationNode();

                            rtnode2.Rotation.Id = rtnode.Rotation.Id;
                            rtnode2.WorkflowNode.Id = nodeToNext.WorkflowNodeToId;
                            rtnode2.FirstNodeId = rtnode.FirstNodeId;
                            rtnode2.SenderRotationNodeId = rtnode.Id;
                            //rtnode2.UserId = (long)nodeToNext.WorkflowNodeTo.UserId;
                            rtnode2.UserId = (long)nodeToNext.WorkflowNodeTo.RotationUsers.FirstOrDefault(c => c.WorkflowNodeId == nodeToNext.WorkflowNodeToId && c.Rotation.Id == rtnode.Rotation.Id).User.Id;
                            rtnode2.PrevWorkflowNodeId = workflowNodeLink.WorkflowNodeToId;// tested OK
                            rtnode2.Status = (int)Constant.RotationStatus.Open;
                            rtnode2.Value = rtnode.Value;// "TRF:" + nodeto.Value + ",ID:" + rtnode.Id + ",MEMBER:" + rtnode.UserId;
                            rtnode2.CreatedAt = DateTime.Now;

                            // check for double rotation node
                            if (!IsExistNode(rtnode2))
                            {
                                db.RotationNodes.Add(rtnode2);
                                long lastProductId = db.RotationNodes.Where(item => item.RotationId == rtnode2.RotationId).Max(item => item.Id);

                                retvalues.Add(createActivityResult(rtnode2.UserId, rtnode.UserId, 1, rtnode2.Rotation.Subject, rtnode2.RotationId, lastProductId, strbit));
                            }
                        }
                        else if (nodeto.SymbolCode == symbolService.getSymbolId("CASE"))
                        {
                            WorkflowNodeLink nodeToNext = null; ;
                            WorkflowNodeLink elseLink = null;
                            foreach (WorkflowNodeLink p in nodeto.WorkflowNodeLinks)
                            {
                                if (!p.Operator.Equals("ELSE"))
                                {
                                    //if (decissionValue(rtnode.Value, p.Value, p.Operator))
                                    //{
                                    //    nodeToNext = p;
                                    //    break;
                                    //}
                                }
                                else
                                    elseLink = p;
                            }
                            if (nodeToNext == null && elseLink != null)
                                nodeToNext = elseLink;

                            if (nodeToNext != null)
                            {
                                RotationNode rtnode2 = new RotationNode();

                                rtnode2.Rotation.Id = rtnode.Rotation.Id;
                                rtnode2.WorkflowNode.Id = nodeToNext.WorkflowNodeToId;
                                rtnode2.FirstNodeId = rtnode.FirstNodeId;
                                rtnode2.SenderRotationNodeId = rtnode.Id;
                                //rtnode2.UserId = (long)nodeToNext.WorkflowNodeTo.UserId;
                                rtnode2.UserId = (long)nodeToNext.WorkflowNodeTo.RotationUsers.FirstOrDefault(c => c.WorkflowNodeId == nodeToNext.WorkflowNodeToId && c.Rotation.Id == rtnode.Rotation.Id).User.Id;
                                rtnode2.PrevWorkflowNodeId = workflowNodeLink.WorkflowNodeToId;// tested OK
                                rtnode2.Status = (int)Constant.RotationStatus.Open;
                                rtnode2.Value = rtnode.Value;
                                rtnode2.CreatedAt = DateTime.Now;
                                db.RotationNodes.Add(rtnode2);
                                db.SaveChanges();

                                //TODO change how to get last id inserted
                                long lastProductId = db.RotationNodes.Where(item => item.RotationId == rtnode2.RotationId).Max(item => item.Id);
                                retvalues.Add(createActivityResult(rtnode2.UserId, rtnode.UserId, 1, rtnode2.Rotation.Subject, rtnode2.RotationId, lastProductId, strbit));
                            }
                        }
                        else if (nodeto.SymbolCode == symbolService.getSymbolId("END"))
                        {
                            if (rtnode.Status.Equals((int)Constant.RotationStatus.Declined))
                                UpdateStatus(db, rtnode.Rotation.Id, (int)Constant.RotationStatus.Open, (int)Constant.RotationStatus.Declined);
                            else
                                UpdateStatus(db, rtnode.Rotation.Id, (int)Constant.RotationStatus.Open, (int)Constant.RotationStatus.Declined);
                            retvalues.Add(createActivityResult(rtnode.UserId, rtnode.UserId, 1, rtnode.Rotation.Subject, rtnode.RotationId, rtnode.Id, "END"));
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
                var companyIdStarted = db.Usages.FirstOrDefault(c => c.Id == usageId && c.IsActive).CompanyId;
                rt.CompanyId = companyIdStarted;
                rt.DateUpdated = DateTime.Now;
                rt.DateStarted = DateTime.Now;

                // first node, node after start symbol
                var workflowNodeLinks = db.WorkflowNodeLinks.Where(c => c.WorkflowNode.WorkflowId == rt.WorkflowId && c.WorkflowNode.SymbolCode == 0).ToList();
                if (workflowNodeLinks == null)
                {
                    retvalues.Add(createActivityResult((int)Constant.RotationStatus.ERROR_WORKFLOW_START_NODE_NOT_FOUND, Constant.RotationStatus.ERROR_WORKFLOW_START_NODE_NOT_FOUND.ToString()));
                    return retvalues; //Invalid rotation
                }

                //check rotation started limit or add when limit passed
                var rotationStartedLimitStatus = subscriptionService.CheckOrAddSpecificUsage(Constant.BusinessPackageItem.Rotation_Started, companyIdStarted, 1, true);
            System.Diagnostics.Debug.WriteLine("LALALA: before check  " + rotationStartedLimitStatus);

                if (!rotationStartedLimitStatus.Equals(Constant.BusinessUsageStatus.OK) )
                {
                    retvalues.Add(createActivityResult((int)rotationStartedLimitStatus, rotationStartedLimitStatus.ToString()));
                    return retvalues;
                }
            System.Diagnostics.Debug.WriteLine("LALALA: after check  " + rotationStartedLimitStatus);

                
                long rotId = rt.Id;

                // send to all activity under start node +
                foreach (WorkflowNodeLink workflowNodeLink in workflowNodeLinks)
                {
            System.Diagnostics.Debug.WriteLine("LALALA: INSIDE  " + workflowNodeLink.WorkflowNodeToId);
                    RotationNode rtnode = new RotationNode();
                    //rtnode.Rotation = rt;
                    rtnode.RotationId = rt.Id;
                    //rtnode.RotationId = rotId;
                    rtnode.WorkflowNodeId = workflowNodeLink.WorkflowNodeToId;
                    rtnode.WorkflowNode = workflowNodeLink.WorkflowNodeTo;
                    rtnode.FirstNodeId = workflowNodeLink.FirstNodeId;
                    //long user = db.RotationUsers.FirstOrDefault(c => c.WorkflowNodeId == workflowNodeLink.WorkflowNodeToId && c.RotationId == rt.Id).UserId.Value;
                    long userNodeId = GetUserId(workflowNodeLink.WorkflowNodeToId, rt.Id);
                    //rtnode.User = user;
                    rtnode.UserId = userNodeId;
                    rtnode.Status = (int)Constant.RotationStatus.Open;
                    rtnode.Value = "";
                    rtnode.CreatedAt = DateTime.Now;
                    db.RotationNodes.Add(rtnode);
                    db.SaveChanges();
                    retvalues.Add(CreateActivityResult(rtnode.UserId, userId, 1, rt.Subject, rtnode.Id, rotationId, exitStatus:Constant.RotationStatus.OK.ToString()));   
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
            if (docs != null && docs.Count() > 0)
            {
                foreach (RotationNodeDoc rnc in docs)
                {
                    RotationNodeDoc dx = new RotationNodeDoc();
                    dx.DocumentId = rnc.Document.Id;
                    dx.FlagAction = rnc.FlagAction;
                    dx.RotationNodeId = rotationNode.Id;
                    dx.RotationId = rotationNode.RotationId;
                    rotationNode.RotationNodeDocs.Add(dx);

                    // update flag action di master rotationNodeDoc member
                    var memberId = rotationNode.UserId;
                    var rotationid = rotationNode.RotationId;
                    var docm = db.DocumentUsers.FirstOrDefault(c => c.DocumentId == rnc.Document.Id && c.UserId == memberId);
                    var rotationUser = db.RotationUsers.FirstOrDefault(rtUsr => rtUsr.RotationId == rotationid && rtUsr.UserId == memberId);
                    if (docm != null)
                    {
                        docm.FlagAction = rnc.FlagAction;
                        // Also Document permission updating related to Rotation User that have permission
                        docm.FlagPermission |= rotationUser.FlagPermission;
                    }
                    else
                    {
                        DocumentUser docmem = new DocumentUser();
                        docmem.DocumentId = rnc.Document.Id;
                        docmem.UserId = memberId;
                        docmem.FlagAction = rnc.FlagAction;
                        docmem.FlagPermission = 6; // default view, add annotate
                        // Also Document permission updating related to Rotation User that have permission
                        docmem.FlagPermission |= rotationUser.FlagPermission;
                        db.DocumentUsers.Add(docmem);
                    }

                    if (rnc.Document != null)
                    { // save annos first before set sign/initial/stamp
                        ICollection<DocumentElementInboxData> docElement = new List<DocumentElementInboxData>();
                        foreach (DocumentElement x in rnc.Document.DocumentElements)
                        {
                            docElement.Add(new DocumentElementInboxData(x));
                        }
                        docElement = docSvr.SaveAnnos(rnc.Document.Id, memberId, "CALLER", docElement);
                    }
                    if ((rnc.FlagAction & (int)Constant.EnumDocumentAction.SIGN) == (int)Constant.EnumDocumentAction.SIGN)
                        docSvr.Signature((long)rnc.Document.Id, memberId, rotationNode.Rotation.Id);
                    if ((rnc.FlagAction & (int)Constant.EnumDocumentAction.PRIVATESTAMP) == (int)Constant.EnumDocumentAction.PRIVATESTAMP)
                        docSvr.Stamp((long)rnc.Document.Id, memberId, rotationNode.Rotation.Id);
                }
                db.SaveChanges();
            }
        }

        private void InsertUpDoc(IEnumerable<RotationNodeUpDoc> docs, ref RotationNode rotationNode)
        {
            if (docs != null && docs.Count() > 0)
            {
                foreach (RotationNodeUpDoc rnc in docs)
                {
                    if (rnc.DocumentId == null)
                    {
                        using (var db = new ServiceContext())
                        {
                            Document upload = new Document();

                            upload.CreatorId = rotationNode.UserId;
                            upload.CreatedAt = DateTime.Now;
                            db.Documents.Add(upload);
                            db.SaveChanges();
                            rnc.DocumentId = upload.Id;
                            rnc.RotationId = rotationNode.RotationId;
                            rnc.RotationNodeId = rotationNode.Id;
                        }
                    }
                    RotationNodeUpDoc dx = new RotationNodeUpDoc();
                    dx.DocumentId = rnc.DocumentId;
                    rotationNode.RotationNodeUpDocs.Add(dx);
                }
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
                                    c.PrevWorkflowNodeId == node.PrevWorkflowNodeId &&
                                    c.UserId == node.UserId &&
                                    (statuses).Contains(c.Status));

                return (ndx != null);
            }
        }

        private void UpdateStatus(ServiceContext db, long rotationId, int previousStatus, int status)
        {
            var rot = db.Rotations.FirstOrDefault(c => c.Id == rotationId);
            rot.Status = status;
            rot.DateUpdated = DateTime.Now;
            foreach (RotationNode rotationNode in rot.RotationNodes)
            {
                if (rotationNode.Status.Equals(previousStatus))
                    rotationNode.Status = status;
            }
        }
    }
}