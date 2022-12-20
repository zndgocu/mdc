using com.doosan.fms.mdc.MdcEngine.Prefab.PrefabComponent.Components;
using com.doosan.fms.mdc.MdcEngine.Prefab.PrefabObject.PrefabEnum;
using com.doosan.fms.model.Base;

namespace com.doosan.fms.mdc.MdcEngine.Prefab.PrefabObject.Prefab
{
    public class PrefabVertex : PrefabBase.PrefabBase
    {
        public PrefabVertex() : base(PrefabType.Vertex)
        {

        }

        protected override void InitializePrefabComponents()
        {
            ComponentContainer.AddPrefabComponent(new PrefabComponentTransform());
        }


        /// <summary>
        /// 버텍스 순차변환 없으므로 델타타임을 곱하지 않음
        /// </summary>
        /// <param name="item"></param>
        /// <param name="deltaTime"></param>
        protected override void UpdatePrefab(FmsModel item, double deltaTime, double mps, double resolution)
        {
            if (item == null) return;
        }
    }
}
