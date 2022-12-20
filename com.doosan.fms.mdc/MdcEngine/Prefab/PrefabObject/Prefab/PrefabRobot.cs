using com.doosan.fms.mdc.MdcEngine.Prefab.PrefabComponent.Components;
using com.doosan.fms.mdc.MdcEngine.Prefab.PrefabObject.PrefabEnum;
using com.doosan.fms.model.Base;

namespace com.doosan.fms.mdc.MdcEngine.Prefab.PrefabObject.Prefab
{
    public class PrefabRobot : PrefabBase.PrefabBase
    {
        public PrefabRobot() : base(PrefabType.Robot, PrefabUpdateType.Animate)
        {

        }

        protected override void InitializePrefabComponents()
        {
            ComponentContainer.AddPrefabComponent(new PrefabComponentTransform());
            ComponentContainer.AddPrefabComponent(new PrefabComponentScale());
        }
        protected override void UpdatePrefab(FmsModel item, double deltaTime, double mps, double resolution)
        {
            if (item == null) return;
        }
    }
}
