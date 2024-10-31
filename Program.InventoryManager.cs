using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Eventing.Reader;
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
        private class InventoryManager : Manager
        {

            private class ContainersMeta
            {
                public IEnumerable<IMyInventory> Inventories;
                // public IEnumerable<IMyGasGenerator> H2Generators;
                // public IEnumerable<IMyReactor> Reactors;
                public IEnumerable<IMyCargoContainer> CargoContainers;
                public IEnumerable<IMyCargoContainer> OreContainers;
                public IEnumerable<IMyCargoContainer> IngotContainers;
                public IEnumerable<IMyCargoContainer> ComponentContainers;
                public IEnumerable<IMyCargoContainer> ToolsContainers;
                public IEnumerable<IMyCargoContainer> AmmoContainers;
            }

            public InventoryManager(Program p) : base(p) { }

            private ContainersMeta Containers => Memo.Of(() =>
            {
                var inventoryBlocks = Util.GetBlocks<IMyTerminalBlock>(block => block.HasInventory && block.CubeGrid == MySelf.CubeGrid);
                var inventories = inventoryBlocks.Where(b =>
                    !(b is IMyGasGenerator)
                    && !(b is IMyReactor)
                    && !(b is IMyParachute)
                    && (b.BlockDefinition.TypeIdString != "MyObjectBuilder_LargeMissileTurret")
                    && (b.BlockDefinition.TypeIdString != "MyObjectBuilder_LargeGatlingTurret")
                ).SelectMany(block =>
                {
                    var inv = new List<IMyInventory>();
                    for (int i = 0; i < block.InventoryCount; i++)
                    {
                        inv.Add(block.GetInventory(i));
                    }
                    return inv;
                });

                var containersMeta = new ContainersMeta
                {
                    Inventories = inventories,
                    // H2Generators = inventoryBlocks.OfType<IMyGasGenerator>(),
                    // Reactors = inventoryBlocks.OfType<IMyReactor>(),
                    CargoContainers = inventoryBlocks.OfType<IMyCargoContainer>().Where(c => c.BlockDefinition.SubtypeId.Contains("Container")),
                    OreContainers = inventoryBlocks.OfType<IMyCargoContainer>().Where(c => c.CustomName.Contains(p.oresTag)),
                    IngotContainers = inventoryBlocks.OfType<IMyCargoContainer>().Where(c => c.CustomName.Contains(p.ingotsTag)),
                    ComponentContainers = inventoryBlocks.OfType<IMyCargoContainer>().Where(c => c.CustomName.Contains(p.componentsTag)),
                    ToolsContainers = inventoryBlocks.OfType<IMyCargoContainer>().Where(c => c.CustomName.Contains(p.toolsTag)),
                    AmmoContainers = inventoryBlocks.OfType<IMyCargoContainer>().Where(c => c.CustomName.Contains(p.ammoTag))

                };
                return containersMeta;
            }, "ContainersMeta", 12);

            public override IEnumerable<object> Run()
            {
                var containers = Containers;
                StatusManager.CurrentStatus.Containers = Containers.CargoContainers.Count();
                StatusManager.CurrentStatus.OreContainers = Containers.OreContainers.Count();
                StatusManager.CurrentStatus.IngotContainers = Containers.IngotContainers.Count();
                StatusManager.CurrentStatus.CompContainers = Containers.ComponentContainers.Count();
                StatusManager.CurrentStatus.ToolsContainers = Containers.ToolsContainers.Count();

                while (containers.Equals(Containers))
                {
                    foreach (var inventory in containers.Inventories)
                    {
                        if (inventory.ItemCount == 0 || (IsInputInventory(inventory) && !IsFull(inventory))) continue;

                        var items = new List<MyInventoryItem>();
                        inventory.GetItems(items);

                        foreach (var item in items)
                        {
                            IEnumerable<IMyCargoContainer> materialContainers = null;
                            var amount = item.Amount;
                            switch (item.Type.TypeId)
                            {
                                case "MyObjectBuilder_Ore":
                                    materialContainers = Containers.OreContainers;
                                    break;
                                case "MyObjectBuilder_Ingot":
                                    materialContainers = Containers.IngotContainers;
                                    break;
                                case "MyObjectBuilder_Component":
                                    materialContainers = Containers.ComponentContainers;
                                    break;
                                case "MyObjectBuilder_PhysicalGunObject":
                                    materialContainers = Containers.ToolsContainers;
                                    break;
                                case "MyObjectBuilder_AmmoMagazine":
                                    materialContainers = Containers.AmmoContainers;
                                    break;
                                default:
                                    break;
                            }
                            if (materialContainers != null && !materialContainers.Any(c => c == inventory.Owner))
                            {
                                var freeContainer = materialContainers.FirstOrDefault(c => c.GetInventory().CanItemsBeAdded(amount, item.Type));
                                if (freeContainer != default(IMyCargoContainer))
                                    inventory.TransferItemTo(freeContainer.GetInventory(), item);
                            }
                        }
                        yield return null;
                    }
                }
            }

            private bool IsInputInventory(IMyInventory i)
            {
                return i.Owner is IMyProductionBlock && (i.Owner as IMyProductionBlock).InputInventory == i;
            }

            private bool IsFull(IMyInventory i)
            {
                return i.VolumeFillFactor >= 0.95f;
            }
        }
    }
}
