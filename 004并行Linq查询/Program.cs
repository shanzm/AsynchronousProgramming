using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _004并行Linq查询
{
    class Program
    {
        static void Main(string[] args)
        {
            //PLinq1();
            PLinqCancel();
        }


        //示例1：简单的演示使用AsParallel()，并与正茬查询对比
        private static void PLinq1()
        {
            //一个简单的小示例，求1到5000中可以整除3的数
            //倒序排序在modThreeIsZero[]中
            int[] intArray = Enumerable.Range(1, 50000000).ToArray();

            Stopwatch sw = new Stopwatch();

            //顺序查询
            sw.Start();
            int[] modThreeIsZero1 = intArray.Select(n => n).Where(n => n % 3 == 0).OrderByDescending(n => n).ToArray();
            sw.Stop();
            Console.WriteLine($"顺序查询，运行时间：{sw.ElapsedMilliseconds}毫秒,可以整除3的个数:{modThreeIsZero1.Count()}");

            //使用AsParallel()实现并行查询
            sw.Restart();
            int[] modThreeIsZero2 = intArray.AsParallel().Select(n => n).Where(n => n % 3 == 0).OrderByDescending(n => n).ToArray();
            sw.Stop();
            Console.WriteLine($"并行查询，运行时间：{sw.ElapsedMilliseconds}毫秒,可以整除3的个数:{modThreeIsZero2.Count()}");

            Console.ReadKey();
        }


        //示例2：取消一个并行查询
        private static void PLinqCancel()
        {
            //具体的作用和含义可以看0030取消一个异步操作
            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken ct = cts.Token;

            int[] intArray = Enumerable.Range(1, 50000000).ToArray();
            Task<int[]> task = Task.Run(() =>
            {
                try
                {
                    int[] modThreeIsZero = intArray.AsParallel().WithCancellation(ct).Select(n => n).Where(n => n % 3 == 0).OrderByDescending(n => n).ToArray();
                    return modThreeIsZero;
                }
                catch (OperationCanceledException ex)
                {

                    Console.WriteLine(ex.Message);//注意：Message的内容就是：已取消该操作
                    return null;
                }
            }
           );

           // cts.Cancel();//调试的时候，通过注释掉此句和不注释掉对比
            Console.WriteLine(task.Result ?.Count());
            Console.WriteLine("不查了，取消PLinq");
            Console.WriteLine();
            Console.ReadKey();

        }
    }
}
