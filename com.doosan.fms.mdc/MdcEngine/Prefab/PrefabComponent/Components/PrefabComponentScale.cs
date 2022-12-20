using com.doosan.fms.mdc.MdcEngine.DataStruct;
using com.doosan.fms.mdc.MdcEngine.Prefab.PrefabComponent.ComponentBase;
using com.doosan.fms.mdc.MdcEngine.Prefab.PrefabComponent.PrefabComponentEnum;
using com.doosan.fms.mdc.MdcEngine.Prefab.PrefabObject.PrefabEnum;
using com.doosan.fms.model.Base;
using Newtonsoft.Json.Linq;
using System.Text.Json.Serialization;
using static com.doosan.fms.commonLib.Extension.VariableExtension;

namespace com.doosan.fms.mdc.MdcEngine.Prefab.PrefabComponent.Components
{
    public class PrefabComponentScale : PrefabComponentBase
    {
        private Vector3 _scale;
        [JsonInclude]
        public Vector3 Scale { get => _scale; }

        public PrefabComponentScale() : base(PrefabComponentType.Scale)
        {
            _scale = new Vector3();
        }
        public override JObject GetJson()
        {
            JObject result = new JObject();

            JObject objPosition = new JObject();
            JProperty scaleX = new JProperty(nameof(_scale.X), _scale.X);
            JProperty scaleY = new JProperty(nameof(_scale.Y), _scale.Y);
            JProperty scaleZ = new JProperty(nameof(_scale.Z), _scale.Z);
            objPosition.Add(scaleX);
            objPosition.Add(scaleY);
            objPosition.Add(scaleZ);

            JProperty scale = new JProperty(SCALE, objPosition);
            JProperty prefabComponentType = new JProperty(nameof(PrefabComponentType), PrefabComponentType.Scale);

            result.Add(scale);
            result.Add(prefabComponentType);

            return result;
        }

        public override void Update(FmsModel item, PrefabType prefabType, double deltaTime, double mps, double resolution)
        {
            if (prefabType == PrefabType.Robot)
            {
                _scale.SetXYZ(mps, mps, mps);
            }
        }
    }
}
