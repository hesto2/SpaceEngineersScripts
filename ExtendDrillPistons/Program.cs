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
        List<IMyLandingGear> ALL_LANDING_GEAR = new List<IMyLandingGear>();
        IMyPistonBase BOTTOM_PISTON;
        IMyShipDrill DRILL;
        List<IMyPistonBase> DRILL_PISTONS = new List<IMyPistonBase>();
        Dictionary<IMyLandingGear, IMyPistonBase>PISTONS_BY_LANDING_GEAR = new Dictionary<IMyLandingGear, IMyPistonBase>();
        Dictionary<IMyPistonBase, IMyPistonBase>LANDING_GEAR_PISTONS_BY_DRILL_PISTON = new Dictionary<IMyPistonBase, IMyPistonBase>();
        IEnumerable<long> landingGearGridIds;
        string SAVED_ARGUMENT;
        

        public void Init()
        {
            List<IMyShipDrill> DRILLS = new List<IMyShipDrill>();
            PISTONS_BY_LANDING_GEAR.Clear();
            LANDING_GEAR_PISTONS_BY_DRILL_PISTON.Clear();
            GridTerminalSystem.GetBlocksOfType<IMyShipDrill>(DRILLS, block=>block.IsSameConstructAs(Me));
            GridTerminalSystem.GetBlocksOfType<IMyPistonBase>(ALL_PISTONS, block => block.IsSameConstructAs(Me));
            GridTerminalSystem.GetBlocksOfType<IMyLandingGear>(ALL_LANDING_GEAR, block => block.IsSameConstructAs(Me));
            landingGearGridIds = ALL_LANDING_GEAR.Select(g => g.CubeGrid.EntityId);

            if(DRILLS.Count == 0 || ALL_PISTONS.Count == 0)
            {
                throw new Exception("Need Drills and Pistons");
            }

            DRILL = DRILLS[0];
            BOTTOM_PISTON = ALL_PISTONS.Find(p => p.TopGrid.EntityId == DRILL.CubeGrid.EntityId);
            DRILL_PISTONS = GetPistonTree(BOTTOM_PISTON, ALL_PISTONS);
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
            IEnumerable<IMyPistonBase> nextPistons = allPistons.Where(p => p.TopGrid.EntityId == tree[0].CubeGrid.EntityId);
            IEnumerable<IMyPistonBase> siblingPistons = allPistons.Where(p => p.CubeGrid.EntityId == tree[0].CubeGrid.EntityId && p.EntityId != tree[0].EntityId);
            Echo($"Finding Next Piston");

            if (nextPistons.Count() == 0)
            {
                Echo("No More pistons found");
                return tree;
            }
           // this one has a landing gear piston on it
            else if(siblingPistons.Count() == 1)
            {
                IEnumerable<IMyPistonBase> pistons = nextPistons.Concat(siblingPistons);
                IMyPistonBase landingGearPiston= landingGearGridIds.Contains(pistons.ElementAt(0).TopGrid.EntityId) ? pistons.ElementAt(0) : pistons.ElementAt(1);
                IMyPistonBase drillPiston = landingGearPiston == pistons.ElementAt(0) ? pistons.ElementAt(1) : pistons.ElementAt(0);
                IMyLandingGear landingGear = ALL_LANDING_GEAR.Find(l => l.CubeGrid.EntityId == landingGearPiston.TopGrid.EntityId);
                tree.Insert(0, drillPiston);
                LANDING_GEAR_PISTONS_BY_DRILL_PISTON.Add(drillPiston, landingGearPiston);
                PISTONS_BY_LANDING_GEAR.Add(landingGear, landingGearPiston);
                return GetNextPistonInHierarchy(tree, allPistons);
            }
            else
            {
                tree.Insert(0, nextPistons.ElementAt(0));
                return GetNextPistonInHierarchy(tree, allPistons);
            }

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
                    IMyPistonBase lp;
                    if(LANDING_GEAR_PISTONS_BY_DRILL_PISTON.TryGetValue(p, out lp))
                    {
                        IMyLandingGear landingGear = PISTONS_BY_LANDING_GEAR.FirstOrDefault(x => x.Value == lp).Key;
                        landingGear.AutoLock = true;
                        if (!landingGear.IsLocked)
                        {
                            lp.Velocity = SPEED;
                        }
                    }
                    break;
                }
            }

            if(found == false)
            {
                Runtime.UpdateFrequency = UpdateFrequency.None;
            }
        }

        public void DisableAutoLockOnAllLandingGear()
        {
            foreach(IMyLandingGear l in PISTONS_BY_LANDING_GEAR.Keys)
            {
                l.AutoLock = false;
            }
        }
        public void RetractAllPistons()
        {
            foreach (IMyPistonBase p in DRILL_PISTONS) 
            {
                p.Velocity = 2;
                p.Retract();
            }
        }

        public void RetractAllLandingGear()
        {
            foreach(IMyLandingGear l in PISTONS_BY_LANDING_GEAR.Keys)
            {
                l.Unlock();
                l.AutoLock = false;
                PISTONS_BY_LANDING_GEAR[l].Velocity = 2;
                PISTONS_BY_LANDING_GEAR[l].Retract();
            }
        }

        //Stops the piston from extending once a landing gear is locked 
        public void CheckLandingGear()
        {
            foreach(IMyLandingGear l in PISTONS_BY_LANDING_GEAR.Keys)
            {
                if (l.IsLocked)
                {
                    PISTONS_BY_LANDING_GEAR[l].Velocity = 0;
                }
            }
        }

        public void Main(string argument, UpdateType updateSource)
        {
            if (argument.Equals("retract"))
            {
                RetractAllLandingGear();
                RetractAllPistons();
                Runtime.UpdateFrequency = UpdateFrequency.None;
                return;
            }
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
                DisableAutoLockOnAllLandingGear();
                Echo($"DRILL PISTONS: {DRILL_PISTONS.Count}");
                Echo($"LANDING GEAR PISTONS: {LANDING_GEAR_PISTONS_BY_DRILL_PISTON.Keys.Count}");
                return;
            }

            Runtime.UpdateFrequency = UpdateFrequency.Update10;
            ExtendNextPiston(SAVED_ARGUMENT);
            CheckLandingGear();
        }
    }
}
