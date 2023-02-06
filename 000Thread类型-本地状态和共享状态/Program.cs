using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _000Thread类型_本地状态和共享状态
{
    internal class Program
    {
        /// <summary>
        /// 说明：
        /// 首先我们知道同一个进程中的线程共享资源，但是CLR为每一个线程分配独立的内存栈，从而保证每一个线程中的局部变量隔离
        /// </summary>
        /// <param name="args"></param>
        private static void Main(string[] args)
        {
            #region 测试线程中的局部变量

            Thread tempThread1 = new Thread(Go1);
            Thread tempThread2 = new Thread(Go1);
            tempThread1.Start();
            tempThread2.Start();
            //这里会完整的打印两次0-4（顺序可能不一样）

            #endregion

            #region 测试多个线程共享变量

            TestSharedVar testSharedVar = new TestSharedVar();
            Thread tempThread3 = new Thread(testSharedVar.Go2);
            tempThread3.Start();
            testSharedVar.Go2();
            //这里只能打印出一次"Done",主线程和tempThread3都使用了同一个testShareVar对象，都使用了_done变量，都执行了Go2()方法
            //当其中一个线程执行了Go2函数将_done修改了，则第二个线程执行Go2的时候不会再打印“Done"
            //这里就引入了一个重要的概念：线程的安全性
            //共享可写状态可能引起间接性错误，可以通过锁机制来避免这个问题

            #endregion

            #region 线程意外修改捕获变量值

            //示例1：循环中多线程对同一变量的操作造成数据异常
            for (int i = 0; i < 5; i++)
            {
                new Thread(() => Console.WriteLine(i)).Start();
                //这里打印的可能出现重复i值：12234(每一次都不一样)
                //问题在于变量i,它在整个循环的生命周期内引用的都是同一块内存位置。
            }

            //解决方法，循环中每次都声明一个新的变量
            for (int i = 0; i < 5; i++)
            {
                int temp = i;//每次循环都是创建一个新的变量temp，其在内存的位置都是不一样的，所以就不会出现之前的问题。
                new Thread(() => Console.WriteLine(temp)).Start();
                //这里打印0到4，但是顺序是不一定的，因为每个线程的其实时间是不确定的。
            }

            //示例2：多线程执行的时候捕获的变量是变量最后的值，和创建线程时变量的值无关
            string text = "t1";
            Thread t1 = new Thread(() => Console.WriteLine(text));
            text = "t2";
            Thread t2 = new Thread(() => Console.WriteLine(text));
            t1.Start();
            t2.Start();
            //注意这里两次打印text，打印的都是t2，因为两个线程Start时，变量text已经被赋值为"t2"

            #endregion

            Console.ReadKey();
        }

        //线程内的局部变量是与其他线程相互隔离的
        public static void Go1()
        {
            //这里的循环变量在执行Go函数的每个线程中都是单独存在的，是相互隔离的
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine(i);
            }
        }
    }

    public class TestSharedVar
    {
        private bool _done;

        public void Go2()
        {
            if (!_done)
            {
                _done = true;
                Console.WriteLine("Done");

                //等待10s，再修改_done的值，这样大概率可以让两个线程都能打印出"Done";
                //Thread.Sleep(10000);
                //_done = true;
            }
        }
    }
}
