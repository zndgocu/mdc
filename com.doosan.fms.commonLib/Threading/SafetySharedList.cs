using System;
using System.Collections.Generic;
using System.Linq;

namespace com.doosan.fms.commonLib.Threading
{
    public class SafetySharedList<T> where T : class, ISafetySharedItem, new()
    {
        private readonly object _locker = new object();
        private readonly List<T> _list = new List<T>();

        public T Add(T item)
        {
            lock (_locker)
            {
                try
                {
                    _list.Add(item);
                    return item;
                }
                catch (Exception)
                {
                    return default(T);
                }
            }
        }

        /// <summary>
        /// return : 
        /// success : item.position.item
        /// fail : param.item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public T GetItem(T item)
        {
            lock (_locker)
            {
                try
                {
                    var position = _list.Where(x => x.IGetItemKey() == item.IGetItemKey()).FirstOrDefault();
                    return position;
                }
                catch (Exception)
                {
                    return item;
                }
            }
        }

        public List<T> ClearAddRange(List<T> item)
        {
            lock (_locker)
            {
                try
                {
                    _list.Clear();
                    _list.AddRange(item);
                    return item;
                }
                catch (Exception)
                {
                    return default(List<T>);
                }
            }
        }

        public List<T> AddRange(List<T> item)
        {
            lock (_locker)
            {
                try
                {
                    _list.AddRange(item);
                    return item;
                }
                catch (Exception)
                {
                    return default(List<T>);
                }
            }
        }

        public void Remove(T item)
        {
            lock (_locker)
            {
                try
                {
                    var position = GetItem(item);
                    _list.Remove(position);
                }
                catch (Exception)
                {
                    return;
                }
            }
        }

        public void RemoveRange(T item)
        {
            lock (_locker)
            {
                try
                {
                    _list.Remove(item);
                }
                catch (Exception)
                {
                    return;
                }
            }
        }

        public void Clear()
        {
            lock (_locker)
            {
                try
                {
                    _list.Clear();
                }
                catch (Exception)
                {
                    return;
                }
            }
        }

        public List<T> Clone()
        {
            List<T> result = new List<T>();
            lock (_locker)
            {
                try
                {
                    foreach (var item in _list)
                    {
                        result.Add(item.Clone<T>());
                    }
                    return result;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public Dictionary<string, T> CloneDictionary()
        {
            Dictionary<string, T> result = new Dictionary<string, T>();
            lock (_locker)
            {
                try
                {
                    foreach (var item in _list)
                    {
                        result.Add(item.IGetItemKey(), item.Clone<T>());
                    }
                    return result;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
    }
}
