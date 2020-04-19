using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRage;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        public class Config : IniConfig
        {
            public string ConfigTitle = "Config";
            public Dictionary<string, MyFixedPoint> ItemList = new Dictionary<string, MyFixedPoint>();
            public Config(IMyProgrammableBlock Me): base(Me)
            {
                List<MyIniKey> keys = new List<MyIniKey>();
                _ini.GetKeys(ConfigTitle, keys);
                for(int i = 0; i < keys.Count; i++)
                {
                    string keyName = keys[i].Name;
                    MyFixedPoint amount = _ini.Get(ConfigTitle, keyName).ToInt32();
                    ItemList.Add(keyName, amount);
                }
            }

        }
    }
}
