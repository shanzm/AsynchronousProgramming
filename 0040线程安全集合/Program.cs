using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _0040线程安全集合
{
    class Program
    {
        static void Main(string[] args)
        {
            ConcurrentBag();
        }

        //https://docs.microsoft.com/zh-cn/dotnet/standard/collections/thread-safe/
        private static void ConcurrentBag()
        {
            ConcurrentBag<int> cBag = new ConcurrentBag<int>();
            Parallel.For(0, 100000, t => cBag.Add(t));
            Console.WriteLine($"经过100000次循环，后cBag中的数据个数{cBag.Count()}");

            Console.ReadKey();

            #region 说明
            //按照我们的逻辑，把0-100000存放到集合list中，
            //但是实际运行的时候，却发现集合list中可能远远不到100000个数字
            //为什么呢，因为多线程对一个数据集合操作，可能造成数据的混乱！
            #endregion
            //把0-100000添加到List中
            //若是使用并行操作则会报错：“ 源数组长度不足。请检查 srcIndex 和长度以及数组的下限。”
            //这不是是说List<int>的长度不够，理论上的List<int>的长度就是int的长度的最大值（.net中使用的int默认是int32即32位，-2^31~2^31）
            //当然你可以通过lock(locker)加锁的方式，来避免这种错误
            //但是完全没有必要，你应该选择使用线程安全集合类型

            //List<int> list = new List<int>();
            ////object locker = new object();
            ////Parallel.For(0, 1000000, t => { lock(locker) { list.Add(t); } });
            //Parallel.For(0, 1000000, t => list.Add(t));
            //Console.WriteLine($"经过100000次循环，后list中的数据个数{list.Count()}");
            //Console.ReadKey();


        }
    }
}

