﻿using Sandbox.Game.EntityComponents;
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
        bool manageRefineries = true;
        bool manageInventories = true;

        string oresTag = "Ores";
        string ingotsTag = "Ingots";
        string componentsTag = "Components";
        string toolsTag = "Tools";
        string ammoTag = "Ammo";

        // end of config
        #endregion

        private QuotaManager quotaManager;
        private RefiningManager refiningManager;
        private InventoryManager inventoryManager;
        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update10;
            TaskManager.AddTask(Util.DisplayLogo("IManager", Me.GetSurface(0)));

            if (manageAssemblers)
            {
                quotaManager = new QuotaManager(this);
                TaskManager.AddTask(quotaManager.Run());
            }

            if (manageRefineries)
            {
                refiningManager = new RefiningManager(this);
                TaskManager.AddTask(refiningManager.Run());
            }

            if (manageInventories)
            {
                inventoryManager = new InventoryManager(this);
                TaskManager.AddTask(inventoryManager.Run());
            }
        }

        public void Main(string argument, UpdateType updateSource)
        {
            if (!updateSource.HasFlag(UpdateType.Update10)) return;
            TaskManager.RunTasks(Runtime.TimeSinceLastRun);
            Echo(StatusManager.RenderStatus(Runtime));
        }
    }
}