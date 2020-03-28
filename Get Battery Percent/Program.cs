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

        List<IMyBatteryBlock> TMP_LIST = new List<IMyBatteryBlock>();
        static IMyProgrammableBlock thisCPU = null;
        void Main(string argument)
        {
            if (argument == "find new" || TMP_LIST.Count == 0)
            {
                GridTerminalSystem.GetBlocksOfType<IMyBatteryBlock>(TMP_LIST);
            }
            float totalCapacity = 0;
            float currentAmount = 0;
            foreach (IMyBatteryBlock b in TMP_LIST)
            {
                if (IsLocal(b, GridTerminalSystem))
                {
                    totalCapacity += b.MaxStoredPower;
                    currentAmount += b.CurrentStoredPower;
                }
            }
            int chargePercent = (int)(currentAmount / totalCapacity * 100);
            IMyTextSurface screen = GridTerminalSystem.GetBlockWithName("BatteryLCD") as IMyTextSurface;
            screen.WriteText($"Battery: \n {chargePercent}%");
        }

        // Returns the Programmble Block which is currently executing this script.
        public static IMyProgrammableBlock getThisCPU(IMyGridTerminalSystem gts)
        {
            if (thisCPU != null)
            {
                return thisCPU;
            }
            List<IMyProgrammableBlock> pbs = new List<IMyProgrammableBlock>();
            gts.GetBlocksOfType<IMyProgrammableBlock>(pbs);
            foreach(IMyProgrammableBlock pb in pbs)
            {
                if (pb.IsRunning)
                {
                    thisCPU = pb;
                    return pb;
                };
            }
            return null;
        }
        public static bool filterPB(IMyProgrammableBlock b)
        {
            return (b as IMyProgrammableBlock).IsRunning;
        }

        // Returns true if a block is on the same grid as the running script's Programmable Block
        public static bool IsLocal(IMyBatteryBlock b, IMyGridTerminalSystem gts)
        {
            return (b.CubeGrid == getThisCPU(gts).CubeGrid);
        }

    }
}
