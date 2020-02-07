using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

//注意异步委托是基于异步编程模型模式（APM）

namespace _006委托异步编程
{
    class Program
    {

        static void Main(string[] args)
        {
            //AddSync();
            AddAsync();
            //AsyncDelegateWithoutReturn();
        }

        //说明：使用.Invoke()执行委托调用的函数（该函数称之为引用函数），这种方式是同步的(也就是step by step）
        //所以在调试的时候，当运行到.Invoke()时，会一直等待他运行结束，才会继续运行后续的代码
        //而且你会发现，所有的程序都是在一个线程中的（都是在主线程中）
        private static void AddSync()
        {
            Func<int, int, int> operateAdd = (int num1, int num2) =>
            {
                Console.WriteLine($"正在执行的线程，线程ID：{Thread.CurrentThread.ManagedThreadId}");
                Thread.Sleep(5000);
                return num1 + num2;
            };

            Console.WriteLine($"正在执行主线程，线程ID：{Thread.CurrentThread.ManagedThreadId}：DoSomethingBeforeInvoke");
            int sum = operateAdd.Invoke(1, 2);//等价于：operateAdd(1, 2);
            Console.WriteLine("运算结果" + sum);
            //因为Invoke()是同步操作， 同步调用Add(),所以我们要等待5s
            Console.WriteLine($"正在执行主线程，线程ID：{Thread.CurrentThread.ManagedThreadId}：DoSomethingAfterInvoke");
            Console.ReadKey();
        }



        //说明：使用BeginInvoke()和EndInvoke()实现异步委托
        //首先使用BeginInvoke()调用方法，返回值是IAsyncResult类型的对象，通过该对象可以查看异步操作的状态

        //BeginInvoke()需要做的全部事情就是缓存调用引用函数后返回的IAsyncResult类型对象，
        //并在准备获取引用函数调用的结果时，把它传给EndInvoke（）
        //之后使用EndInvoke()操作IAsyncResult对象，获取异步操作的结果。

        private static void AddAsync()
        {
            Func<int, int, int> operateAdd = (int num1, int num2) =>
            {
                Console.WriteLine($"正在执行的线程，线程ID：{Thread.CurrentThread.ManagedThreadId}:执行异步委托中");
                Thread.Sleep(5000);
                return num1 + num2;
            };
            Console.WriteLine($"正在执行主线程，线程ID：{Thread.CurrentThread.ManagedThreadId}:DoSomethingBeforeInvoke");
            IAsyncResult result = operateAdd.BeginInvoke(1, 2, null, null);//此处后两个参数是两个参数必须是System.AsyncCallback和System.Object类型的对象，暂时按下不表，看下面说明
            //这里使用IAsyncResult类型对象的IsCompleted属性，用于判断是否完成BeginInvoke()
            //while (!result.IsCompleted)
            //{
            //    Thread. Sleep(1000);
            //    Console.WriteLine($"继续执行主线程，线程ID：{Thread.CurrentThread.ManagedThreadId}:……");
            //}

            //也可是使用IAsyncResult类型对象的AsyncWaitHandle属性，该属性返回一个WaitOne()方法，可以设置等待的最长时间
            //如果超时则返回flase,在这里就可以继续运行主线程了，如果在等待时间之前次线程中的操作完成了，则在这里运行次线程中的操作。
            while (!result.AsyncWaitHandle.WaitOne(6000, false))//等到3s,在这里3s的等待中operateAdd()是完不成的，所以还是会先继续主线程操作
            {
                Console.WriteLine($"继续执行主线程，线程ID：{Thread.CurrentThread.ManagedThreadId}:……");
            }
            int sum = operateAdd.EndInvoke(result);
            Console.WriteLine("异步操作结果" + sum);
            Console.WriteLine($"正在执行主线程，线程ID：{Thread.CurrentThread.ManagedThreadId}:DoSomethingAfterInvoke");
            Console.ReadKey();
        }


        //如果异步调用一个无返回值的方法，仅仅调用BeginInvoke（）就可以了。
        //在这种情况下，我们不需要缓存IAsyncResult兼容对象，也不需要首先调用EndInvoke（）（因为没有收到返回值）
        private static void AsyncDelegateWithoutReturn()
        {
            Action del = () =>
            {
                for (int i = 0; i < 100; i++)
                {
                    Console.WriteLine($"当前执行的线程，线程ID:{Thread.CurrentThread.ManagedThreadId},异步中，当前循环{i}");
                };
            };
            Console.WriteLine($"当前执行的线程，线程ID:{Thread.CurrentThread.ManagedThreadId}:DoSomethingBeforeAsync...");
            del.BeginInvoke(null, null);
            for (int i = 100; i < 200; i++)
            {
                Console.WriteLine($"当前执行的线程，线程ID:{Thread.CurrentThread.ManagedThreadId},主线程，当前循环{i}");
            }
            Console.ReadKey();
        }
    }
}
