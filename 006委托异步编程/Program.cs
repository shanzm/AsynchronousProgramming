using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Messaging;
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
            //AddAsync();
            AddAsyncWithCallBack();
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
        //之后使用EndInvoke()操作IAsyncResult对象，获取异步操作的结果，同时释放次线程使用的资源。
        //因为EndInvoke是为开启的线程进行清理，所以必须确保对每一个BeginInvoke都调用EndInvoke。
        private static void AddAsync()
        {
            Func<int, int, int> operateAdd = (int num1, int num2) =>
            {
                Console.WriteLine($"正在执行的线程，线程ID：{Thread.CurrentThread.ManagedThreadId}:执行异步委托中");
                Thread.Sleep(5000);
                return num1 + num2;
            };
            Console.WriteLine($"正在执行主线程，线程ID：{Thread.CurrentThread.ManagedThreadId}:DoSomethingBeforeInvoke");
            IAsyncResult result = operateAdd.BeginInvoke(1, 2, null, null);//前面的参数是委托调用的方法的参数，此处最后两个参数必须是System.AsyncCallback和System.Object类型的对象，暂时按下不表，看下面说明
            //这里使用IAsyncResult类型对象的IsCompleted属性，用于判断是否完成BeginInvoke()
            //while (!result.IsCompleted)
            //{
            //    Thread. Sleep(1000);
            //    Console.WriteLine($"继续执行主线程，线程ID：{Thread.CurrentThread.ManagedThreadId}:……");
            //}

            //也可是使用IAsyncResult类型对象的AsyncWaitHandle属性，该属性返回一个WaitOne()方法，可以设置等待的最长时间
            //如果超时则返回flase,在这里就可以继续运行主线程了，如果在等待时间之前次线程中的操作完成了，则在这里运行次线程中的操作。
            while (!result.AsyncWaitHandle.WaitOne(3000, true))//等待3s,在这里3s的等待中operateAdd()是完不成的，所以还是会先继续主线程操作
            {
                Console.WriteLine($"继续执行主线程，线程ID：{Thread.CurrentThread.ManagedThreadId}:……");
            }
            int sum = operateAdd.EndInvoke(result);
            Console.WriteLine("异步操作结果" + sum);
            Console.WriteLine($"正在执行主线程，线程ID：{Thread.CurrentThread.ManagedThreadId}:DoSomethingAfterInvoke");
            Console.ReadKey();
        }


        //使用回调模式，回调函数会在异步方法结束后执行
        //原始线程中你该干什么就干什么，当异步方法结束后，回调函数会自动去处理
        private static void AddAsyncWithCallBack()
        {
            Func<int, int, int> operateAdd = (int num1, int num2) =>
             {
                 Thread.Sleep(3000);
                 return num1 + num2;
             };
            Console.WriteLine($"当前执行的线程，线程ID:{Thread.CurrentThread.ManagedThreadId}:DoSomethingBeforeAsync...");

            IAsyncResult iar = operateAdd.BeginInvoke(1, 2, addCallBack, null);
            //异步方法的返回值在回调方法中已经处理，所以其实这里的返回值iar没有使用

            for (int i = 0; i < 6; i++)
            {
                Thread.Sleep(1000);
                Console.WriteLine($"当前执行的线程，线程ID:{Thread.CurrentThread.ManagedThreadId}:...");
            }

            Console.ReadKey();
        }

        //用作回调函数，注意返回类型必须为void ,参数必须为IAsyncResult类型，
        static void addCallBack(IAsyncResult iar)
        {
            AsyncResult ar = (AsyncResult)iar;//注意BeginInvoke(）返回的IAsyncResult接口引用，其中内部对象是AsyncResult类型对象
            int result = ((Func<int, int, int>)ar.AsyncDelegate).EndInvoke(iar);//获取调用BeginInvoke 的异步委托委托（这里就是operateAdd），并执行EndInvoke()
            Console.WriteLine($"当前执行的线程，线程ID:{Thread.CurrentThread.ManagedThreadId}:异步操作的结果{result}");
        }
    }
}
