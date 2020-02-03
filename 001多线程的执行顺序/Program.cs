using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _001多线程的执行顺序
{
    class Program
    {
        static void Main(string[] args)
        {
            #region 说明
            ///每次执行的结果都是不一样的
            ///使用循环生成100个线程，每一个线程的名字都是当前循环次数命名的，
            ///我们的的每一个线程的中的函数是打印当前线程的名字和当前的循环
            ///实施你会发现当前线程的执行的时候都已经不是定义该线程时所在的循环了
            ///而且定义的100个线程的执行顺序并不是按照我们定义的执行顺序执行的
            ///这个例子就是为了展示出：线程的分配和启动需要一定的时间，执行不是实时的，而且线程执行的是无序或并行的
            #endregion
            for (int i = 0; i < 100; i++)
            {
                Thread t = new Thread(() => Console.WriteLine($"当前的线程的名字:{Thread.CurrentThread.Name},当前的循环次数:{i}"));
                t.Name = "线程" + i;//设置线程的名字
                t.IsBackground = true;//设置线程为后台线程
                t.Start();//在当前循环中就执行该线程
            }
            Console.ReadKey();
        }
    }
}
