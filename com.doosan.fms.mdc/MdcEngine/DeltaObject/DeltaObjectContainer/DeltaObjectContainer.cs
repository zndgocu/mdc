using com.doosan.fms.mdc.MdcEngine.DeltaObject.DeltaEnum;
using com.doosan.fms.mdc.MdcEngine.DeltaObject.ObjectBase;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace com.doosan.fms.mdc.MdcEngine.DeltaObject.DeltaObjectContainer
{
    public class DeltaObjectContainer
    {
        private List<DeltaObjectBase> _deltaItems;

        [JsonInclude]
        public List<DeltaObjectBase> DeltaItems { get => _deltaItems; set => _deltaItems = value; }

        public DeltaObjectContainer()
        {
            _deltaItems = new List<DeltaObjectBase>();
        }

        public DeltaObjectBase GetDeltaItem(DeltaObjectType deltaObjectType)
        {
            return _deltaItems.Where(x => x.GetDeltaObjectType() == deltaObjectType).FirstOrDefault();
        }


        public void AddDeltaItems(List<DeltaObjectBase> items)
        {
            foreach (var item in items)
            {
                AddDeltaItem(item);
            }
        }

        public bool AddDeltaItem(DeltaObjectBase item)
        {
            var already = _deltaItems.Where(x => x.GetDeltaObjectType() == item.GetDeltaObjectType()).FirstOrDefault();
            if (already == null)
            {
                _deltaItems.Add(item);
                return true;
            }

            return false;
        }

        public void Update()
        {
            Parallel.ForEach(_deltaItems, item =>
            {
                item.Update();
            });
        }

        public void Clear()
        {
            _deltaItems.Clear();
        }
    }
}
