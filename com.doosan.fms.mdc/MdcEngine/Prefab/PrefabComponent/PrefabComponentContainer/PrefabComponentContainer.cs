using com.doosan.fms.mdc.MdcEngine.Prefab.PrefabComponent.ComponentBase;
using com.doosan.fms.mdc.MdcEngine.Prefab.PrefabComponent.PrefabComponentEnum;
using com.doosan.fms.mdc.MdcEngine.Prefab.PrefabObject.PrefabEnum;
using com.doosan.fms.model.Base;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace com.doosan.fms.mdc.MdcEngine.Prefab.PrefabComponent.PrefabComponentContainer
{
    public class PrefabComponentContainer
    {
        private List<PrefabComponentBase> _prefabComponents;

        [JsonInclude]
        public List<PrefabComponentBase> PrefabComponents { get => _prefabComponents; }

        public PrefabComponentContainer()
        {
            _prefabComponents = new List<PrefabComponentBase>();
        }


        public bool GetPrefabComponent(PrefabComponentType prefabComponentType, out PrefabComponentBase prefabComponentBase)
        {
            prefabComponentBase = _prefabComponents.Where(x => x.GetPrefabComponentType() == prefabComponentType).FirstOrDefault();
            return !(prefabComponentBase == null);
        }

        public JObject GetJson()
        {
            JObject result = new JObject();
            JArray prefabComponentsArray = new JArray();
            JProperty prefabComponents = new JProperty(nameof(PrefabComponents), prefabComponentsArray);

            result.Add(prefabComponents);

            foreach (var component in _prefabComponents)
            {
                prefabComponentsArray.Add(component.GetJson());
                //result = result + component.GetJson();
            }

            return result;
        }

        public void Update(FmsModel item, PrefabType prefabType, double deltaTime, double mps, double resolution)
        {
            Parallel.ForEach(_prefabComponents, component =>
                component.Update(item, prefabType, deltaTime, mps, resolution)
            );
        }

        public void AddPrefabComponents(List<PrefabComponentBase> items)
        {
            foreach (var item in items)
            {
                AddPrefabComponent(item);
            }
        }

        public bool AddPrefabComponent(PrefabComponentBase item)
        {
            var already = _prefabComponents.Where(x => x.GetPrefabComponentType() == item.GetPrefabComponentType()).FirstOrDefault();
            if (already == null)
            {
                _prefabComponents.Add(item);
                return true;
            }

            return false;
        }
    }
}
