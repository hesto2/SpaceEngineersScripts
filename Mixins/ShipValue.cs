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

        public enum ShipDisplayValue {
            BatteryPercentRemaining,
            StorageCapacityUsed,
            ThrustCapacityUsed
        }

        public class ShipValue
        {
            Action<string> Echo;
            public ShipValue(ShipDisplayValue displayValue, IMyGridTerminalSystem GridTerminalSystem, IMyProgrammableBlock Me, Action<string> Echo)
            {
                this.Me = Me;
                this.GridTerminalSystem = GridTerminalSystem;
                this.DisplayValue = displayValue;
                this.Echo = Echo;
                GetRelevantBlocks();
            }

            private static Dictionary<ShipDisplayValue, string> ShipLCDDisplayValues = new Dictionary<ShipDisplayValue, string>{
                {ShipDisplayValue.BatteryPercentRemaining, "Battery"},
                {ShipDisplayValue.StorageCapacityUsed, "Cargo Capactiy"},
                {ShipDisplayValue.ThrustCapacityUsed, "Thrust Capactiy"} 
            };

            IMyGridTerminalSystem GridTerminalSystem;
            IMyProgrammableBlock Me;
            ShipDisplayValue DisplayValue;
            List<IMyTerminalBlock> blocks;


            public int GetValue(ShipDisplayValue value)
            {
                switch (value){
                    case ShipDisplayValue.BatteryPercentRemaining:
                        return BlockUtils.GetBatteryPercentRemaining(this.blocks.Cast<IMyBatteryBlock>().ToList());
                    case ShipDisplayValue.StorageCapacityUsed:
                        return BlockUtils.GetStorageCapacityRemaining(this.blocks.Cast<IMyCargoContainer>().ToList(), Echo);
                    case ShipDisplayValue.ThrustCapacityUsed:
                        return BlockUtils.GetThrustCapacityRemaining(this.blocks.Cast<IMyThrust>().ToList(), GridTerminalSystem);
                    default:
                        return 0;
                }
            }

            public string GetDisplayableValue()
            {
                int result = GetValue(DisplayValue);
                string lcdDisplayValue;
                ShipValue.ShipLCDDisplayValues.TryGetValue(DisplayValue, out lcdDisplayValue);
                return $"{lcdDisplayValue}:\n{result}%";
            }

            private void GetRelevantBlocks()
            {
                List<IMyTerminalBlock> found = new List<IMyTerminalBlock>();
                switch (DisplayValue){
                    case ShipDisplayValue.BatteryPercentRemaining:
                        GridTerminalSystem.GetBlocksOfType<IMyBatteryBlock>(found, b => b.IsSameConstructAs(Me));
                        this.blocks = found;
                        break;
                    case ShipDisplayValue.StorageCapacityUsed:
                        GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(found, b => b.IsSameConstructAs(Me));
                        this.blocks = found;
                        break;
                    case ShipDisplayValue.ThrustCapacityUsed:
                        GridTerminalSystem.GetBlocksOfType<IMyThrust>(found, b => b.IsSameConstructAs(Me));
                        this.blocks = found;
                        break;
                        
                }
            }
        }
    }
}
