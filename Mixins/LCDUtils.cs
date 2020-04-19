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
        public class LCDUtils
        {
            public static IMyTextSurface GetScreen(MyIni _ini, IMyGridTerminalSystem GridTerminalSystem, IMyProgrammableBlock Me)
            {
                LCDConfigItem config  = new LCDConfigItem(_ini, Me);
                if (config.IsProvider)
                {
                    IMyTextSurfaceProvider surfaceProvider = GridTerminalSystem.GetBlockWithName(config.BlockName) as IMyTextSurfaceProvider;
                    return surfaceProvider.GetSurface(config.ProviderScreenIndex);
                }
                else
                {
                    return GridTerminalSystem.GetBlockWithName(config.BlockName) as IMyTextSurface;
                }
            }

        }
    }
}
