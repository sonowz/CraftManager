using System;
using System.IO;
using System.Collections.Generic;
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

        public void Start()
        {
            //GameEvents.onEditorShipModified.Add(OnShipModified);
            try
            {
                FetchShipFiles();
            }
            catch (Exception e)
            {
                print(e.StackTrace);
            }
            //UI = gameObject.AddComponent<SMUI>();
            EditorLogic.fetch.loadBtn.onClick.AddListener(onLoadBtn);
            EditorLogic.fetch.saveBtn.onClick.AddListener(onSaveBtn);

            /*CraftBrowserDialogHandler i= new CraftBrowserDialogHandler();
            print(i.GetHashCode());
            i = CraftBrowserDialogHandler._Spawn(EditorFacility.VAB, HighLogic.SaveFolder, i.OnFileSelected, i.OnBrowseCancelled, true);
            print(i.GetHashCode());*/
            /*CraftBrowserDialogHandler i = GameObject.Find("DialogCanvas").AddComponent<CraftBrowserDialogHandler>();
            i.init(shipData);*/
        }

        private void FetchShipFiles()
        {
            shipData = new ShipData();
            SMLogic.shipData = shipData;

            string FolderPath = KSPUtil.ApplicationRootPath + "saves/" + HighLogic.SaveFolder + "/Ships/";
            FileInfo[] fileInfo = new DirectoryInfo(FolderPath).GetFiles("*.craft", SearchOption.AllDirectories);

            foreach (FileInfo f in fileInfo)
                FetchShipFile(f);
            shipData.selectedTag.Add("_All", "_All");
            if(EditorLogic.fetch.ship.shipFacility == EditorFacility.SPH)
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
                if(!s.tags.Contains("_SPH"))
                        s.tags.Add("_SPH");
            else if(!s.tags.Contains("_VAB"))
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
            string desc = craft.partCount + " parts in " + craft.stageCount + " stages.";
            while (desc.Length < 35 - (int)Mathf.Log10(craft.partCount) - (int)Mathf.Log10(craft.stageCount))
                desc += " ";
            string ret = "<color=#FFBC00><size=14><b>" + ship.name + "</b></size></color>\n" +
                         "<color=#C0C4B0>" + desc + "</color><color=#9EB757>Cost : " + craft.template.totalCost.ToString("0.00") + "</color>\n" +
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

        public void Update()
        {
            SMLogic.originalBrowser = (KSP.UI.Screens.CraftBrowserDialog)GameObject.FindObjectOfType(typeof(KSP.UI.Screens.CraftBrowserDialog));
            if(SMLogic.originalBrowser != null)
                if(SMLogic.enableOriginal == true)
                    SMLogic.originalBrowser.gameObject.transform.localScale = Vector3.one;
                else
                    SMLogic.originalBrowser.gameObject.transform.localScale = Vector3.zero;
        }

        public void onLoadBtn()
        {
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

        //EditorLogic.fetch.loadBtn
        //EditorLogic.LoadShipFromFile(craftFile);
    }
}
