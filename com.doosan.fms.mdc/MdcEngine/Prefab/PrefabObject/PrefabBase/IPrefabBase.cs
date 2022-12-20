using com.doosan.fms.mdc.MdcEngine.DataStruct;
using com.doosan.fms.mdc.MdcEngine.Prefab.PrefabComponent.Components;
using com.doosan.fms.mdc.MdcEngine.Prefab.PrefabObject.PrefabEnum;
using com.doosan.fms.model.Base;
using Newtonsoft.Json.Linq;

namespace com.doosan.fms.mdc.MdcEngine.Prefab.PrefabObject.PrefabBase
{
    public interface IPrefabBase
    {
        public JObject IGetJson();
        public PrefabType GetPrefabType();
        public void Update(FmsModel item, double deltaTime, double mps, double resolution);
        public string GetKey();
        public void IUpdateTransform(FmsModel item, Vector3 _prevPosition, Vector3 _prevRotation, double deltaTime, double mps, double resolution, ref PrefabComponentTransform transform);
    }
}
