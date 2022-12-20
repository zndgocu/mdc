using com.doosan.fms.mdc.MdcEngine.Prefab.PrefabComponent.Components;
using com.doosan.fms.mdc.MdcEngine.Prefab.PrefabObject.PrefabEnum;
using com.doosan.fms.model.Base;

namespace com.doosan.fms.mdc.MdcEngine.Prefab.PrefabObject.Prefab
{
    public class PrefabLane : PrefabBase.PrefabBase
    {
        public PrefabLane() : base(PrefabType.Lane)
        {

        }

        protected override void InitializePrefabComponents()
        {
            ComponentContainer.AddPrefabComponent(new PrefabComponentTransform());
        }

        protected override void UpdatePrefab(FmsModel item, double deltaTime, double mps, double resolution)
        {
            if (item == null) return;
        }

    }
}
