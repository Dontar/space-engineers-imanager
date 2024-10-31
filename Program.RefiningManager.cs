using Sandbox.ModAPI.Ingame;
using System.Collections.Generic;
using System.Linq;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;

namespace IngameScript
{
    partial class Program
    {
        private class RefiningManager : Manager
        {
            public RefiningManager(Program p) : base(p) { }

            List<IMyRefinery> Refineries => Memo.Of(() => Util.GetBlocks<IMyRefinery>(b => b.CubeGrid == MySelf.CubeGrid), "refineries", 10);

            public override IEnumerable<object> Run()
            {
                var refineries = Refineries;
                StatusManager.CurrentStatus.RefineriesCount = refineries.Count.ToString();

                while (refineries.Equals(Refineries))
                {
                    foreach (var refinery in refineries)
                    {
                        StatusManager.CurrentStatus.CurrentRefineryName = refinery.CustomName;
                        var priorityList = Memo.Of(() =>
                        {
                            var ini = new MyIni();
                            if (!ini.TryParse(refinery.CustomData) || refinery.CustomData.Length == 0)
                            {
                                refinery.CustomData = SetupRefinery(refinery);
                            };
                            var iniKeys = new List<MyIniKey>();
                            ini.GetKeys(iniKeys);
                            return iniKeys.ConvertAll(key => new { key.Name, Value = ini.Get(key).ToInt32(0) });
                        }, refinery.CustomName, 4);

                        var inventory = refinery.InputInventory;

                        var items = new List<MyInventoryItem>();
                        inventory.GetItems(items);
                        StatusManager.CurrentStatus.CurrentRefineryItems = items.Count.ToString();
                        if (items.Count < 2) continue;

                        items.Sort((a, b) =>
                        {
                            var aPriority = priorityList.Find(p => p.Name == a.Type.SubtypeId)?.Value ?? 0;
                            var bPriority = priorityList.Find(p => p.Name == b.Type.SubtypeId)?.Value ?? 0;
                            var result = bPriority.CompareTo(aPriority);
                            if (result > 0)
                            {
                                inventory.TransferItemTo(inventory, items.IndexOf(a), items.IndexOf(b), true, a.Amount);
                            }
                            return result;
                        });

                        yield return null;
                    }
                }
            }

            private string SetupRefinery(IMyRefinery refinery)
            {
                var allowedItems = new List<MyItemType>();
                refinery.InputInventory.GetAcceptedItems(allowedItems);
                MyIni ini = new MyIni();
                foreach (var item in allowedItems.Select((item, idx) => new { item, idx }))
                {
                    ini.Set("Priority", item.item.SubtypeId, allowedItems.Count - item.idx);
                }
                ini.SetSectionComment("Priority", "Higher number means higher priority");
                return ini.ToString();
            }
        }
    }
}
