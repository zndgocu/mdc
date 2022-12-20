using com.doosan.fms.mdc.MdcEngine.DeltaObject.DeltaEnum;
using com.doosan.fms.mdc.MdcEngine.DeltaObject.DeltaObjectContainer;
using com.doosan.fms.mdc.MdcEngine.DeltaObject.ObjectBase;
using com.doosan.fms.mdc.MdcEngine.DeltaObject.Objects;
using com.doosan.fms.mdc.MdcEngine.Prefab.PrefabObject.PrefabBase;
using com.doosan.fms.mdc.MdcEngine.Prefab.PrefabObject.PrefabContainer;
using com.doosan.fms.mdc.MdcEngine.Prefab.PrefabObject.PrefabEnum;
using com.doosan.fms.model.Base;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using static com.doosan.fms.commonLib.Extension.VariableExtension;

namespace com.doosan.fms.mdc.MdcEngine.Managers
{
    public class WorldManager
    {
        private const double MPS = 0.018384d;
        private const double RESOULTION = 0.05d;

        private double UNLIMITED = 9999999999999;

        public int StateHubReconnectCounter = 0;
        public int StateProgramCounter = 0;

        private DeltaObjectContainer _deltaObjectContainer;
        private PrefabContainer _prefabContainer;

        private List<DeltaObjectType> _usedDeltaItem;


        [JsonInclude]
        public DeltaObjectContainer DeltaObjectContainer { get => _deltaObjectContainer; set => _deltaObjectContainer = value; }

        [JsonInclude]
        public PrefabContainer PrefabContainer { get => _prefabContainer; set => _prefabContainer = value; }


        /// <summary>
        /// 이 함수는 1번만 실행되어야 합니다.
        /// 이전 데이터는 삭제되고 새로운 데이터가 생성됩니다.
        /// </summary>
        /// <param name="deltaItems"></param>
        public bool InitializeDeltaItems(ref string message, params DeltaObjectType[] deltaItems)
        {
            _deltaObjectContainer.Clear();
            foreach (var type in deltaItems)
            {
                try
                {
                    if (_usedDeltaItem.Where(x => x == type).Count() > 0) throw new Exception("duplicate type");
                    DeltaObjectBase typeItem = DeltaObjectBase.New(type);
                    if (typeItem != null)
                    {
                        _deltaObjectContainer.AddDeltaItem(typeItem);
                    }
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                    return false;
                }
            }

            InitializeMapData();
            return true;
        }
        private void InitializeMapData()
        {
            try
            {
                DeltaObjectBase deltaMapBase = _deltaObjectContainer.GetDeltaItem(DeltaObjectType.DeltaMap);
                if (deltaMapBase == null) return;
                DeltaMap deltaMap = (DeltaMap)deltaMapBase;
                deltaMap.Mps = MPS;
                deltaMap.Resolution = RESOULTION;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
        }

        public bool IsUpdate(out double deltaTime)
        {
            deltaTime = UNLIMITED;
            var deltaTimer = _deltaObjectContainer.GetDeltaItem(DeltaObjectType.DeltaTimer) as DeltaTimer;
            if (deltaTimer == null) return false;
            return deltaTimer.IsUpdate(out deltaTime);
        }

        public void HitDelta()
        {
            var deltaTimer = _deltaObjectContainer.GetDeltaItem(DeltaObjectType.DeltaTimer) as DeltaTimer;
            if (deltaTimer == null) return;
            deltaTimer.HitDelta();
        }

        #region 생성자
        public WorldManager()
        {
            _deltaObjectContainer = new DeltaObjectContainer();
            _usedDeltaItem = new List<DeltaObjectType>();
            _prefabContainer = new PrefabContainer();
        }
        #endregion


        public bool InitializePrefabs(Dictionary<PrefabType, List<FmsModel>> prefabDatas, ref string message)
        {
            try
            {
                _prefabContainer.Clear();
                foreach (PrefabType type in Enum.GetValues(typeof(PrefabType)))
                {
                    var typePrefabGroups = prefabDatas.Where(x => x.Key == type).Select(z => z.Value).FirstOrDefault();
                    if (typePrefabGroups == null) continue;
                    typePrefabGroups.ForEach(x =>
                    {
                        PrefabBase prefab = PrefabBase.New(type);
                        prefab.SetKey(x.IGetKey());
                        _prefabContainer.AddPrefab(prefab);
                    });
                }
            }
            catch (System.Exception ex)
            {
                message = ex.Message;
                return false;
            }
            return true;
        }

        #region logicLoop
        /// <summary>
        /// 순환 코드에서 반드시 업데이트되어야 합니다.
        /// </summary>
        public bool UpdatePrefab(List<FmsModel> items, double elapsedMilsec, double mps, double resolution, PrefabUpdateType prefabUpdateType)
        {
            _prefabContainer.Update(items, elapsedMilsec, mps, resolution, prefabUpdateType); // <- delta
            return true;
        }


        public void StartDelta(double fps)
        {
            var deltaTimer = _deltaObjectContainer.GetDeltaItem(DeltaObjectType.DeltaTimer) as DeltaTimer;
            if (deltaTimer == null) throw new Exception("deltatimer not found");
            deltaTimer.Start(fps);
        }

        public void UpdateDelta()
        {
            _deltaObjectContainer.Update();
        }

        public double GetHitMillsec()
        {
            var deltaTimer = _deltaObjectContainer.GetDeltaItem(DeltaObjectType.DeltaTimer) as DeltaTimer;
            if (deltaTimer == null) throw new Exception("deltatimer not found");
            return deltaTimer.GetHitMillsec();
        }

        public double GetCalcFps()
        {
            var deltaTimer = _deltaObjectContainer.GetDeltaItem(DeltaObjectType.DeltaTimer) as DeltaTimer;
            if (deltaTimer == null) throw new Exception("deltatimer not found");
            return deltaTimer.GetCalcFps();
        }

        public JObject GetJson(PrefabUpdateType prefabSendType = PrefabUpdateType.All)
        {
            try
            {
                JObject result = new JObject();
                result.Add(PREFABS, _prefabContainer.GetJson(prefabSendType));
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool GetDeltaMapData(out double mps, out double resolution)
        {
            try
            {
                var deltaMap = _deltaObjectContainer.GetDeltaItem(DeltaObjectType.DeltaMap) as DeltaMap;
                if (deltaMap == null) throw new Exception("DeltaMap not found");
                mps = deltaMap.Mps;
                resolution = deltaMap.Resolution;
                return true;
            }
            catch (Exception)
            {
                mps = 0;
                resolution = 0;
                return false;
            }
        }
        #endregion
    }
}
