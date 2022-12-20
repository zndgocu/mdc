using System.Data;

namespace com.doosan.fms.model.Interface
{
    public interface IFmsModel
    {
        public string GetInitializeQuery();
        public string GetCreateQuery();
        public string GetUpdateQuery();
        public string GetDeleteQuery();
        public void SetMember(DataRow row);
        public string ISetKey(string key);
        public string IGetKey();

    }
}
