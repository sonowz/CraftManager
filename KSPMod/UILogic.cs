using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using KSP.UI.Screens;

namespace KSPMod
{
    static class UILogic
    {
        public static ShipData shipData;
        public static SaveManager SaveManagerClass;
        public static CraftBrowserDialog originalBrowser = null;
        public static bool enableOriginal = false;
        public static Ship selectedShip = null;

        // public void Update(); declared in SaveManager.cs

        public static bool LoadSelectedShip(CraftBrowser.LoadType t)
        {
            if (selectedShip == null)
                return false;
            if (t == CraftBrowser.LoadType.Normal)
                EditorLogic.LoadShipFromFile(selectedShip.file.FullName);
            else if (EditorLogic.RootPart == null)
                return false;
            else
                ShipConstruction.LoadSubassembly(selectedShip.file.FullName);
            return true;
        }

        public static bool DeleteSelectedShip()
        {
            if (selectedShip == null)
                return false;
            if (System.IO.File.Exists(selectedShip.file.FullName) == false)
                return false;
            shipData.remove(selectedShip);
            System.IO.File.Delete(selectedShip.file.FullName);
            selectedShip = null;
            return true;
        }

        public static void ReloadShipData()
        {
            SaveManagerClass.FetchShipFiles();
        }

        public static IList<UI_TagWindow> Tag_getList()
        {
            SortedList<string, UI_TagWindow> ret = new SortedList<string, UI_TagWindow>();
            var list = shipData.getTags().OrderBy(key => key);
            if (list != null)
            {
                foreach (string key in shipData.getTags().OrderBy(key => key))
                {
                    UI_TagWindow m = new UI_TagWindow();
                    m.tag = key;
                    m.selected = shipData.selectedTag.ContainsKey(key);
                    if (m.tag != "_All")
                        ret.Add(m.tag, m);
                }
                return ret.Values;
            }
            else
                return new List<UI_TagWindow>();
        }

        private static bool ListHasSelectedShip = false;
        public static IList<UI_CraftWindow> Craft_getList()
        {
                if (!ListHasSelectedShip)
                    selectedShip = null;
                List<UI_CraftWindow> ret = new List<UI_CraftWindow>();
                var list = shipData.query();
                if (list != null)
                    foreach (KeyValuePair<string, Ship> pair in shipData.query())
                    {
                        UI_CraftWindow m = new UI_CraftWindow();
                        m.craft = pair.Value;
                        if (m.craft == selectedShip)
                            m.selected = true;
                        else
                            m.selected = false;
                        ret.Add(m);
                    }
                ListHasSelectedShip = false;
                return ret;
        }
        public static void Craft_updateShipSelection(Ship ship, bool prevState)
        {
            if(prevState == false)
                selectedShip = ship;
            ListHasSelectedShip = true;
        }

        public static void onDestroy()
        {
            selectedShip = null;
            ListHasSelectedShip = false;
            enableOriginal = false;
            SaveManagerClass.onUIDestroy();
        }
    }

    class UI_TagWindow
    {
        public string tag;
        public bool selected;
        public void update(bool selected)
        {
            if (this.selected != selected)
            {
                if (selected == true)
                    UILogic.shipData.selectedTag.Add(tag, tag);
                else
                    UILogic.shipData.selectedTag.Remove(tag);
            }
        }
    }

    class UI_CraftWindow
    {
        public Ship craft;
        public bool selected;
        public void update(bool selected)
        {
            if (selected)
                UILogic.Craft_updateShipSelection(craft, this.selected);
        }
    }
}
