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
        public class Config:IniConfig
        {

            public List<ConfigItem> DisplayItems = new List<ConfigItem>();
            public Config(IMyProgrammableBlock Me, IMyGridTerminalSystem gridTerminalSystem, Action<string> Echo) : base(Me)
            {
                List<string> keys = new List<string>();
                _ini.GetSections(keys);
                foreach(string key in keys)
                {
                    DisplayItems.Add(new ConfigItem(Me, gridTerminalSystem, key, Echo));
                }
            }
        }
    }
}
