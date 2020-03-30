using System;
using System.Collections.Generic;
using System.Linq;

using DRD.Models;
using DRD.Models.View;
using DRD.Models.API;
using DRD.Models.Custom;

using DRD.Service.Context;

using Newtonsoft.Json;


namespace DRD.Service
{
    class WorkflowDeepService
    {


        
        private static RotationNodeDoc DeepCopy(RotationNodeDoc source)
        {

            var DeserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };

            return JsonConvert.DeserializeObject<RotationNodeDoc>(JsonConvert.SerializeObject(source), DeserializeSettings);

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
                         //DocumentUser =
                         //    (from dm in d.Document.DocumentUsers
                         //     where dm.UserId == memId // default inbox member
                         //     select new DocumentUser
                         //     {
                         //         Id = dm.Id,
                         //         DocumentId = dm.DocumentId,
                         //         UserId = dm.UserId,
                         //         FlagAction = dm.FlagAction,
                         //     }).FirstOrDefault(),
                         
                         DocumentElements =
                             (from documentElement in d.Document.DocumentElements
                              select new DocumentElement
                              {
                                  Id = documentElement.Id,
                                  Document = documentElement.Document,
                                  Page = documentElement.Page,
                                  LeftPosition = documentElement.LeftPosition,
                                  TopPosition = documentElement.TopPosition,
                                  WidthPosition = documentElement.WidthPosition,
                                  HeightPosition = documentElement.HeightPosition,
                                  Color = documentElement.Color,
                                  BackColor = documentElement.BackColor,
                                  Data = documentElement.Data,
                                  Data2 = documentElement.Data2,
                                  Rotation = documentElement.Rotation,
                                  ScaleX = documentElement.ScaleX,
                                  ScaleY = documentElement.ScaleY,
                                  TransitionX = documentElement.TransitionX,
                                  TransitionY = documentElement.TransitionY,
                                  StrokeWidth = documentElement.StrokeWidth,
                                  Opacity = documentElement.Opacity,
                                  Flag = documentElement.Flag,
                                  FlagCode = documentElement.FlagCode,
                                  FlagDate = documentElement.FlagDate,
                                  FlagImage = documentElement.FlagImage,
                                  CreatorId = documentElement.CreatorId,
                                  ElementId = documentElement.ElementId,
                                  UserId = documentElement.UserId,
                                  CreatedAt = documentElement.CreatedAt,
                                  UpdatedAt = documentElement.UpdatedAt,
                                  ElementTypeId = documentElement.ElementTypeId
                              }).ToList(),
                     }
                 }).ToList();

            if (result != null /*&& curRnId != 0*/)
            {
                // assign permission
                //DocumentService docSvr = new DocumentService();
                foreach (RotationNodeDoc rnd in result)
                {
                    //if (rnd.Document.DocumentUser == null)
                    //{
                    //    rnd.Document.DocumentUser = new DocumentUser();
                    //    rnd.Document.DocumentUser.UserId = memId;
                    //    rnd.Document.DocumentUser.DocumentId = (long)rnd.Document.Id;
                    //}
                    //if (curRnId == 0)
                    //    curRnId = -rnd.RotationNode.Rotation.Id;
                    //rnd.Document.DocumentUser.FlagPermission = docSvr.GetPermission(memId, curRnId, (long)rnd.Document.Id);
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
        public long Save(RotationItem prod)
        {
            Rotation product;
            using (var db = new ServiceContext())
            {
                System.Diagnostics.Debug.WriteLine("seek for rotation");
                if (prod.Id != 0)
                {
                    product = db.Rotations.FirstOrDefault(c => c.Id == prod.Id);
                    System.Diagnostics.Debug.WriteLine("seek for rotation "+product.Id);
                }
                else
                {
                    product = new Rotation();
                }
                product.Subject = prod.Subject;
                Workflow workflowitem = db.Workflows.FirstOrDefault(w => w.Id == prod.WorkflowId);
                product.Workflow = workflowitem;
                product.Remark = prod.Remark;
                product.Status = prod.Status;
                product.CreatorId = prod.CreatorId;
                product.UserId = prod.UserId;
                if (prod.Id == 0)
                {
                    product.DateCreated = DateTime.Now;
                    db.Rotations.Add(product);
                    workflowitem.TotalUsed = workflowitem.TotalUsed + 1;
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
                    bool startPersonAded = false;
                    // save detail
                    for (var x = 0; x < cxnew; x++)
                    {
                        var ep = prod.RotationUsers.ElementAt(x); // get the data
                        System.Diagnostics.Debug.WriteLine(ep);
                        var newItem = new RotationUser();
                        newItem.Rotation = product;
                        newItem.WorkflowNodeId = ep.Id;
                        var wfl = db.WorkflowNodes.FirstOrDefault(c => c.Id == ep.WorkflowNodeId);
                        
                        if (!startPersonAded)
                        {
                            var checkIsStartNode = (from  workflowNode in db.WorkflowNodes
                                                    join wfndLink in db.WorkflowNodeLinks on workflowNode.Id equals wfndLink.WorkflowNodeId
                                                    where workflowNode.WorkflowId == wfl.WorkflowId
                                                    && workflowNode.SymbolCode == 0
                                                    && wfndLink.WorkflowNodeToId == ep.WorkflowNodeId //this node is a target node from start Node
                                                    select new
                                                    {
                                                        isFound = true
                                                    }).ToList();
                            System.Diagnostics.Debug.WriteLine("CHECK START NODE :: " + checkIsStartNode.Count);
                            // if this RotationUser is startNode.
                            if (checkIsStartNode.Count == 1)
                            {
                                newItem.isStartPerson = true;
                                // only one person can be startNode
                                startPersonAded = true;
                            }
                        }
                        newItem.WorkflowNode = wfl;
                        newItem.FlagPermission = ep.FlagPermission;
                        User gotUser = db.Users.FirstOrDefault(usr => usr.Id == ep.UserId);
                        newItem.User = gotUser;
                        newItem.UserId = gotUser.Id;
                        db.RotationUsers.Add(newItem);

                        db.SaveChanges();
                    }
                }
                else if (cxold > cxnew)
                {
                    var dremove = db.RotationUsers.Where(c => c.Rotation.Id == product.Id).Take(cxold - cxnew).ToList();
                    db.RotationUsers.RemoveRange(dremove);
                    db.SaveChanges();
                    // save detail
                    var dnew = db.RotationUsers.Where(c => c.Rotation.Id == product.Id).ToList();
                    int v = 0;
                    foreach (RotationUser d in dnew)
                    {
                        var epos = prod.RotationUsers.ElementAt(v);
                        var wfl = db.WorkflowNodes.FirstOrDefault(c => c.Id == epos.WorkflowNodeId);
                        d.WorkflowNodeId = wfl.Id;
                        d.WorkflowNode = wfl;
                        wfl.RotationUsers.Add(d);
                        User gotUser = db.Users.FirstOrDefault(usr => usr.Id == epos.UserId);
                        d.User = gotUser;
                        d.UserId = gotUser.Id;
                        d.FlagPermission = epos.FlagPermission;
                        //d.FlagAction = epos.FlagAction;
                        //d.CxDownload = epos.CxDownload;
                        //d.CxPrint = epos.CxPrint;
                        //db.SaveChanges();
                        v++;
                        db.SaveChanges();
                    }
                }
                // kalau jumlah sama harus di cek setiap elemennya apakah tidak berubah
                else {
                    var rotationUserOld = db.RotationUsers.Where(c => c.Rotation.Id == product.Id).ToList();
                    int v = 0;
                    foreach (RotationUser userItem in rotationUserOld)
                    {
                        var epos = prod.RotationUsers.ElementAt(v);
                        var wfl = db.WorkflowNodes.FirstOrDefault(c => c.Id == epos.WorkflowNodeId);
                        userItem.WorkflowNodeId = wfl.Id;
                        userItem.WorkflowNode = wfl;
                        wfl.RotationUsers.Add(userItem);
                        User gotUser = db.Users.FirstOrDefault(usr => usr.Id == epos.UserId);
                        userItem.User = gotUser;
                        userItem.UserId = gotUser.Id;
                        userItem.FlagPermission = epos.FlagPermission;
                        //d.FlagAction = epos.FlagAction;
                        //d.CxDownload = epos.CxDownload;
                        //d.CxPrint = epos.CxPrint;
                        //db.SaveChanges();
                        v++;
                        db.SaveChanges();
                    }
                }
                // Tags
                var tagItemListInDb = db.TagItems.Where(tagitemdb => tagitemdb.RotationId == product.Id).ToList();
                if(prod.Tags != null)
                foreach (string tag in prod.Tags)
                {
                    var tagfromDB = db.Tags.FirstOrDefault(tagdb => tagdb.Name.ToLower().Equals(tag.ToLower()));
                    if (tagfromDB == null)
                    {
                        tagfromDB = new Tag();
                        tagfromDB.Name = tag;
                        db.Tags.Add(tagfromDB);
                        db.SaveChanges();
                    }
                    var tagItemFromDb = tagItemListInDb.FirstOrDefault(tagitemdb => tagitemdb.TagId == tagfromDB.Id && tagitemdb.RotationId == product.Id);
                    if(tagItemFromDb == null)
                    {
                        tagItemFromDb = new TagItem();
                        tagItemFromDb.RotationId = product.Id;
                        tagItemFromDb.Rotation = product;
                        tagItemFromDb.TagId = tagfromDB.Id;
                        tagItemFromDb.Tag = tagfromDB;
                        System.Diagnostics.Debug.WriteLine("[[ID of tag]] " + tagfromDB.Id + " " + product.Id);
                        db.TagItems.Add(tagItemFromDb);
                        product.TagItems.Add(tagItemFromDb);
                    }
                    else
                    {
                        tagItemListInDb.Remove(tagItemFromDb);
                    }
                }
                if (tagItemListInDb.Count() != 0)
                {
                    foreach(var item in tagItemListInDb)
                    {
                        db.TagItems.Remove(item);
                    }
                }
                db.SaveChanges();

                return product.Id;
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
        
    }
}
