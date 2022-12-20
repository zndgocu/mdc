using com.doosan.fms.commonLib.Helper;
using com.doosan.fms.mdc.MdcEngine.DeltaObject.DeltaEnum;
using com.doosan.fms.mdc.MdcEngine.DeltaObject.Objects;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace com.doosan.fms.mdc.MdcEngine.DeltaObject.ObjectBase
{
    public class DeltaObjectBase : IDeltaObjectBase
    {
        public static Dictionary<DeltaObjectType, Type> DeltaObjectTypeMap = new Dictionary<DeltaObjectType, Type>()
        {
            { DeltaObjectType.DeltaTimer, typeof(DeltaTimer) },
            { DeltaObjectType.DeltaMap, typeof(DeltaMap) },
        };

        private DeltaObjectType _deltaObejctType;

        [JsonInclude]
        public DeltaObjectType DeltaObejctType { get => _deltaObejctType; }

        public static DeltaObjectBase New(DeltaObjectType deltaObjectType)
        {
            try
            {
                Type mapperType;
                if (DeltaObjectTypeMap.TryGetValue(deltaObjectType, out mapperType) == false) return null;
                return ReflectHelper.Constructor<DeltaObjectBase>(mapperType, null, null);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public DeltaObjectBase(DeltaObjectType deltaObejctType)
        {
            _deltaObejctType = deltaObejctType;
        }


        public DeltaObjectType GetDeltaObjectType()
        {
            return _deltaObejctType;
        }

        public void Update()
        {
            UpdateDelta();
        }
        protected virtual void UpdateDelta()
        {

        }
    }
}
