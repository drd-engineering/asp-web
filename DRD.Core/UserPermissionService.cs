using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;
using System.Based.Core.Entity;
using DRD.Domain;
using System.Configuration;
using System.Data.Entity.Infrastructure;
using System.Based.Core;

namespace DRD.Service
{
    public class UserPermissionService
    {
        private readonly string _connString;

        //public DocumentService(string connString)
        //{
        //    _connString = connString;
        //}

        public UserPermissionService()
        {
            _connString = ConfigConstant.CONSTRING;
        }

        public JsonPermission GetByMemberId(long id)
        {

            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from a in db.MemberPermissions
                     where a.MemberId == id
                     select new JsonPermission
                     {
                         Id = a.Id,
                         MemberId = a.MemberId,
                         IsAll = ((a.Flag & 2) == 2),
                         Member = new DtoMemberLite
                         {
                             Id = a.Member.Id,
                             Name = a.Member.Name,
                             Email = a.Member.Email,
                             Number = a.Member.Number,
                             Phone = a.Member.Phone,
                             ImageProfile = a.Member.ImageProfile,
                             ImageQrCode = a.Member.ImageQrCode,
                             UserGroup = a.Member.UserGroup,
                             CompanyName = a.Member.Company.Name,
                         },
                         Companies =
                            (from c in a.MemberSubscribes
                             select new JsonPermissionCompany
                             {
                                 Id = c.Id,
                                 CompanyId = c.CompanyId,
                                 Name = c.Company.Name,
                                 IsSelected = ((c.Flag & 1) == 1),
                                 IsAll = ((c.Flag & 2) == 2),
                                 Projects =
                                    (from x in c.MemberProjects
                                     select new JsonPermissionProject
                                     {
                                         Id = x.Id,
                                         ProjectId = x.ProjectId,
                                         Name = x.Project.Name,
                                         IsSelected = ((x.Flag & 1) == 1),
                                         IsAll = ((x.Flag & 2) == 2),
                                         Workflows =
                                            (from y in x.MemberWorkflows
                                             select new JsonPermissionWorkflow
                                             {
                                                 Id = y.Id,
                                                 WorkflowId = y.WorkflowId,
                                                 Name = y.Workflow.Name,
                                                 IsSelected = ((y.Flag & 1) == 1),
                                                 IsAll = ((y.Flag & 2) == 2),
                                                 Rotations =
                                                    (from z in y.MemberRotations
                                                     select new JsonPermissionRotation
                                                     {
                                                         Id = z.Id,
                                                         RotationId = z.RotationId,
                                                         Subject = z.Rotation.Subject,
                                                         IsSelected = ((z.Flag & 1) == 1),
                                                     }).ToList(),
                                             }).ToList(),
                                     }).ToList(),
                             }).ToList(),
                     }).FirstOrDefault();


                if (result == null)
                {
                    result = new JsonPermission();
                    var mem = db.Members.FirstOrDefault(c => c.Id == id);
                    result.MemberId = mem.Id;
                    result.Member = new DtoMemberLite
                    {
                        Id = mem.Id,
                        Number = mem.Number,
                        Name = mem.Name,
                        Email = mem.Email,
                        Phone = mem.Phone,
                        ImageProfile = mem.ImageProfile,
                        ImageQrCode = mem.ImageQrCode,
                        UserGroup = mem.UserGroup,
                        CompanyName = mem.Company.Name,
                    };
                }
                return result;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prod"></param>
        /// <returns></returns>

        public int Save(JsonPermission permit)
        {

            MemberPermission permission;
            int result = 0;

            using (var db = new DrdContext(_connString))
            {
                if (permit.Id != 0)
                    permission = db.MemberPermissions.FirstOrDefault(c => c.MemberId == permit.MemberId);
                else
                    permission = new MemberPermission();

                permission.MemberId = permit.MemberId;
                permission.Flag = (permit.IsAll ? 2 : 0);
                permission.UserId = permit.UserId;

                if (permit.Id == 0)
                {
                    permission.DateCreated = DateTime.Now;
                    db.MemberPermissions.Add(permission);
                }
                else
                    permission.DateUpdated = DateTime.Now;

                result = db.SaveChanges();

                if (permit.Companies == null)
                    return result;

                // delete existing 
                if (permit.Id != 0)
                {
                    var rots = db.MemberRotations.Where(c => c.MemberWorkflow.MemberProject.MemberSubscribe.MemberPermission.MemberId == permit.MemberId).ToList();
                    db.MemberRotations.RemoveRange(rots);
                    var wfs = db.MemberWorkflows.Where(c => c.MemberProject.MemberSubscribe.MemberPermission.MemberId == permit.MemberId).ToList();
                    db.MemberWorkflows.RemoveRange(wfs);
                    var projs = db.MemberProjects.Where(c => c.MemberSubscribe.MemberPermission.MemberId == permit.MemberId).ToList();
                    db.MemberProjects.RemoveRange(projs);
                    var subs = db.MemberSubscribes.Where(c => c.MemberPermission.MemberId == permit.MemberId).ToList();
                    db.MemberSubscribes.RemoveRange(subs);
                }

                // save as new
                foreach (JsonPermissionCompany com in permit.Companies)
                {
                    MemberSubscribe memSub = new MemberSubscribe();
                    memSub.MemberPermissionId = permission.Id;
                    memSub.CompanyId = com.CompanyId;
                    memSub.Flag = (com.IsSelected ? 1 : 0) + (com.IsAll ? 2 : 0);
                    db.MemberSubscribes.Add(memSub);
                    db.SaveChanges();
                    if (com.Projects != null)
                    {
                        foreach (JsonPermissionProject proj in com.Projects)
                        {
                            MemberProject memProj = new MemberProject();
                            memProj.MemberSubscribeId = memSub.Id;
                            memProj.ProjectId = proj.ProjectId;
                            memProj.Flag = (proj.IsSelected ? 1 : 0) + (proj.IsAll ? 2 : 0);
                            db.MemberProjects.Add(memProj);
                            db.SaveChanges();
                            if (proj.Workflows != null)
                            {
                                foreach (JsonPermissionWorkflow wf in proj.Workflows)
                                {
                                    MemberWorkflow memWf = new MemberWorkflow();
                                    memWf.MemberProjectId = memProj.Id;
                                    memWf.WorkflowId = wf.WorkflowId;
                                    memWf.Flag = (wf.IsSelected ? 1 : 0) + (wf.IsAll ? 2 : 0);
                                    db.MemberWorkflows.Add(memWf);
                                    db.SaveChanges();
                                    if (wf.Rotations != null)
                                    {
                                        foreach (JsonPermissionRotation rot in wf.Rotations)
                                        {
                                            MemberRotation memRot = new MemberRotation();
                                            memRot.MemberWorkflowId = memWf.Id;
                                            memRot.RotationId = rot.RotationId;
                                            memRot.Flag = (rot.IsSelected ? 1 : 0);
                                            db.MemberRotations.Add(memRot);
                                            db.SaveChanges();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                return result;
            }

        }

    }
}

