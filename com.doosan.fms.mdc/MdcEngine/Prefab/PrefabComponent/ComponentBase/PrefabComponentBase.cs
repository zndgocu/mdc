using com.doosan.fms.mdc.MdcEngine.Prefab.PrefabComponent.PrefabComponentEnum;
using com.doosan.fms.mdc.MdcEngine.Prefab.PrefabObject.PrefabEnum;
using com.doosan.fms.model.Base;
using Newtonsoft.Json.Linq;
using System.Text.Json.Serialization;

namespace com.doosan.fms.mdc.MdcEngine.Prefab.PrefabComponent.ComponentBase
{
    public class PrefabComponentBase : IPrefabComponentBase
    {
        private PrefabComponentType _prefabComponentType;

        [JsonInclude]
        public PrefabComponentType PrefabComponentType { get => _prefabComponentType; }

        public PrefabComponentBase(PrefabComponentType prefabComponentType)
        {
            _prefabComponentType = prefabComponentType;
        }


        public PrefabComponentType GetPrefabComponentType()
        {
            return _prefabComponentType;
        }

        public void IUpdate(FmsModel item, PrefabType prefabType, double deltaTime, double mps, double resolution)
        {
            Update(item, prefabType, deltaTime, mps, resolution);
        }

        public virtual void Update(FmsModel item, PrefabType prefabType, double deltaTime, double mps, double resolution)
        {

        }

        public JObject IGetJson()
        {
            return GetJson();
        }

        public virtual JObject GetJson()
        {
            return null;
        }
    }
}
