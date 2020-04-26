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
            public static IMyTextSurface GetScreen(IMyGridTerminalSystem gridTerminalSystem, IMyProgrammableBlock Me)
            {
                LCDConfigItem config  = new LCDConfigItem(Me);
                return GetScreen(gridTerminalSystem, Me, config);
            }

            public static IMyTextSurface GetScreen(IMyGridTerminalSystem gridTerminalSystem, IMyProgrammableBlock Me, LCDConfigItem config)
            {
                List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
                gridTerminalSystem.SearchBlocksOfName(config.BlockName, blocks, b => b.IsSameConstructAs(Me));
                if (blocks.Count == 0)
                {
                    throw new Exception($"No blocks with name \"{config.BlockName}\" found");
                }
                if (blocks.Count > 1)
                {
                    throw new Exception($"Multiple blocks with name \"{config.BlockName}\" found");
                }

                if (config.IsProvider)
                {

                    IMyTextSurfaceProvider surfaceProvider = blocks[0] as IMyTextSurfaceProvider;
                    return surfaceProvider.GetSurface(config.ProviderScreenIndex);
                }
                else
                {
                    return blocks[0] as IMyTextSurface;
                }
            }

            public static void WriteToScreen(LCDConfigItem config, string text, IMyProgrammableBlock Me, IMyGridTerminalSystem gridTerminalSystem)
            {
                IMyTextSurface screen = GetScreen(gridTerminalSystem, Me, config);
                screen.WriteText(text);
            }

        }
    }
}
