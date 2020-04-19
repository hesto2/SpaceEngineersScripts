﻿using Sandbox.Game.EntityComponents;
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
            string CONFIG_TITLE = "LCDConfig";
            public string BlockName;
            public bool IsProvider;
            public int ProviderScreenIndex;
            
            // Expects the ini to have a line for [LCDConfig]
            public LCDConfigItem(IMyProgrammableBlock Me) : base( Me)
            {
                BlockName = _ini.Get(CONFIG_TITLE, "BlockName").ToString();
                IsProvider= _ini.Get(CONFIG_TITLE, "IsProvider").ToBoolean();
                ProviderScreenIndex = _ini.Get(CONFIG_TITLE, "ProviderScreenIndex").ToInt32();
            }

        }
    }
}
