using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _000Thread类型_主线程和次线程
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //Demo1
            //只有Main()创建的一个线程（主线程），所以程序会按照顺序执行下面两个函数,打印的结果就是显示从0到100
            //Write0to50();
            //Write51to100();

            //Demo2
            //使用Thread对象创建新线程并执行Write0to50(),把Write51to100()保留在主线程
            //因为两个线程是并行的，所以打印的结果不是按顺序从0到100
            //每次运行结果不同,但是一般还都是有许多数字是连贯的，
            //这是因为操作系统会给每一个线程分配一个“时间片段”以模拟同步处理
            //Thread t = new Thread(Write0to50);//注意Thread()的参数是一个委托
            //t.Start();
            //Write51to100();

            //Demo3
            //这里模拟一下有些线程需要等待大量的时间（分为两种：IO密集型和CPU密集型）
            //这里使用Tread.Sleep()等待5秒，模拟需要等待的时间（注意使用Thread.Sleep()属于IO密集型）
            //因为存在这种需要等待大量时间的线程，若是同步编程(Synchronous Programming)即step by step，所以我们要引入异步编程
            Thread t = new Thread(Write0to50);
            t.Start();
            Thread.Sleep(5000);//主线程等待五秒，注意是主线程等待五秒
            Write51to100();
            Console.ReadKey();
        }

        private static void Write0to50()
        {
            for (int i = 0; i <= 50; i++)
            {
                Console.Write(i + " ");
            }
        }

        private static void Write51to100()
        {
            //Console.WriteLine();
            for (int i = 51; i <= 100; i++)
            {
                Console.Write(i + " ");
            }
        }
    }
}
