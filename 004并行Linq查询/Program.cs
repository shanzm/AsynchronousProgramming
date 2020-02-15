using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#region 说明
//System.Linq名称空间中有一个`ParallelEnumerable`类，该类中的方法可以分解Linq查询的工作，使其分布在多个线程上,即实现并行查询。

//为并行运行而设计的LINQ查询称为** PLINQ查询**。

//下面让我们先简单的理一理：

//首先我们都知道`Enumerable`类为`IEnumberable<T>`接口扩展了一系列的静态方法。（就是我们使用Linq方法语法的中用的哪些常用的静态方法，自行F12）

//正如MSDN中所说：“ParallelEnumberable是Enumberable的并行等效项”，`ParallelEnumberable`类则是`Enumerable`类的并行版本，

//F12查看定义可以看到`ParallelEnumerable`类中几乎所有的方法都是对`ParallelQuery<TSource>`接口的扩展，

//但是，在`ParallelEnumberable`类有一个重要的例外，`AsParallel()` 方法还对`IEnumerable<T>`接口的扩展，并且返回的是一个`ParallelQuery<TSource>`类型的对象，

//所以呢？凡是实现类IEnumberable<T>集合可以通过调用静态方法`AsParallel()`,返回一个ParallelQuery<TSource> 类型的对象，之后就可以使用ParallelEnumerable类中的异步版本的静态查询方法了！

#endregion

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
            //AsParallel()方法返回ParallelQuery&lt;TSourc>类型对象。因为返回的类型，所以编译器选择的Select()、Where()等方法是ParallelEnumerable.Where()，而不是Enumerable.Where()。
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
                catch (OperationCanceledException ex)//一旦PLinq中取消查询就会触发OperationCanceledException异常
                {
                    Console.WriteLine(ex.Message);//注意：Message的内容就是：已取消该操作
                    return null;
                }
            }
           );
            Console.WriteLine("取消PLinq?Y/N");
            string input = Console.ReadLine();
            if (input.ToLower().Equals("y"))
            {
                cts.Cancel();
                Console.WriteLine("取消了PLinq！");//undone:怎么验证已经真的取消了
            }
            else
            {
                Console.WriteLine("Loading……");
                Console.WriteLine(task.Result.Count());
            }

            Console.ReadKey();

        }
    }
}
