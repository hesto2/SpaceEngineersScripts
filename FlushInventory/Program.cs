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
        }
        List<IMyInventory> MyInventories = new List<IMyInventory>();
        List<IMyInventory> TargetInventories = new List<IMyInventory>();
        public void Main(string argument, UpdateType updateSource)
        {
            //if (argument.Equals("refresh") || TargetInventories.Count == 0)
            //{
            List<IMyShipConnector> connectors = new List<IMyShipConnector>();
            GridTerminalSystem.GetBlocksOfType<IMyShipConnector>(connectors, b => b.IsSameConstructAs(Me));
            if (connectors.Count == 0 || connectors[0].Status != MyShipConnectorStatus.Connected)
            {
                throw new Exception("Ship needs a connector");
            }
            
            IMyShipConnector targetConnector = connectors[0].OtherConnector;

            List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
            List<IMyTerminalBlock> targetBlocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(blocks, b => b.IsSameConstructAs(Me));
            GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(targetBlocks, b => !b.IsSameConstructAs(Me));

            MyInventories = InventoryUtils.GetInventoriesFromBlocks(blocks);
            TargetInventories = InventoryUtils.GetInventoriesFromBlocks(targetBlocks);
            //}

            MyInventories.ForEach(EmptyInventory);
        }

        void EmptyInventory(IMyInventory inventory)
        {
            List<MyInventoryItem> items = new List<MyInventoryItem>();
            inventory.GetItems(items);
            items.ForEach(item => InventoryUtils.TransferItemToAvailableInventory(item, item.Amount, inventory, TargetInventories));
        }
    }
}
