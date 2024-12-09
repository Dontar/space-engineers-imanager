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
            public string RefineriesCount;
            public string CurrentRefineryName;
            public string CurrentRefineryItems;
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

        static readonly IEnumerator<string> Spinner = new string[] { "/", "-", "\\", "|" }.AsEnumerable().GetEnumerator();

        static void Log(string message)
        {
            CurrentStatus.debug.Clear();
            CurrentStatus.debug.AppendLine(message);
        }

        static string RenderStatus(IMyGridProgramRuntimeInfo Runtime)
        {
            if (!Spinner.MoveNext())
            {
                Spinner.Reset();
                Spinner.MoveNext();
            }

            var runtimeText = new StringBuilder();
            runtimeText.AppendLine("Dontar's Inventory Manager - " + Spinner.Current);
            runtimeText.AppendLine(BIG_DIVIDER);
            runtimeText.AppendLine();
            runtimeText.AppendLine("QuotaManager");
            runtimeText.AppendLine(SMALL_DIVIDER);
            runtimeText.AppendLine($"  Managing: {CurrentStatus.AssemblersCount ?? "0"} Assemblers");
            runtimeText.AppendLine($"  Processing: {CurrentStatus.QuotaItemsCount} Quotas");
            runtimeText.AppendLine();
            runtimeText.AppendLine("RefineryManager");
            runtimeText.AppendLine(SMALL_DIVIDER);
            runtimeText.AppendLine($"  Managing: {CurrentStatus.RefineriesCount} Refineries");
            runtimeText.AppendLine("  Processing:");
            runtimeText.AppendLine($"    Refinery: {CurrentStatus.CurrentRefineryName ?? "None"}");
            runtimeText.AppendLine($"    Sortable: {CurrentStatus.CurrentRefineryItems ?? "0"}");
            runtimeText.AppendLine();
            runtimeText.AppendLine("InventoryManager");
            runtimeText.AppendLine(SMALL_DIVIDER);
            runtimeText.AppendLine($"  Monitoring: {CurrentStatus.Containers} Containers");
            runtimeText.AppendLine("  Processing:");
            runtimeText.AppendLine($"    Ores: {CurrentStatus.OreContainers}");
            runtimeText.AppendLine($"    Ingots: {CurrentStatus.IngotContainers}");
            runtimeText.AppendLine($"    Components: {CurrentStatus.CompContainers}");
            runtimeText.AppendLine($"    Tools: {CurrentStatus.ToolsContainers}");
            // runtimeText.AppendLine($"  Inventory: {CurrentStatus.CurrentInventory}");
            // runtimeText.AppendLine($"    Material: {CurrentStatus.CurrentMaterial}");
            runtimeText.AppendLine();
            // runtimeText.AppendLine("CacheManager");
            // runtimeText.AppendLine(SMALL_DIVIDER);
            // if (CurrentStatus.cache != null)
            // {
            //     foreach (var cacheItem in CurrentStatus.cache)
            //     {
            //         runtimeText.AppendLine($"  {cacheItem.Key}: {cacheItem.Value.Age}");
            //     }
            // }
            // else
            // {
            //     runtimeText.AppendLine("  No cache items found yet.");
            // }
            // runtimeText.AppendLine();
            runtimeText.AppendLine("Runtime Info");
            runtimeText.AppendLine(SMALL_DIVIDER);
            runtimeText.AppendLine($"Last Run: {Runtime.LastRunTimeMs}ms");
            runtimeText.AppendLine($"Time Since Last Run: {Runtime.TimeSinceLastRun.TotalMilliseconds}ms");
            runtimeText.AppendLine($"Instruction Count: {Runtime.CurrentInstructionCount}/{Runtime.MaxInstructionCount}");
            runtimeText.AppendLine();
            runtimeText.AppendLine(CurrentStatus.debug.ToString());

            return runtimeText.ToString();
        }
    }
}