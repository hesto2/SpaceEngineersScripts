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
        public class ConfigItem
        {
            public string BlockName;
            public bool IsGroup = false;
            public double Speed = 1;
            public bool Disabled = false;
            public List<IMyPistonBase> Pistons = new List<IMyPistonBase>();
            public List<IMyMotorStator> Rotors = new List<IMyMotorStator>();
            public bool IsPiston = false;

            public ConfigItem(MyIni _ini, string key, IMyGridTerminalSystem GridTerminalSystem) {
                this.BlockName = _ini.Get(key, "BlockName").ToString();
                this.IsGroup= _ini.Get(key, "IsGroup").ToBoolean();
                this.Speed= _ini.Get(key, "Speed").ToDouble(1);
                this.Disabled = _ini.Get(key, "Disabled").ToBoolean();
                this.IsPiston = _ini.Get(key, "IsPiston").ToBoolean();
                if (!this.IsGroup)
                {
                    if (this.IsPiston)
                    {
                        this.Pistons.Add(GridTerminalSystem.GetBlockWithName(this.BlockName) as IMyPistonBase) ;
                        if(Pistons.Count == 0)
                        {
                            throw new Exception($"Couldn't find piston group for {key}");
                        }
                    }
                    else
                    {
                        this.Rotors.Add(GridTerminalSystem.GetBlockWithName(this.BlockName) as IMyMotorStator) ;
                        if(Rotors.Count == 0)
                        {
                            throw new Exception($"Couldn't find rotor group for {key}");
                        }
                    }
                }
                else 
                {
                    IMyBlockGroup Blocks = GridTerminalSystem.GetBlockGroupWithName(this.BlockName);
                    if (this.IsPiston)
                    {
                        Blocks.GetBlocksOfType<IMyPistonBase>(this.Pistons);
                        if(Pistons.Count == 0)
                        {
                            throw new Exception($"Couldn't find pistons for {key}");
                        }
                    }
                    else
                    {
                        Blocks.GetBlocksOfType<IMyMotorStator>(this.Rotors);
                        if(Rotors.Count == 0)
                        {
                            throw new Exception($"Couldn't find rotors for {key}");
                        }
                    }
                }
            }

            private void ModifyPistonSpeed(IMyPistonBase p, double speed)
            {
                p.Velocity = (float)speed * (float)Speed;
            }

            private void ModifyRotorSpeed(IMyMotorStator p, double speed)
            {
                p.TargetVelocityRPM = (float)speed * (float)Speed;
            }
            public void Reset()
            {
                ModifySpeed(0);
            }
            public void ModifySpeed(float speed) {
                foreach(IMyPistonBase p in Pistons)
                {
                    if(p != null)
                    {
                        ModifyPistonSpeed(p, speed);
                    }
                }

                foreach(IMyMotorStator r in Rotors)
                {
                    if(r != null)
                    {
                        ModifyRotorSpeed(r, speed);
                    }
                }
            }
        }
    }
}
