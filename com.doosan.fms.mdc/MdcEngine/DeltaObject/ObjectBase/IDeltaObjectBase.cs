using com.doosan.fms.mdc.MdcEngine.DeltaObject.DeltaEnum;

namespace com.doosan.fms.mdc.MdcEngine.DeltaObject.ObjectBase
{
    public interface IDeltaObjectBase
    {
        public DeltaObjectType GetDeltaObjectType();
        public void Update();
    }
}
