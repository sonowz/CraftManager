using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KSPMod
{
    class ShipData
    {
        public SortedList<string, string> selectedTag = new SortedList<string, string>();

        private Dictionary<string, SortedList<string, Ship>> data = new Dictionary<string, SortedList<string, Ship>>();
        private List<Ship> allShipList = new List<Ship>();

        public void insert(Ship ship)
        {
            foreach (string tag in ship.tags)
            {
                if (data.ContainsKey(tag) == false)
                {
                    data.Add(tag, new SortedList<string, Ship>());
                }
                data[tag].Add(ship.name + ship.type, ship);
            }
            if (allShipList.Contains(ship) == false)
                allShipList.Add(ship);
        }

        public void remove(Ship ship)
        {
            foreach (string tag in ship.tags)
            {
                data[tag].Remove(ship.name + ship.type);
                if (data[tag].Count == 0)
                {
                    data.Remove(tag);
                    if(selectedTag.ContainsKey(tag))
                        selectedTag.Remove(tag);
                }
            }
            allShipList.Remove(ship);
        }

        public Ship tryGetShip(string name, string type)
        {
            foreach (Ship ship in allShipList)
                if (ship.name == name && ship.type == type)
                    return ship;
            return null;
        }

        /// <summary>
        /// return key is ship.name + ship.type
        /// </summary>
        public IEnumerable<KeyValuePair<string, Ship>> query()
        {
            if (selectedTag.Count == 0 || data.Count == 0)
                return null;
            IEnumerable<KeyValuePair<string, Ship>> ret = data[selectedTag.Keys[0]];
            foreach (string tag in selectedTag.Keys)
            {
                if (data.ContainsKey(tag) == false)
                    Debug.Log(tag);
                else
                ret = ret.Intersect(data[tag]);
            }
            return ret;
        }

        public IEnumerable<string> getTags()
        {
            return data.Keys;
        }
    }

    class Ship
    {
        public List<string> tags = new List<string>();
        public string name;
        public string description;
        public string type;
        public System.IO.FileInfo file;
        public Texture2D thumbnail;
    }
}
