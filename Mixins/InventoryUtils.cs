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
        public class InventoryUtils
        {
            public static int FindNumberOfItemInInventoriesBySubType(List<IMyInventory> inventories, string subTypeId)
            {
                int total = 0;
                inventories.ForEach(i =>
                {
                    Nullable<MyInventoryItem> item = FindItemInInventoryBySubType(i, subTypeId);
                    if (item.HasValue)
                    {
                        total = total + item.Value.Amount.ToIntSafe();
                    }
                });
                return total;
            }
            public static Nullable<MyInventoryItem> FindItemInInventoryBySubType(IMyInventory inventory, string itemSubType){
                List<MyInventoryItem> items = new List<MyInventoryItem>();
                inventory.GetItems(items);
                for(int i = 0; i < items.Count; i++)
                {
                    MyInventoryItem item = items[i];
                    if(item.Type.SubtypeId == itemSubType)
                    {
                        return item;
                    }
                }
                return null;
            }
            public static bool TransferItemToAvailableInventory(MyInventoryItem item, MyFixedPoint amount, IMyInventory sourceInventory, List<IMyInventory> targetInventories)
            {
                // Break it up into small chunks and insert them so that it can check if items can be added better
                const int INSERTION_AMOUNT = 10;
                int amountRemaining = amount.ToIntSafe();
                bool allFull = false;
                
                while(amountRemaining > 0 && allFull == false)
                {
                    MyFixedPoint amountToInsert = amountRemaining < INSERTION_AMOUNT ? amountRemaining : INSERTION_AMOUNT;
                    IMyInventory targetInventory = targetInventories.Find(i => i.CanItemsBeAdded(amountToInsert, item.Type) && i.CanTransferItemTo(sourceInventory, item.Type));
                    if (targetInventory != null)
                    {
                        int itemIndex = GetItemIndex(item, sourceInventory);
                        sourceInventory.TransferItemTo(targetInventory, item, amountToInsert);
                        amountRemaining = amountRemaining - amountToInsert.ToIntSafe();
                        continue;
                    }
                    allFull = true;
                }
               
                return amountRemaining <= 0;
            }

            public static int GetItemIndex(MyInventoryItem item, IMyInventory inventory)
            {
                List<MyInventoryItem> items = new List<MyInventoryItem>();
                inventory.GetItems(items);
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i].ItemId == item.ItemId)
                    {
                        return i;
                    }
                }
                return -1;
            }
        }
    }
}
