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
        // This file contains your actual script.
        //
        // You can either keep all your code here, or you can create separate
        // code files to make your program easier to navigate while coding.
        //
        // In order to add a new utility class, right-click on your project, 
        // select 'New' then 'Add Item...'. Now find the 'Space Engineers'
        // category under 'Visual C# Items' on the left hand side, and select
        // 'Utility Class' in the main area. Name it in the box below, and
        // press OK. This utility class will be merged in with your code when
        // deploying your final script.
        //
        // You can also simply create a new utility class manually, you don't
        // have to use the template if you don't want to. Just do so the first
        // time to see what a utility class looks like.
        // 
        // Go to:
        // https://github.com/malware-dev/MDK-SE/wiki/Quick-Introduction-to-Space-Engineers-Ingame-Scripts
        //
        // to learn more about ingame scripts.
        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update1;
        }

        InputReader reader = null;
        IMyMotorStator rotorX;
        IMyMotorStator rotorY;
        IMyMotorStator rotorZ;
        List<IMyPistonBase> pistons = new List<IMyPistonBase>();
        public void Main(string argument, UpdateType updateSource)
        {
           if(reader == null || argument.Equals("refresh"))
            {
                reader = new InputReader(GridTerminalSystem, Echo);
                rotorX = GridTerminalSystem.GetBlockWithName("Rotor 2") as IMyMotorStator;
                rotorY = GridTerminalSystem.GetBlockWithName("Rotor") as IMyMotorStator;
                GridTerminalSystem.GetBlockGroupWithName("Pistons").GetBlocksOfType<IMyPistonBase>(pistons) ;
            }
            //IMyTextSurface screen = GridTerminalSystem.GetBlockWithName("LCD") as IMyTextSurface;
            //screen.WriteText(input);

            rotorX.TargetVelocityRPM = 0;
            rotorY.TargetVelocityRPM = 0;
            foreach(IMyPistonBase p in pistons)
            {
                p.Velocity = 0;
            }
            Input input = reader.ReadInput();
            if (input.D) {
                rotorX.TargetVelocityRPM = 5;
            }
            if (input.A)
            {
                rotorX.TargetVelocityRPM = -5;
            }
            if (input.C)
            {
                rotorY.TargetVelocityRPM = -5;
            }
            if (input.Space)
            {
                rotorY.TargetVelocityRPM = 5;
            }
            Echo(pistons.Count.ToString());
            if (input.W)
            {
                foreach(IMyPistonBase p in pistons)
                {
                    p.Velocity = 1f; 
                }
               
            }
            if (input.S)
            {
                foreach(IMyPistonBase p in pistons)
                {
                    p.Velocity = -1f;
                }
            }

        }
    }
}
