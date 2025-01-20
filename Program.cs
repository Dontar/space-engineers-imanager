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
        bool manageRefineries = false;
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
            Runtime.UpdateFrequency = UpdateFrequency.Update10;
            Util.Init(this);
            TaskManager.AddTask(Util.DisplayLogo("IManager", Me.GetSurface(0)), 1.7f);

            if (manageAssemblers)
            {
                TaskManager.AddTask(QuotaManager(), 1.5f);
            }

            if (manageRefineries)
            {
                TaskManager.AddTask(RefineriesManager());
            }

            if (manageInventories)
            {
                TaskManager.AddTask(InventoryManager(), 1.3f);
            }

            TaskManager.AddTask(Util.StatusMonitor(this));
        }

        public void Main(string argument, UpdateType updateSource)
        {
            if (!updateSource.HasFlag(UpdateType.Update10)) return;
            TaskManager.RunTasks(Runtime.TimeSinceLastRun);
        }
    }
}
