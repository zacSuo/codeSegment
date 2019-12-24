using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace single
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //Demo item1 = Demo.getInstance();

                Type t = Type.GetType("single.Demo");
                ConstructorInfo[] tInfoArray = t.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);
                Demo item2 = (Demo)tInfoArray[0].Invoke(null);
                Demo item3 = (Demo)tInfoArray[0].Invoke(null);

                Console.WriteLine(item3.GetHashCode());
                Console.WriteLine(item2.GetHashCode());

            }
            catch(Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
            Console.ReadKey();
        }
    }

    /// <summary>
    /// 饿汉式
    /// </summary>
    class Demo1
    {
        private Demo1() { }
        private static Demo1 demoItem = new Demo1();
        public Demo1 getInstance()
        {
            return demoItem;
        }
    }

    /// <summary>
    /// 懒汉式
    /// </summary>
    class Demo
    {
        private static object objLock = new object();
        private Demo()
        {
            lock (objLock)
            {
                if (demoItem != null)
                {
                    throw new Exception("不要试图用反射破坏单例模式");
                }
            }
        }
        private volatile static Demo demoItem;
        public static Demo getInstance()
        {
            if (demoItem == null)
            {
                lock (objLock)
                {
                    if (demoItem == null)
                    {
                        demoItem = new Demo();
                    }
                }
            }
            return demoItem;
        }
    }


    /// <summary>
    /// 内部静态类
    /// </summary>
    class Demo3
    {
        private Demo3() { }
        private static class InnerHelper
        {
            public static Demo3 demoItem = new Demo3();
        }
        public Demo3 getInstance()
        {
            return InnerHelper.demoItem;
        }
    }
}
