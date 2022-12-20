using System;
using System.Reflection;

namespace com.doosan.fms.commonLib.Helper
{
    public static class ReflectHelper
    {
        public static object Constructor(Type type, Type[] parmsType, object[] parm)
        {
            try
            {
                if (parmsType == null) parmsType = new Type[] { };
                if (parm == null) parm = new object[] { };
                return type.GetConstructor(parmsType).Invoke(parm);
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static T Constructor<T>(Type type, Type[] parmsType, object[] parm) where T : class
        {
            try
            {
                return (T)(Constructor(type, parmsType, parm));
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        public static object Instance(Type type)
        {
            try
            {
                object t = Activator.CreateInstance(type);
                return t;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static T Instance<T>(Type type) where T : class
        {
            try
            {
                object t = Activator.CreateInstance(type);
                return t as T;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static object StaticMethod(Type type, bool baseSearch, string func, object[] parm)
        {
            try
            {
                if (baseSearch == true)
                {
                    return type.BaseType.GetMethod(func, BindingFlags.Public | BindingFlags.Static).Invoke(null, parm);
                }
                return type.GetMethod(func, BindingFlags.Public | BindingFlags.Static).Invoke(null, parm);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">반환타입</typeparam>
        /// <param name="type"></param>
        /// <param name="func"></param>
        /// <param name="parm"></param>
        /// <returns></returns>
        public static T StaticMethod<T>(Type type, bool baseSearch, string func, object[] parm)
        {
            try
            {
                return (T)(StaticMethod(type, baseSearch, func, parm));
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        public static object StaticMethodGeneric(Type type, bool baseSearch, string func, object[] parm, params Type[] types)
        {
            try
            {
                if (baseSearch == true)
                {
                    return type.BaseType.GetMethod(func, BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(types).Invoke(null, parm);
                }
                return type.GetMethod(func, BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(types).Invoke(null, parm);
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static T StaticMethodGeneric<T>(Type type, bool baseSearch, string func, object[] parm, params Type[] types)
        {
            try
            {
                return (T)(StaticMethodGeneric(type, baseSearch, func, parm, types));
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }





        public static object Method(Type type, bool baseSearch, string func, object[] parm)
        {
            try
            {
                var runtime = Instance(type);
                if (runtime == null) throw new Exception();
                if (baseSearch == true)
                {
                    return (type.BaseType.GetMethod(func).Invoke(runtime, parm));
                }
                return (type.GetMethod(func).Invoke(runtime, parm));
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">반환타입</typeparam>
        /// <param name="type">런타임 타입</param>
        /// <param name="func"></param>
        /// <param name="parm"></param>
        /// <returns></returns>
        public static T Method<T>(Type type, bool baseSearch, string func, object[] parm)
        {
            try
            {
                return (T)(Method(type, baseSearch, func, parm));
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        public static object MethodGeneric(Type type, bool baseSearch, string func, object[] parm, params Type[] types)
        {
            try
            {
                var runtime = Instance(type);
                if (baseSearch == true)
                {
                    return type.BaseType.GetMethod(func).MakeGenericMethod(types).Invoke(runtime, parm);
                }
                return type.GetMethod(func).MakeGenericMethod(types).Invoke(runtime, parm);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static T MethodGeneric<T>(Type type, bool baseSearch, string func, object[] parm, params Type[] types)
        {
            try
            {
                return (T)(MethodGeneric(type, baseSearch, func, parm, types));
            }
            catch (Exception)
            {
                return default(T);
            }
        }
    }
}
