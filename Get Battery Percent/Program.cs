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

        MyIni _ini = new MyIni();
        List<IMyBatteryBlock> BATTERIES = new List<IMyBatteryBlock>();
        IMyTextSurface TargetScreen;
        public void Main(string argument)
        {
            if (argument == "refresh" || BATTERIES.Count == 0)
            {
                GridTerminalSystem.GetBlocksOfType<IMyBatteryBlock>(BATTERIES, b => b.IsSameConstructAs(Me));
                TargetScreen = LCDUtils.GetScreen(_ini, GridTerminalSystem, Me);
            }

            float totalCapacity = BlockUtils.GetSumOfBlockAttribute<IMyBatteryBlock>(b => b.MaxStoredPower, BATTERIES);
            float currentAmount = BlockUtils.GetSumOfBlockAttribute<IMyBatteryBlock>(b => b.CurrentStoredPower, BATTERIES);

            int chargePercent = (int)(currentAmount / totalCapacity * 100);
            TargetScreen.WriteText($"Battery: \n {chargePercent}%");
        }
    }
}
