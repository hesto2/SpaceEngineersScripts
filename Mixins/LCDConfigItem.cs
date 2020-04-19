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
        public class LCDConfigItem
        {
            public string BlockName;
            public bool IsProvider;
            public int ProviderScreenIndex;
            string CONFIG_TITLE = "LCDConfig";
            
            // Expects the ini to have a line for [LCDConfig]
            public LCDConfigItem(MyIni _ini_, IMyProgrammableBlock Me)
            {
                MyIniParseResult result;
                if (!_ini_.TryParse(Me.CustomData, out result))
                    throw new Exception(result.ToString());
                BlockName = _ini_.Get(CONFIG_TITLE, "BlockName").ToString();
                IsProvider= _ini_.Get(CONFIG_TITLE, "IsProvider").ToBoolean();
                ProviderScreenIndex = _ini_.Get(CONFIG_TITLE, "ProviderScreenIndex").ToInt32();
            }

        }
    }
}
