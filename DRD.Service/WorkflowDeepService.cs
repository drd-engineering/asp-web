using DRD.Models;
using DRD.Models.View;
using DRD.Service.Context;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;


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
                     ActionStatus = d.ActionStatus,
                     RotationNode = new RotationNode
                     {
                         Rotation = d.RotationNode.Rotation,
                     },
                     Document = new Document
                     {
                         Extention = d.Document.Extention,
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
                              select new DocumentAnnotation
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
                                  Text = documentElement.Text,
                                  Unknown = documentElement.Unknown,
                                  Rotation = documentElement.Rotation,
                                  ScaleX = documentElement.ScaleX,
                                  ScaleY = documentElement.ScaleY,
                                  TransitionX = documentElement.TransitionX,
                                  TransitionY = documentElement.TransitionY,
                                  StrokeWidth = documentElement.StrokeWidth,
                                  Opacity = documentElement.Opacity,
                                  Flag = documentElement.Flag,
                                  AssignedAnnotationCode = documentElement.AssignedAnnotationCode,
                                  AssignedAt = documentElement.AssignedAt,
                                  AssignedAnnotationImageFileName = documentElement.AssignedAnnotationImageFileName,
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
      
        private bool checkIdExist(long id)
        {
            using (var db = new ServiceContext())
            {

                var count = db.Rotations.Where(rotation => rotation.Id == id).FirstOrDefault();

                return count != null;

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rot"></param>
        /// <returns></returns>
        public long Save(RotationItem rot)
        {
            Rotation product;
            using var db = new ServiceContext();
            if (rot.Id != 0)
            {
                product = db.Rotations.FirstOrDefault(c => c.Id == rot.Id);
            }
            else
            {

                product = new Rotation();

                while (checkIdExist(product.Id))
                {
                    product.Id = Utilities.RandomLongGenerator(minimumValue: Constant.MINIMUM_VALUE_ID, maximumValue: Constant.MAXIMUM_VALUE_ID);
                }

            }

            product.Name = rot.Subject;
            Workflow workflowitem = db.Workflows.FirstOrDefault(w => w.Id == rot.WorkflowId);
            product.Workflow = workflowitem;
            product.Description = rot.Remark;
            product.Status = rot.Status;
            product.CreatorId = rot.CreatorId;
            product.UserId = rot.UserId;

            if (rot.Id == 0)
            {
                product.CreatedAt = DateTime.Now;
                db.Rotations.Add(product);
                workflowitem.TotalUsed = workflowitem.TotalUsed + 1;
            }
            else
                product.UpdatedAt = DateTime.Now;

            var result = db.SaveChanges();

            //
            // prepare data detail
            //
            var cxold = db.RotationUsers.Count(c => c.Rotation.Id == product.Id);
            var cxnew = rot.RotationUsers.Count();
            if (cxold < cxnew)
            {
                bool startPersonAded = false;
                // save detail
                for (var x = 0; x < cxnew; x++)
                {
                    var ep = rot.RotationUsers.ElementAt(x); // get the data
                    var newItem = new RotationUser();
                    newItem.Rotation = product;
                    newItem.WorkflowNodeId = ep.Id;
                    var wfl = db.WorkflowNodes.FirstOrDefault(c => c.Id == ep.WorkflowNodeId);

                    if (!startPersonAded)
                    {
                        var checkIsStartNode = (from workflowNode in db.WorkflowNodes
                                                join wfndLink in db.WorkflowNodeLinks on workflowNode.Id equals wfndLink.SourceId
                                                where workflowNode.WorkflowId == wfl.WorkflowId
                                                && workflowNode.SymbolCode == 0
                                                && wfndLink.TargetId == ep.WorkflowNodeId //this node is a target node from start Node
                                                select new
                                                {
                                                    isFound = true
                                                }).ToList();
                        // if this RotationUser is startNode.
                        if (checkIsStartNode.Count == 1)
                        {
                            newItem.IsStartPerson = true;
                            // only one person can be startNode
                            startPersonAded = true;
                        }
                    }
                    newItem.WorkflowNode = wfl;
                    newItem.ActionPermission = ep.ActionPermission;
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
                    var epos = rot.RotationUsers.ElementAt(v);
                    var wfl = db.WorkflowNodes.FirstOrDefault(c => c.Id == epos.WorkflowNodeId);
                    d.WorkflowNodeId = wfl.Id;
                    d.WorkflowNode = wfl;
                    wfl.RotationUsers.Add(d);
                    User gotUser = db.Users.FirstOrDefault(usr => usr.Id == epos.UserId);
                    d.User = gotUser;
                    d.UserId = gotUser.Id;
                    d.ActionPermission = epos.ActionPermission;
                    //d.FlagAction = epos.FlagAction;
                    //d.CxDownload = epos.CxDownload;
                    //d.CxPrint = epos.CxPrint;
                    //db.SaveChanges();
                    v++;
                    db.SaveChanges();
                }
            }
            // kalau jumlah sama harus di cek setiap elemennya apakah tidak berubah
            else
            {
                var rotationUserOld = db.RotationUsers.Where(c => c.Rotation.Id == product.Id).ToList();
                int v = 0;
                foreach (RotationUser userItem in rotationUserOld)
                {
                    var epos = rot.RotationUsers.ElementAt(v);
                    var wfl = db.WorkflowNodes.FirstOrDefault(c => c.Id == epos.WorkflowNodeId);
                    userItem.WorkflowNodeId = wfl.Id;
                    userItem.WorkflowNode = wfl;
                    wfl.RotationUsers.Add(userItem);
                    User gotUser = db.Users.FirstOrDefault(usr => usr.Id == epos.UserId);
                    userItem.User = gotUser;
                    userItem.UserId = gotUser.Id;
                    userItem.ActionPermission = epos.ActionPermission;
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
            if (rot.Tags != null)
                foreach (string tag in rot.Tags)
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
                    if (tagItemFromDb == null)
                    {
                        tagItemFromDb = new TagItem();
                        tagItemFromDb.RotationId = product.Id;
                        tagItemFromDb.Rotation = product;
                        tagItemFromDb.TagId = tagfromDB.Id;
                        tagItemFromDb.Tag = tagfromDB;
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
                foreach (var item in tagItemListInDb)
                {
                    db.TagItems.Remove(item);
                }
            }
            db.SaveChanges();

            return product.Id;
        }
    }
}
