﻿using System;
using System.Collections.Generic;
using System.Linq;

using DRD.Models;
using DRD.Models.API;
using DRD.Models.Custom;
using DRD.Models.View.List;
using DRD.Service.Context;

using Newtonsoft.Json;


namespace DRD.Service
{
    class WorkflowDeepService
    {
        private readonly string _connString;
        private string _appZoneAccess;

        public WorkflowDeepService(string appZoneAccess, string connString)
        {
            _appZoneAccess = appZoneAccess;
            _connString = connString;
        }
        public WorkflowDeepService(string appZoneAccess)
        {
            _appZoneAccess = appZoneAccess;
            _connString = Constant.CONSTRING;
        }
        public WorkflowDeepService()
        {
            _connString = Constant.CONSTRING;
        }
        private static RotationNodeDoc DeepCopy(RotationNodeDoc source)
        {

            var DeserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };

            return JsonConvert.DeserializeObject<RotationNodeDoc>(JsonConvert.SerializeObject(source), DeserializeSettings);

        }

        public Rotation assignNodes(ServiceContext db, Rotation rot, long memberId, IDocumentService docSvr)
        {
            Rotation rotation = rot;

            rotation.RotationNodes =
                (from rotationNode in db.RotationNodes
                 where rotationNode.Rotation.Id == rotation.Id
                 orderby rotationNode.DateCreated
                 select new RotationNode
                 {
                     Id = rotationNode.Id,
                     DateCreated = rotationNode.DateCreated,
                     Status = rotationNode.Status,
                     Value = rotationNode.Value,
                     PrevWorkflowNodeId = rotationNode.PrevWorkflowNodeId,
                     SenderRotationNodeId = rotationNode.SenderRotationNodeId,
                     User = new User
                     {
                         Id = rotationNode.User.Id,
                         Name = rotationNode.User.Name,
                         ImageProfile = rotationNode.User.ImageProfile,
                         ImageInitials = rotationNode.User.ImageInitials,
                         ImageSignature = rotationNode.User.ImageSignature,
                         ImageStamp = rotationNode.User.ImageStamp,
                         ImageKtp1 = rotationNode.User.ImageKtp1,
                         ImageKtp2 = rotationNode.User.ImageKtp2,
                     },
                     WorkflowNode = new WorkflowNode
                     {
                         Id = rotationNode.WorkflowNode.Id,
                         Caption = rotationNode.WorkflowNode.Caption
                     },
                     RotationNodeRemarks =
                     (from rotationNodeRemark in rotationNode.RotationNodeRemarks
                      select new RotationNodeRemark
                      {
                          Id = rotationNodeRemark.Id,
                          Remark = rotationNodeRemark.Remark,
                          DateStamp = rotationNodeRemark.DateStamp,
                      }).ToList(),
                 }).ToList();


            foreach (RotationNode rotationNode in rotation.RotationNodes)
            {
                // set note for waiting pending member
                if (rotationNode.Status.Equals("02"))
                {
                    rotationNode.Note = "Waiting for action from: ";
                    var rtpending = db.RotationNodes.FirstOrDefault(c => c.Id == rotationNode.Id);
                    var wfnto = rtpending.WorkflowNode.WorkflowNodeLinks.FirstOrDefault(c => c.Symbol.Code.Equals(Constant.EnumActivityAction.SUBMIT.ToString()));

                    var nodetos = db.WorkflowNodeLinks.Where(c => c.WorkflowNodeId == wfnto.WorkflowNodeToId && c.Symbol.Code.Equals(Constant.EnumActivityAction.SUBMIT.ToString())).ToList();
                    foreach (WorkflowNodeLink workflowNodeLink in nodetos)
                    {
                        int[] statuses = { (int)RotationItem.StatusName.Open, (int)RotationItem.StatusName.Revision };
                        if (statuses.Contains(workflowNodeLink.WorkflowNodes.RotationNodes.FirstOrDefault().Status))
                            rotationNode.Note += workflowNodeLink.WorkflowNodes.RotationUsers.FirstOrDefault(c => c.WorkflowNodeId == workflowNodeLink.WorkflowNodeId).User.Name + " | " + workflowNodeLink.WorkflowNodes.Caption + ", ";
                    }

                    if (rotationNode.Note.EndsWith(", "))
                        rotationNode.Note = rotationNode.Note.Substring(0, rotationNode.Note.Length - 2);

                }
                // set note for alter link
                else if (rotationNode.Status.Equals((int) RotationItem.StatusName.Open))
                {
                    var node = db.WorkflowNodeLinks.FirstOrDefault(c => c.WorkflowNodeId == rotationNode.WorkflowNode.Id && c.Symbol.Code.Equals(Constant.EnumActivityAction.ALTER.ToString()));
                    if (node != null)
                    {
                        if (node.Operator != null)
                        {
                            var start = rotationNode.DateCreated;
                            double val = 0;
                            if (Double.TryParse(node.Value, out val))
                            {
                                if (node.Operator.Equals("HOUR"))
                                    start = start.AddHours(val);
                                else
                                    start = start.AddDays(val);
                            }

                            rotationNode.Note = "There is a flow alter on " + start.ToString("dd/MM/yyyy HH:mm");
                        }
                        else
                            rotationNode.Note = "There is a flow alter, there has been no determination of the period.";
                    }
                }

                if (rotationNode.Id == rotation.RotationNodeId && rotation.User.Id == rotationNode.User.Id && rotationNode.Status.Equals((int) RotationItem.StatusName.Open) && (rotationNode.PrevWorkflowNodeId != null || rotationNode.SenderRotationNodeId != null))
                {
                    if (rotationNode.PrevWorkflowNodeId != null)
                    {
                        var wfn = db.WorkflowNodes.FirstOrDefault(c => c.Id == rotationNode.PrevWorkflowNodeId);
                        if (wfn.Symbol.Code.Equals("PARALLEL"))
                        {
                            var wfnIds = db.WorkflowNodeLinks.Where(c => c.WorkflowNodeToId == wfn.Id).Select(c => c.WorkflowNodeId).ToArray();
                            var rns = db.RotationNodes.Where(c => wfnIds.Contains(c.WorkflowNode.Id)).ToList();
                            List<RotationNodeDoc> listDoc = new List<RotationNodeDoc>();
                            List<RotationNodeUpDoc> listUpDoc = new List<RotationNodeUpDoc>();
                            foreach (RotationNode rnx in rns)
                            {
                                var d = assignNodeDocs(db, rnx.Id, memberId/*rotation.UserId*/, rot.RotationNodeId, docSvr);
                                if (d.Count > 0)
                                    listDoc.AddRange(d);

                                var ud = assignNodeUpDocs(db, rnx.Id);
                                if (ud.Count > 0)
                                    listUpDoc.AddRange(ud);

                            }
                            if (listDoc.Count > 0)
                                rotationNode.RotationNodeDocs = listDoc;
                            if (listUpDoc.Count > 0)
                                rotationNode.RotationNodeUpDocs = listUpDoc;
                        }
                    }
                    else
                    {

                    }
                }
                else
                {
                    //rotationNode.RotationNodeDocs = assignNodeDocs(db, rotationNode.Id, rotation.UserId, rot.RotationNodeId);
                    rotationNode.RotationNodeDocs = assignNodeDocs(db, rotationNode.Id, memberId/*rotationNode.UserId*/, rot.RotationNodeId, docSvr);
                    rotationNode.RotationNodeUpDocs = assignNodeUpDocs(db, rotationNode.Id);

                }

                //document summaries document
                foreach (RotationNodeDoc rotationNodeDoc in rotationNode.RotationNodeDocs)
                {
                    // get anno
                    foreach (DocumentElement documentElement in rotationNodeDoc.Document.DocumentElements)
                    {
                        if (documentElement.ElementId == null || documentElement.ElementId == 0) continue;
                        if (documentElement.ElementType.Code.Equals("SIGNATURE") || documentElement.ElementType.Code.Equals("INITIAL") || documentElement.ElementType.Code.Equals("PRIVATESTAMP"))
                        {
                            var user = db.Users.FirstOrDefault(c => c.Id == documentElement.ElementId);
                            documentElement.Element.UserId = user.Id;
                            documentElement.Element.Name = user.Name;
                            documentElement.Element.Foto = user.ImageProfile;
                        }
                        else if (documentElement.ElementType.Code.Equals("STAMP"))
                        {
                            var stmp = db.Stamps.FirstOrDefault(c => c.Id == documentElement.ElementId);
                            documentElement.Element.Name = stmp.Descr;
                            documentElement.Element.Foto = stmp.StampFile;
                        }
                    }

                    var dx = rotation.SumRotationNodeDocs.FirstOrDefault(c => c.Document.Id == rotationNodeDoc.Document.Id);
                    if (dx != null)
                    {
                        dx.FlagAction |= rotationNodeDoc.FlagAction;
                    }
                    else
                    {
                        rotation.SumRotationNodeDocs.Add(DeepCopy(rotationNodeDoc));
                    }

                }
                //attchment summaries 
                //foreach (RotationNodeUpDoc rotationNodeDoc in rotationNode.RotationNodeUpDocs)
                //{
                //    var dx = rotation.SumRotationNodeUpDocs.FirstOrDefault(c => c.Document.FileName.Equals(rotationNodeDoc.Document.FileName));
                //    if (dx == null)
                //        rotation.SumRotationNodeUpDocs.Add(rotationNodeDoc);
                //}
            }


            return rotation;
        }

        private List<RotationNodeDoc> assignNodeDocs(ServiceContext db, long rnId, long memId, long curRnId, IDocumentService docSvr)
        {
            //rotationNode.RotationNodeDocs =
            var result =
                (from d in db.RotationNodeDocs
                 where d.RotationNode.Id == rnId
                 select new RotationNodeDoc
                 {
                     Id = d.Id,
                     FlagAction = d.FlagAction,
                     RotationNode = new RotationNode
                     {
                         Rotation = d.RotationNode.Rotation,
                     },
                     Document = new Document
                     {
                         Title = d.Document.Title,
                         FileName = d.Document.FileName,
                         FileSize = d.Document.FileSize,
                         DocumentUser =
                             (from dm in d.Document.DocumentUsers
                              where dm.UserId == memId // default inbox member
                              select new DocumentUser
                              {
                                  Id = dm.Id,
                                  DocumentId = dm.DocumentId,
                                  UserId = dm.UserId,
                                  FlagAction = dm.FlagAction,
                              }).FirstOrDefault(),
                         
                         DocumentElements =
                             (from documentElement in d.Document.DocumentElements
                              select new DocumentElement
                              {
                                  Id = documentElement.Id,
                                  Document = documentElement.Document,
                                  Page = documentElement.Page,
                                  LeftPos = documentElement.LeftPos,
                                  TopPos = documentElement.TopPos,
                                  WidthPos = documentElement.WidthPos,
                                  HeightPos = documentElement.HeightPos,
                                  Color = documentElement.Color,
                                  BackColor = documentElement.BackColor,
                                  Data = documentElement.Data,
                                  Data2 = documentElement.Data2,
                                  Rotation = documentElement.Rotation,
                                  ScaleX = documentElement.ScaleX,
                                  ScaleY = documentElement.ScaleY,
                                  TransX = documentElement.TransX,
                                  TransY = documentElement.TransY,
                                  StrokeWidth = documentElement.StrokeWidth,
                                  Opacity = documentElement.Opacity,
                                  Flag = documentElement.Flag,
                                  FlagCode = documentElement.FlagCode,
                                  FlagDate = documentElement.FlagDate,
                                  FlagImage = documentElement.FlagImage,
                                  CreatorId = documentElement.CreatorId,
                                  ElementId = documentElement.ElementId,
                                  UserId = documentElement.UserId,
                                  DateCreated = documentElement.DateCreated,
                                  DateUpdated = documentElement.DateUpdated,
                                  ElementType = new ElementType
                                  {
                                      Id = documentElement.ElementType.Id,
                                      Code = documentElement.ElementType.Code,
                                  }
                              }).ToList(),
                     }
                 }).ToList();

            if (result != null /*&& curRnId != 0*/)
            {
                // assign permission
                //DocumentService docSvr = new DocumentService();
                foreach (RotationNodeDoc rnd in result)
                {
                    if (rnd.Document.DocumentUser == null)
                    {
                        rnd.Document.DocumentUser = new DocumentUser();
                        rnd.Document.DocumentUser.UserId = memId;
                        rnd.Document.DocumentUser.DocumentId = (long)rnd.Document.Id;
                    }
                    if (curRnId == 0)
                        curRnId = -rnd.RotationNode.Rotation.Id;
                    rnd.Document.DocumentUser.FlagPermission = docSvr.GetPermission(memId, curRnId, (long)rnd.Document.Id);
                }
            }

            return result;// rotationNode.RotationNodeDocs;
        }
        private List<RotationNodeUpDoc> assignNodeUpDocs(ServiceContext db, long rnId)
        {
            //rotationNode.RotationNodeUpDocs =
            var result =
                (from ud in db.RotationNodeUpDocs
                 where ud.RotationNode.Id == rnId
                 select new RotationNodeUpDoc
                 {
                     Id = ud.Id,
                     DocumentId = ud.DocumentId,
                     //Document = new Document
                     //{
                     //    FileFlag = ud.Document.FileFlag,
                     //    FileName = ud.Document.FileName,
                     //    FileSize = ud.Document.FileSize,
                     //    CreatorId = ud.Document.CreatorId,
                     //    DateCreated = ud.Document.DateCreated,
                     //}
                 }).ToList();

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prod"></param>
        /// <returns></returns>
        public long Save(Rotation prod)
        {
            Rotation product;
            using (var db = new ServiceContext())
            {
                if (prod.Id != 0)
                {
                    //if (!string.IsNullOrEmpty(prod.Code))
                    //{
                    //    var cxmember = db.Rotations.Count(c => c.Code == prod.Code && c.Id != prod.Id);
                    //    if (cxmember > 0)
                    //        return -1;
                    //}

                    product = db.Rotations.FirstOrDefault(c => c.Id == prod.Id);
                }
                else
                {
                    //if (!string.IsNullOrEmpty(prod.Code))
                    //{
                    //    var cxmember = db.Rotations.Count(c => c.Code == prod.Code);
                    //    if (cxmember > 0)
                    //        return -1;
                    //}

                    product = new Rotation();
                }
                product.Subject = prod.Subject;
                product.Workflow.Id = prod.Workflow.Id;
                product.Remark = prod.Remark;
                product.Status = prod.Status;
                product.CreatorId = prod.CreatorId;
                product.User.Id = prod.User.Id;
                if (prod.Id == 0)
                {
                    product.DateCreated = DateTime.Now;
                    db.Rotations.Add(product);
                }
                else
                    product.DateUpdated = DateTime.Now;

                var result = db.SaveChanges();

                //
                // prepare data detail
                //
                var cxold = db.RotationUsers.Count(c => c.Rotation.Id == product.Id);
                var cxnew = prod.RotationUsers.Count();
                if (cxold < cxnew)
                {
                    var ep = prod.RotationUsers.ElementAt(0); // get 1 data for sample
                    for (var x = cxold; x < cxnew; x++)
                    {
                        RotationUser aii = new RotationUser();
                        aii.Rotation.Id = product.Id;
                        aii.WorkflowNodeId = ep.WorkflowNodeId;
                        aii.User.Id = ep.User.Id;
                        aii.FlagPermission = ep.FlagPermission;

                        db.RotationUsers.Add(aii);
                    }
                    db.SaveChanges();
                }
                else if (cxold > cxnew)
                {
                    var dremove = db.RotationUsers.Where(c => c.Rotation.Id == product.Id).Take(cxold - cxnew).ToList();
                    db.RotationUsers.RemoveRange(dremove);
                    db.SaveChanges();
                }

                // save detail
                var dnew = db.RotationUsers.Where(c => c.Rotation.Id == product.Id).ToList();
                int v = 0;
                foreach (RotationUser d in dnew)
                {
                    var epos = prod.RotationUsers.ElementAt(v);
                    d.WorkflowNodeId = epos.WorkflowNodeId;
                    d.User.Id = epos.User.Id;
                    d.FlagPermission = epos.FlagPermission;
                    //d.FlagAction = epos.FlagAction;
                    //d.CxDownload = epos.CxDownload;
                    //d.CxPrint = epos.CxPrint;
                    v++;
                }
                db.SaveChanges();

                return product.Id;

            }

        }

        private ActivityItem createActivityResult(long memberId, int exitCode = 1)
        {
            using (var db = new ServiceContext())
            {
                ActivityItem ret = new ActivityItem();
                var mem = db.Users.FirstOrDefault(c => c.Id == memberId);

                ret = new ActivityItem();
                ret.ExitCode = exitCode;
                ret.Email = mem.Email;
                ret.UserId = memberId;
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

        public List<ActivityItem> Start(long id)
        {
            List<ActivityItem> retvalues = new List<ActivityItem>();

            using (var db = new ServiceContext())
            {
                var rt = db.Rotations.FirstOrDefault(c => c.Id == id);
                if (!rt.Status.Equals((int) RotationItem.StatusName.Open))
                {
                    retvalues.Add(createActivityResult(-1));
                    return retvalues; //Invalid rotation
                }

                //var plan = db.MemberPlans.FirstOrDefault(c => c.UserId == rt.User.Id && c.IsDefault);
                //if (plan == null)
                //{
                //    retvalues.Add(createActivityResult(-2));
                //    return retvalues; //Invalid member plan
                //}

                //if (plan.ValidPackage < DateTime.Now)
                //{
                //    retvalues.Add(createActivityResult(-3));
                //    return retvalues; //expired package
                //}

                //if (plan.RotationCount + plan.RotationCountAdd - plan.RotationCountUsed <= 0)
                //{
                //    retvalues.Add(createActivityResult(-4));
                //    return retvalues; //The used rotation exceeds the data packet quota number
                //}

                //save usage rotation 
                //plan.RotationCountUsed++;
                //update rotation
                rt.Status = (int)RotationItem.StatusName.In_Progress;
                rt.DateUpdated = DateTime.Now;
                rt.DateCreated = DateTime.Now;
                rt.DateStarted = DateTime.Now;
                var result = db.SaveChanges();

                // first node, node after start symbol
                var wfstarts = db.WorkflowNodeLinks.Where(c => c.WorkflowNodes.WorkflowId == rt.Workflow.Id && c.WorkflowNodes.Symbol.Code.Equals("START")).ToList();
                if (wfstarts.Count() == 0)
                {
                    retvalues.Add(createActivityResult(-1));
                    return retvalues; //Invalid rotation
                }

                // send to all activity under start node 
                foreach (WorkflowNodeLink wfnl in wfstarts)
                {
                    RotationNode rtnode = new RotationNode();
                    rtnode.Rotation.Id = id;
                    rtnode.WorkflowNode.Id = wfnl.WorkflowNodeToId;
                    rtnode.SenderRotationNodeId = null;
                    rtnode.UserId = (long)wfnl.WorkflowNodeTos.RotationUsers.FirstOrDefault(c => c.WorkflowNodeId == wfnl.WorkflowNodeToId && c.Rotation.Id == id).User.Id;
                    rtnode.Status = (int)RotationItem.StatusName.Open;
                    rtnode.Value = "";
                    rtnode.DateCreated = DateTime.Now;
                    db.RotationNodes.Add(rtnode);

                    retvalues.Add(createActivityResult(rtnode.UserId));
                }
                db.SaveChanges();

                return retvalues;

            }

        }

        public List<ActivityItem> ProcessActivity(ProcessActivity param, Constant.EnumActivityAction bit, IDocumentService docSvr)
        {
            List<ActivityItem> retvalues = new List<ActivityItem>();

            using (var db = new ServiceContext())
            {
                var strbit = bit.ToString();
                //get current rotation node
                RotationNode rtnode = db.RotationNodes.FirstOrDefault(c => c.Id == param.RotationNodeId);
                #region submit penerima transfer
                //get node is tranfer 
                if (strbit.Equals("SUBMIT") && rtnode.WorkflowNode.Symbol.Code.Equals("ACTIVITY") && rtnode.SenderRotationNodeId != null)// && rtnode.Value != null && rtnode.Value.StartsWith("TRF"))
                {
                    // get sender node value
                    RotationNode senderRotNode = rtnode.RotationNode_SenderRotationNodeId;
                    long senderNodeId = senderRotNode.Id;
                    long senderUserId = senderRotNode.UserId;
                    long senderWfNodeId = senderRotNode.WorkflowNode.Id;
                    long? prevWfNodeId = rtnode.PrevWorkflowNodeId;
                    // get transfer node
                    var prevWfNode = db.WorkflowNodes.FirstOrDefault(c => c.Id == prevWfNodeId);
                    if (prevWfNode.Symbol.Code.Equals("TRANSFER"))
                    {
                        decimal transfer = decimal.Parse(prevWfNode.Value);
                        if (transfer > 0)
                        {
                            //MemberDepositTrxService mdtSvr = new MemberDepositTrxService();
                            
                            // potong deposit
                            var nxd = db.RotationNodes.FirstOrDefault(c => c.Id == senderNodeId);
                        }
                    }
                }
                #endregion penerima transfer

                //set last node to in progress
                rtnode.Status = (int)RotationItem.StatusName.In_Progress; // currrent is submit
                rtnode.Value = param.Value;
                rtnode.DateUpdated = DateTime.Now;
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
                    rtnode.Status = (int)RotationItem.StatusName.Revision;
                else if (strbit.Equals("ALTER"))
                    rtnode.Status = (int)RotationItem.StatusName.Altered;
                else if (strbit.Equals("REJECT"))
                    rtnode.Status = (int)RotationItem.StatusName.Declined;

                var wfnodes = db.WorkflowNodeLinks.Where(c => c.WorkflowNodeId == rtnode.WorkflowNode.Id && c.Symbol.Code.Equals(strbit)).ToList();
                List<RotationNode> rotnodes = new List<RotationNode>();
                foreach (WorkflowNodeLink wfnl in wfnodes)
                {
                    var nodeto = wfnl.WorkflowNodeTos;
                    if (nodeto.Symbol.Code.Equals("ACTIVITY"))
                    {

                        RotationNode rtnode2 = new RotationNode();

                        rtnode2.Rotation.Id = rtnode.Rotation.Id;
                        rtnode2.WorkflowNode.Id = wfnl.WorkflowNodeToId;
                        rtnode2.SenderRotationNodeId = rtnode.Id;
                        //rtnode2.UserId = (long)wfnl.WorkflowNodeTos.UserId;
                        rtnode2.UserId = (long)wfnl.WorkflowNodeTos.RotationUsers.FirstOrDefault(c => c.WorkflowNodeId == wfnl.WorkflowNodeToId && c.Rotation.Id == rtnode.Rotation.Id).User.Id;
                        rtnode2.PrevWorkflowNodeId = wfnl.WorkflowNodeId;// tested OK
                        rtnode2.Status = (int)RotationItem.StatusName.Open;
                        rtnode2.Value = rtnode.Value;
                        rtnode2.DateCreated = DateTime.Now;
                        db.RotationNodes.Add(rtnode2);
                        retvalues.Add(createActivityResult(rtnode2.UserId));
                    }
                    else if (nodeto.Symbol.Code.Equals("PARALLEL"))
                    {
                        bool ispending = false;
                        foreach (WorkflowNodeLink wfnlx in wfnl.WorkflowNodeTos.WorkflowNodeLinkTos)
                        {
                            var rotnode = db.RotationNodes.FirstOrDefault(c => c.WorkflowNode.Id == wfnlx.WorkflowNodeId);
                            if (rotnode.WorkflowNode.Id != rtnode.WorkflowNode.Id)
                            {
                                //var isready02 = false;
                                //RotationNode retnode02 = null;
                                if (rotnode.Status.Equals("05"))
                                {
                                    // checking apakah 05 sudah menjadi 02 di id lain
                                    var retnode02 = db.RotationNodes.FirstOrDefault(c =>
                                                     c.Rotation.Id == rotnode.Rotation.Id &&
                                                     c.UserId == rotnode.UserId &&
                                                     c.PrevWorkflowNodeId == rotnode.PrevWorkflowNodeId &&
                                                     c.WorkflowNode.Id == wfnlx.WorkflowNodeId &&
                                                     c.Status.Equals(RotationItem.StatusName.Pending));
                                    //isready02 = (retnode02 != null);
                                    if (retnode02 != null) rotnode = retnode02;
                                }
                                int[] statuses = { (int)RotationItem.StatusName.Open, (int)RotationItem.StatusName.Revision };
                                if ((statuses).Contains(rotnode.Status))
                                {
                                    ispending = true;
                                    break;
                                }
                                else if (rotnode.Status.Equals((int)RotationItem.StatusName.Pending))
                                {
                                    rotnodes.Add(rotnode);
                                }
                            }
                        }
                        if (ispending)
                        {
                            rtnode.Status = (int)RotationItem.StatusName.Pending;
                            continue;
                        }
                        else
                        {
                            // ubah semua status jadi 01, yang pending (02) menjadi in progress
                            foreach (RotationNode rotn in rotnodes)
                            {
                                rotn.Status = (int)RotationItem.StatusName.In_Progress;
                            }

                            foreach (WorkflowNodeLink lnk in nodeto.WorkflowNodeLinkTos)
                            {
                                RotationNode rtnode2 = new RotationNode();

                                rtnode2.Rotation.Id = rtnode.Rotation.Id;
                                rtnode2.WorkflowNode.Id = lnk.WorkflowNodeToId;
                                rtnode2.SenderRotationNodeId = rtnode.Id;
                                //rtnode2.UserId = (long)lnk.WorkflowNodeTos.UserId;
                                rtnode2.UserId = (long)lnk.WorkflowNodeTos.RotationUsers.FirstOrDefault(c => c.WorkflowNodeId == lnk.WorkflowNodeToId && c.Rotation.Id == rtnode.Rotation.Id).User.Id;
                                rtnode2.PrevWorkflowNodeId = wfnl.WorkflowNodeToId; // tested OK
                                rtnode2.Status = (int)RotationItem.StatusName.Open;
                                rtnode2.Value = rtnode.Value;
                                rtnode2.DateCreated = DateTime.Now;
                                db.RotationNodes.Add(rtnode2);
                                retvalues.Add(createActivityResult(rtnode2.UserId));
                            }
                        }
                    }
                    else if (nodeto.Symbol.Code.Equals("DECISION"))
                    {
                        WorkflowNodeLink nodeToNext;
                        //if (decissionValue(rtnode.Value, nodeto.Value, nodeto.Operator))
                        //    nodeToNext = nodeto.WorkflowNodeLinks.FirstOrDefault(c => c.Symbol.Code.Equals("YES"));
                        //else
                            nodeToNext = nodeto.WorkflowNodeLinks.FirstOrDefault(c => c.Symbol.Code.Equals("NO"));

                        RotationNode rtnode2 = new RotationNode();

                        rtnode2.Rotation.Id = rtnode.Rotation.Id;
                        rtnode2.WorkflowNode.Id = nodeToNext.WorkflowNodeToId;
                        rtnode2.SenderRotationNodeId = rtnode.Id;
                        //rtnode2.UserId = (long)nodeToNext.WorkflowNodeTos.UserId;
                        rtnode2.UserId = (long)nodeToNext.WorkflowNodeTos.RotationUsers.FirstOrDefault(c => c.WorkflowNodeId == nodeToNext.WorkflowNodeToId && c.Rotation.Id == rtnode.Rotation.Id).User.Id;
                        rtnode2.PrevWorkflowNodeId = wfnl.WorkflowNodeId;//tested OK
                        rtnode2.Status = (int) RotationItem.StatusName.Open;
                        rtnode2.Value = rtnode.Value;
                        rtnode2.DateCreated = DateTime.Now;
                        db.RotationNodes.Add(rtnode2);
                        retvalues.Add(createActivityResult(rtnode2.UserId));
                    }
                    else if (nodeto.Symbol.Code.Equals("TRANSFER"))
                    {
                        WorkflowNodeLink nodeToNext;
                        nodeToNext = nodeto.WorkflowNodeLinks.FirstOrDefault();

                        RotationNode rtnode2 = new RotationNode();

                        rtnode2.Rotation.Id = rtnode.Rotation.Id;
                        rtnode2.WorkflowNode.Id = nodeToNext.WorkflowNodeToId;
                        rtnode2.SenderRotationNodeId = rtnode.Id;
                        //rtnode2.UserId = (long)nodeToNext.WorkflowNodeTos.UserId;
                        rtnode2.UserId = (long)nodeToNext.WorkflowNodeTos.RotationUsers.FirstOrDefault(c => c.WorkflowNodeId == nodeToNext.WorkflowNodeToId && c.Rotation.Id == rtnode.Rotation.Id).User.Id;
                        rtnode2.PrevWorkflowNodeId = wfnl.WorkflowNodeToId;// tested OK
                        rtnode2.Status = (int)RotationItem.StatusName.Open;
                        rtnode2.Value = rtnode.Value;// "TRF:" + nodeto.Value + ",ID:" + rtnode.Id + ",MEMBER:" + rtnode.UserId;
                        rtnode2.DateCreated = DateTime.Now;

                        // check for double rotation node
                        if (!isExistNode(rtnode2))
                        {
                            db.RotationNodes.Add(rtnode2);
                            retvalues.Add(createActivityResult(rtnode2.UserId));
                        }
                    }
                    else if (nodeto.Symbol.Code.Equals("CASE"))
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
                            rtnode2.SenderRotationNodeId = rtnode.Id;
                            //rtnode2.UserId = (long)nodeToNext.WorkflowNodeTos.UserId;
                            rtnode2.UserId = (long)nodeToNext.WorkflowNodeTos.RotationUsers.FirstOrDefault(c => c.WorkflowNodeId == nodeToNext.WorkflowNodeToId && c.Rotation.Id == rtnode.Rotation.Id).User.Id;
                            rtnode2.PrevWorkflowNodeId = wfnl.WorkflowNodeToId;// tested OK
                            rtnode2.Status = (int) RotationItem.StatusName.Open;
                            rtnode2.Value = rtnode.Value;
                            rtnode2.DateCreated = DateTime.Now;
                            db.RotationNodes.Add(rtnode2);
                            retvalues.Add(createActivityResult(rtnode2.UserId));
                        }
                    }
                    else if (nodeto.Symbol.Code.Equals("END"))
                    {
                        if (rtnode.Status.Equals((int) RotationItem.StatusName.Declined))
                            updateAllStatus(db, rtnode.Rotation.Id, (int) RotationItem.StatusName.Declined);
                        else
                            updateAllStatus(db, rtnode.Rotation.Id, (int) RotationItem.StatusName.Completed);
                    }

                }

                var result = db.SaveChanges();

                return retvalues;
            }
        }

        private bool isExistNode(RotationNode node)
        {
            using (var db = new ServiceContext())
            {
                int[] statuses = { (int)RotationItem.StatusName.Open, 
                    (int)RotationItem.StatusName.In_Progress,
                    (int)RotationItem.StatusName.Pending,
                    (int)RotationItem.StatusName.Completed };

                var ndx = db.RotationNodes.FirstOrDefault(c =>
                                    c.Rotation.Id == node.Rotation.Id &&
                                    c.WorkflowNode.Id == node.WorkflowNode.Id &&
                                    c.PrevWorkflowNodeId == node.PrevWorkflowNodeId &&
                                    c.UserId == node.UserId &&
                                    (statuses).Contains(c.Status));

                return (ndx != null);
            }
        }

        //private bool decissionValue(string srcVal, string dstVal, string oprt)
        //{
        //    List<Comparator> objs = new List<Comparator>();

        //    decimal src = 0;
        //    decimal dst = 0;
        //    bool issrcnum = decimal.TryParse(srcVal, out src);
        //    bool isdstnum = decimal.TryParse(dstVal, out dst);

        //    List<Comparator> result = null;
        //    if (issrcnum && isdstnum)
        //    {
        //        objs.Add(new Comparator(src, dst));
        //        result = (from c in objs select new Comparator { number1 = c.number1, number2 = c.number2, }).Where("number1" + oprt + "number2").ToList();
        //    }
        //    else
        //    {
        //        objs.Add(new Comparator(srcVal, dstVal));
        //        result = (from c in objs select new Comparator { value1 = c.value1, value2 = c.value2, }).Where("value1" + oprt + "value2").ToList();
        //    }
        //    return result.Count() > 0;
        //}

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
                    if (rnc.Document != null) // save annos first before set sign/initial/stamp
                        docSvr.SaveAnnos((long)rnc.Document.Id, memberId, "CALLER", rnc.Document.DocumentElements);
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
                            upload.DateCreated = DateTime.Now;
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
    }
}