using com.doosan.fms.mdc.MdcEngine.DeltaObject.DeltaEnum;
using com.doosan.fms.mdc.MdcEngine.DeltaObject.ObjectBase;

namespace com.doosan.fms.mdc.MdcEngine.DeltaObject.Objects
{
    public class DeltaMap : DeltaObjectBase
    {
        private double _mps;
        private double _resolution;
        public double Mps { get => _mps; set => _mps = value; }
        public double Resolution { get => _resolution; set => _resolution = value; }

        public DeltaMap() : base(DeltaObjectType.DeltaMap)
        {
            _mps = 0;
            _resolution = 0;
        }
        public DeltaMap(double mps, double resolution) : base(DeltaObjectType.DeltaMap)
        {
            _mps = mps;
            _resolution = resolution;
        }

    }
}
