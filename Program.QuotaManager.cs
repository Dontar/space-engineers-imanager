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
        const string AM = "AmmoMagazine";
        const string PGO = "PhysicalGunObject";
        const string C = "Component";
        struct ItemMeta
        {
            public string type;
            public string BlueprintId;
            public string ItemID;
            public ItemMeta(string type, string BlueprintId, string ItemID = null)
            {
                this.type = type;
                this.BlueprintId = BlueprintId;
                this.ItemID = ItemID ?? BlueprintId;
            }
            public MyItemType GetItemType()
            {
                return MyItemType.Parse($"MyObjectBuilder_{type}/{ItemID}");
            }

            public MyDefinitionId GetBlueprintId()
            {
                return MyDefinitionId.Parse($"MyObjectBuilder_BlueprintDefinition/{BlueprintId}");
            }
        }
        readonly Dictionary<string, ItemMeta> Items = new Dictionary<string, ItemMeta> {
                { "Steel Plate",           new ItemMeta(C, "SteelPlate")},
                { "Interior Plate",        new ItemMeta(C, "InteriorPlate")},
                { "Construction Comp.",    new ItemMeta(C, "ConstructionComponent", "Construction")},
                { "Computer",              new ItemMeta(C, "ComputerComponent", "Computer")},
                { "Display",               new ItemMeta(C, "Display")},
                { "Motor",                 new ItemMeta(C, "MotorComponent", "Motor")},
                { "Girder",                new ItemMeta(C, "GirderComponent", "Girder")},
                { "Small Steel Tube",      new ItemMeta(C, "SmallTube")},
                { "Large Steel Tube",      new ItemMeta(C, "LargeTube")},
                { "Bulletproof Glass",     new ItemMeta(C, "BulletproofGlass")},
                { "Metal Grid",            new ItemMeta(C, "MetalGrid")},
                { "Power Cell",            new ItemMeta(C, "PowerCell")},
                { "Solar Cell",            new ItemMeta(C, "SolarCell")},
                { "Radio-comm Comp.",      new ItemMeta(C, "RadioCommunicationComponent", "RadioCommunication")},
                { "Detector Comp.",        new ItemMeta(C, "DetectorComponent", "Detector")},
                { "Canvas",                new ItemMeta(C, "Position0030_Canvas", "Canvas")},

                { "Medical Comp.",         new ItemMeta(C, "MedicalComponent", "Medical")},
                { "Thruster Comp.",        new ItemMeta(C, "ThrustComponent", "Thrust")},
                { "Explosives",            new ItemMeta(C, "ExplosivesComponent", "Explosives")},
                { "Gravity Comp.",         new ItemMeta(C, "GravityGeneratorComponent", "GravityGenerator")},
                { "Reactor Comp.",         new ItemMeta(C, "ReactorComponent", "Reactor")},
                { "Superconductor",        new ItemMeta(C, "Superconductor")},

                { "Gatling Ammo Box",      new ItemMeta(AM, "Position0080_NATO_25x184mmMagazine", "NATO_25x184mm")},
                { "Autocannon Magazine",   new ItemMeta(AM, "Position0090_AutocannonClip", "AutocannonClip")},
                { "Assault Cannon Shell",  new ItemMeta(AM, "Position0110_MediumCalibreAmmo", "MediumCalibreAmmo")},
                { "Artillery Shell",       new ItemMeta(AM, "Position0120_LargeCalibreAmmo", "LargeCalibreAmmo")},
                { "Rocket",                new ItemMeta(AM, "Position0100_Missile200mm", "Missile200mm")},
                { "Small Railgun Sabot",   new ItemMeta(AM, "Position0130_SmallRailgunAmmo", "SmallRailgunAmmo")},
                { "Large Railgun Sabot",   new ItemMeta(AM, "Position0140_LargeRailgunAmmo", "LargeRailgunAmmo")},

                { "MR-20 Rifle Magazine",  new ItemMeta(AM, "Position0040_AutomaticRifleGun_Mag_20rd", "AutomaticRifleGun_Mag_20rd")},
                { "MR-30E Rifle Magazine", new ItemMeta(AM, "Position0070_UltimateAutomaticRifleGun_Mag_30rd", "UltimateAutomaticRifleGun_Mag_30rd")},
                { "MR-50A Rifle Magazine", new ItemMeta(AM, "Position0050_RapidFireAutomaticRifleGun_Mag_50rd", "RapidFireAutomaticRifleGun_Mag_50rd")},
                { "MR-8P Rifle Magazine",  new ItemMeta(AM, "Position0060_PreciseAutomaticRifleGun_Mag_5rd", "PreciseAutomaticRifleGun_Mag_5rd")},
                { "S-10 Pistol Magazine",  new ItemMeta(AM, "Position0010_SemiAutoPistolMagazine", "SemiAutoPistolMagazine")},
                { "S-10E Pistol Magazine", new ItemMeta(AM, "Position0030_ElitePistolMagazine", "ElitePistolMagazine")},
                { "S-20A Pistol Magazine", new ItemMeta(AM, "Position0020_FullAutoPistolMagazine", "FullAutoPistolMagazine")},
                { "Flare Gun Clip",        new ItemMeta(AM, "Position0005_FlareGunMagazine", "FlareClip")},

                { "Fireworks Blue",        new ItemMeta(AM, "Position0007_FireworksBoxBlue", "FireworksBoxBlue")},
                { "Fireworks Green",       new ItemMeta(AM, "Position00071_FireworksBoxGreen", "FireworksBoxGreen")},
                { "Fireworks Pink",        new ItemMeta(AM, "Position00074_FireworksBoxPink", "FireworksBoxPink")},
                { "Fireworks Rainbow",     new ItemMeta(AM, "Position00075_FireworksBoxRainbow", "FireworksBoxRainbow")},
                { "Fireworks Red",         new ItemMeta(AM, "Position00072_FireworksBoxRed", "FireworksBoxRed")},
                { "Fireworks Yellow",      new ItemMeta(AM, "Position00073_FireworksBoxYellow", "FireworksBoxYellow")},

                { "Hydrogen Bottle",       new ItemMeta("GasContainerObject", "Position0020_HydrogenBottle", "HydrogenBottle")},
                { "Oxygen Bottle",         new ItemMeta("OxygenContainerObject", "Position0010_OxygenBottle", "OxygenBottle")},

                { "Elite Grinder",         new ItemMeta(PGO, "Position0040_AngleGrinder4", "AngleGrinder4Item")},
                { "Elite Hand Drill",      new ItemMeta(PGO, "Position0080_HandDrill4", "HandDrill4Item")},
                { "Elite Welder",          new ItemMeta(PGO, "Position0120_Welder4", "Welder4Item")},

                { "Enhanced Grinder",      new ItemMeta(PGO, "Position0020_AngleGrinder2", "AngleGrinder2Item")},
                { "Enhanced Hand Drill",   new ItemMeta(PGO, "Position0060_HandDrill2", "HandDrill2Item")},
                { "Enhanced Welder",       new ItemMeta(PGO, "Position0100_Welder2", "Welder2Item")},

                { "Proficient Grinder",    new ItemMeta(PGO, "Position0030_AngleGrinder3", "AngleGrinder3Item")},
                { "Proficient Hand Drill", new ItemMeta(PGO, "Position0070_HandDrill3", "HandDrill3Item")},
                { "Proficient Welder",     new ItemMeta(PGO, "Position0110_Welder3", "Welder3Item")},

                { "MR-20 Rifle",           new ItemMeta(PGO, "Position0040_AutomaticRifle", "AutomaticRifleItem")},
                { "MR-30E Rifle",          new ItemMeta(PGO, "Position0070_UltimateAutomaticRifle", "UltimateAutomaticRifleItem")},
                { "MR-50A Rifle",          new ItemMeta(PGO, "Position0050_RapidFireAutomaticRifle", "RapidFireAutomaticRifleItem")},
                { "MR-8P Rifle",           new ItemMeta(PGO, "Position0060_PreciseAutomaticRifle", "PreciseAutomaticRifleItem")},

                { "S-10 Pistol",           new ItemMeta(PGO, "Position0010_SemiAutoPistol", "SemiAutoPistolItem")},
                { "S-10E Pistol",          new ItemMeta(PGO, "Position0030_EliteAutoPistol", "ElitePistolItem")},
                { "S-20A Pistol",          new ItemMeta(PGO, "Position0020_FullAutoPistol", "FullAutoPistolItem")},

                { "Flare Gun",             new ItemMeta(PGO, "Position0005_FlareGun", "FlareGunItem")},

                { "RO-1 Rocket Launcher",  new ItemMeta(PGO, "Position0080_BasicHandHeldLauncher", "BasicHandHeldLauncherItem")},
                { "PRO-1 Rocket Launcher", new ItemMeta(PGO, "Position0090_AdvancedHandHeldLauncher", "AdvancedHandHeldLauncherItem")},

                { "Welder",                new ItemMeta(PGO, "Position0090_Welder", "WelderItem")},
                { "Grinder",               new ItemMeta(PGO, "Position0010_AngleGrinder", "AngleGrinderItem")},
                { "Hand Drill",            new ItemMeta(PGO, "Position0050_HandDrill", "HandDrillItem")},
            };

        Dictionary<string, int> Config => Memo.Of(() =>
        {
            var ini = new MyIni();
            if (!ini.TryParse(Me.CustomData) || Me.CustomData == "")
            {
                foreach (var entry in Items)
                {
                    var sectionName = entry.Value.type == "AmmoMagazine" ? "Ammo" : entry.Value.type == "PhysicalGunObject" ? "Tools" : "Components";
                    ini.Set(sectionName, entry.Key, 0);
                }
                Me.CustomData = ini.ToString();
            }
            var iniKeys = new List<MyIniKey>();
            ini.GetKeys(iniKeys);
            var result = iniKeys.ToDictionary(k => k.Name, v => ini.Get(v).ToInt32());
            return result;
        }, "config", Memo.Refs(Me.CustomData));

        IEnumerable<IMyAssembler> Assemblers => Memo.Of(() => Util.GetBlocks<IMyAssembler>(AssemblerFilter), "assemblers", 100);
        IEnumerable<IMyInventory> Inventories => Memo.Of(() =>
        {
            var invBlocks = Util.GetBlocks<IMyTerminalBlock>(block => block.HasInventory && block.IsSameConstructAs(Me) && Assemblers.Any(a => a.OutputInventory.IsConnectedTo(block.GetInventory())));
            return invBlocks.SelectMany(block =>
            {
                var inventories = new List<IMyInventory>();
                for (int i = 0; i < block.InventoryCount; i++)
                {
                    var inventory = block.GetInventory(i);
                    if (inventory != null && inventory.ItemCount > 0)
                    {
                        inventories.Add(inventory);
                    }
                }
                return inventories;
            }).ToArray();
        }, "inventories", 4);

        IEnumerable<object> QuotaManager()
        {
            var quotaList = Config;
            while (quotaList.Equals(Config))
            {
                var priorityList = new Dictionary<MyDefinitionId, int>();
                foreach (var quotaItem in quotaList)
                {
                    int quota = Math.Max(quotaItem.Value, 0);
                    var Disassemble = quotaItem.Value < 0;
                    var item = Items[quotaItem.Key];
                    var currentAmounts = GetInventoryItemsNeeded(item);
                    var neededAmount = quota - (currentAmounts[0] - currentAmounts[1]);

                    if (neededAmount > 0)
                    {
                        priorityList.Add(item.GetBlueprintId(), currentAmounts[0] * 100 / quota);

                        QueueItems(item, neededAmount);
                    }
                    else if (Disassemble && neededAmount < 0)
                    {
                        QueueItems(item, Math.Abs(neededAmount), MyAssemblerMode.Disassembly);
                    }
                }
                if (priorityList.Count > 0) SortAssemblerQueue(priorityList);
                yield return null;
            }
        }

        void SortAssemblerQueue(Dictionary<MyDefinitionId, int> priorityList)
        {
            foreach (var assembler in Assemblers)
            {
                var qItems = new List<MyProductionItem>();
                assembler.GetQueue(qItems);
                qItems.Sort((a, b) =>
                {
                    var result = priorityList[a.BlueprintId].CompareTo(priorityList[b.BlueprintId]);
                    if (result < 0) assembler.MoveQueueItemRequest(a.ItemId, qItems.IndexOf(b));
                    return result;
                });
            }
        }

        bool AssemblerFilter(IMyAssembler block)
        {
            return block.IsSameConstructAs(Me)
                // && block.CooperativeMode == false
                // && block.Mode == MyAssemblerMode.Assembly
                && (useSurvivalKits || block.BlockDefinition.TypeIdString != "MyObjectBuilder_SurvivalKit");
        }

        void QueueItems(ItemMeta item, decimal neededAmount, MyAssemblerMode mode = MyAssemblerMode.Assembly)
        {
            CurrentStatus.AssemblersCount = Assemblers.Count().ToString();

            var BlueprintId = item.GetBlueprintId();
            var assemblers = Assemblers.Where(a =>
                a.CanUseBlueprint(BlueprintId)
                && a.CooperativeMode == false
                && (a.Mode == mode || a.IsQueueEmpty)
            );
            var count = assemblers.Count();

            if (neededAmount < 1 || count < 1) return;

            if (neededAmount < 2)
            {
                var assembler = assemblers.First();
                assembler.Mode = mode;
                assembler.AddQueueItem(BlueprintId, neededAmount);
                return;
            }

            var amount = Math.Round(neededAmount / count);
            foreach (var assembler in assemblers)
            {
                assembler.Mode = mode;
                assembler.AddQueueItem(BlueprintId, amount);
            }
        }

        int[] GetInventoryItemsNeeded(ItemMeta item)
        {
            int inventoryItemAmount = 0;
            int queuedItemAmount = 0;

            inventoryItemAmount += Inventories.Sum(inv => inv.GetItemAmount(item.GetItemType()).ToIntSafe());

            foreach (var assembler in Assemblers)
            {
                var qItems = new List<MyProductionItem>();
                assembler.GetQueue(qItems);
                queuedItemAmount += qItems.Where(qItem => qItem.BlueprintId == item.GetBlueprintId()).Sum(qItem => qItem.Amount.ToIntSafe());
            }
            return new int[] { inventoryItemAmount, queuedItemAmount };
        }
    }
}
