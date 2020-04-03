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
        public class Input
        {
            public bool W;
            public bool S;
            public bool A;
            public bool D;
            public bool Q;
            public bool E;
            public bool C;
            public bool Space;
            public float MouseX;
            public float MouseY;
            
            public Input(Vector3 MoveIndicator, Vector2 RotateIndicator, float RollIndicator)
            {
                this.W = MoveIndicator.Z < 0;
                this.S = MoveIndicator.Z > 0;
                this.A = MoveIndicator.X < 0;
                this.D = MoveIndicator.X > 0;
                this.Q = RollIndicator < 0;
                this.E = RollIndicator > 0;
                this.C = MoveIndicator.Y < 0;
                this.Space = MoveIndicator.Y > 0;
                this.MouseX = RotateIndicator.Y;
                this.MouseY = RotateIndicator.X;
            }
        }
        public class InputReader
        {
            IMyGridTerminalSystem GridTerminalSystem;
            IMyShipController ShipController;
            public InputReader(IMyGridTerminalSystem GridTerminalSystem,string CockpitName)
            {
                this.GridTerminalSystem = GridTerminalSystem;
                this.ShipController = GridTerminalSystem.GetBlockWithName(CockpitName) as IMyShipController;
            }

            public Input ReadInput()
            {
                //return $"Move: {this.ShipController.MoveIndicator.ToString()}\n Rotate: {this.ShipController.RotationIndicator.ToString()}\n Roll: {this.ShipController.RollIndicator.ToString()}";
                return new Input(this.ShipController.MoveIndicator, this.ShipController.RotationIndicator, this.ShipController.RollIndicator);
            }
        }
    }
}
