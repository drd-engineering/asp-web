﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Models.View.Menus
{
    public class UserMenu
    {
        public List<Menu> menus;

        public UserMenu()
        {        
            menus= new List<Menu>();
            menus.Add(new Menu(
                Code: "2049", 
                SecondaryKey: null,
                Name: "Storage", 
                Icon: null,
                UrlPage: null, 
                ChildCount: 1, 
                ParentCode: "0",
                ItemType: 0,
                ObjectName: null,
                IsActive: false));
            menus.Add(new Menu(
                Code: "2050",
                SecondaryKey: "WORKFLOW",
                Name: "workflow",
                Icon: "icon.cog3",
                UrlPage: "/workflow/list",
                ChildCount: 5,
                ParentCode: "2069",
                ItemType: 0,
                ObjectName: null,
                IsActive: false));
            menus.Add(new Menu(
                Code: "2051",
                SecondaryKey: null,
                Name: "Project",
                Icon: null,
                UrlPage: null,
                ChildCount: 2,
                ParentCode: "0",
                ItemType: 0,
                ObjectName: null,
                IsActive: false));
            menus.Add(new Menu(
                Code: "2052",
                SecondaryKey: "INBOX",
                Name: "INBOX",
                Icon: "icon-envelope",
                UrlPage: "inbox/list",
                ChildCount: 3,
                ParentCode: "2086",
                ItemType: 0,
                ObjectName: null,
                IsActive: false));
            menus.Add(new Menu(
                Code: "2055",
                SecondaryKey: "PENDING",
                Name: "Pending",
                Icon: "icon-stop",
                UrlPage: "pending/list",
                ChildCount: 2,
                ParentCode: "2093",
                ItemType: 0,
                ObjectName: null,
                IsActive: false));
            menus.Add(new Menu(
                Code: "2056",
                SecondaryKey: "SIGNED",
                Name: "Signed",
                Icon: "icon-pencil",
                UrlPage: "signed/list",
                ChildCount: 4,
                ParentCode: "2093",
                ItemType: 0,
                ObjectName: null,
                IsActive: false));
            menus.Add(new Menu(
                Code: "2059",
                SecondaryKey: null,
                Name: "Storage",
                Icon: null,
                UrlPage: null,
                ChildCount: 5,
                ParentCode: "0",
                ItemType: 0,
                ObjectName: null,
                IsActive: false));
            menus.Add(new Menu(
                Code: "2060",
                SecondaryKey: "ROTATION",
                Name: "Rotation",
                Icon: "icon-loop",
                UrlPage: null,
                ChildCount: 2,
                ParentCode: "2051",
                ItemType: 0,
                ObjectName: null,
                IsActive: false));
            menus.Add(new Menu(
                Code: "2061",
                SecondaryKey: "DRDRIVE",
                Name: "DRD Drive",
                Icon: "icon-cloud-upload",
                UrlPage: "drdrive",
                ChildCount: 12,
                ParentCode: "2051",
                ItemType: 0,
                ObjectName: null,
                IsActive: false));
            menus.Add(new Menu(
                Code: "2062",
                SecondaryKey: "ALTERED",
                Name: "Altered",
                Icon: "icon-split",
                UrlPage: "/altered/list",
                ChildCount: 1,
                ParentCode: "2053",
                ItemType: 0,
                ObjectName: null,
                IsActive: false));
            menus.Add(new Menu(
                Code: "20663",
                SecondaryKey: "REVISED",
                Name: "Revised",
                Icon: "icon-reset",
                UrlPage: "/revised/list",
                ChildCount: 3,
                ParentCode: "0",
                ItemType: 0,
                ObjectName: null,
                IsActive: false));
            menus.Add(new Menu(
                Code: "2064",
                SecondaryKey: "DOCUMENT",
                Name: "Document",
                Icon: "icon-files-empty",
                UrlPage: "/document/list",
                ChildCount: 12,
                ParentCode: "2086",
                ItemType: 0,
                ObjectName: null,
                IsActive: false));
            menus.Add(new Menu(
                Code: "2065",
                SecondaryKey: "INPROGRESS",
                Name: "In Progress",
                Icon: null,
                UrlPage: "/inprogress/list",
                ChildCount: 1,
                ParentCode: "2060",
                ItemType: 0,
                ObjectName: null,
                IsActive: false));
            menus.Add(new Menu(
                Code: "2066",
                SecondaryKey: "DECLINED",
                Name: "Declined",
                Icon: null,
                UrlPage: "/declined/list",
                ChildCount: 2,
                ParentCode: "2060",
                ItemType: 0,
                ObjectName: null,
                IsActive: false));
            menus.Add(new Menu(
                Code: "2067",
                SecondaryKey: "COMPLETED",
                Name: "Completed",
                Icon: null,
                UrlPage: "/completed/list",
                ChildCount: 3,
                ParentCode: "0",
                ItemType: 0,
                ObjectName: null,
                IsActive: false));
            menus.Add(new Menu(
                Code: "2067",
                SecondaryKey: "COMPLETED",
                Name: "Completed",
                Icon: null,
                UrlPage: "/completed/list",
                ChildCount: 3,
                ParentCode: "0",
                ItemType: 0,
                ObjectName: null,
                IsActive: false));
            menus.Add(new Menu(
                Code: "2067",
                SecondaryKey: "COMPLETED",
                Name: "Completed",
                Icon: null,
                UrlPage: "/completed/list",
                ChildCount: 3,
                ParentCode: "0",
                ItemType: 0,
                ObjectName: null,
                IsActive: false));
            menus.Add(new Menu(
                Code: "2067",
                SecondaryKey: "COMPLETED",
                Name: "Completed",
                Icon: null,
                UrlPage: "/completed/list",
                ChildCount: 3,
                ParentCode: "0",
                ItemType: 0,
                ObjectName: null,
                IsActive: false));
            menus.Add(new Menu(
                Code: "2067",
                SecondaryKey: "COMPLETED",
                Name: "Completed",
                Icon: null,
                UrlPage: "/completed/list",
                ChildCount: 3,
                ParentCode: "0",
                ItemType: 0,
                ObjectName: null,
                IsActive: false));
            menus.Add(new Menu(
                Code: "2067",
                SecondaryKey: "COMPLETED",
                Name: "Completed",
                Icon: null,
                UrlPage: "/completed/list",
                ChildCount: 3,
                ParentCode: "0",
                ItemType: 0,
                ObjectName: null,
                IsActive: false));
            menus.Add(new Menu(
                Code: "2067",
                SecondaryKey: "COMPLETED",
                Name: "Completed",
                Icon: null,
                UrlPage: "/completed/list",
                ChildCount: 3,
                ParentCode: "0",
                ItemType: 0,
                ObjectName: null,
                IsActive: false));
        }
    }
}
  