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

            List<IMyShipConnector> connectors = new List<IMyShipConnector>();
            GridTerminalSystem.GetBlocksOfType<IMyShipConnector>(connectors, b => b.IsSameConstructAs(Me));
            if (connectors.Count == 0 || connectors[0].Status != MyShipConnectorStatus.Connected)
            {
                throw new Exception("Ship needs a connector that is connected");
            }

            IMyShipConnector targetConnector = connectors[0].OtherConnector;

            List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
            List<IMyTerminalBlock> sourceBlocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(blocks, b => b.IsSameConstructAs(Me));
            GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(sourceBlocks, b => !b.IsSameConstructAs(Me));

            MyInventories = InventoryUtils.GetInventoriesFromBlocks(blocks);
            SourceInventories = InventoryUtils.GetInventoriesFromBlocks(sourceBlocks);

            if (argument.Equals("flush"))
            {
                FlushInventories();
            }
            else if(argument.Equals("import"))
            {
                ImportItemsIntoInventory();
            }

        }

        void ImportItemsIntoInventory()
        {
            Config config = new Config(Me);
            Dictionary<string, MyFixedPoint> ItemsToGet = config.ItemList;
            Dictionary<string, MyFixedPoint>.Enumerator en = ItemsToGet.GetEnumerator();
            while (en.MoveNext())
            {
                int CurrentAmountInInventory = InventoryUtils.FindNumberOfItemInInventoriesBySubType(MyInventories, en.Current.Key);
                int amountToGet = en.Current.Value.ToIntSafe() - CurrentAmountInInventory;
                if (amountToGet > 0)
                {
                    Echo($"Getting {amountToGet} {en.Current.Key}");
                    AddItemToInventory(en.Current.Key, amountToGet, config.FetchAmount);
                }
            }
        }

        bool AddItemToInventory(string type, int amountRequired, int fetchAmount)
        {
            int amountRemaining = amountRequired;
            for (int i = 0; i < SourceInventories.Count; i++)
            {
                IMyInventory sourceInventory = SourceInventories[i];
                Nullable<MyInventoryItem> item = InventoryUtils.FindItemInInventoryBySubType(sourceInventory, type);
                if (item.HasValue)
                {
                    int amountToAdd = item.Value.Amount.ToIntSafe() > amountRemaining ? amountRemaining : item.Value.Amount.ToIntSafe();
                    bool result = InventoryUtils.TransferItemToAvailableInventory(item.Value, (MyFixedPoint)amountToAdd, sourceInventory, MyInventories, fetchAmount);
                    if (result)
                    {
                        amountRemaining = amountRemaining - amountToAdd;
                    }
                }
                if (amountRemaining <= 0)
                {
                    return true;
                }
            };

            return false;
        }

        void FlushInventories()
        {
            Config config = new Config(Me);
            MyInventories.ForEach(i=>EmptyInventory(i, config.FetchAmount));
        }

        void EmptyInventory(IMyInventory inventory, int fetchAmount)
        {
            List<MyInventoryItem> items = new List<MyInventoryItem>();
            inventory.GetItems(items);
            items.ForEach(item => InventoryUtils.TransferItemToAvailableInventory(item, item.Amount, inventory, SourceInventories, fetchAmount));
        }
    }
}
