using com.doosan.fms.commonLib.Helper;
using com.doosan.fms.mdc.MdcEngine.DataStruct;
using com.doosan.fms.mdc.MdcEngine.Prefab.PrefabComponent.Components;
using com.doosan.fms.mdc.MdcEngine.Prefab.PrefabComponent.PrefabComponentContainer;
using com.doosan.fms.mdc.MdcEngine.Prefab.PrefabObject.Prefab;
using com.doosan.fms.mdc.MdcEngine.Prefab.PrefabObject.PrefabEnum;
using com.doosan.fms.model.Base;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace com.doosan.fms.mdc.MdcEngine.Prefab.PrefabObject.PrefabBase
{
    public class PrefabBase : IPrefabBase
    {
        public JObject IGetJson()
        {
            JObject result = new JObject();

            JProperty key = new JProperty("Key", _key);
            JProperty type = new JProperty("PrefabType", _prefabType);
            JProperty updateType = new JProperty("PrefabUpdateType", _prefabUpdateType);
            JProperty componentContainer = new JProperty("ComponentContainer", ComponentContainer.GetJson());


            result.Add(key);
            result.Add(type);
            result.Add(updateType);
            result.Add(componentContainer);

            //rtnObj = ComponentContainer.GetJson();
            return result;
        }

        public static Dictionary<PrefabType, Type> PrefabTypeMap = new Dictionary<PrefabType, Type>()
        {
            { PrefabType.Vertex, typeof(PrefabVertex) },
            { PrefabType.Lane, typeof(PrefabLane) },
            { PrefabType.Robot, typeof(PrefabRobot) },
        };


        private string _key = "";
        private PrefabType _prefabType;
        private PrefabUpdateType _prefabUpdateType;
        private PrefabComponentContainer _componentContainer;

        [JsonInclude]
        public string Key { get => _key; }
        [JsonInclude]
        public PrefabType PrefabType { get => _prefabType; set => _prefabType = value; }
        [JsonInclude]
        public PrefabComponentContainer ComponentContainer { get => _componentContainer; }
        [JsonInclude]
        public PrefabUpdateType PrefabUpdateType { get => _prefabUpdateType; set => _prefabUpdateType = value; }

        public PrefabBase(PrefabType prefabType, PrefabUpdateType prefabSendType = PrefabUpdateType.Background)
        {
            _prefabType = prefabType;
            _prefabUpdateType = prefabSendType;
            _componentContainer = new PrefabComponentContainer();
            InitializePrefabComponents();
        }

        public static PrefabBase New(PrefabType prefabType)
        {
            try
            {
                Type mapperType;
                if (PrefabTypeMap.TryGetValue(prefabType, out mapperType) == false) return null;
                return ReflectHelper.Constructor<PrefabBase>(mapperType, null, null);
            }
            catch (Exception)
            {
                return null;
            }
        }


        public PrefabType GetPrefabType()
        {
            return _prefabType;
        }

        public void Update(FmsModel item, double deltaTime, double mps, double resolution)
        {
            if (item == null) return;
            UpdatePrefab(item, deltaTime, mps, resolution);
            ComponentContainer.Update(item, PrefabType, deltaTime, mps, resolution);
        }
        public void UpdateFirst(FmsModel item, double deltaTime, double mps, double resolution)
        {
            if (item == null) return;
            UpdatePrefab(item, deltaTime, mps, resolution);
            ComponentContainer.Update(item, PrefabType, deltaTime, mps, resolution);
        }

        protected virtual void UpdatePrefab(FmsModel item, double deltaTime, double mps, double resolution)
        {
            return;
        }

        protected virtual void InitializePrefabComponents()
        {

        }

        public string GetKey()
        {
            return _key;
        }

        public string SetKey(string key)
        {
            _key = key;
            return _key;
        }

        public void IUpdateTransform(FmsModel item, Vector3 _prevPosition, Vector3 _prevRotation, double deltaTime, double mps, double resolution, ref PrefabComponentTransform transform)
        {
            if (transform == null) return;
            UpdateTransform(item, _prevPosition, _prevRotation, deltaTime, mps, resolution, ref transform);
        }

        protected virtual void UpdateTransform(FmsModel item, Vector3 _prevPosition, Vector3 _prevRotation, double deltaTime, double mps, double resolution, ref PrefabComponentTransform transform)
        {
            return;
        }


    }
}
