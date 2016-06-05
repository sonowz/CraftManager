using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KSPtest
{
    [KSPAddon(KSPAddon.Startup.SpaceCentre, true)]
    public class WidgetOverride : MonoBehaviour
    {
        public void Start()
        {
            //if (GameEvents.Modifiers == null || GameEvents.Modifiers.OnCurrencyModified == null)
                print("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");
            //else
                GameEvents.Modifiers.OnCurrencyModified.Add(OnModified2);
            GameEvents.OnFundsChanged.Add(OnModified1);
        }

        public void OnModified1(double d, TransactionReasons t)
         {
             print("MMMM");
             print(d);
            
         }

        public void OnModified2(CurrencyModifierQuery c)
        {
            print("XXXX");
            /*if (Funding.Instance != null)
                Debug.Log(Funding.Instance.Funds.ToString());*/
        }

        public static void Log(string msg)
        {
            Debug.Log("[@@WidgetOverride@@]: " + msg);
        }
    }

}