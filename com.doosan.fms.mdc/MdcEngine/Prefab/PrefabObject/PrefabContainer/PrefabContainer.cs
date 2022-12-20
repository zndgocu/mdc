using com.doosan.fms.commonLib.Extension;
using com.doosan.fms.mdc.MapData.Dependency.com.doosan.fms.model;
using com.doosan.fms.mdc.MdcEngine.Prefab.PrefabObject.PrefabEnum;
using com.doosan.fms.model.Base;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace com.doosan.fms.mdc.MdcEngine.Prefab.PrefabObject.PrefabContainer
{
    public class PrefabContainer
    {

        //<--dictionary 구성 prefabbase.type, List<id>
        private Dictionary<PrefabType, List<PrefabBase.PrefabBase>> _prefabs;


        [JsonInclude]
        public Dictionary<PrefabType, List<PrefabBase.PrefabBase>> Prefabs { get => _prefabs; set => _prefabs = value; }

        public PrefabContainer()
        {
            _prefabs = new Dictionary<PrefabType, List<PrefabBase.PrefabBase>>();
            foreach (PrefabType type in Enum.GetValues(typeof(PrefabType)))
            {
                _prefabs.Add(type, new List<PrefabBase.PrefabBase>());
            }
        }

        public void Clear()
        {
            _prefabs.Clear();
        }

        public PrefabBase.PrefabBase GetPrefabComponent(PrefabType prefabType)
        {
            List<PrefabBase.PrefabBase> groupPrefabBase;
            if (_prefabs.TryGetValue(prefabType, out groupPrefabBase) == false) return null;

            return groupPrefabBase.Where(x => x.GetPrefabType() == prefabType).FirstOrDefault();
        }
        public void AddPrefabs(List<PrefabBase.PrefabBase> items)
        {
            foreach (var item in items)
            {
                AddPrefab(item);
            }
        }

        public bool AddPrefab(PrefabBase.PrefabBase item)
        {
            List<PrefabBase.PrefabBase> groupPrefabBase;
            if (_prefabs.TryGetValue(item.PrefabType, out groupPrefabBase) == false)
            {
                groupPrefabBase = new List<PrefabBase.PrefabBase>();
                _prefabs.Add(item.PrefabType, groupPrefabBase);
            }

            var already = groupPrefabBase.Where(x => x.GetKey() == item.GetKey()).FirstOrDefault();
            if (already == null)
            {
                groupPrefabBase.Add(item);
                return true;
            }

            return false;
        }

        public void Update(List<FmsModel> items, double elapsedMilsec, double mps, double resolution, PrefabUpdateType prefabUpdateType)
        {
            Parallel.ForEach(_prefabs, groupPrefabs =>
            {
                List<FmsModel> groupItems = items.Where(x => x.GetType() == TypeConverter.ConvertPrefabTypeToFmsModelType(groupPrefabs.Value[0].PrefabType)).ToList();
                Parallel.ForEach(groupPrefabs.Value, prefab =>
                {
                    if (prefabUpdateType == PrefabUpdateType.All || prefabUpdateType == prefab.PrefabUpdateType)
                    {
                        FmsModel prefabData = groupItems.Where(x => x.IGetKey() == prefab.GetKey()).FirstOrDefault();
                        if (prefabData != null)
                        {
                            prefab.Update(prefabData, elapsedMilsec, mps, resolution);
                        }
                    }
                });
            });
        }

        public JObject GetJson(PrefabUpdateType prefabSendType = PrefabUpdateType.All)
        {
            JObject result = new JObject();

            foreach (var prefabGroup in _prefabs)
            {
                JArray prefabArray = new JArray();
                JProperty prefabProp = new JProperty(prefabGroup.Key.GetDescription(), prefabArray);
                foreach (var prefab in prefabGroup.Value)
                {
                    if (prefabSendType == prefab.PrefabUpdateType || prefabSendType == PrefabUpdateType.All)
                    {
                        prefabArray.Add(prefab.IGetJson());
                    }
                }
                result.Add(prefabProp);
            }
            return result;
        }
    }
}
