using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SendEmailService
{
    public class MethodHelper
    {
        #region -- Static (implement Singleton pattern) --

        private static volatile MethodHelper _instance;

        private static readonly object _syncRoot = new Object();

        public static MethodHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance == null)
                        {
                            _instance = new MethodHelper();
                        }
                    }
                }
                return _instance;
            }
        }

        #endregion 

        public string ConvertListToString(List<string> lstString = null, List<int> lstInt = null)
        {
            string strConvert = "";
            if (lstString != null)
                strConvert = string.Join(",", lstString);
            if (lstInt != null)
                strConvert = string.Join(",", lstInt);
            return strConvert;
        }

        public string ConvertBoolToStr(bool bolValue)
        {
            return bolValue ? "1" : "0";
        }

        public void ConvertToObject(IDataReader reader, dynamic lstObj)
        {
            try
            {
                while (reader.Read())
                {
                    Exception objError = null;
                    dynamic obj = Activator.CreateInstance(lstObj.GetType().GetGenericArguments()[0]);
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        if (!Convert.IsDBNull(reader[i]))
                        {
                            dynamic type = obj.GetType().GetProperty(reader.GetName(i).ToUpper());

                            if (type != null)
                            {
                                //DLL Npgsql mới phân biệt giữa timespan với datetime nên phải trick
                                if (reader[i].GetType().Name == typeof(TimeSpan).Name
                                    && (type.PropertyType.Name == typeof(DateTime).Name
                                    || (type.PropertyType.Name == typeof(Nullable<>).Name
                                        && type.PropertyType.GenericTypeArguments[0].Name == typeof(DateTime).Name))
                                    )
                                {
                                    TimeSpan time = (TimeSpan)reader[i];
                                    type.SetValue(obj, new DateTime() + time, null);

                                }
                                else
                                {
                                    try
                                    {
                                        type.SetValue(obj, reader[i], null);
                                    }
                                    catch (Exception ex)
                                    {
                                        objError = ex;
                                    }
                                }
                            }
                        }
                    }

                    if (objError != null)
                        throw objError;

                    lstObj.Add(obj);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        public string ConvertNullToStr(object objValue)
        {
            return (objValue == null || objValue == DBNull.Value) ? string.Empty : objValue.ToString();
        }

        public static void CopyObject<T>(object sourceObject, ref T destObject)
        {
            //  If either the source, or destination is null, return
            if (sourceObject == null || destObject == null)
                return;

            //  Get the type of each object
            Type sourceType = sourceObject.GetType();
            Type targetType = destObject.GetType();

            //  Loop through the source properties
            foreach (PropertyInfo p in sourceType.GetProperties())
            {
                //  Get the matching property in the destination object
                PropertyInfo targetObj = targetType.GetProperty(p.Name);
                //  If there is none, skip
                if (targetObj == null)
                    continue;

                //  Set the value in the destination
                targetObj.SetValue(destObject, p.GetValue(sourceObject, null), null);
            }
        }
    }
}
