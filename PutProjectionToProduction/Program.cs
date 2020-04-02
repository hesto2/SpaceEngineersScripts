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
    class ProjectorDetails
    {
        public Dictionary<string, int> BlocksRemaining = new Dictionary<string, int>();

        public ProjectorDetails(String detailedInfo, Action<string> Echo)
        {
            string pattern = @"(.*): (.*)";
            System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(pattern, System.Text.RegularExpressions.RegexOptions.Multiline);
            System.Text.RegularExpressions.Match m = r.Match(detailedInfo);

            while (m.Success)
            {
                int numberRequired;
                if (m.Groups.Count == 3 && int.TryParse(m.Groups[2].Value, out numberRequired))
                {
                    BlocksRemaining.Add(m.Groups[1].Value, numberRequired);
                }
                m = m.NextMatch();
            }
        }
    }
    partial class Program : MyGridProgram
    {
        IMyAssembler assembler;
        public void AddToProduction(string componentType, int amount)
        {
            MyDefinitionId definition = MyDefinitionId.Parse($"MyObjectBuilder_BlueprintDefinition/{ComponentUtils.GetComponentSubType(componentType)}");
            assembler.AddQueueItem(definition, (double)amount);
        }
        
        public void Main(string argument, UpdateType updateSource)
        {
            //Create support for an ini file that contains the name of the target assembler and an optional LCD to indicate percentage as well as the id of the projector
            // Also create a flag for large or small grid requirements
            Dictionary<string, int> REQUIRED_COMPONENTS = new Dictionary<string, int>();
            List<IMyProjector> LIST = new List<IMyProjector>();
            GridTerminalSystem.GetBlocksOfType<IMyProjector>(LIST);
            assembler = GridTerminalSystem.GetBlockWithName("Basic Assembler") as IMyAssembler;
            if (LIST.Count == 0)
            {
                throw new Exception("Projector is required");
            }

            // Parse detailed info from projector
            IMyProjector projector = LIST[0];
            ProjectorDetails details = new ProjectorDetails(projector.DetailedInfo, Echo);

            // Convert parsed info into a list of required components
            foreach (String blockName in details.BlocksRemaining.Keys)
            {

                Dictionary<string, int> requiredComponentsForBlock;
                if (ComponentUtils.SmallShipComponentPieces.TryGetValue(blockName, out requiredComponentsForBlock))
                {
                    foreach (string componentName in requiredComponentsForBlock.Keys)
                    {
                        int currentAmount;
                        REQUIRED_COMPONENTS.TryGetValue(componentName, out currentAmount);
                        // Multiply the new required components by the total amount of that block we are needing to build
                        currentAmount += (requiredComponentsForBlock.GetValueOrDefault(componentName) * details.BlocksRemaining[blockName]);
                        REQUIRED_COMPONENTS[componentName] = currentAmount;
                    }
                }
            }


            foreach(string k in REQUIRED_COMPONENTS.Keys)
            {
                this.AddToProduction(k, REQUIRED_COMPONENTS[k]);
            }
        }
    }
}
/*
    SAMPLE DETAILED INFO
    Type: Projector
    Max Required Input: 100 W

    Build progress: 0/98
    BlocksRemaining:
    Armor blocks: 83
    Wheel Suspension 3x3 Right: 2
    Wheel Suspension 3x3 Left: 2
    Control Seat: 1
    Battery: 1
    Ore Detector: 1
    Beacon: 1
    Small Cargo Container: 2
    Connector: 1
    Antenna: 1
    Spotlight: 1
    Rotor: 1
    Interior Light: 1
 */
