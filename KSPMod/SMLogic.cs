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
    static class SMLogic
    {
        public static ShipData shipData;
        public static CraftBrowserDialog originalBrowser = null;
        public static bool enableOriginal = false;
        public static Ship selectedShip = null;

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

        public static void DeleteSelectedShip()
        {

        }

        

        #region UI Functions
        public static IList<UI_TagWindow> UI_Tag_getList()
            {
                SortedList<string, UI_TagWindow> ret = new SortedList<string, UI_TagWindow>();
                foreach (string key in shipData.getTags().OrderBy(key => key))
                {
                    UI_TagWindow m = new UI_TagWindow();
                    m.tag = key;
                    m.selected = shipData.selectedTag.ContainsKey(key);
                    if(m.tag != "_All")
                        ret.Add(m.tag, m);
                }
                return ret.Values;
            }

            private static bool ListHasSelectedShip = false;
            public static IList<UI_CraftWindow> UI_Craft_getList()
            {
                if (!ListHasSelectedShip)
                    selectedShip = null;
                List<UI_CraftWindow> ret = new List<UI_CraftWindow>();
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
            public static void UI_Craft_updateShipSelection(Ship ship, bool prevState)
            {
                if(prevState == false)
                    selectedShip = ship;
                ListHasSelectedShip = true;
            }

            public static void UI_onDestroy()
            {
                selectedShip = null;
                ListHasSelectedShip = false;
                enableOriginal = false;
            }
        #endregion
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
                    SMLogic.shipData.selectedTag.Add(tag, tag);
                else
                    SMLogic.shipData.selectedTag.Remove(tag);
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
                SMLogic.UI_Craft_updateShipSelection(craft, this.selected);
        }
    }
}
