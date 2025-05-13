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
    partial class Program
    {
        class StatusData
        {
            public string AssemblersCount;
            public int QuotaItemsCount;
            // public Dictionary<string, Memo.CacheValue> cache = Memo._dependencyCache;
            public StringBuilder debug = new StringBuilder();
            public int Containers;
            public int OreContainers;
            public int IngotContainers;
            public int CompContainers;
            public int ToolsContainers;
            // public string CurrentInventory;
            // public string CurrentMaterial;
        }

        static readonly StatusData CurrentStatus = new StatusData();

        const string BIG_DIVIDER = "===============================";
        const string SMALL_DIVIDER = "-----------------------";

        static void Log(string message)
        {
            CurrentStatus.debug.Clear();
            CurrentStatus.debug.AppendLine(message);
        }

        void RenderStatus()
        {
            var runtimeText = new StringBuilder();
            runtimeText.AppendLine("Dontar's Inventory Manager");
            runtimeText.AppendLine(BIG_DIVIDER);
            runtimeText.AppendLine();
            runtimeText.AppendLine("QuotaManager");
            runtimeText.AppendLine(SMALL_DIVIDER);
            runtimeText.AppendLine($"  Managing: {CurrentStatus.AssemblersCount ?? "0"} Assemblers");
            runtimeText.AppendLine($"  Processing: {CurrentStatus.QuotaItemsCount} Quotas");
            runtimeText.AppendLine();
            runtimeText.AppendLine("InventoryManager");
            runtimeText.AppendLine(SMALL_DIVIDER);
            runtimeText.AppendLine($"  Monitoring: {CurrentStatus.Containers} Containers");
            runtimeText.AppendLine("  Processing:");
            runtimeText.AppendLine($"    Ores: {CurrentStatus.OreContainers}");
            runtimeText.AppendLine($"    Ingots: {CurrentStatus.IngotContainers}");
            runtimeText.AppendLine($"    Components: {CurrentStatus.CompContainers}");
            runtimeText.AppendLine($"    Tools: {CurrentStatus.ToolsContainers}");

            runtimeText.AppendLine(CurrentStatus.debug.ToString());

            Util.Echo(runtimeText.ToString());
        }
    }
}