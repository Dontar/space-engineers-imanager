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

        struct QuotaConfig
        {
            public string Name;
            public int QuotaMin;
            public int QuotaMax;
        }
        IEnumerable<QuotaConfig> Config => Memo.Of(() =>
        {
            var ini = new MyIni();
            if (!ini.TryParse(Me.CustomData) || Me.CustomData == "")
            {
                foreach (var entry in Items)
                {
                    var sectionName = entry.Value.type == "AmmoMagazine" ? "Ammo" : entry.Value.type == "PhysicalGunObject" ? "Tools" : "Components";
                    ini.Set(sectionName, entry.Key, "0/0");
                }
                Me.CustomData = ini.ToString();
            }
            var iniKeys = new List<MyIniKey>();
            ini.GetKeys(iniKeys);
            var result = iniKeys.Select(v =>
            {
                var value = ini.Get(v).ToString().Split('/');
                return new QuotaConfig { Name = v.Name, QuotaMin = int.Parse(value[0]), QuotaMax = int.Parse(value[1]) };
            }).Where(v => v.QuotaMin > 0 || v.QuotaMax > 0).ToArray();

            CurrentStatus.QuotaItemsCount = result.Length;

            return result;
        }, "config", Memo.Refs(Me.CustomData));

        IEnumerable<IMyAssembler> Assemblers => Memo.Of(() =>
        {
            var assemblers = Util.GetBlocks<IMyAssembler>(b => 
                b.IsSameConstructAs(Me) && 
                Util.IsNotIgnored(b) && 
                (useSurvivalKits || b.BlockDefinition.TypeIdString != "MyObjectBuilder_SurvivalKit")
            ).ToArray();
            CurrentStatus.AssemblersCount = assemblers.Length.ToString();
            return assemblers;
        }, "assemblers", 100);

        IEnumerable<IMyInventory> Inventories => Memo.Of(() =>
        {
            var inventories = new List<IMyInventory>();
            var blocks = Util.GetBlocks<IMyTerminalBlock>(b => b.HasInventory && Util.IsNotIgnored(b) && b.IsSameConstructAs(Me));
            foreach (var b in blocks)
            {
                for (int i = 0; i < b.InventoryCount; i++)
                {
                    inventories.Add(b.GetInventory(i));
                }
            }
            return inventories.ToArray();
        }, "inventories", 100);

        IEnumerable<object> QuotaManager()
        {
            var quotaList = Config;
            while (quotaList.Equals(Config))
            {
                foreach (var quotaItem in quotaList)
                {
                    var QuotaMin = quotaItem.QuotaMin;
                    var QuotaMax = quotaItem.QuotaMax > 0 ? (int)MathHelper.Max(QuotaMin, quotaItem.QuotaMax) : 0;
                    var item = Items[quotaItem.Name];
                    var currentAmounts = GetInventoryItemsCount(item);
                    var neededAmount = QuotaMin - (currentAmounts[0] + currentAmounts[1]);
                    var excessAmount = currentAmounts[0] - QuotaMax;

                    if (neededAmount > 0)
                    {
                        QueueItems(item, neededAmount);
                    }
                    else if (QuotaMax > 0 && excessAmount > 0)
                    {
                        QueueItems(item, excessAmount, MyAssemblerMode.Disassembly);
                    }
                    yield return null;
                }
            }
        }

        void QueueItems(ItemMeta item, int neededAmount, MyAssemblerMode mode = MyAssemblerMode.Assembly)
        {
            var BlueprintId = item.GetBlueprintId();
            var assemblers = Assemblers.Where(a =>
                a.CanUseBlueprint(BlueprintId)
                && a.CooperativeMode == false
                && (a.Mode == mode || a.IsQueueEmpty)
            );
            var count = assemblers.Count();

            if (count < 1) return;

            var amount = neededAmount <= count ? neededAmount : (int)Math.Floor((decimal)neededAmount / count);
            foreach (var assembler in assemblers)
            {
                assembler.Mode = mode;
                assembler.AddQueueItem(BlueprintId, (MyFixedPoint)amount);
                // Break the loop if the needed amount is less than the number of assemblers
                // as the required items have already been queued.
                if (neededAmount <= count) break;
            }
        }

        int[] GetInventoryItemsCount(ItemMeta item)
        {
            var inventoryItemAmount = Inventories.Sum(inv => inv.GetItemAmount(item.GetItemType()).ToIntSafe());

            Func<MyProductionItem, int> sum = qItem => qItem.Amount.ToIntSafe();
            Func<MyProductionItem, bool> where = qItem => qItem.BlueprintId == item.GetBlueprintId();
            var queuedItemAmount = Assemblers.Sum(a =>
            {
                var qItems = new List<MyProductionItem>();
                a.GetQueue(qItems);
                return qItems.Where(where).Sum(sum);
            });

            return new int[] { inventoryItemAmount, queuedItemAmount };
        }
    }
}
