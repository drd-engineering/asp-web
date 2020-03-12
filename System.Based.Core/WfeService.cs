using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;
using DRD.Domain;
using System.Configuration;
using Newtonsoft.Json;
using System.Based.Core.Entity;

namespace System.Based.Core
{
    public class WfeService
    {
        private readonly string _connString;
        private string _appZoneAccess;

        public WfeService(string appZoneAccess, string connString)
        {
            _appZoneAccess = appZoneAccess;
            _connString = connString;
        }
        public WfeService(string appZoneAccess)
        {
            _appZoneAccess = appZoneAccess;
            _connString = ConfigConstant.CONSTRING;
        }
        public WfeService()
        {
            _connString = ConfigConstant.CONSTRING;
        }
        private static DtoRotationNodeDoc DeepCopy(DtoRotationNodeDoc source)
        {

            var DeserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };

            return JsonConvert.DeserializeObject<DtoRotationNodeDoc>(JsonConvert.SerializeObject(source), DeserializeSettings);

        }

        public DtoRotation assignNodes(DrdContext db, DtoRotation rot, long memberId, IDocumentService docSvr)
        {
            DtoRotation rotation = rot;

            rotation.RotationNodes =
                (from x in db.RotationNodes
                 join s in db.StatusCodes on x.Status equals s.Code
                 where x.RotationId == rotation.Id
                 orderby x.DateCreated
                 select new DtoRotationNode
                 {
                     Id = x.Id,
                     DateCreated = x.DateCreated,
                     Status = x.Status,
                     Value = x.Value,
                     WorkflowNodeId = x.WorkflowNodeId,
                     PrevWorkflowNodeId = x.PrevWorkflowNodeId,
                     SenderRotationNodeId = x.SenderRotationNodeId,
                     MemberId = x.MemberId,
                     Member = new DtoMember
                     {
                         Id = x.Member.Id,
                         Number = x.Member.Number,
                         Name = x.Member.Name,
                         ImageProfile = x.Member.ImageProfile,
                         ImageInitials = x.Member.ImageInitials,
                         ImageSignature = x.Member.ImageSignature,
                         ImageStamp = x.Member.ImageStamp,
                         ImageKtp1 = x.Member.ImageKtp1,
                         ImageKtp2 = x.Member.ImageKtp2,
                     },
                     StatusCode = new DtoStatusCode
                     {
                         Id = s.Id,
                         Code = s.Code,
                         Descr = s.Descr,
                         Icon = s.Icon,
                         BackColor = s.BackColor,
                         TextColor = s.TextColor,
                     },
                     WorkflowNode = new DtoWorkflowNode
                     {
                         Id = x.WorkflowNode.Id,
                         Caption = x.WorkflowNode.Caption,
                         BackColor = s.BackColor,
                         TextColor = s.TextColor,
                     },
                     RotationNodeRemarks =
                     (from r in x.RotationNodeRemarks
                      select new DtoRotationNodeRemark
                      {
                          Id = r.Id,
                          Remark = r.Remark,
                          DateStamp = r.DateStamp,
                      }).ToList(),
                     //RotationNodeDocs =
                     //(from d in x.RotationNodeDocs
                     // select new DtoRotationNodeDoc
                     // {
                     //     Id = d.Id,
                     //     DocumentId = d.DocumentId,
                     //     FlagAction = d.FlagAction,

                     //     Document = new DtoDocument
                     //     {
                     //         Title = d.Document.Title,
                     //         FileNameOri = d.Document.FileNameOri,
                     //         FileName = d.Document.FileName,
                     //         ExtFile = d.Document.ExtFile,
                     //         FileSize = d.Document.FileSize,
                     //         DocumentMember =
                     //            (from dm in d.Document.DocumentMembers
                     //             where dm.MemberId == rotation.MemberId // default inbox member
                     //             select new DtoDocumentMember
                     //             {
                     //                 Id = dm.Id,
                     //                 DocumentId = dm.DocumentId,
                     //                 MemberId = dm.MemberId,
                     //                 FlagPermission = dm.FlagPermission,
                     //                 FlagAction = dm.FlagAction,
                     //             }).FirstOrDefault(),
                     //         DocumentMembers =
                     //           (from dm in d.Document.DocumentMembers
                     //            select new DtoDocumentMember
                     //            {
                     //                Id = dm.Id,
                     //                DocumentId = dm.DocumentId,
                     //                MemberId = dm.MemberId,
                     //                FlagPermission = dm.FlagPermission,
                     //                FlagAction = dm.FlagAction,
                     //            }).ToList(),
                     //         DocumentAnnotates =
                     //             (from da in d.Document.DocumentAnnotates
                     //              select new DtoDocumentAnnotate
                     //              {
                     //                  Id = da.Id,
                     //                  DocumentId = da.DocumentId,
                     //                  Page = da.Page,
                     //                  AnnotateTypeId = da.AnnotateTypeId,
                     //                  LeftPos = da.LeftPos,
                     //                  TopPos = da.TopPos,
                     //                  WidthPos = da.WidthPos,
                     //                  HeightPos = da.HeightPos,
                     //                  Color = da.Color,
                     //                  BackColor = da.BackColor,
                     //                  Data = da.Data,
                     //                  Data2 = da.Data2,
                     //                  Rotation = da.Rotation,
                     //                  ScaleX = da.ScaleX,
                     //                  ScaleY = da.ScaleY,
                     //                  TransX = da.TransX,
                     //                  TransY = da.TransY,
                     //                  StrokeWidth = da.StrokeWidth,
                     //                  Opacity = da.Opacity,
                     //                  Flag = da.Flag,
                     //                  FlagCode = da.FlagCode,
                     //                  FlagDate = da.FlagDate,
                     //                  FlagImage = da.FlagImage,
                     //                  CreatorId = da.CreatorId,
                     //                  AnnotateId = da.AnnotateId,
                     //                  UserId = da.UserId,
                     //                  DateCreated = da.DateCreated,
                     //                  DateUpdated = da.DateUpdated,
                     //                  AnnotateType = new DtoAnnotateType
                     //                  {
                     //                      Id = da.AnnotateType.Id,
                     //                      Code = da.AnnotateType.Code,
                     //                  }
                     //              }).ToList(),
                     //     }
                     // }).ToList(),
                     //RotationNodeUpDocs =
                     //    (from ud in x.RotationNodeUpDocs
                     //     select new DtoRotationNodeUpDoc
                     //     {
                     //         Id = ud.Id,
                     //         DocumentUploadId = ud.DocumentUploadId,
                     //         DocumentUpload = new DtoDocumentUpload
                     //         {
                     //             FileFlag = ud.DocumentUpload.FileFlag,
                     //             FileName = ud.DocumentUpload.FileName,
                     //             FileNameOri = ud.DocumentUpload.FileNameOri,
                     //             ExtFile = ud.DocumentUpload.ExtFile,
                     //             FileSize = ud.DocumentUpload.FileSize,
                     //             CreatorId = ud.DocumentUpload.CreatorId,
                     //             DateCreated = ud.DocumentUpload.DateCreated,
                     //         }
                     //     }).ToList(),
                 }).ToList();


            foreach (DtoRotationNode rn in rotation.RotationNodes)
            {
                // set note for waiting pending member
                if (rn.Status.Equals("02"))
                {
                    rn.Note = "Waiting for action from: ";
                    var rtpending = db.RotationNodes.FirstOrDefault(c => c.Id == rn.Id);
                    var wfnto = rtpending.WorkflowNode.WorkflowNodeLinks_WorkflowNodeId.FirstOrDefault(c => c.Symbol.Code.Equals(ConfigConstant.EnumActivityAction.SUBMIT.ToString()));

                    var nodetos = db.WorkflowNodeLinks.Where(c => c.WorkflowNodeToId == wfnto.WorkflowNodeToId && c.Symbol.Code.Equals(ConfigConstant.EnumActivityAction.SUBMIT.ToString())).ToList();
                    foreach (WorkflowNodeLink wx in nodetos)
                    {
                        if (("00,05").Contains(wx.WorkflowNode_WorkflowNodeId.RotationNodes.FirstOrDefault().Status))
                            rn.Note += wx.WorkflowNode_WorkflowNodeId.RotationMembers.FirstOrDefault(c => c.WorkflowNodeId == wx.WorkflowNodeId).Member.Name + " | " + wx.WorkflowNode_WorkflowNodeId.Caption + ", ";
                    }

                    if (rn.Note.EndsWith(", "))
                        rn.Note = rn.Note.Substring(0, rn.Note.Length - 2);

                }
                // set note for alter link
                else if (rn.Status.Equals("00"))
                {
                    var node = db.WorkflowNodeLinks.FirstOrDefault(c => c.WorkflowNodeId == rn.WorkflowNodeId && c.Symbol.Code.Equals(ConfigConstant.EnumActivityAction.ALTER.ToString()));
                    if (node != null)
                    {
                        if (node.Operator != null)
                        {
                            var start = rn.DateCreated;
                            double val = 0;
                            if (Double.TryParse(node.Value, out val))
                            {
                                if (node.Operator.Equals("HOUR"))
                                    start = start.AddHours(val);
                                else
                                    start = start.AddDays(val);
                            }

                            rn.Note = "There is a flow alter on " + start.ToString("dd/MM/yyyy HH:mm");
                        }
                        else
                            rn.Note = "There is a flow alter, there has been no determination of the period.";
                    }
                }

                ////bind doc and up doc
                //bool isSingle = true;
                //if (rn.PrevWorkflowNodeId != null)
                //{
                //    var wfn = db.WorkflowNodes.FirstOrDefault(c => c.Id == rn.PrevWorkflowNodeId);
                //    if (wfn.Symbol.Code.Equals("PARALLEL"))
                //    {
                //        isSingle = false;
                //        var wfnIds = db.WorkflowNodeLinks.Where(c => c.WorkflowNodeToId == wfn.Id).Select(c => c.WorkflowNodeId).ToArray();
                //        var rns = db.RotationNodes.Where(c => wfnIds.Contains(c.WorkflowNodeId)).ToList();
                //        foreach (RotationNode rnx in rns)
                //        {
                //            var rn2 = new DtoRotationNode();
                //            rn2.Id = rnx.Id;
                //            rn2.MemberId = rnx.MemberId;
                //            //rn.RotationNodeDocs = assignNodeDocs(db, rn2);
                //            //rn.RotationNodeUpDocs = assignNodeUpDocs(db, rn2);
                //        }
                //    }
                //}

                //if (isSingle)
                //{
                //    rn.RotationNodeDocs = assignNodeDocs(db, rn);
                //    rn.RotationNodeUpDocs = assignNodeUpDocs(db, rn);
                //}

                if (rn.Id == rotation.RotationNodeId && rotation.MemberId == rn.MemberId && rn.Status.Equals("00") && (rn.PrevWorkflowNodeId != null || rn.SenderRotationNodeId != null))
                {
                    if (rn.PrevWorkflowNodeId != null)
                    {
                        var wfn = db.WorkflowNodes.FirstOrDefault(c => c.Id == rn.PrevWorkflowNodeId);
                        if (wfn.Symbol.Code.Equals("PARALLEL"))
                        {
                            var wfnIds = db.WorkflowNodeLinks.Where(c => c.WorkflowNodeToId == wfn.Id).Select(c => c.WorkflowNodeId).ToArray();
                            var rns = db.RotationNodes.Where(c => wfnIds.Contains(c.WorkflowNodeId)).ToList();
                            List<DtoRotationNodeDoc> listDoc = new List<DtoRotationNodeDoc>();
                            List<DtoRotationNodeUpDoc> listUpDoc = new List<DtoRotationNodeUpDoc>();
                            foreach (RotationNode rnx in rns)
                            {
                                var d = assignNodeDocs(db, rnx.Id, memberId/*rotation.MemberId*/, rot.RotationNodeId, docSvr);
                                if (d.Count > 0)
                                    listDoc.AddRange(d);

                                var ud = assignNodeUpDocs(db, rnx.Id);
                                if (ud.Count > 0)
                                    listUpDoc.AddRange(ud);

                            }
                            if (listDoc.Count > 0)
                                rn.RotationNodeDocs = listDoc;
                            if (listUpDoc.Count > 0)
                                rn.RotationNodeUpDocs = listUpDoc;
                        }
                    }
                    else
                    {

                    }
                }
                else
                {
                    //rn.RotationNodeDocs = assignNodeDocs(db, rn.Id, rotation.MemberId, rot.RotationNodeId);
                    rn.RotationNodeDocs = assignNodeDocs(db, rn.Id, memberId/*rn.MemberId*/, rot.RotationNodeId, docSvr);
                    rn.RotationNodeUpDocs = assignNodeUpDocs(db, rn.Id);

                }

                //document summaries document
                foreach (DtoRotationNodeDoc doc in rn.RotationNodeDocs)
                {
                    // get anno
                    foreach (DtoDocumentAnnotate da in doc.Document.DocumentAnnotates)
                    {
                        if (da.AnnotateId == null || da.AnnotateId == 0) continue;
                        if (da.AnnotateType.Code.Equals("SIGNATURE") || da.AnnotateType.Code.Equals("INITIAL") || da.AnnotateType.Code.Equals("PRIVATESTAMP"))
                        {
                            var mem = db.Members.FirstOrDefault(c => c.Id == da.AnnotateId);
                            da.Annotate.Number = mem.Number;
                            da.Annotate.Name = mem.Name;
                            da.Annotate.Foto = mem.ImageProfile;
                        }
                        else if (da.AnnotateType.Code.Equals("STAMP"))
                        {
                            var stmp = db.Stamps.FirstOrDefault(c => c.Id == da.AnnotateId);
                            da.Annotate.Name = stmp.Descr;
                            da.Annotate.Foto = stmp.StampFile;
                        }
                    }

                    var dx = rotation.SumRotationNodeDocs.FirstOrDefault(c => c.DocumentId == doc.DocumentId);
                    if (dx != null)
                    {
                        dx.FlagAction |= doc.FlagAction;
                    }
                    else {
                        rotation.SumRotationNodeDocs.Add(DeepCopy(doc));
                    }

                }
                //attchment summaries 
                foreach (DtoRotationNodeUpDoc doc in rn.RotationNodeUpDocs)
                {
                    var dx = rotation.SumRotationNodeUpDocs.FirstOrDefault(c => c.DocumentUpload.FileName.Equals(doc.DocumentUpload.FileName));
                    if (dx == null)
                        rotation.SumRotationNodeUpDocs.Add(doc);
                }
            }


            return rotation;
        }

        private List<DtoRotationNodeDoc> assignNodeDocs(DrdContext db, long rnId, long memId, long curRnId, IDocumentService docSvr)
        {
            //rn.RotationNodeDocs =
            var result =
                (from d in db.RotationNodeDocs
                 where d.RotationNodeId == rnId
                 select new DtoRotationNodeDoc
                 {
                     Id = d.Id,
                     DocumentId = d.DocumentId,
                     FlagAction = d.FlagAction,
                     RotationNode = new DtoRotationNode
                     {
                         RotationId = d.RotationNode.RotationId,
                     },
                     Document = new DtoDocument
                     {
                         Title = d.Document.Title,
                         FileNameOri = d.Document.FileNameOri,
                         FileName = d.Document.FileName,
                         ExtFile = d.Document.ExtFile,
                         FileSize = d.Document.FileSize,
                         DocumentMember =
                             (from dm in d.Document.DocumentMembers
                              where dm.MemberId == memId // default inbox member
                              select new DtoDocumentMember
                              {
                                  Id = dm.Id,
                                  DocumentId = dm.DocumentId,
                                  MemberId = dm.MemberId,
                                  //FlagPermission = dm.FlagPermission,
                                  FlagAction = dm.FlagAction,
                              }).FirstOrDefault(),
                         //DocumentMembers =
                         //(from dm in d.Document.DocumentMembers
                         // select new DtoDocumentMember
                         // {
                         //     Id = dm.Id,
                         //     DocumentId = dm.DocumentId,
                         //     MemberId = dm.MemberId,
                         //     //FlagPermission = dm.FlagPermission,
                         //     FlagAction = dm.FlagAction,
                         // }).ToList(),
                         DocumentAnnotates =
                             (from da in d.Document.DocumentAnnotates
                              select new DtoDocumentAnnotate
                              {
                                  Id = da.Id,
                                  DocumentId = da.DocumentId,
                                  Page = da.Page,
                                  AnnotateTypeId = da.AnnotateTypeId,
                                  LeftPos = da.LeftPos,
                                  TopPos = da.TopPos,
                                  WidthPos = da.WidthPos,
                                  HeightPos = da.HeightPos,
                                  Color = da.Color,
                                  BackColor = da.BackColor,
                                  Data = da.Data,
                                  Data2 = da.Data2,
                                  Rotation = da.Rotation,
                                  ScaleX = da.ScaleX,
                                  ScaleY = da.ScaleY,
                                  TransX = da.TransX,
                                  TransY = da.TransY,
                                  StrokeWidth = da.StrokeWidth,
                                  Opacity = da.Opacity,
                                  Flag = da.Flag,
                                  FlagCode = da.FlagCode,
                                  FlagDate = da.FlagDate,
                                  FlagImage = da.FlagImage,
                                  CreatorId = da.CreatorId,
                                  AnnotateId = da.AnnotateId,
                                  UserId = da.UserId,
                                  DateCreated = da.DateCreated,
                                  DateUpdated = da.DateUpdated,
                                  AnnotateType = new DtoAnnotateType
                                  {
                                      Id = da.AnnotateType.Id,
                                      Code = da.AnnotateType.Code,
                                  }
                              }).ToList(),
                     }
                 }).ToList();

            if (result != null /*&& curRnId != 0*/)
            {
                // assign permission
                //DocumentService docSvr = new DocumentService();
                foreach (DtoRotationNodeDoc rnd in result)
                {
                    if (rnd.Document.DocumentMember == null)
                    {
                        rnd.Document.DocumentMember = new DtoDocumentMember();
                        rnd.Document.DocumentMember.MemberId = memId;
                        rnd.Document.DocumentMember.DocumentId = (long)rnd.DocumentId;
                    }
                    if (curRnId == 0)
                        curRnId = -rnd.RotationNode.RotationId;
                    rnd.Document.DocumentMember.FlagPermission = docSvr.GetPermission(memId, curRnId, (long)rnd.DocumentId);
                }
            }

            return result;// rn.RotationNodeDocs;
        }
        private List<DtoRotationNodeUpDoc> assignNodeUpDocs(DrdContext db, long rnId)
        {
            //rn.RotationNodeUpDocs =
            var result =
                (from ud in db.RotationNodeUpDocs
                 where ud.RotationNodeId == rnId
                 select new DtoRotationNodeUpDoc
                 {
                     Id = ud.Id,
                     DocumentUploadId = ud.DocumentUploadId,
                     DocumentUpload = new DtoDocumentUpload
                     {
                         FileFlag = ud.DocumentUpload.FileFlag,
                         FileName = ud.DocumentUpload.FileName,
                         FileNameOri = ud.DocumentUpload.FileNameOri,
                         ExtFile = ud.DocumentUpload.ExtFile,
                         FileSize = ud.DocumentUpload.FileSize,
                         CreatorId = ud.DocumentUpload.CreatorId,
                         DateCreated = ud.DocumentUpload.DateCreated,
                     }
                 }).ToList();

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prod"></param>
        /// <returns></returns>
        public long Save(DtoRotation prod)
        {
            Rotation product;
            using (var db = new DrdContext(_connString))
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
                else {
                    //if (!string.IsNullOrEmpty(prod.Code))
                    //{
                    //    var cxmember = db.Rotations.Count(c => c.Code == prod.Code);
                    //    if (cxmember > 0)
                    //        return -1;
                    //}

                    product = new Rotation();
                }
                product.Subject = prod.Subject;
                product.WorkflowId = prod.WorkflowId;
                product.Remark = prod.Remark;
                product.Status = prod.Status;
                product.CreatorId = prod.CreatorId;
                product.MemberId = prod.MemberId;
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
                var cxold = db.RotationMembers.Count(c => c.RotationId == product.Id);
                var cxnew = prod.RotationMembers.Count();
                if (cxold < cxnew)
                {
                    var ep = prod.RotationMembers.ElementAt(0); // get 1 data for sample
                    for (var x = cxold; x < cxnew; x++)
                    {
                        RotationMember aii = new RotationMember();
                        aii.RotationId = product.Id;
                        aii.WorkflowNodeId = ep.WorkflowNodeId;
                        aii.MemberId = ep.MemberId;
                        aii.FlagPermission = ep.FlagPermission;
                        //aii.FlagAction = ep.FlagAction;
                        //aii.CxDownload = ep.CxDownload;
                        //aii.CxPrint = ep.CxPrint;

                        db.RotationMembers.Add(aii);
                    }
                    db.SaveChanges();
                }
                else if (cxold > cxnew)
                {
                    var dremove = db.RotationMembers.Where(c => c.RotationId == product.Id).Take(cxold - cxnew).ToList();
                    db.RotationMembers.RemoveRange(dremove);
                    db.SaveChanges();
                }

                // save detail
                var dnew = db.RotationMembers.Where(c => c.RotationId == product.Id).ToList();
                int v = 0;
                foreach (RotationMember d in dnew)
                {
                    var epos = prod.RotationMembers.ElementAt(v);
                    d.WorkflowNodeId = epos.WorkflowNodeId;
                    d.MemberId = epos.MemberId;
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

        private JsonActivityResult createActivityResult(long memberId, int exitCode = 1)
        {
            using (var db = new DrdContext(_connString))
            {
                JsonActivityResult ret = new JsonActivityResult();
                var mem = db.Members.FirstOrDefault(c => c.Id == memberId);

                ret = new JsonActivityResult();
                ret.ExitCode = exitCode;
                ret.Email = mem.Email;
                ret.MemberId = memberId;
                ret.MemberName = mem.Name;
                return ret;
            }
        }

        private JsonActivityResult createActivityResult(int exitCode)
        {
            JsonActivityResult ret = new JsonActivityResult();
            ret.ExitCode = exitCode;
            return ret;
        }

        public List<JsonActivityResult> Start(long id, IMemberDepositTrxService mdtSvr)
        {
            List<JsonActivityResult> retvalues = new List<JsonActivityResult>();

            using (var db = new DrdContext(_connString))
            {
                var rt = db.Rotations.FirstOrDefault(c => c.Id == id);
                if (!rt.Status.Equals("00"))
                {
                    retvalues.Add(createActivityResult(-1));
                    return retvalues; //Invalid rotation
                }

                var plan = db.MemberPlans.FirstOrDefault(c => c.MemberId == rt.MemberId && c.IsDefault);
                if (plan == null)
                {
                    retvalues.Add(createActivityResult(-2));
                    return retvalues; //Invalid member plan
                }

                if (plan.ValidPackage < DateTime.Now)
                {
                    retvalues.Add(createActivityResult(-3));
                    return retvalues; //expired package
                }

                if (plan.RotationCount + plan.RotationCountAdd - plan.RotationCountUsed <= 0)
                {
                    retvalues.Add(createActivityResult(-4));
                    return retvalues; //The used rotation exceeds the data packet quota number
                }

                //save usage rotation 
                plan.RotationCountUsed++;
                //update rotation
                rt.Status = "01";
                rt.Price = plan.RotationPrice;
                rt.DateUpdated = DateTime.Now;
                rt.DateStatus = DateTime.Now;
                rt.DateStarted = DateTime.Now;
                var result = db.SaveChanges();

                // first node, node after start symbol
                var wfstarts = db.WorkflowNodeLinks.Where(c => c.WorkflowNode_WorkflowNodeId.WorkflowId == rt.WorkflowId && c.WorkflowNode_WorkflowNodeId.Symbol.Code.Equals("START")).ToList();
                if (wfstarts.Count() == 0)
                {
                    retvalues.Add(createActivityResult(-1));
                    return retvalues; //Invalid rotation
                }

                // send to all activity under start node 
                foreach (WorkflowNodeLink wfnl in wfstarts)
                {
                    RotationNode rtnode = new RotationNode();
                    rtnode.RotationId = id;
                    rtnode.WorkflowNodeId = wfnl.WorkflowNodeToId;
                    rtnode.SenderRotationNodeId = null;
                    rtnode.MemberId = (long)wfnl.WorkflowNode_WorkflowNodeToId.RotationMembers.FirstOrDefault(c => c.WorkflowNodeId == wfnl.WorkflowNodeToId && c.RotationId == id).MemberId;
                    rtnode.Status = "00";
                    rtnode.Value = "";
                    rtnode.DateCreated = DateTime.Now;
                    db.RotationNodes.Add(rtnode);

                    retvalues.Add(createActivityResult(rtnode.MemberId));
                }
                db.SaveChanges();

                return retvalues;

            }

        }

        public List<JsonActivityResult> ProcessActivity(JsonProcessActivity param, ConfigConstant.EnumActivityAction bit, IDocumentService docSvr, IMemberDepositTrxService mdtSvr)
        {
            List<JsonActivityResult> retvalues = new List<JsonActivityResult>();

            using (var db = new DrdContext(_connString))
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
                    long senderMemberId = senderRotNode.MemberId;
                    long senderWfNodeId = senderRotNode.WorkflowNodeId;
                    long? prevWfNodeId = rtnode.PrevWorkflowNodeId;
                    // get transfer node
                    var prevWfNode = db.WorkflowNodes.FirstOrDefault(c => c.Id == prevWfNodeId);
                    if (prevWfNode.Symbol.Code.Equals("TRANSFER"))
                    {
                        decimal transfer = decimal.Parse(prevWfNode.Value);
                        if (transfer > 0)
                        {
                            //MemberDepositTrxService mdtSvr = new MemberDepositTrxService();
                            var balance = mdtSvr.GetDepositBalance(senderMemberId);
                            if (balance < transfer)
                            {
                                retvalues.Add(createActivityResult(-1));
                                return retvalues;
                            }

                            // potong deposit
                            var nxd = db.RotationNodes.FirstOrDefault(c => c.Id == senderNodeId);
                            DtoMemberDepositTrx trx = new DtoMemberDepositTrx();
                            var idxNo = "00000" + senderNodeId.ToString();
                            trx.TrxNo = "TRF" + DateTime.Today.ToString("yyMMddfff") + "-" + idxNo.Substring(idxNo.Length - 6, 6);
                            trx.TrxDate = DateTime.Today;
                            trx.TrxType = "NODETRF";
                            trx.TrxId = senderNodeId;
                            trx.Descr = "Funds transfer (" + rtnode.Rotation.Subject + "\\" + nxd.WorkflowNode.Caption + ":to " + rtnode.Member.Name + ")";
                            trx.MemberId = senderMemberId;
                            trx.Amount = transfer;
                            trx.DbCr = 1;
                            mdtSvr.Save(trx);

                            // nambah deposit
                            trx = new DtoMemberDepositTrx();
                            idxNo = "00000" + rtnode.Id.ToString();
                            trx.TrxNo = "RCP" + DateTime.Today.ToString("yyMMddfff") + "-" + idxNo.Substring(idxNo.Length - 6, 6);
                            trx.TrxDate = DateTime.Today;
                            trx.TrxType = "NODERCP";
                            trx.TrxId = rtnode.Id;
                            trx.Descr = "Fund receive (" + rtnode.Rotation.Subject + "\\" + rtnode.WorkflowNode.Caption + ":from " + nxd.Member.Name + ")";
                            trx.MemberId = rtnode.MemberId;
                            trx.Amount = transfer;
                            trx.DbCr = 0;
                            mdtSvr.Save(trx);
                        }
                    }
                }
                #endregion penerima transfer

                //set last node to in progress
                rtnode.Status = "01"; // currrent is submit
                rtnode.Value = param.Value;
                rtnode.DateUpdated = DateTime.Now;
                rtnode.Rotation.DateStatus = DateTime.Now;
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
                    rtnode.Status = "05";
                else if (strbit.Equals("ALTER"))
                    rtnode.Status = "06";
                else if (strbit.Equals("REJECT"))
                    rtnode.Status = "98";

                //TODO SymbolCode == 20 is only for submit activity. Make a universal identifier
                var wfnodes = db.WorkflowNodeLinks.Where(c => c.WorkflowNodeId == rtnode.WorkflowNodeId && c.SymbolCode == 20).ToList();
                List<RotationNode> rotnodes = new List<RotationNode>();
                foreach (WorkflowNodeLink wfnl in wfnodes)
                {
                    var nodeto = wfnl.WorkflowNode_WorkflowNodeToId;
                    if (nodeto.Symbol.Code.Equals("ACTIVITY"))
                    {

                        RotationNode rtnode2 = new RotationNode();

                        rtnode2.RotationId = rtnode.RotationId;
                        rtnode2.WorkflowNodeId = wfnl.WorkflowNodeToId;
                        rtnode2.SenderRotationNodeId = rtnode.Id;
                        //rtnode2.MemberId = (long)wfnl.WorkflowNode_WorkflowNodeToId.MemberId;
                        rtnode2.MemberId = (long)wfnl.WorkflowNode_WorkflowNodeToId.RotationMembers.FirstOrDefault(c => c.WorkflowNodeId == wfnl.WorkflowNodeToId && c.RotationId == rtnode.RotationId).MemberId;
                        rtnode2.PrevWorkflowNodeId = wfnl.WorkflowNodeId;// tested OK
                        rtnode2.Status = "00";
                        rtnode2.Value = rtnode.Value;
                        rtnode2.DateCreated = DateTime.Now;
                        db.RotationNodes.Add(rtnode2);
                        retvalues.Add(createActivityResult(rtnode2.MemberId));
                    }
                    else if (nodeto.Symbol.Code.Equals("PARALLEL"))
                    {
                        bool ispending = false;
                        foreach (WorkflowNodeLink wfnlx in wfnl.WorkflowNode_WorkflowNodeToId.WorkflowNodeLinks_WorkflowNodeToId)
                        {
                            var rotnode = db.RotationNodes.FirstOrDefault(c => c.WorkflowNodeId == wfnlx.WorkflowNodeId);
                            if (rotnode.WorkflowNodeId != rtnode.WorkflowNodeId)
                            {
                                //var isready02 = false;
                                //RotationNode retnode02 = null;
                                if (rotnode.Status.Equals("05"))
                                {
                                    // checking apakah 05 sudah menjadi 02 di id lain
                                    var retnode02 = db.RotationNodes.FirstOrDefault(c =>
                                                     c.RotationId == rotnode.RotationId &&
                                                     c.MemberId == rotnode.MemberId &&
                                                     c.PrevWorkflowNodeId == rotnode.PrevWorkflowNodeId &&
                                                     c.WorkflowNodeId == wfnlx.WorkflowNodeId &&
                                                     c.Status.Equals("02"));
                                    //isready02 = (retnode02 != null);
                                    if (retnode02 != null) rotnode = retnode02;
                                }

                                if (("00,05").Contains(rotnode.Status))
                                {
                                    ispending = true;
                                    break;
                                }
                                else if (rotnode.Status.Equals("02"))
                                {
                                    rotnodes.Add(rotnode);
                                }
                            }
                        }
                        if (ispending)
                        {
                            rtnode.Status = "02";
                            continue;
                        }
                        else
                        {
                            // ubah semua status jadi 01, yang pending (02) menjadi in progress
                            foreach (RotationNode rotn in rotnodes)
                            {
                                rotn.Status = "01";
                            }

                            foreach (WorkflowNodeLink lnk in nodeto.WorkflowNodeLinks_WorkflowNodeId)
                            {
                                RotationNode rtnode2 = new RotationNode();

                                rtnode2.RotationId = rtnode.RotationId;
                                rtnode2.WorkflowNodeId = lnk.WorkflowNodeToId;
                                rtnode2.SenderRotationNodeId = rtnode.Id;
                                //rtnode2.MemberId = (long)lnk.WorkflowNode_WorkflowNodeToId.MemberId;
                                rtnode2.MemberId = (long)lnk.WorkflowNode_WorkflowNodeToId.RotationMembers.FirstOrDefault(c => c.WorkflowNodeId == lnk.WorkflowNodeToId && c.RotationId == rtnode.RotationId).MemberId;
                                rtnode2.PrevWorkflowNodeId = wfnl.WorkflowNodeToId; // tested OK
                                rtnode2.Status = "00";
                                rtnode2.Value = rtnode.Value;
                                rtnode2.DateCreated = DateTime.Now;
                                db.RotationNodes.Add(rtnode2);
                                retvalues.Add(createActivityResult(rtnode2.MemberId));
                            }
                        }
                    }
                    else if (nodeto.Symbol.Code.Equals("DECISION"))
                    {
                        WorkflowNodeLink nodeToNext;
                        if (decissionValue(rtnode.Value, nodeto.Value, nodeto.Operator))
                            nodeToNext = nodeto.WorkflowNodeLinks_WorkflowNodeId.FirstOrDefault(c => c.Symbol.Code.Equals("YES"));
                        else
                            nodeToNext = nodeto.WorkflowNodeLinks_WorkflowNodeId.FirstOrDefault(c => c.Symbol.Code.Equals("NO"));

                        RotationNode rtnode2 = new RotationNode();

                        rtnode2.RotationId = rtnode.RotationId;
                        rtnode2.WorkflowNodeId = nodeToNext.WorkflowNodeToId;
                        rtnode2.SenderRotationNodeId = rtnode.Id;
                        //rtnode2.MemberId = (long)nodeToNext.WorkflowNode_WorkflowNodeToId.MemberId;
                        rtnode2.MemberId = (long)nodeToNext.WorkflowNode_WorkflowNodeToId.RotationMembers.FirstOrDefault(c => c.WorkflowNodeId == nodeToNext.WorkflowNodeToId && c.RotationId == rtnode.RotationId).MemberId;
                        rtnode2.PrevWorkflowNodeId = wfnl.WorkflowNodeId;//tested OK
                        rtnode2.Status = "00";
                        rtnode2.Value = rtnode.Value;
                        rtnode2.DateCreated = DateTime.Now;
                        db.RotationNodes.Add(rtnode2);
                        retvalues.Add(createActivityResult(rtnode2.MemberId));
                    }
                    else if (nodeto.Symbol.Code.Equals("TRANSFER"))
                    {
                        WorkflowNodeLink nodeToNext;
                        nodeToNext = nodeto.WorkflowNodeLinks_WorkflowNodeId.FirstOrDefault();

                        RotationNode rtnode2 = new RotationNode();

                        rtnode2.RotationId = rtnode.RotationId;
                        rtnode2.WorkflowNodeId = nodeToNext.WorkflowNodeToId;
                        rtnode2.SenderRotationNodeId = rtnode.Id;
                        //rtnode2.MemberId = (long)nodeToNext.WorkflowNode_WorkflowNodeToId.MemberId;
                        rtnode2.MemberId = (long)nodeToNext.WorkflowNode_WorkflowNodeToId.RotationMembers.FirstOrDefault(c => c.WorkflowNodeId == nodeToNext.WorkflowNodeToId && c.RotationId == rtnode.RotationId).MemberId;
                        rtnode2.PrevWorkflowNodeId = wfnl.WorkflowNodeToId;// tested OK
                        rtnode2.Status = "00";
                        rtnode2.Value = rtnode.Value;// "TRF:" + nodeto.Value + ",ID:" + rtnode.Id + ",MEMBER:" + rtnode.MemberId;
                        rtnode2.DateCreated = DateTime.Now;

                        // check for double rotation node
                        if (!isExistNode(rtnode2))
                        {
                            db.RotationNodes.Add(rtnode2);
                            retvalues.Add(createActivityResult(rtnode2.MemberId));
                        }
                    }
                    else if (nodeto.Symbol.Code.Equals("CASE"))
                    {
                        WorkflowNodeLink nodeToNext = null; ;
                        WorkflowNodeLink elseLink = null;
                        foreach (WorkflowNodeLink p in nodeto.WorkflowNodeLinks_WorkflowNodeId)
                        {
                            if (!p.Operator.Equals("ELSE"))
                            {
                                if (decissionValue(rtnode.Value, p.Value, p.Operator))
                                {
                                    nodeToNext = p;
                                    break;
                                }
                            }
                            else
                                elseLink = p;
                        }
                        if (nodeToNext == null && elseLink != null)
                            nodeToNext = elseLink;

                        if (nodeToNext != null)
                        {
                            RotationNode rtnode2 = new RotationNode();

                            rtnode2.RotationId = rtnode.RotationId;
                            rtnode2.WorkflowNodeId = nodeToNext.WorkflowNodeToId;
                            rtnode2.SenderRotationNodeId = rtnode.Id;
                            //rtnode2.MemberId = (long)nodeToNext.WorkflowNode_WorkflowNodeToId.MemberId;
                            rtnode2.MemberId = (long)nodeToNext.WorkflowNode_WorkflowNodeToId.RotationMembers.FirstOrDefault(c => c.WorkflowNodeId == nodeToNext.WorkflowNodeToId && c.RotationId == rtnode.RotationId).MemberId;
                            rtnode2.PrevWorkflowNodeId = wfnl.WorkflowNodeToId;// tested OK
                            rtnode2.Status = "00";
                            rtnode2.Value = rtnode.Value;
                            rtnode2.DateCreated = DateTime.Now;
                            db.RotationNodes.Add(rtnode2);
                            retvalues.Add(createActivityResult(rtnode2.MemberId));
                        }
                    }
                    else if (nodeto.Symbol.Code.Equals("END"))
                    {
                        if (rtnode.Status.Equals("98"))
                            updateAllStatus(db, rtnode.RotationId, "98");
                        else
                            updateAllStatus(db, rtnode.RotationId, "90");
                    }

                }

                var result = db.SaveChanges();

                return retvalues;
            }
        }

        private bool isExistNode(RotationNode node)
        {
            using (var db = new DrdContext(_connString))
            {
                var ndx = db.RotationNodes.FirstOrDefault(c =>
                                    c.RotationId == node.RotationId &&
                                    c.WorkflowNodeId == node.WorkflowNodeId &&
                                    c.PrevWorkflowNodeId == node.PrevWorkflowNodeId &&
                                    c.MemberId == node.MemberId &&
                                    ("00,01,02,90").Contains(c.Status));

                return (ndx != null);
            }
        }

        private bool decissionValue(string srcVal, string dstVal, string oprt)
        {
            List<JsonCompareObject> objs = new List<JsonCompareObject>();

            decimal src = 0;
            decimal dst = 0;
            bool issrcnum = decimal.TryParse(srcVal, out src);
            bool isdstnum = decimal.TryParse(dstVal, out dst);

            List<JsonCompareObject> result = null;
            if (issrcnum && isdstnum)
            {
                objs.Add(new JsonCompareObject(src, dst));
                result = (from c in objs select new JsonCompareObject { number1 = c.number1, number2 = c.number2, }).Where("number1" + oprt + "number2").ToList();
            }
            else {
                objs.Add(new JsonCompareObject(srcVal, dstVal));
                result = (from c in objs select new JsonCompareObject { value1 = c.value1, value2 = c.value2, }).Where("value1" + oprt + "value2").ToList();
            }
            return result.Count() > 0;
        }

        private void updateAllStatus(DrdContext db, long rotationId, string status)
        {
            var rot = db.Rotations.FirstOrDefault(c => c.Id == rotationId);
            rot.Status = status;
            rot.DateStatus = DateTime.Now;
            foreach (RotationNode rn in rot.RotationNodes)
            {
                rn.Status = status;
            }
        }

        private void insertDoc(IEnumerable<DtoRotationNodeDoc> docs, DrdContext db, ref RotationNode rn, IDocumentService docSvr)
        {
            if (docs != null && docs.Count() > 0)
            {
                var distincts =
                    (from c in docs
                     select new DtoRotationNodeDoc
                     {
                         DocumentId = c.DocumentId,
                         FlagAction = c.FlagAction,
                     }).ToList();

                foreach (DtoRotationNodeDoc rnc in docs)
                {
                    RotationNodeDoc dx = new RotationNodeDoc();
                    dx.DocumentId = rnc.DocumentId;
                    dx.FlagAction = rnc.FlagAction;
                    rn.RotationNodeDocs.Add(dx);

                    // update flag action di master doc member
                    var memberId = rn.MemberId;
                    var docm = db.DocumentMembers.FirstOrDefault(c => c.DocumentId == rnc.DocumentId && c.MemberId == memberId);
                    if (docm != null)
                        docm.FlagAction = rnc.FlagAction;
                    else
                    {
                        DocumentMember docmem = new DocumentMember();
                        docmem.DocumentId = (long)rnc.DocumentId;
                        docmem.MemberId = memberId;
                        docmem.FlagAction = rnc.FlagAction;
                        db.DocumentMembers.Add(docmem);
                    }

                    //DocumentService docSvr = new DocumentService();
                    if (rnc.Document != null) // save annos first before set sign/initial/stamp
                        docSvr.SaveAnnos((long)rnc.DocumentId, memberId, "CALLER", rnc.Document.DocumentAnnotates);
                    if ((rnc.FlagAction & (int)ConfigConstant.EnumDocumentAction.SIGN) == (int)ConfigConstant.EnumDocumentAction.SIGN)
                        docSvr.Signature((long)rnc.DocumentId, memberId, rn.RotationId);
                    if ((rnc.FlagAction & (int)ConfigConstant.EnumDocumentAction.PRIVATESTAMP) == (int)ConfigConstant.EnumDocumentAction.PRIVATESTAMP)
                        docSvr.Stamp((long)rnc.DocumentId, memberId, rn.RotationId);
                }
            }
        }

        private void insertUpDoc(IEnumerable<DtoRotationNodeUpDoc> docs, ref RotationNode rn)
        {
            if (docs != null && docs.Count() > 0)
            {
                foreach (DtoRotationNodeUpDoc rnc in docs)
                {
                    if (rnc.DocumentUploadId == null)
                    {
                        using (var db = new DrdContext(_connString))
                        {
                            DocumentUpload upload = new DocumentUpload();
                            upload.FileName = rnc.DocumentUpload.FileName;
                            upload.FileNameOri = rnc.DocumentUpload.FileNameOri;
                            upload.ExtFile = rnc.DocumentUpload.ExtFile;
                            upload.FileFlag = rnc.DocumentUpload.FileFlag;
                            upload.FileSize = rnc.DocumentUpload.FileSize;
                            upload.CreatorId = rn.MemberId;
                            upload.DateCreated = DateTime.Now;
                            db.DocumentUploads.Add(upload);
                            db.SaveChanges();
                            rnc.DocumentUploadId = upload.Id;
                        }
                    }
                    RotationNodeUpDoc dx = new RotationNodeUpDoc();
                    dx.DocumentUploadId = rnc.DocumentUploadId;
                    rn.RotationNodeUpDocs.Add(dx);
                }
            }
        }
    }
}
