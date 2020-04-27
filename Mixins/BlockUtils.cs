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
        public static class BlockUtils
        {
            public static IMyShipController GetShipController(IMyGridTerminalSystem gridTerminalSystem, IMyProgrammableBlock Me)
            {
                List<IMyCockpit> cockpits = new List<IMyCockpit>();
                gridTerminalSystem.GetBlocksOfType<IMyCockpit>(cockpits, b => b.IsSameConstructAs(Me));
                IMyCockpit mainCockpit = cockpits.Find(c => c.IsMainCockpit) == null ? cockpits[0] : cockpits.Find(c => c.IsMainCockpit);
                return mainCockpit as IMyShipController;
            }
            public static float GetSumOfBlockAttribute<T>(Func<T, float> getAttributeFunc, List<T> LIST) where T : class
            {
                float sum = 0;
                foreach(T l in LIST)
                {
                    sum += getAttributeFunc(l);
                }

                return sum;
            }

            public static int GetBatteryPercentRemaining(List<IMyBatteryBlock> blocks)
            {
                float totalCapacity = BlockUtils.GetSumOfBlockAttribute<IMyBatteryBlock>(b => b.MaxStoredPower, blocks);
                float currentAmount = BlockUtils.GetSumOfBlockAttribute<IMyBatteryBlock>(b => b.CurrentStoredPower, blocks);

                int percent = (int)(currentAmount / totalCapacity * 100);
                return percent;
            }
            public static int GetStorageCapacityRemaining(List<IMyCargoContainer> blocks, Action<string> Echo)
            {
                float totalCapacity = BlockUtils.GetSumOfBlockAttribute<IMyCargoContainer>(b => b.GetInventory().MaxVolume.ToIntSafe(), blocks);
                float currentAmount = BlockUtils.GetSumOfBlockAttribute<IMyCargoContainer>(b => b.GetInventory().CurrentVolume.ToIntSafe(), blocks);

                int percent = (int)(currentAmount / (totalCapacity == 0 ? 1 : totalCapacity) * 100);
                return percent;
            }

            public static List<IMyThrust> GetThrustersEmittingInDirection(List<IMyThrust> thrusters, Base6Directions.Direction direction)
            {
                return thrusters.FindAll(b => b.Orientation.Forward == direction);
            }
            public static int GetThrustCapacityRemaining(List<IMyThrust> blocks, IMyShipController shipController, Action<string> Echo)
            {
                //TODO: Need to calculate this
                float localGravity = 9.81f;
                int newtonsRequiredToLiftShip= (int)(shipController.CalculateShipMass().TotalMass * localGravity);
                float maximumThrustInNewtons = BlockUtils.GetSumOfBlockAttribute<IMyThrust>(b => b.MaxEffectiveThrust, blocks);

                int percent = (int)(newtonsRequiredToLiftShip / maximumThrustInNewtons * 100);
                return percent;
            }
        }
    }
}
