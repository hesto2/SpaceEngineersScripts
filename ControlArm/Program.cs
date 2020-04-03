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


        MyIni _ini = new MyIni();
        InputReader reader = null;
        Dictionary<BindableKey, ConfigItem> Bindings = new Dictionary<BindableKey, ConfigItem>();
        enum BindableKey
        {
            AD, 
            WS, 
            CSpace, 
            QE, 
            MouseX, 
            MouseY, 
        }

        public void MapConfigs()
        {
            foreach(BindableKey k in Enum.GetValues(typeof(BindableKey)))
            {
                Bindings[k] = new ConfigItem(_ini, k.ToString(), GridTerminalSystem);
            }
        }



        public void Main(string argument, UpdateType updateSource)
        {
           if(reader == null || argument.Equals("refresh"))
            {
                Bindings.Clear();
                MyIniParseResult result;
                if(!_ini.TryParse(Me.CustomData, out result))
                {
                    throw new Exception(result.ToString());
                }

                this.MapConfigs();
                
                reader = new InputReader(GridTerminalSystem, _ini.Get("Cockpit", "BlockName").ToString("Cockpit"));
            }

            Input input = reader.ReadInput();

            Dictionary<BindableKey, bool> positiveMappings = new Dictionary<BindableKey, bool>() { 
                {BindableKey.AD, input.D },
                {BindableKey.WS, input.W },
                {BindableKey.CSpace, input.Space },
                {BindableKey.QE, input.E },
            };

            Dictionary<BindableKey, bool> negativeMappings = new Dictionary<BindableKey, bool>() { 
                {BindableKey.AD, input.A },
                {BindableKey.WS, input.S },
                {BindableKey.CSpace, input.C},
                {BindableKey.QE, input.Q },
            };

            foreach(BindableKey k in Enum.GetValues(typeof(BindableKey)))
            {
                if (!Bindings.ContainsKey(k)){
                    continue;
                }

                Bindings[k].Reset();
                // Handle Mouse bindings
                if(k == BindableKey.MouseX)
                {
                    Bindings[k].ModifySpeed(input.MouseX);
                    continue;
                }

                if(k == BindableKey.MouseY)
                {
                    Bindings[k].ModifySpeed(input.MouseY);
                    continue;
                }


                //Handle Keyboard Bindings
                if (positiveMappings[k]) 
                {
                    Bindings[k].ModifySpeed(1f);
                }
                else if(negativeMappings[k])
                {
                    Bindings[k].ModifySpeed(-1f);
                }
            }

        }
    }
}
