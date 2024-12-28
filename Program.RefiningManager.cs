using Sandbox.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;

namespace IngameScript
{
    partial class Program
    {
        IEnumerable<IMyRefinery> Refineries => Memo.Of(() => Util.GetBlocks<IMyRefinery>(b => b.CubeGrid == Me.CubeGrid), "refineries", 10);
        IEnumerable<object> RefineriesManager()
        {
            var refineries = Refineries;
            CurrentStatus.RefineriesCount = refineries.Count().ToString();

            while (refineries.Equals(Refineries))
            {
                foreach (var refinery in refineries)
                {
                    CurrentStatus.CurrentRefineryName = refinery.CustomName;
                    var inventory = refinery.InputInventory;
                    var priorityList = Memo.Of(() =>
                    {
                        var ini = new MyIni();
                        if (!ini.TryParse(refinery.CustomData) || refinery.CustomData.Length == 0)
                        {
                            refinery.CustomData = SetupRefinery(refinery);
                        };
                        var iniKeys = new List<MyIniKey>();
                        ini.GetKeys(iniKeys);
                        iniKeys.Sort((a, b) => ini.Get(b).ToInt32().CompareTo(ini.Get(a).ToInt32()));
                        return iniKeys.ToDictionary(k => k.Name, v => ini.Get(v).ToInt32());
                    }, refinery.CustomName, Memo.Refs(refinery.CustomData));

                    var items = new List<MyInventoryItem>();
                    inventory.GetItems(items);
                    CurrentStatus.CurrentRefineryItems = items.Count.ToString();
                    if (items.Count < 2) continue;

                    items.Sort((a, b) =>
                    {
                        var aPriority = priorityList[a.Type.SubtypeId];
                        var bPriority = priorityList[b.Type.SubtypeId];
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
            refinery.UseConveyorSystem = false;
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
