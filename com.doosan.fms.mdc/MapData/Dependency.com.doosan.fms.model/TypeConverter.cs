using com.doosan.fms.mdc.MdcEngine.Prefab.PrefabObject.PrefabEnum;
using com.doosan.fms.model.Runtime;
using System;
using System.Collections.Generic;

namespace com.doosan.fms.mdc.MapData.Dependency.com.doosan.fms.model
{
    public static class TypeConverter
    {
        private static readonly Dictionary<PrefabType, Type> DIC_PREFABTYPE_FMSMODELTYPE = new Dictionary<PrefabType, Type>
        {
            { PrefabType.Vertex, typeof(VertexInfo)  },
            { PrefabType.Lane, typeof(LaneInfo)},
            { PrefabType.Robot, typeof(RobotData)},
        };

        public static Type ConvertPrefabTypeToFmsModelType(PrefabType _prefabType)
        {
            Type outType = default(Type);
            bool success = false;
            try
            {
                success = DIC_PREFABTYPE_FMSMODELTYPE.TryGetValue(_prefabType, out outType);
            }
            catch (Exception)
            {
                return outType;
            }
            return outType;
        }
    }
}
