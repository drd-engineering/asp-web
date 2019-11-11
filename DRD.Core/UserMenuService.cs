using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Based.Core.Entity.User;
using DRD.Domain;
using System.Based.Core;

namespace DRD.Service
{
    public class UserMenuService
    {
        private readonly string _connString;

        public UserMenuService(string connString)
        {
            _connString = connString;
        }

        public UserMenuService()
        {
            _connString = ConfigConstant.CONSTRING_USER;
        }

        public IEnumerable<JsonUserMenu> GetByUserId(long id)
        {
            using (var db = new DrdUserContext(_connString))
            {
                var user = db.UserMasters.FirstOrDefault(c => c.Id == id);

                if (user.GroupMasterId == null)
                {
                    var result =
                        (from c in db.UserMenus
                         join x in db.MenuMasters on c.MenuMasterId equals x.Id
                         where c.UserMasterId == id && c.AccessBit == 1
                         orderby x.SeqNo
                         select new JsonUserMenu
                         {
                             Id = c.Id,
                             AccessBit = c.AccessBit,
                             MenuMasterId = c.MenuMasterId,
                             UserMasterId = c.UserMasterId,
                             ChildCount = db.MenuMasters.Count(x => x.ParentId == c.MenuMasterId),
                             MenuMaster = new JsonMenuMaster
                             {
                                 Id = x.Id,
                                 Caption = x.Caption,
                                 Icon = x.Icon,
                                 IsFraming = x.IsFraming,
                                 ItemType = x.ItemType,
                                 Link = x.Link,
                                 ObjectName = x.ObjectName,
                                 ParentId = x.ParentId,
                                 SecondaryKey = x.SecondaryKey,
                                 SeqNo = x.SeqNo,
                             }
                         }).ToList();


                    return result;
                }
                else
                {
                    var result =
                        (from c in db.GroupMenus
                         join x in db.MenuMasters on c.MenuMasterId equals x.Id
                         where c.GroupMasterId == user.GroupMasterId && c.AccessBit == 1
                         orderby x.SeqNo
                         select new JsonUserMenu
                         {
                             Id = c.Id,
                             AccessBit = c.AccessBit,
                             MenuMasterId = c.MenuMasterId,
                             UserMasterId = user.Id,
                             ChildCount = db.MenuMasters.Count(x => x.ParentId == c.MenuMasterId),
                             MenuMaster = new JsonMenuMaster
                             {
                                 Id = x.Id,
                                 Caption = x.Caption,
                                 Icon = x.Icon,
                                 IsFraming = x.IsFraming,
                                 ItemType = x.ItemType,
                                 Link = x.Link,
                                 ObjectName = x.ObjectName,
                                 ParentId = x.ParentId,
                                 SecondaryKey = x.SecondaryKey,
                                 SeqNo = x.SeqNo,
                             }
                         }).ToList();


                    return result;
                }
            }
        }

        public IEnumerable<JsonUserMenu> GetByUserGroup(string groupName)
        {
            using (var db = new DrdUserContext(_connString))
            {
                //var user = db.UserMasters.FirstOrDefault(c => c.GroupMaster.Name.Equals(groupName));

                var result =
                    (from c in db.GroupMenus
                     join x in db.MenuMasters on c.MenuMasterId equals x.Id
                     where c.GroupMaster.Name.Equals(groupName) && c.AccessBit == 1
                     orderby x.SeqNo
                     select new JsonUserMenu
                     {
                         Id = c.Id,
                         AccessBit = c.AccessBit,
                         MenuMasterId = c.MenuMasterId,
                         //UserMasterId = user.Id,
                         ChildCount = db.MenuMasters.Count(x => x.ParentId == c.MenuMasterId),
                         MenuMaster = new JsonMenuMaster
                         {
                             Id = x.Id,
                             Caption = x.Caption,
                             Icon = x.Icon,
                             IsFraming = x.IsFraming,
                             ItemType = x.ItemType,
                             Link = x.Link,
                             ObjectName = x.ObjectName,
                             ParentId = x.ParentId,
                             SecondaryKey = x.SecondaryKey,
                             SeqNo = x.SeqNo,
                         }
                     }).ToList();


                return result;
            }
        }
        public IEnumerable<JsonUserMenu> GetByUserGroup(string groupName, string secondaryKeys)
        {
            using (var db = new DrdUserContext(_connString))
            {
                //var user = db.UserMasters.FirstOrDefault(c => c.GroupMaster.Name.Equals(groupName));

                var result =
                    (from c in db.GroupMenus
                     join x in db.MenuMasters on c.MenuMasterId equals x.Id
                     where c.GroupMaster.Name.Equals(groupName) && secondaryKeys.Contains(c.MenuMaster.SecondaryKey) //c.AccessBit == 1 
                     orderby x.SeqNo
                     select new JsonUserMenu
                     {
                         Id = c.Id,
                         AccessBit = c.AccessBit,
                         MenuMasterId = c.MenuMasterId,
                         //UserMasterId = user.Id,
                         ChildCount = db.MenuMasters.Count(x => x.ParentId == c.MenuMasterId),
                         MenuMaster = new JsonMenuMaster
                         {
                             Id = x.Id,
                             Caption = x.Caption,
                             Icon = x.Icon,
                             IsFraming = x.IsFraming,
                             ItemType = x.ItemType,
                             Link = x.Link,
                             ObjectName = x.ObjectName,
                             ParentId = x.ParentId,
                             SecondaryKey = x.SecondaryKey,
                             SeqNo = x.SeqNo,
                         }
                     }).ToList();


                return result;
            }
        }

        //public List<JsonMenu> GetMenus(long userId, int activeId)
        //{

        //    List<JsonMenu> menus = new List<JsonMenu>();

        //    UserMenuService usvr = new UserMenuService();
        //    IEnumerable<JsonUserMenu> umenus = usvr.GetByUserId(userId);
        //    MenuService msvr = new MenuService();
        //    foreach (JsonUserMenu menu in umenus)
        //    {
        //        string slink = menu.MenuMaster.Link;
        //        if (!slink.Equals("") && !menu.MenuMaster.SecondaryKey.Equals("FUNC"))
        //            slink += "?mid=" + msvr.Encrypt(userId + "," + menu.MenuMaster.Id.ToString());

        //        menus.Add(new JsonMenu(menu.MenuMaster.Id.ToString(), menu.MenuMaster.SecondaryKey, menu.MenuMaster.Caption, slink, menu.ChildCount, menu.MenuMaster.ParentId.ToString(), menu.MenuMaster.Icon, (menu.MenuMasterId == activeId)));
        //    }


        //    return menus;
        //}

        public List<JsonMenu> GetMenus(long userId, string userGroup, int activeId)
        {

            List<JsonMenu> menus = new List<JsonMenu>();

            UserMenuService usvr = new UserMenuService();
            IEnumerable<JsonUserMenu> umenus = usvr.GetByUserGroup(userGroup);
            MenuService msvr = new MenuService();
            foreach (JsonUserMenu menu in umenus)
            {
                string slink = menu.MenuMaster.Link;
                if (!slink.Equals("") && !menu.MenuMaster.SecondaryKey.Equals("FUNC"))
                    slink += "?mid=" + msvr.Encrypt(userId + "," + menu.MenuMaster.Id.ToString());

                menus.Add(new JsonMenu(menu.MenuMaster.Id.ToString(), menu.MenuMaster.SecondaryKey, menu.MenuMaster.Caption, slink, menu.ChildCount, menu.MenuMaster.ParentId.ToString(), menu.MenuMaster.Icon, menu.MenuMaster.ObjectName, menu.MenuMaster.ItemType, (menu.MenuMasterId == activeId)));
            }
            return menus;
        }


        public List<JsonMenu> GetDashboardMenus(long userId, string userGroup, int activeId)
        {

            List<JsonMenu> menus = new List<JsonMenu>();

            UserMenuService usvr = new UserMenuService();
            IEnumerable<JsonUserMenu> umenus = usvr.GetByUserGroup(userGroup, "INBOX,INPROGRESS,COMPLETED,CONTACT,DOCUMENT,DRDRIVE,ROTATION,DECLINED,WORKFLOW");
            MenuService msvr = new MenuService();
            foreach (JsonUserMenu menu in umenus)
            {
                string slink = menu.MenuMaster.Link;
                if (!slink.Equals("") && !menu.MenuMaster.SecondaryKey.Equals("FUNC"))
                    slink += "?mid=" + msvr.Encrypt(userId + "," + menu.MenuMaster.Id.ToString());

                menus.Add(new JsonMenu(menu.MenuMaster.Id.ToString(), menu.MenuMaster.SecondaryKey, menu.MenuMaster.Caption, slink, menu.ChildCount, menu.MenuMaster.ParentId.ToString(), menu.MenuMaster.Icon, menu.MenuMaster.ObjectName, menu.MenuMaster.ItemType, (menu.MenuMasterId == activeId)));
            }
            return menus;
        }

        //public bool ValidUserMenu(long userId, int menuId)
        //{
        //    using (var db = new DrdUserContext(_connString))
        //    {
        //        var user = db.UserMasters.FirstOrDefault(c => c.Id == userId);
        //        if (user.GroupMasterId == null)
        //        {
        //            var menu = db.UserMenus.FirstOrDefault(c => c.UserMasterId == userId && c.MenuMasterId == menuId && c.AccessBit == 1);
        //            return (menu != null);
        //        }
        //        else
        //        {
        //            var menu = db.GroupMenus.FirstOrDefault(c => c.GroupMasterId == user.GroupMasterId && c.MenuMasterId == menuId);
        //            return (menu != null);
        //        }
        //    }
        //}

        public bool ValidGroupMenu(string groupName, int menuId)
        {
            using (var db = new DrdUserContext(_connString))
            {
                var menu = db.GroupMenus.FirstOrDefault(c => c.GroupMaster.Name.Equals(groupName) && c.MenuMasterId == menuId);
                if (menu == null || menu.AccessBit == 0)
                    return false;

                return true;
            }
        }

    }
}
