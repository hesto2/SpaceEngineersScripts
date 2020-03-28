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
        List<IMyPistonBase> ALL_PISTONS = new List<IMyPistonBase>();
        IMyPistonBase BOTTOM_PISTON;
        IMyShipDrill DRILL;
        List<IMyPistonBase> DRILL_PISTONS = new List<IMyPistonBase>();
        string SAVED_ARGUMENT;
        

        public void Init()
        {
            List<IMyShipDrill> DRILLS = new List<IMyShipDrill>();
            GridTerminalSystem.GetBlocksOfType<IMyShipDrill>(DRILLS, block=>block.IsSameConstructAs(Me));
            GridTerminalSystem.GetBlocksOfType<IMyPistonBase>(ALL_PISTONS, block => block.IsSameConstructAs(Me));

            if(DRILLS.Count == 0 || ALL_PISTONS.Count == 0)
            {
                throw new Exception("Need Drills and Pistons");
            }

            DRILL = DRILLS[0];
            BOTTOM_PISTON = ALL_PISTONS.Find(p => p.TopGrid.EntityId == DRILL.CubeGrid.EntityId);
            DRILL_PISTONS = GetPistonTree(BOTTOM_PISTON, ALL_PISTONS);
            Echo(DRILL_PISTONS.Count.ToString());
        }


        public List<IMyPistonBase> GetPistonTree(IMyPistonBase basePiston, List<IMyPistonBase> allPistons)
        {
            List<IMyPistonBase> tree = new List<IMyPistonBase>();
            tree.Add(basePiston);
            GetNextPistonInHierarchy(tree, allPistons);
            return tree;
        }

        public List<IMyPistonBase> GetNextPistonInHierarchy(List<IMyPistonBase>tree, List<IMyPistonBase> allPistons)
        {
            IMyPistonBase nextPiston = allPistons.Find(p => p.TopGrid.EntityId == tree[0].CubeGrid.EntityId);
            if (nextPiston == null)
            {
                Echo("No piston found");
                return tree;
            }
            Echo("Finding Next Piston");

            tree.Insert(0, nextPiston);
            return GetNextPistonInHierarchy(tree, allPistons);
        }

        public void StopAll()
        {
            foreach (IMyPistonBase p in DRILL_PISTONS)
            {
                p.Velocity = 0;
            }
        }

        public void ExtendNextPiston(string argument)
        {

            float SPEED;
            float.TryParse(argument, out SPEED);
            Echo(argument);
            if(SPEED == 0)
            {
                SPEED = .5f;
            }

            if(DRILL_PISTONS.Count == 0)
            {
                return;
            }

            bool found = false;
            foreach(IMyPistonBase p in DRILL_PISTONS)
            {
               if(p.Status == PistonStatus.Extended)
                {
                    continue;
                }
                else
                {
                    Echo($"Extending at {SPEED}m/s");
                    p.Velocity = SPEED;
                    found = true;
                    break;
                }
            }

            if(found == false)
            {
                Runtime.UpdateFrequency = UpdateFrequency.None;
            }
        }

        public void Main(string argument, UpdateType updateSource)
        {
            if(updateSource == UpdateType.Trigger || updateSource == UpdateType.Terminal)
            {
                SAVED_ARGUMENT = argument;
            }
            if (argument.Equals("stop") || (Runtime.UpdateFrequency != UpdateFrequency.None && (updateSource == UpdateType.Trigger || updateSource == UpdateType.Terminal)))
            {
                Echo("Stopping");
                Runtime.UpdateFrequency = UpdateFrequency.None;
                StopAll();
                return;
            }

            if(DRILL_PISTONS.Count == 0 || argument.Equals("refresh"))
            {
                Init();
            }

            Runtime.UpdateFrequency = UpdateFrequency.Update10;
            ExtendNextPiston(SAVED_ARGUMENT);
        }
    }
}
