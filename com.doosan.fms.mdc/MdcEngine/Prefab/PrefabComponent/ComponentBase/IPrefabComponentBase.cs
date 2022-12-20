using com.doosan.fms.mdc.MdcEngine.Prefab.PrefabComponent.PrefabComponentEnum;
using com.doosan.fms.mdc.MdcEngine.Prefab.PrefabObject.PrefabEnum;
using com.doosan.fms.model.Base;
using Newtonsoft.Json.Linq;

namespace com.doosan.fms.mdc.MdcEngine.Prefab.PrefabComponent.ComponentBase
{
    public interface IPrefabComponentBase
    {
        public JObject IGetJson();
        public PrefabComponentType GetPrefabComponentType();
        public void IUpdate(FmsModel item, PrefabType prefabType, double deltaTime, double mps, double resolution);
    }
}
