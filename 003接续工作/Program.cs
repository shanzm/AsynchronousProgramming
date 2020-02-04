using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

//求解1-5000能求被3整除的个数过程需要许多时间，我把它定义为一个Task
//我们需要在求出结果后打印出结果，这里怎么操作呢？
//若是直接使用task.Result则会一直等待运算过程
//若是使用while(!task.IsComplation){...}，你无法判断Task何时结束，一旦Task结束则会中断后续操作
//这里就是需要为Task加上接续工作

namespace _003接续工作
{
    class Program
    {
        static void Main(string[] args)
        {
            //ContinueWith();

            //Awaiter();

            TaskDelay1();

            //TaskDelay2();
        }

        //方式1：为task添加接续工作，使用task.ContinueWith()
        //task1.ContinueWith(...task2..)表示当task1结束后接着运行task2任务
        //注意:ContinueWith()的返回值亦是Task类型对象，即新创建的任务
        //可以为接续工作task2继续添加接续工作task3
        //同时注意ContinueWith()中的委托是有参数的
        private static void ContinueWith()
        {
            Console.WriteLine("task执行前...");
            Task<int> task1 = Task.Run(() => Enumerable.Range(1, 5000).Count(n => (n % 3) == 0));
            Task task2 = task1.ContinueWith(t => Console.WriteLine($"当你看到这句话则task1结束了，1-5000中能被3整除的个数{task1.Result}"));
            Task task3 = task2.ContinueWith(t => Console.WriteLine($"当你看到这句话则task2也结束了"));
            Console.WriteLine($"task1及其接续工作正在执行中," + "\t\n" + "我们现在正在执行其他的后续代码");
            Console.ReadKey();
        }

        //方法2：为task添加接续工作，使用awaiter(即等待者)
        //首先使用task.GetAwaiter()为相关的task创建一个等待者
        //注意对比，使用awaiter.GetResult();
        private static void Awaiter()
        {
            Console.WriteLine("task执行前...");
            Task<int> task1 = Task.Run(() => Enumerable.Range(1, 5000).Count(n => (n % 3) == 0));
            var awaiter = task1.GetAwaiter();
            //awaiter.OnCompleted(() => Console.WriteLine($"当你看到这句话则task1结束了，1-5000中能被3整除的个数{task1.Result}"));
            awaiter.OnCompleted(() => Console.WriteLine($"当你看到这句话则task1结束了，1-5000中能被3整除的个数{awaiter.GetResult()}"));
            Console.WriteLine($"task1及其接续工作正在执行中," + "\t\n" + "我们现在正在执行其他的后续代码");
            Console.ReadKey();
        }

        //方法1：使用Task.Delay()和ContinueWith实现延迟工作
        //其实就相当于实现Thread.Sleep()的异步版本
        private static void TaskDelay1()
        {
            //新建异步任务，30毫秒秒后执行
            Task.Delay(30).ContinueWith(c =>
            {
                for (int i = 0; i < 50; i++)
                {
                    Console.WriteLine(i + "这是Task在运行");
                }
            });

            for (int i = 0; i < 100; i++)
            {
                Console.WriteLine(i + "这是Task之后的程序在运行");
            }
            //调试的时候你会发现，刚开始的时候的时候是先显示的"i这是Task之后的程序在运行"
            //之后在等带了30毫秒，后就会开始显示"这是Task在运行"，和"i这是Task之后的程序在运行"交叉显示
            //若是你使用Thread.Sleep（）,则会程序一直在等待，直到等待结束才会运行后续的代码
            //而这里就相当于给给Thread.Sleep()一个加了接续工作，且这个接续工作是异步的。

            Console.ReadKey();
        }

        //方法2：使用Task.Delay()和Awaiter实现延迟工作
        private static void TaskDelay2()
        {
            Task.Delay(30).GetAwaiter().OnCompleted(() =>
                {
                    for (int i = 0; i < 50; i++)
                    {
                        Console.WriteLine(i + "这是Awaiter在运行行");
                    }
                });
            for (int i = 0; i < 100; i++)
            {
                Console.WriteLine(i + "这是Awaiter之后的程序在运行行");
            }
            Console.ReadKey();
        }
    }
}
