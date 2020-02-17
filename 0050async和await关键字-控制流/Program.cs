using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


#region 说明
//调试你就会发现，总共有3个线程
//首先Main()创建一个主线程
//接着第一个awati表达式创建一个线程
//之后第二个await表达式创建一个线程
//最后延续任务和第二个await表达式在同一个线程中
//注意！注意！注意！：这里其实是不一定的，每次的调试结果大概率会出现上面的结果，但是有时也会出现延续任务和第一个await表达式在同一个线程中

//但是你要是只有一个await表达式，则延续任务不会和最后一个await表达式在同一个线程，它会也创建一个线程，所以也是三个线程
//我测试了有三个await表达式，则第二个和第三个在同一个线程，延续工作单独占已给线程
#endregion

namespace _0050async和await关键字_控制流
{
    class Program
    {
        //在这里就Main()就是异步方法的--调用方法
        static void Main(string[] args)
        {
            Console.WriteLine($"-01-.当前线程ID：{Thread.CurrentThread.ManagedThreadId,2}------------调用方法中---调用异步方法之前的代码所在线程");
            Task<int> result = SumAsync(1, 2);
            Console.WriteLine($"-03-.当前线程ID：{Thread.CurrentThread.ManagedThreadId,2}------------调用方法中---调用异步方法之后的代码所在线程");
            result.ContinueWith(t => Console.WriteLine($"-10-.当前线程ID：{Thread.CurrentThread.ManagedThreadId,2}------------这是延续任务的线程"
                  + "-异步操作结果为：" + t.Result));
            Console.WriteLine($"-04-.当前线程ID：{Thread.CurrentThread.ManagedThreadId,2}------------调用方法中---延续任务之后的代码所在线程");
            for (int i = 0; i < 9; i++)
            {
                Thread.Sleep(1000);
                Console.WriteLine($"----.当前线程ID：{Thread.CurrentThread.ManagedThreadId,2}--------------------------调用方法中---延续任务之后的for循环代码所在线程");
            }
            Console.ReadKey();
        }

        //异步方法
        private static async Task<int> SumAsync(int num1, int num2)
        {
            Console.WriteLine($"-02-.当前线程ID：{Thread.CurrentThread.ManagedThreadId,2}------------异步方法中---第1个await表达式之前的代码所在线程---运行到此，后台开始执行第1个await表达式");
            int sum1 = await Task.Run(() => { Thread.Sleep(3000); Console.WriteLine($"-05-.当前线程ID：{Thread.CurrentThread.ManagedThreadId,2}------------第1个await完成---这是第1个await表达式的线程"); return num1 + num2; });
            Console.WriteLine($"-06-.当前线程ID：{Thread.CurrentThread.ManagedThreadId,2}------------异步方法中---第1个await表达式之后的代码所在线程");
            Console.WriteLine($"-07-.当前线程ID：{Thread.CurrentThread.ManagedThreadId,2}------------异步方法中---第2个await表达式之前的代码所在线程---运行到此，后台开始执行第1个await表达式");
            int sum2 = await Task.Run(() => { Thread.Sleep(3000); Console.WriteLine($"-08-.当前线程ID：{Thread.CurrentThread.ManagedThreadId,2}------------第2个await完成---这是第2个await表达式的线程"); return num1 + num2; });
            Console.WriteLine($"-09-.当前线程ID：{Thread.CurrentThread.ManagedThreadId,2}------------异步方法中---第2个await表达式之后的代码所在线程");
            // int sum3 = await Task.Run(() => { Thread.Sleep(3000); Console.WriteLine($"-9-.当前线程ID：{Thread.CurrentThread.ManagedThreadId,2}------------第3个await完成---这是第3个await表达式的线程"); return num1 + num2; });
            //return sum1 + sum2+sum3;
            return sum1 + sum2;
        }
    }
}
