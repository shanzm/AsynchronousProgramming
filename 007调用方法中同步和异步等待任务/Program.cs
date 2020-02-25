using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _007调用方法中同步和异步等待任务
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"当前线程ID：{Thread.CurrentThread.ManagedThreadId,2 }:Task之前...");
            Task<int> t1 = DoAsync(2000);
            Task<int> t2 = DoAsync(6000);
            //Task.WaitAll(t1, t2);//等待t1和t2都完毕后才进行后续的代码（即阻塞了主线程）
            //Task.WaitAny(t1, t2);//等待t1和t2中有任一个完成（调试的时候，你就会发现当t1完成后就开始执行后续的循环代码）
            //Task.WhenAll(t1, t2);//异步等待t1和t2两个完成（调试的时候你会发现任务t1和t2都在新的线程中执行，主线程继续执行后续的循环代码）
            //Task.WhenAny(t1, t2);//异步等待t1和t2中任一个完成（调试的时候你就会发现两个任务分别在新线程中执行，主线程继续执行后续的循环代码，当t1完成后，继续后续的循环代码）

            //Task.WhenAll(t1, t2).ContinueWith(t => Console.WriteLine($"当前线程ID：{Thread.CurrentThread.ManagedThreadId,2 }:延续任务，两个异步操作返回值是一个int[],其中元素分别是{t.Result[0]}、{t.Result[1]}"));//注意返回值
            Task.WhenAny(t1, t2).ContinueWith(t => Console.WriteLine($"当前线程ID：{Thread.CurrentThread.ManagedThreadId,2 }:延续任务，第一个完成的异步操作返回值是{t.Result.Result}"));//注意返回值


            for (int i = 0; i < 8; i++)
            {
                Thread.Sleep(1000);
                Console.WriteLine($"当前线程ID：{Thread.CurrentThread.ManagedThreadId,2}:循环中");
            }

            Console.ReadKey();
        }

        private static async Task<int> DoAsync(int num)
        {
            int result = await Task.Run(() => { Thread.Sleep(num); Console.WriteLine($"当前线程ID：{Thread.CurrentThread.ManagedThreadId,2}:异步操作之等待：{num}s"); return num; });
            return result;
        }
    }
}
