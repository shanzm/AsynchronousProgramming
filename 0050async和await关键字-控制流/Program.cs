using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _0050async和await关键字_控制流
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"-1-.正在执行主线程，线程ID：{Thread.CurrentThread.ManagedThreadId}");
            Task<int> result = SumAsync(1, 2);
            Console.WriteLine($"-3-.正在执行主线程，线程ID：{Thread.CurrentThread.ManagedThreadId}");
            result.ContinueWith(t => Console.WriteLine($"-6-.正在执行的线程，线程ID：{Thread.CurrentThread.ManagedThreadId}-" + "-异步操作结果为：" + result.Result));
            Console.WriteLine($"-4-.正在执行主线程，线程ID：{Thread.CurrentThread.ManagedThreadId}");
            for (int i = 0; i < 10; i++)
            {
                Thread.Sleep(1000);
                Console.WriteLine(i);
            }
            Console.ReadKey();
        }
        private static async Task<int> SumAsync(int num1, int num2)
        {
            Console.WriteLine($"-2-.正在执行的线程，线程ID：{Thread.CurrentThread.ManagedThreadId}");

            int sum = await Task.Run(() => { Thread.Sleep(3000); return num1 + num2; });

            Console.WriteLine($"-5-.正在执行的线程，线程ID：{Thread.CurrentThread.ManagedThreadId}");

            return sum;
        }
    }
}
