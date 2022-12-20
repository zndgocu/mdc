namespace com.doosan.fms.commonLib.Threading
{
    public interface ISafetySharedItem
    {
        public string ISetItemKey(string key);
        public string IGetItemKey();
        public T Clone<T>() where T : class, new();
    }
}
