using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Library;
using VRageMath;
using VRageRender.ExternalApp;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        #region mdk preserve
        // Configuration
        bool manageAssemblers = true;
        bool useSurvivalKits = false;
        bool manageInventories = true;

        string oresTag = "Ores";
        string ingotsTag = "Ingots";
        string componentsTag = "Components";
        string toolsTag = "Tools";
        string ammoTag = "Ammo";

        // end of config
        #endregion

        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update100;
            Util.Init(this);
            TaskManager.AddTask(Util.StatusMonitor(this));

            if (manageAssemblers)
            {
                TaskManager.AddTask(QuotaManager(), 1.5f);
            }

            if (manageInventories)
            {
                TaskManager.AddTask(InventoryManager(), 1.3f);
            }

            TaskManager.AddTask(Util.DisplayLogo("IManager", Me.GetSurface(0)), 1.7f);
        }

        public void Main(string argument, UpdateType updateSource)
        {
            if (!updateSource.HasFlag(UpdateType.Update100)) return;
            RenderStatus();
            TaskManager.RunTasks(Runtime.TimeSinceLastRun);
        }
    }
}
