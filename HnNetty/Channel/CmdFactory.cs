using System;
using System.Collections.Generic;
using System.Reflection;

namespace HnNetty.Channel
{
    public class CmdFactory
    {
        public static IDictionary<string, Type> serviceClassCatalog = new Dictionary<string, Type>();//定义一个键值对接口对象
        public static Assembly assembly = Assembly.GetExecutingAssembly(); // 获取当前程序集

        static CmdFactory()
        {
        }

        public static void Init()
        {
            //获得程序集里的所有类
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                if (type.BaseType == (typeof(CommandBase)))
                {
                    serviceClassCatalog.Add(type.FullName, type);
                }
            }
        }

        public static CommandBase GetClassInstance(byte cmd)
        {
            //foreach (var service in serviceClassCatalog)
            //{
            //    var field = service.Value.GetProperties("Key",BindingFlags.Public);
            //    if ((byte)field.GetValue(service) == cmd)
            //    {
            //        return (CommandBase)assembly.CreateInstance(service.Value.FullName);
            //    }
            //}
            return null;
        }
    }
}