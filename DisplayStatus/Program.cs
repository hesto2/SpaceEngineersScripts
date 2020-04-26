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
    partial class Program : MyGridProgram
    {

        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update100;
        }

        Config config;
        public void Main(string argument, UpdateType updateSource)
        {
            if(config == null || argument.Equals("refresh"))
            {
                config = initConfig();
            }
            
            foreach(ConfigItem item in config.DisplayItems)
            {
                LCDUtils.WriteToScreen(item, item.ShipValue.GetDisplayableValue(), Me, GridTerminalSystem);
            }
            
        }

        Config initConfig()
        {
            return new Config(Me, GridTerminalSystem, Echo);
        }
    }
}
