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
using Sandbox.Game.GameSystems;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {

        public Program()
        {
        }

        List<IMyAssembler> Assemblers = new List<IMyAssembler>();
        List<IMyInventory> ContainerInventories = new List<IMyInventory>();
        const int ITEM_AMOUNT_THRESHOLD = 250;
        public void Main(string argument, UpdateType updateSource)
        {
            if (argument.Equals("refresh"))
            {
                GridTerminalSystem.GetBlocksOfType<IMyAssembler>(Assemblers, (IMyAssembler assembler) => assembler.IsSameConstructAs(Me));
                List<IMyCargoContainer> Containers = new List<IMyCargoContainer>();
                GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(Containers, (IMyCargoContainer container) => container.IsSameConstructAs(Me));
                ContainerInventories = Containers.Select(container => container.GetInventory()).ToList();
            }
            
            Assemblers.ForEach(UnClogAssembler);
        }

        void UnClogAssembler(IMyAssembler assembler)
        {
            List<MyInventoryItem> items = new List<MyInventoryItem>();
            IMyInventory assemblerInventory = assembler.InputInventory;
            assemblerInventory.GetItems(items, (MyInventoryItem item) => item.Amount >= ITEM_AMOUNT_THRESHOLD);
            items.ForEach((MyInventoryItem item) => TransferItemToAvailableInventory(item, (MyFixedPoint) (item.Amount - ITEM_AMOUNT_THRESHOLD), assemblerInventory, ContainerInventories));
        }

        void TransferItemToAvailableInventory(MyInventoryItem item, MyFixedPoint amount, IMyInventory inventory, List<IMyInventory> targetInventories )
        {
            IMyInventory targetInventory = targetInventories.Find(i => i.CanItemsBeAdded(amount, item.Type));
            if(targetInventory != null)
            {
                int itemIndex = GetItemIndex(item, inventory);
                inventory.TransferItemTo(targetInventory, item, amount);
            }
        }

        int GetItemIndex(MyInventoryItem item, IMyInventory inventory)
        {
            List<MyInventoryItem> items = new List<MyInventoryItem>();
            inventory.GetItems(items);
            for(int i = 0; i < items.Count; i++)
            {
               if(items[i].ItemId == item.ItemId)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
