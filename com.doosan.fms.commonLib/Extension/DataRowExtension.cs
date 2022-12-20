using System;
using System.Data;

namespace com.doosan.fms.commonLib.Extension
{
    public static class DataRowExtension
    {
        public static T TryParse<T>(this DataRow row, string fieldName)
        {
            if (row.Table.Columns.Contains(fieldName) == false) return default(T);
            return (T)(Convert.ChangeType(row[fieldName], typeof(T)));
        }

    }
}
