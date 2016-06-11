using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using KSP.UI.Screens;

namespace KSPMod
{
    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    class SaveManager : MonoBehaviour
    {
        private ShipData shipData;
        private SMUI UI;
        private bool needsFileReload = false;

        public void Start()
        {
            //GameEvents.onEditorShipModified.Add(OnShipModified);
            try
            {
                FetchShipFiles();
                UILogic.SaveManagerClass = this;
                EditorLogic.fetch.loadBtn.onClick.AddListener(onLoadBtn);
                EditorLogic.fetch.saveBtn.onClick.AddListener(onSaveBtn);
                onUIDestroy();  // Initialize UI components
            }
            catch (Exception e)
            {
                print(e.StackTrace);
            }
            /*CraftBrowserDialogHandler i= new CraftBrowserDialogHandler();
            print(i.GetHashCode());
            i = CraftBrowserDialogHandler._Spawn(EditorFacility.VAB, HighLogic.SaveFolder, i.OnFileSelected, i.OnBrowseCancelled, true);
            print(i.GetHashCode());*/
            /*CraftBrowserDialogHandler i = GameObject.Find("DialogCanvas").AddComponent<CraftBrowserDialogHandler>();
            i.init(shipData);*/
        }

        public void FetchShipFiles()
        {
            shipData = new ShipData();
            UILogic.shipData = shipData;

            string FolderPath = KSPUtil.ApplicationRootPath + "saves/" + HighLogic.SaveFolder + "/Ships/";
            FileInfo[] fileInfo = new DirectoryInfo(FolderPath).GetFiles("*.craft", SearchOption.AllDirectories);

            foreach (FileInfo f in fileInfo)
                FetchShipFile(f);
            shipData.selectedTag.Add("_All", "_All");
            if (EditorLogic.fetch.ship.shipFacility == EditorFacility.SPH)
                shipData.selectedTag.Add("_SPH", "_SPH");
            else
                shipData.selectedTag.Add("_VAB", "_VAB");
        }

        private void FetchShipFile(FileInfo f)
        {
            string dir = f.FullName;
            ConfigNode config = ConfigNode.Load(dir);

            Ship s = new Ship();
            s.type = config.GetValue("type");
            s.tags = ExtractTags(config);
            s.tags.Add("_All");
            if (s.type == "SPH")
            {
                if (!s.tags.Contains("_SPH"))
                    s.tags.Add("_SPH");
            }
            else if (!s.tags.Contains("_VAB"))
                s.tags.Add("_VAB");
            s.name = config.GetValue("ship");
            s.file = f;
            s.description = getCraftDescription(s);
            s.thumbnail = getThumbnail(s.name, s.type);
            shipData.insert(s);
        }

        private List<string> ExtractTags(ConfigNode config)
        {
            string desc = config.GetValue("description");
            MatchCollection mc = Regex.Matches(desc, @"(#[^#\s¨]+)");
            List<string> ret = new List<string>();
            foreach (Match m in mc)
                ret.Add(m.Value.Substring(1));
            return ret;
        }

        private string getCraftDescription(Ship ship)
        {
            CraftEntry craft = CraftEntry.Create(ship.file, false, null);
            string ret = "<color=#FFBC00><size=14><b>" + ship.name + "</b></size></color>";
            if (craft.isValid == false)
                ret += "<color=#FF0000> Contains invalid parts</color>";
            string desc = craft.partCount + " parts in " + craft.stageCount + " stages.";
            while (desc.Length < 35 - (int)Mathf.Log10(craft.partCount) - (int)Mathf.Log10(craft.stageCount))
                desc += " ";
            ret += "\n<color=#C0C4B0>" + desc + "</color><color=#9EB757>Cost : " + craft.template.totalCost.ToString("0.00") + "</color>\n" +
                         "<color=#BD6428>";
            foreach (string tag in ship.tags)
                if(tag != "_All" && tag != "_SPH" && tag != "_VAB")
                    ret += "#" + tag + " ";
            ret += "</color>";
            return ret;
        }

        private Texture2D getThumbnail(string shipName, string subfolder)
        {
            string filePath = KSPUtil.ApplicationRootPath + "/thumbs/" + HighLogic.SaveFolder + "_" + subfolder + "_" + shipName + ".png";
            Texture2D thumbnail = new Texture2D(0, 0);
            if (System.IO.File.Exists(filePath))
                thumbnail.LoadImage(System.IO.File.ReadAllBytes(filePath));
            return thumbnail;
        }

        private bool newUnsavedWindow = true;
        GameObject unsavedWindow = null;
        public void Update()
        {
            // Since UILogic is static class, there's no update() in it
            if (UILogic.originalBrowser == null)
                UILogic.originalBrowser = (KSP.UI.Screens.CraftBrowserDialog)GameObject.FindObjectOfType(typeof(KSP.UI.Screens.CraftBrowserDialog));
            if (UILogic.originalBrowser != null)
            {
                if(UI != null)
                {
                    if (UILogic.enableOriginal == true)
                        UILogic.originalBrowser.gameObject.transform.localScale = Vector3.one;
                    else
                        UILogic.originalBrowser.gameObject.transform.localScale = Vector3.zero;
                }
            }

            // Find window which fires when loadButton is clicked while current craft is unsaved
            // There's no API that points the window, so just manually find it
            /*unsavedWindow = GameObject.Find("Load Craft dialog handler");
             if (unsavedWindow != null && newUnsavedWindow == true)
             {
                 newUnsavedWindow = false;
                 if (UI != null)
                     Destroy(UI);
                 foreach (Button b in unsavedWindow.GetComponentsInChildren<Button>())
                 {
                     Text t = b.GetComponentInChildren<Text>();
                     if (t != null)
                     {
                         if (t.text == "Save and Continue")
                         {
                             b.onClick.AddListener(onSaveBtn);
                             b.onClick.AddListener(ResetAllAndLoad);
                         }
                         if (t.text == "Don't Save")
                             b.onClick.AddListener(ResetAllAndLoad);
                     }
                 }
             }
             else if(unsavedWindow == null)
                 newUnsavedWindow = true;*/

            if (GameObject.Find("Delete File dialog handler") != null)
                needsFileReload = true;

        }

        public void ResetAllAndLoad()
        {
            if (UI != null)
                Destroy(UI);
            if (unsavedWindow != null)
                Destroy(unsavedWindow);
            foreach (KSP.UI.Screens.CraftBrowserDialog dialog in GameObject.FindObjectsOfType(typeof(KSP.UI.Screens.CraftBrowserDialog)))
                dialog.OnBrowseCancelled();
            /*foreach (Text t in GameObject.FindObjectsOfType<Text>())
            {
                
                if (t.text == "Select a Craft to Load")
                {
                    print(t.transform.parent.gameObject.name);
                    Destroy(t.transform.parent.gameObject);
                }
            }*/
            EditorLogic.fetch.loadBtn.onClick.Invoke();
        }

        public void onLoadBtn()
        {
            waitCount = 0;
            StartCoroutine("waitOriginalDialog");     
        }

        private int waitCount = 0;
        public IEnumerator waitOriginalDialog()
        {
            while (GameObject.Find("CraftBrowser(Clone)") == null)
            {
                if (waitCount < 30)
                    waitCount++;
                else
                    needsFileReload = true; // Case when 'unsaved craft dialog' appears
                yield return null;
            }
            if(needsFileReload)
                FetchShipFiles();
            needsFileReload = false;
            if(UI == null)
                UI = gameObject.AddComponent<SMUI>();
        }

        //private ConfigNode currentShip = null;
        public void onSaveBtn()
        {
            print("onsavebtn called");
            Ship ship = shipData.tryGetShip(ShipConstruction.ShipConfig.GetValue("ship"), ShipConstruction.ShipConfig.GetValue("type"));
            if (ship != null)
            {
                print("No Name Change");
                shipData.remove(ship);
                FetchShipFile(ship.file);
            }
            else
                FetchShipFiles();
        }

        public void onUIDestroy()
        {
            UI = null;
            foreach (KSP.UI.Screens.CraftBrowserDialog dialog in GameObject.FindObjectsOfType(typeof(KSP.UI.Screens.CraftBrowserDialog)))
                dialog.OnBrowseCancelled();
            foreach (GameObject go in GameObject.FindObjectsOfType<GameObject>())
            {
                if (go.name == "CraftBrowser(Clone)")
                    Destroy(go);
            }
        }

        //EditorLogic.fetch.loadBtn
        //EditorLogic.LoadShipFromFile(craftFile);
    }
}
