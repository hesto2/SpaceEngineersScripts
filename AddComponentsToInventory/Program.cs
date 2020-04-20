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
        List<IMyInventory> SourceInventories = new List<IMyInventory>();
        public void Main(string argument, UpdateType updateSource)
        {
               
            if(argument.Equals("refresh") || SourceInventories.Count == 0)
            {
                List<IMyShipConnector> connectors = new List<IMyShipConnector>();
                GridTerminalSystem.GetBlocksOfType<IMyShipConnector>(connectors, b => b.IsSameConstructAs(Me));
                if (connectors.Count == 0 || connectors[0].Status != MyShipConnectorStatus.Connected)
                {
                    throw new Exception("Ship needs a connector");
                }

                IMyShipConnector targetConnector = connectors[0].OtherConnector;

                List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
                List<IMyTerminalBlock> sourceBlocks = new List<IMyTerminalBlock>();
                GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(blocks, b=>b.IsSameConstructAs(Me));
                GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(sourceBlocks, b => !b.IsSameConstructAs(Me));

                MyInventories = GetInventoriesFromBlocks(blocks);
                SourceInventories = GetInventoriesFromBlocks(sourceBlocks);
            }

            Dictionary<string, MyFixedPoint> ItemsToGet = new Config(Me).ItemList;
            Dictionary<string, MyFixedPoint>.Enumerator en = ItemsToGet.GetEnumerator();
            while (en.MoveNext())
            {
                int CurrentAmountInInventory = InventoryUtils.FindNumberOfItemInInventoriesBySubType(MyInventories, en.Current.Key);
                int amountToGet = en.Current.Value.ToIntSafe() - CurrentAmountInInventory;
                Echo($"Amount to get {amountToGet}");
                if(amountToGet > 0)
                {
                    AddItemToInventory(en.Current.Key, amountToGet);
                }
            }
        }

        bool AddItemToInventory(string type, int amountRequired)
        {
            int amountRemaining = amountRequired;
            for(int i = 0; i < SourceInventories.Count; i++)
            {
                IMyInventory sourceInventory = SourceInventories[i];
                Nullable<MyInventoryItem> item = InventoryUtils.FindItemInInventoryBySubType(sourceInventory, type);
                if(item.HasValue)
                {
                    int amountToAdd = item.Value.Amount.ToIntSafe() > amountRemaining ? amountRemaining : item.Value.Amount.ToIntSafe();
                    bool result = InventoryUtils.TransferItemToAvailableInventory(item.Value, (MyFixedPoint) amountToAdd, sourceInventory, MyInventories);
                    if (result)
                    {
                        amountRemaining = amountRemaining - amountToAdd;
                    }
                }
                if(amountRemaining <= 0)
                {
                    return true;
                }
            };

            return false;
        }
        
        List<IMyInventory> GetInventoriesFromBlocks(List<IMyTerminalBlock> blocks)
        {
            return blocks.FindAll(b => b.InventoryCount > 0).Select(b=>b.GetInventory()).ToList();
        }
    }
}
