using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using KSP.UI.Screens;

namespace KSPMod
{
    class SMUI : MonoBehaviour
    {
        private GUIStyle g_window;
        private GUIStyle g_toggle;
        private GUIStyle g_box;
        private GUIStyle g_button;
        private GUIStyle g_tagEntry;
        private GUIStyle g_craftEntry;
        private GUIStyle g_scrollbar;
        private GUIStyle g_label;

        public void Start()
        {
            g_window = HighLogic.Skin.window;
            g_toggle = HighLogic.Skin.toggle;
            g_box = HighLogic.Skin.box;
            g_button = HighLogic.Skin.button;
            g_scrollbar = HighLogic.Skin.verticalScrollbar;
            g_label = HighLogic.Skin.label;
            g_tagEntry = g_button;

            g_craftEntry = new GUIStyle(HighLogic.Skin.customStyles[0]);
            g_craftEntry.fontSize = 12;
            g_craftEntry.alignment = TextAnchor.MiddleLeft;

        }

        public void OnGUI()
        {
            if (SMLogic.enableOriginal == false)
            {
                GUI.Window(0, new Rect(Screen.width / 2.0f - 300, Screen.height / 2.0f - 250, 120, 500), TagWindow, "Select Tag", g_window);
                GUI.Window(1, new Rect(Screen.width / 2.0f - 170, Screen.height / 2.0f - 250, 400, 500), CraftWindow, "Select Craft", g_window);
                if(SMLogic.selectedShip != null)
                    GUI.Window(2, new Rect(Screen.width / 2.0f + 240, Screen.height / 2.0f - 250, 200, 240), PreviewWindow, "Preview", g_window);
            }
            else
            {
                if (SMLogic.originalBrowser == null)
                    destroy();
                else if (GUI.Button(new Rect(Screen.width / 2.0f + 150, Screen.height / 2.0f - 240, 20, 20), "O", g_button))
                    SMLogic.enableOriginal = false;
            }
        }

        private Vector2 tagScrollPosition = Vector2.zero;
        public void TagWindow(int id)
        { 
            const int ITEMHEIGHT = 30;
            int i = 0;
            var list = SMLogic.UI_Tag_getList();
            tagScrollPosition = GUI.BeginScrollView(new Rect(5, 40, 110, 450), tagScrollPosition, new Rect(5, 0, 90, Mathf.Max(ITEMHEIGHT * list.Count, 450)), false, true);
            foreach (UI_TagWindow item in list)
            {
                item.update(GUI.Toggle(new Rect(5, ITEMHEIGHT* i, 90, (ITEMHEIGHT-5)), item.selected, item.tag, g_tagEntry));
                i++;
            }
            GUI.Box(new Rect(0, 0, 0, 0), "");
            GUI.EndScrollView();
        }

        private bool deleteConfirm = false;
        private Vector2 craftScrollPosition = Vector2.zero;
        public void CraftWindow(int id)
        {
            const int ITEMHEIGHT = 60;
            int i = 0;
            var list = SMLogic.UI_Craft_getList();
            craftScrollPosition = GUI.BeginScrollView(new Rect(5, 40, 390, 400), craftScrollPosition, new Rect(5, 0, 370, Mathf.Max(ITEMHEIGHT * list.Count, 400)), false, true);
            foreach (UI_CraftWindow item in list)
            {
                item.update(GUI.Toggle(new Rect(15, ITEMHEIGHT * i, 360, (ITEMHEIGHT - 5)), item.selected, item.craft.description, g_craftEntry));
                if(item.craft.thumbnail.width != 0)
                    GUI.Label(new Rect(370 - (ITEMHEIGHT - 10), ITEMHEIGHT * i + 5, ITEMHEIGHT - 10, ITEMHEIGHT - 10), item.craft.thumbnail);
                i++;
            }
            GUI.Box(new Rect(0, 0, 0, 0), "");
            GUI.EndScrollView();

            if (GUI.Button(new Rect(320, 460, 70, 30), "<color=#CCFF00>Load</color>", g_button))
            {
                if(SMLogic.LoadSelectedShip(CraftBrowser.LoadType.Normal))
                    destroy();
            }


            if (GUI.Button(new Rect(240, 460, 70, 30), "<color=#F79303>Merge</color>", g_button))
            {
                if(SMLogic.LoadSelectedShip(CraftBrowser.LoadType.Merge))
                    destroy();
            }

            if (GUI.Button(new Rect(10, 460, 70, 30), "<color=#FF0000>Delete</color>", g_button))
                deleteConfirm = !deleteConfirm;

            if (deleteConfirm)
            {
                GUI.Label(new Rect(90, 460, 120, 30), "Are you sure?");
                if (GUI.Button(new Rect(180, 460, 30, 30), "<color=#FF0000>Y</color>", g_button))
                    SMLogic.DeleteSelectedShip();
            }
            else
            {
                if (GUI.Button(new Rect(90, 460, 70, 30), "Cancel", g_button))
                    destroy();
            }

            if (GUI.Button(new Rect(370, 10, 20, 20), "O", g_button))
                SMLogic.enableOriginal = true;
        }

        public void PreviewWindow(int id)
        {
            if(SMLogic.selectedShip.thumbnail.width != 0)
                GUI.Box(new Rect(10, 50, 180, 180), SMLogic.selectedShip.thumbnail, g_box);
        }

        private void destroy()
        {
            SMLogic.originalBrowser.OnBrowseCancelled();
            SMLogic.UI_onDestroy();
            Destroy(this);
        }
       
    }
}
