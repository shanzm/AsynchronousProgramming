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
            //AddAsyncWithCallBack();
            AddAsyncWithCallBack2();

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
        //首先使用BeginInvoke()调用方法，返回值是IAsyncResult接口引用（其内部是一个AsyncResult对象）

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


        //说明：使用回调模式，回调方法会在异步委托调用的方法结束后执行
        //原始线程中你该干什么就干什么，当异步方法结束后，回调函数会自动去处理
        //注意回调方法是运行在次线程中的
        private static void AddAsyncWithCallBack()
        {
            Func<int, int, int> operateAdd = (int num1, int num2) =>
             {
                 Thread.Sleep(3000);
                 return num1 + num2;
             };
            Console.WriteLine($"当前执行的线程，线程ID:{Thread.CurrentThread.ManagedThreadId}:DoSomethingBeforeAsync...");

            IAsyncResult iar = operateAdd.BeginInvoke(1, 2, new AsyncCallback(addCallBack), null);
            //IAsyncResult iar = operateAdd.BeginInvoke(1, 2, addCallBack, null);
            //因为倒数第二个参数是一个AsyncCallBack委托类型的对象，所以写作：new AsyncCallBack(addCallBack）
            //或是在之前声明一个AsyncCallBack类型的委托对象，然后用这个对象做参数
            //亦可以直接用回调方法做参数,写作：addCallBack
            //其实这里只是为了方便理解，回调方法可以在这里只直接定义为一个(IAsyncRequest iar)=>{……}
            //异步方法的返回值在回调方法中已经处理，所以其实这里的返回值iar没有使用

            for (int i = 0; i < 6; i++)
            {
                Thread.Sleep(1000);
                Console.WriteLine($"当前执行主线程，线程ID:{Thread.CurrentThread.ManagedThreadId}:...");
            }

            Console.ReadKey();
        }

        //用作回调函数，注意返回类型必须为void ,参数必须为IAsyncResult类型，
        static void addCallBack(IAsyncResult iar)
        {
            AsyncResult ar = (AsyncResult)iar;//注意BeginInvoke(）返回的IAsyncResult接口引用，其中内部对象是AsyncResult类型对象
            int result = ((Func<int, int, int>)ar.AsyncDelegate).EndInvoke(iar);//获取调用BeginInvoke 的异步委托委托（这里就是operateAdd），并执行EndInvoke()
            Console.WriteLine($"当前执行的新线程，线程ID:{Thread.CurrentThread.ManagedThreadId}:异步操作的结果{result}");
        }


        //说明:使用BeginInvoke()的最后一个参数，该参数用于从主线程中传递一个数据到次线程中的回调方法中
        //注意这个参数是Object类型，所以可以传入任何类型的数据，
        //在回调方法中使用IAsyncResult对象的AsyncState属性获取，注意获取到的是object类型的数据，需要强转为真实类型
        private static void AddAsyncWithCallBack2()
        {
            Func<int, int, int> operateAdd = (int num1, int num2) =>
            {
                Thread.Sleep(3000);
                return num1 + num2;
            };
            Console.WriteLine($"当前执行的线程，线程ID:{Thread.CurrentThread.ManagedThreadId}:DoSomethingBeforeAsync...");

            AsyncCallback addCallBack = (IAsyncResult ia) =>
            {
                //AsyncResult ar = (AsyncResult)ia;
                //int result = ((Func<int, int, int>)ar.AsyncDelegate).EndInvoke(ia);
                //改用下面写法，增强健壮性
                AsyncResult ar = ia as AsyncResult;
                Func<int, int, int> del = ar.AsyncDelegate as Func<int, int, int>;
                int result = del.EndInvoke(ia);

                Console.WriteLine($"当前执行的新线程，线程ID:{Thread.CurrentThread.ManagedThreadId},异步操作的结果:{result}");
                string state = (string)ia.AsyncState;//使用IAsyncResult对象的AsyncState属性获取BeginInvoke的最后一个参数
                Console.WriteLine($"当前执行的新线程，线程ID:{Thread.CurrentThread.ManagedThreadId},BeginInvoke的最后一个参数:{state}");//state这里是“shanzm”
            };

            IAsyncResult iar = operateAdd.BeginInvoke(1, 2, addCallBack, "shanzm");

            for (int i = 0; i < 6; i++)
            {
                Thread.Sleep(1000);
                Console.WriteLine($"当前执行主线程，线程ID:{Thread.CurrentThread.ManagedThreadId}:...");
            }

            Console.ReadKey();
        }

    }
}
