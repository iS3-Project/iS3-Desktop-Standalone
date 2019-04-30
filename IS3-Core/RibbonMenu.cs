using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iS3.Core
{
    public class iS3MenuItem
    {
        public string displayPath { get; set; }
        public string displayName { get; set; }
        public bool isLargeImage { get; set; }
        public string imageSource { get; set; }

        public DFunc dFunc { get; set; }
        public iS3MenuItem(string displayPath, string displayName, bool isLargeImage, string imageSource, DFunc dFunc)
        {
            this.displayPath = displayPath;
            this.displayName = displayName;
            this.isLargeImage = isLargeImage;
            this.imageSource = imageSource;
            this.dFunc = dFunc;
        }
    }
    public class iS3MenuRibbon
    {
        public List<iS3MenuTab> MenuTabs { get; set; }
        public iS3MenuRibbon()
        {
            MenuTabs = new List<iS3MenuTab>();
        }
        public void AddiS3MenuItem(iS3MenuItem item)
        {
            List<string> strList = item.displayPath.Split('|').ToList();
            iS3MenuTab menuTab = getiS3MenuTabByName(strList[0]);
            if (menuTab == null)
            {
                menuTab = new iS3MenuTab() { Header = strList[0] };
                MenuTabs.Add(menuTab);
            }
            iS3MenuGroup menuGroup = menuTab.getiS3MenuGroupByName(strList[1]);
            if (menuGroup == null)
            {
                menuGroup = new iS3MenuGroup() { Header = strList[1] };
                menuTab.MenuGroups.Add(menuGroup);
            }
            menuGroup.MenuButtons.Add(new iS3MenuButton()
            {
                DFunc = item.dFunc,
                IsLargeImage = item.isLargeImage,
                ImageSource = item.imageSource,
                Label = item.displayName
            });
        }
        public iS3MenuTab getiS3MenuTabByName(string MenuTabName)
        {
            foreach (iS3MenuTab tab in MenuTabs)
            {
                if (tab.Header == MenuTabName)
                {
                    return tab;
                }
            }
            return null;
        }
    }
    public class iS3MenuTab
    {
        public string Header { get; set; }
        public List<iS3MenuGroup> MenuGroups { get; set; }
        public iS3MenuTab()
        {
            MenuGroups = new List<iS3MenuGroup>();
        }
        public iS3MenuGroup getiS3MenuGroupByName(string MenuGroupName)
        {
            foreach (iS3MenuGroup group in MenuGroups)
            {
                if (group.Header == MenuGroupName)
                {
                    return group;
                }
            }
            return null;
        }
    }
    public class iS3MenuGroup
    {
        public string Header { get; set; }
        public List<iS3MenuButton> MenuButtons { get; set; }
        public iS3MenuGroup()
        {
            MenuButtons = new List<iS3MenuButton>();
        }
    }
    public class iS3MenuButton
    {
        public DFunc DFunc { get; set; }

        public bool IsLargeImage { get; set; }
        public string ImageSource { get; set; }
        public string Label { get; set; }
    }
    public class DFunc
    {
        // Summary:
        //     Call back function
        public delegate void DelegateFunc();
        public DelegateFunc func;
        public DFunc(DelegateFunc func)
        {
            this.func = func;
        }
    }
}
