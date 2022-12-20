using com.doosan.fms.commonLib.Threading;
using com.doosan.fms.model.Interface;
using com.doosan.fms.model.Interface.Dependency;
using System;
using System.Collections.Generic;
using System.Data;

namespace com.doosan.fms.model.Base
{
    public class FmsModel : IFmsModel, ISafetySharedItem, IMonitoringData
    {
        public FmsModel()
        {

        }

        public FmsModel(FmsModel fmsModel)
        {
            _key = fmsModel.IGetKey();
        }

        private string _key = "";
        public static string GenInitializeQuery<T>() where T : FmsModel, new()
        {
            T t = new();
            return t.GetInitializeQuery();
        }
        public virtual double GetTransformPositionY()
        {
            return 0;
        }
        public virtual double GetTransformPositionX()
        {
            return 0;
        }

        public string GetInitializeQuery()
        {
            return GetInitializeQueryRuntime();
        }
        protected virtual string GetInitializeQueryRuntime()
        {
            return "";
        }

        public string GetCreateQuery()
        {
            return GetCreateQueryRuntime();
        }
        protected virtual string GetCreateQueryRuntime()
        {
            return "";
        }
        public string GetUpdateQuery()
        {
            return GetUpdateQueryRuntime();
        }
        protected virtual string GetUpdateQueryRuntime()
        {
            return "";
        }

        public void SetMember(DataRow row)
        {
            SetMemberRuntime(row);
        }
        protected virtual void SetMemberRuntime(DataRow row)
        {
            return;
        }


        public T Clone<T>() where T : class, new()
        {
            try
            {
                return this.CloneRuntime() as T;
            }
            catch (Exception)
            {
                return default(T);
            }
        }
        protected virtual FmsModel CloneRuntime()
        {
            return null;
        }

        public virtual string LogData()
        {
            return "";
        }


        public static FmsModel GetModel<T>(DataRow row) where T : FmsModel, new()
        {
            T t = new T();
            t.SetMember(row);
            return t;
        }

        public static List<FmsModel> GetModels<T>(DataTable table) where T : FmsModel, new()
        {
            List<FmsModel> models = new List<FmsModel>();
            foreach (DataRow row in table.Rows)
            {
                models.Add(GetModel<T>(row));
            }
            return models;
        }

        public double IGetTransformPositionX()
        {
            return GetTransformPositionX();
        }

        public double IGetTransformPositionY()
        {
            return GetTransformPositionY();
        }

        public double IGetTransformPositionZ()
        {
            return GetTransformPositionZ();
        }
        public virtual double GetTransformPositionZ()
        {
            return 0;
        }

        public double IGetTransformRotationX()
        {
            return GetTransformRotationX();
        }
        public virtual double GetTransformRotationX()
        {
            return 0;
        }

        public double IGetTransformRotationY()
        {
            return GetTransformRotationY();
        }
        public virtual double GetTransformRotationY()
        {
            return 0;
        }

        public double IGetTransformRotationZ()
        {
            return GetTransformRotationZ();
        }
        public virtual double GetTransformRotationZ()
        {
            return 0;
        }

        public string ISetKey(string key)
        {
            return SetKey(key);
        }

        protected virtual string SetKey(string key)
        {
            _key = key;
            return _key;
        }

        public string IGetKey()
        {
            if (string.IsNullOrEmpty(_key))
            {
                return ISetKey(GetKey());
            }

            return _key;
        }
        protected virtual string GetKey()
        {
            return _key;
        }

        public string ISetItemKey(string key)
        {
            return ISetKey(key);
        }

        public string IGetItemKey()
        {
            return IGetKey();
        }

        public string GetDeleteQuery()
        {
            return GetDeleteQueryRuntime();
        }
        protected virtual string GetDeleteQueryRuntime()
        {
            return "";
        }

    }
}
