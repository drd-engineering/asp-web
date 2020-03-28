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

        public int Start(long userId, long rotationId, long subscriptionId)
        {
            
            var returnItem = StartProcess(userId, rotationId, subscriptionId);
            InboxService inboxService = new InboxService();
            /*MemberService memberService = new MemberService();*/
            List<int> returnValue = new List<int>();
            foreach (ActivityItem act in returnItem)
            {
                returnValue.Add(inboxService.CreateInbox(act));
                /*MemberService.sendEmailInbox(act);*/
            }
            return returnValue[0];
        }

        // subscription Id is either userId or companyId
        public List<ActivityItem> StartProcess(long userId, long rotationId, long subscriptionId)
        {
            if (!subscriptionService.isSubscriptionValid(userId, subscriptionId))
            {
                System.Diagnostics.Debug.WriteLine("::DEBUG:: GAK VALID  :: ");
                return null;
            }
            List<ActivityItem> retvalues = new List<ActivityItem>();

            using (var db = new ServiceContext())
            {
                var rt = db.Rotations.FirstOrDefault(c => c.Id == rotationId);
                if (!rt.Status.Equals((int)Constant.RotationStatus.Open))
                {
                    retvalues.Add(createActivityResult(-1));
                    return retvalues; //Invalid rotation
                }

                //update rotation
                rt.Status = (int)Constant.RotationStatus.In_Progress;
                var companyIdStarted = db.PlanBusinesses.FirstOrDefault(c => c.Id == subscriptionId && c.IsActive).CompanyId;
                rt.CompanyId = companyIdStarted;
                rt.DateUpdated = DateTime.Now;
                rt.DateStarted = DateTime.Now;
                System.Diagnostics.Debug.WriteLine("::DEBUG:: " + rt.Status+ " :: ");

                // first node, node after start symbol
                var workflowNodeLinks = db.WorkflowNodeLinks.Where(c => c.WorkflowNode.WorkflowId == rt.WorkflowId && c.WorkflowNode.SymbolCode == 0).ToList();
                if (workflowNodeLinks == null)
                {
                    retvalues.Add(createActivityResult(-5));
                    System.Diagnostics.Debug.WriteLine("REACHED ERROR WORKFLOWNODE:: ");
                    return retvalues; //Invalid rotation
                }

                long rotId = rt.Id;

                // send to all activity under start node +
                foreach (WorkflowNodeLink workflowNodeLink in workflowNodeLinks)
                {
                    RotationNode rtnode = new RotationNode();
                    //rtnode.Rotation = rt;
                    rtnode.RotationId = rt.Id;
                    //rtnode.RotationId = rotId;
                    rtnode.WorkflowNodeId = workflowNodeLink.WorkflowNodeToId;
                    rtnode.WorkflowNode = workflowNodeLink.WorkflowNodeTo;
                    rtnode.FirstNodeId = workflowNodeLink.FirstNodeId;
                    System.Diagnostics.Debug.WriteLine("REACHED CREATE RNODE:: " + rtnode.WorkflowNodeId + " : " + workflowNodeLink.WorkflowNodeToId);
                    //long user = db.RotationUsers.FirstOrDefault(c => c.WorkflowNodeId == workflowNodeLink.WorkflowNodeToId && c.RotationId == rt.Id).UserId.Value;
                    long userNodeId = getUserId(workflowNodeLink.WorkflowNodeToId, rt.Id);
                    System.Diagnostics.Debug.WriteLine("REACHED CREATE RNODE :: USER :: " + userNodeId);
                    //rtnode.User = user; 
                    rtnode.UserId = userNodeId;
                    rtnode.Status = (int)Constant.RotationStatus.Open;
                    rtnode.Value = "";
                    rtnode.CreatedAt = DateTime.Now;
                    db.RotationNodes.Add(rtnode);
                    System.Diagnostics.Debug.WriteLine("REACHED ADD RNODE:: " + rt.WorkflowId);
                    db.SaveChanges();
                    retvalues.Add(createActivityResult(rtnode.UserId, userId, 1, rt.Subject, rtnode.Id, rotationId));
                }
                db.SaveChanges();
                return retvalues;
            }

        }
        public int ProcessActivity(ProcessActivity parameter, Constant.EnumActivityAction enumActivityAction)
        {

            var ret = ProcessActivity(parameter, enumActivityAction, new DocumentService());
            EmailService emailService = new EmailService();
            foreach (ActivityItem act in ret)
            {
                emailService.sendEmailInbox(act);
            }
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
                insertDoc(param.RotationNodeDocs, db, ref rtnode, docSvr);
                insertUpDoc(param.RotationNodeUpDocs, ref rtnode);

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
                    System.Diagnostics.Debug.WriteLine("TEST INTO REVISION SECTION::");

                    var workflowNodeLink = db.WorkflowNodeLinks.Where(c => c.WorkflowNodeId == rtnode.WorkflowNodeId).FirstOrDefault();
                    RotationNode rtnode2 = new RotationNode();

                    rtnode2.RotationId = rtnode.RotationId;
                    rtnode2.WorkflowNodeId = workflowNodeLink.FirstNodeId;
                    rtnode2.FirstNodeId = workflowNodeLink.FirstNodeId;
                    rtnode2.SenderRotationNodeId = rtnode.Id;
                    rtnode2.UserId = (long)workflowNodeLink.FirstNode.RotationUsers.FirstOrDefault(c => c.WorkflowNodeId == workflowNodeLink.FirstNodeId && c.RotationId == rtnode.RotationId).User.Id;
                    rtnode2.PrevWorkflowNodeId = workflowNodeLink.WorkflowNodeId;// tested OK
                    rtnode2.Status = (int)Constant.RotationStatus.Open;
                    rtnode2.Value = rtnode.Value;
                    rtnode2.CreatedAt = DateTime.Now;
                    db.RotationNodes.Add(rtnode2);

                    //TODO change how to get last id inserted
                    long lastProductId = db.RotationNodes.Where(item => item.RotationId == rtnode2.RotationId).Max(item => item.Id);
                    retvalues.Add(createActivityResult(rtnode2.UserId, rtnode.UserId, 1, rtnode2.Rotation.Subject, rtnode2.RotationId, lastProductId, strbit));
                }
                else if (strbit.Equals("REJECT"))
                {
                    rtnode.Status = (int)Constant.RotationStatus.Declined;
                    updateAllStatus(db, rtnode.Rotation.Id, (int)Constant.RotationStatus.Declined);
                }
                else if (strbit.Equals("SUBMIT")) 
                {
                    System.Diagnostics.Debug.WriteLine("::MASUK:: SUBMIT  :: ");
                    Symbol symbol = symbolService.getSymbol(strbit);
                    int symbolCode = symbol == null ? 0 : symbol.Id;
                    var wfnodes = db.WorkflowNodeLinks.Where(c => c.WorkflowNodeId == rtnode.WorkflowNodeId && c.SymbolCode == symbolCode).ToList();
                    List<RotationNode> rotnodes = new List<RotationNode>();
                    int x = 0;
                    foreach (WorkflowNodeLink workflowNodeLink in wfnodes)
                    {

                        System.Diagnostics.Debug.WriteLine("TEST SYMBOL ACTIVITY::" + symbolService.getSymbolId("ACTIVITY"));
                        var nodeto = workflowNodeLink.WorkflowNodeTo;



                        if (nodeto.SymbolCode == symbolService.getSymbolId("ACTIVITY"))
                        {
                            System.Diagnostics.Debug.WriteLine("TEST INTO ACTIVITY SECTION::");
                            /*                            RotationNode rtnode2 = new RotationNode();*/
                            RotationNode rtnode2= db.RotationNodes.Create<RotationNode>();

                            rtnode2.RotationId = rtnode.RotationId;
                            rtnode2.WorkflowNodeId = workflowNodeLink.WorkflowNodeToId;
                            rtnode2.FirstNodeId = workflowNodeLink.FirstNodeId;
                            rtnode2.SenderRotationNodeId = rtnode.Id;
                            rtnode2.UserId = (long)workflowNodeLink.WorkflowNodeTo.RotationUsers.FirstOrDefault(c => c.WorkflowNodeId == workflowNodeLink.WorkflowNodeToId && c.RotationId == rtnode.RotationId).User.Id;
                            rtnode2.PrevWorkflowNodeId = workflowNodeLink.WorkflowNodeId;// tested OK
                            rtnode2.Status = (int)Constant.RotationStatus.Open;
                            rtnode2.Value = rtnode.Value;
                            rtnode2.CreatedAt = DateTime.Now;
                            db.RotationNodes.Add(rtnode2);


                            //TODO change how to get last id inserted
                            long lastProductId = db.RotationNodes.Where(item => item.RotationId == rtnode2.RotationId).Max(item => item.Id);
                            retvalues.Add(createActivityResult(rtnode2.UserId, rtnode.UserId, 1, rtnode2.Rotation.Subject, rtnode2.RotationId, lastProductId, strbit));

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
                                    rtnode2.FirstNodeId = workflowNodeLink.FirstNodeId;
                                    rtnode2.SenderRotationNodeId = rtnode.Id;
                                    //rtnode2.UserId = (long)lnk.WorkflowNodeTo.UserId;
                                    rtnode2.UserId = (long)lnk.WorkflowNodeTo.RotationUsers.FirstOrDefault(c => c.WorkflowNodeId == lnk.WorkflowNodeToId && c.Rotation.Id == rtnode.Rotation.Id).User.Id;
                                    rtnode2.PrevWorkflowNodeId = workflowNodeLink.WorkflowNodeToId; // tested OK
                                    rtnode2.Status = (int)Constant.RotationStatus.Open;
                                    rtnode2.Value = rtnode.Value;
                                    rtnode2.CreatedAt = DateTime.Now;
                                    db.RotationNodes.Add(rtnode2);

                                    long lastProductId = db.RotationNodes.Where(item => item.RotationId == rtnode2.RotationId).Max(item => item.Id);
                                    retvalues.Add(createActivityResult(rtnode2.UserId, rtnode.UserId, 1, rtnode2.Rotation.Subject, rtnode2.RotationId, lastProductId, strbit));
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
                            rtnode2.FirstNodeId = workflowNodeLink.FirstNodeId;
                            rtnode2.SenderRotationNodeId = rtnode.Id;
                            //rtnode2.UserId = (long)nodeToNext.WorkflowNodeTo.UserId;
                            rtnode2.UserId = (long)nodeToNext.WorkflowNodeTo.RotationUsers.FirstOrDefault(c => c.WorkflowNodeId == nodeToNext.WorkflowNodeToId && c.Rotation.Id == rtnode.Rotation.Id).User.Id;
                            rtnode2.PrevWorkflowNodeId = workflowNodeLink.WorkflowNodeId;//tested OK
                            rtnode2.Status = (int)Constant.RotationStatus.Open;
                            rtnode2.Value = rtnode.Value;
                            rtnode2.CreatedAt = DateTime.Now;
                            db.RotationNodes.Add(rtnode2);

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
                            rtnode2.FirstNodeId = workflowNodeLink.FirstNodeId;
                            rtnode2.SenderRotationNodeId = rtnode.Id;
                            //rtnode2.UserId = (long)nodeToNext.WorkflowNodeTo.UserId;
                            rtnode2.UserId = (long)nodeToNext.WorkflowNodeTo.RotationUsers.FirstOrDefault(c => c.WorkflowNodeId == nodeToNext.WorkflowNodeToId && c.Rotation.Id == rtnode.Rotation.Id).User.Id;
                            rtnode2.PrevWorkflowNodeId = workflowNodeLink.WorkflowNodeToId;// tested OK
                            rtnode2.Status = (int)Constant.RotationStatus.Open;
                            rtnode2.Value = rtnode.Value;// "TRF:" + nodeto.Value + ",ID:" + rtnode.Id + ",MEMBER:" + rtnode.UserId;
                            rtnode2.CreatedAt = DateTime.Now;

                            // check for double rotation node
                            if (!isExistNode(rtnode2))
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
                                rtnode2.FirstNodeId = workflowNodeLink.FirstNodeId;
                                rtnode2.SenderRotationNodeId = rtnode.Id;
                                //rtnode2.UserId = (long)nodeToNext.WorkflowNodeTo.UserId;
                                rtnode2.UserId = (long)nodeToNext.WorkflowNodeTo.RotationUsers.FirstOrDefault(c => c.WorkflowNodeId == nodeToNext.WorkflowNodeToId && c.Rotation.Id == rtnode.Rotation.Id).User.Id;
                                rtnode2.PrevWorkflowNodeId = workflowNodeLink.WorkflowNodeToId;// tested OK
                                rtnode2.Status = (int)Constant.RotationStatus.Open;
                                rtnode2.Value = rtnode.Value;
                                rtnode2.CreatedAt = DateTime.Now;
                                db.RotationNodes.Add(rtnode2);
                                

                                //TODO change how to get last id inserted
                                long lastProductId = db.RotationNodes.Where(item => item.RotationId == rtnode2.RotationId).Max(item => item.Id);
                                retvalues.Add(createActivityResult(rtnode2.UserId, rtnode.UserId, 1, rtnode2.Rotation.Subject, rtnode2.RotationId, lastProductId, strbit));

                            }
                        }
                        else if (nodeto.SymbolCode == symbolService.getSymbolId("END"))
                        {
                            if (rtnode.Status.Equals((int)Constant.RotationStatus.Declined))
                                updateAllStatus(db, rtnode.Rotation.Id, (int)Constant.RotationStatus.Declined);
                            else
                                updateAllStatus(db, rtnode.Rotation.Id, (int)Constant.RotationStatus.Completed);
                        }

                    }
                }
                var result = db.SaveChanges();

                InboxService inboxService = new InboxService();
                EmailService emailService = new EmailService();
                foreach (ActivityItem act in retvalues)
                {
                    System.Diagnostics.Debug.WriteLine(":: MASUK AKHIR :: ");
                    emailService.sendEmailInbox(act);
                    inboxService.GenerateNewInbox(act);
                }

                return retvalues;
            }
        }

        private bool isExistNode(RotationNode node)
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
        private void updateAllStatus(ServiceContext db, long rotationId, int status)
        {
            var rot = db.Rotations.FirstOrDefault(c => c.Id == rotationId);
            rot.Status = status;
            rot.DateUpdated = DateTime.Now;
            foreach (RotationNode rotationNode in rot.RotationNodes)
            {
                rotationNode.Status = status;
            }
        }

        private void insertDoc(IEnumerable<RotationNodeDoc> docs, ServiceContext db, ref RotationNode rotationNode, IDocumentService docSvr)
        {
            if (docs != null && docs.Count() > 0)
            {
                var distincts =
                    (from c in docs
                     select new RotationNodeDoc
                     {
                         Document = c.Document,
                         FlagAction = c.FlagAction,
                     }).ToList();

                foreach (RotationNodeDoc rnc in docs)
                {
                    RotationNodeDoc dx = new RotationNodeDoc();
                    dx.Document.Id = rnc.Document.Id;
                    dx.FlagAction = rnc.FlagAction;
                    rotationNode.RotationNodeDocs.Add(dx);

                    // update flag action di master rotationNodeDoc member
                    var memberId = rotationNode.UserId;
                    var docm = db.DocumentUsers.FirstOrDefault(c => c.DocumentId == rnc.Document.Id && c.UserId == memberId);
                    if (docm != null)
                        docm.FlagAction = rnc.FlagAction;
                    else
                    {
                        DocumentUser docmem = new DocumentUser();
                        docmem.DocumentId = (long)rnc.Document.Id;
                        docmem.UserId = memberId;
                        docmem.FlagAction = rnc.FlagAction;
                        db.DocumentUsers.Add(docmem);
                    }

                    //DocumentService docSvr = new DocumentService();
                    if (rnc.Document != null)
                    { // save annos first before set sign/initial/stamp
                        ICollection<DocumentElementInboxData> docElement = new List<DocumentElementInboxData>();
                        foreach (DocumentElement x in rnc.Document.DocumentElements)
                        {
                            docElement.Add(new DocumentElementInboxData(x));
                        }
                        docSvr.SaveAnnos((long)rnc.Document.Id, memberId, "CALLER", docElement);
                    }
                    if ((rnc.FlagAction & (int)Constant.EnumDocumentAction.SIGN) == (int)Constant.EnumDocumentAction.SIGN)
                        docSvr.Signature((long)rnc.Document.Id, memberId, rotationNode.Rotation.Id);
                    if ((rnc.FlagAction & (int)Constant.EnumDocumentAction.PRIVATESTAMP) == (int)Constant.EnumDocumentAction.PRIVATESTAMP)
                        docSvr.Stamp((long)rnc.Document.Id, memberId, rotationNode.Rotation.Id);
                }
            }
        }

        private void insertUpDoc(IEnumerable<RotationNodeUpDoc> docs, ref RotationNode rotationNode)
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
                        }
                    }
                    RotationNodeUpDoc dx = new RotationNodeUpDoc();
                    dx.DocumentId = rnc.DocumentId;
                    rotationNode.RotationNodeUpDocs.Add(dx);
                }
            }
        }

        private ActivityItem createActivityResult(long userId, long previousUserId, int exitCode, string rotationName, long rotationId, long rotationNodeId, string lastActivityStatus)
        {
            using (var db = new ServiceContext())
            {
                ActivityItem ret = createActivityResult(userId, previousUserId, exitCode, rotationName, rotationNodeId,rotationId);
                ret.LastActivityStatus = lastActivityStatus;
                return ret;
            }
        }

        private ActivityItem createActivityResult(long userId, long previousUserId, int exitCode, string rotationName, long rotationNodeId, long rotationId)
        {
            System.Diagnostics.Debug.WriteLine(":: MASUK AKHIR :: "+ userId + previousUserId + exitCode + rotationName + rotationNodeId + rotationId);
            using (var db = new ServiceContext())
            {
                ActivityItem ret = new ActivityItem();
                ret.RotationId = rotationId;
                ret.ExitCode = exitCode;

                var mem = db.Users.FirstOrDefault(c => c.Id == userId);
                ret.UserId = userId;
                ret.UserName = mem.Name;
                ret.Email = mem.Email;
                
                var prev = db.Users.FirstOrDefault(c => c.Id == previousUserId);
                ret.PreviousUserId = userId;
                ret.PreviousUserName = prev.Name;
                ret.PreviousEmail = prev.Email;


                ret.RotationName = rotationName;
                ret.RotationNodeId = rotationNodeId;
                return ret;
            }
        }

        private ActivityItem createActivityResult(long userId, int exitCode)
        {
            using (var db = new ServiceContext())
            {
                ActivityItem ret = new ActivityItem();
                var mem = db.Users.FirstOrDefault(c => c.Id == userId);
                ret.ExitCode = exitCode;
                ret.Email = mem.Email;
                ret.UserId = userId;
                ret.UserName = mem.Name;
                return ret;
            }
        }

        private ActivityItem createActivityResult(int exitCode)
        {
            ActivityItem ret = new ActivityItem();
            ret.ExitCode = exitCode;
            return ret;
        }

        private long getUserId(long WorkflowNodeToId, long RotationId)
        {
            using (var db = new ServiceContext())
            {
                return db.RotationUsers.FirstOrDefault(c => c.WorkflowNodeId == WorkflowNodeToId && c.RotationId == RotationId).UserId.Value;
            }
        }
    }
}
