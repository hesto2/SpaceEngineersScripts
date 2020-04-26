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
        public class LCDConfigItem : IniConfig
        {
            string ConfigTitle = "LCDConfig";
            public string BlockName;
            public bool IsProvider;
            public int ProviderScreenIndex;
            
            // Expects the ini to have a line for [LCDConfig]
            public LCDConfigItem(IMyProgrammableBlock Me) : base(Me)
            {
                BlockName = _ini.Get(ConfigTitle, "BlockName").ToString();
                IsProvider= _ini.Get(ConfigTitle, "IsProvider").ToBoolean();
                ProviderScreenIndex = _ini.Get(ConfigTitle, "ProviderScreenIndex").ToInt32();
            }

            public LCDConfigItem(IMyProgrammableBlock Me, string configTitle) : base(Me)
                {
                ConfigTitle = configTitle;
                BlockName = _ini.Get(ConfigTitle, "BlockName").ToString();
                IsProvider = _ini.Get(ConfigTitle, "IsProvider").ToBoolean();
                ProviderScreenIndex = _ini.Get(ConfigTitle, "ProviderScreenIndex").ToInt32();
            }

        }
    }
}
