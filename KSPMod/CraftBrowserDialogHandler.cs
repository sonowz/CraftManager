using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KSP.UI.Screens;
using UnityEngine;

namespace KSPMod
{
    class CraftBrowserDialogHandler : CraftBrowserDialog
    {
        private CraftEntry selectedShip;
        public CraftBrowserDialogHandler()
        {
        }
        public void init(ShipData shipData)
        {
            print(shipData.query().First().Value.file.FullName);
            try
            {
                selectedShip = CraftEntry.Create(shipData.query().First().Value.file, false, onShipSelected);
                AddCraftEntryWidget(selectedShip, GetComponent<RectTransform>());
            } catch (Exception e) { print("asfsgdgd"); }
 
            
            print(transform.position);
            print(selectedShip.transform.position);
        }

        
        

        public void onShipSelected(CraftEntry ship)
        {

        }
    }
}
