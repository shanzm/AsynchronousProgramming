﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


#region 说明
//《C#高级编程：13.2》：现在，在NET4.5中，推出了另外一种新的方式来实现异步编程：基于任务的异步模式（TAP）。
//这种模式是基于.NET4.0中新增的Task类型，并通过async和await关键字来使用编译器功能。


//《C#高级编程：13.3》：async和await关键字只是编译器功能，编译器会用Task类创建代码。
//如果不使用这两个关键字，也可以用C#4.0和Task类的方法来实现同样的功能，只是没有那么方便。
//本节介绍了编译器用async和await关键字能做什么，
//如何采用简单的方式创建异步方法，如何并行调用多个异步方法，以及如何修改已经实现异步模式的类，以使用新的关键字。
#endregion

namespace _005async和await关键字
{
    class Program
    {
        static void Main(string[] args)
        {
            //FirstAsyncWithTReturn();

            //FirstAsyncWithTaskReturn();

            //MultipleMethod();

            MultipleMethod2();

            Console.ReadKey();
        }

        //1.调用异步方法SumAsync
        //并为期添加一个延续工作，接收并处理异步操作的返回值Task<T>
        private static void FirstAsyncWithTReturn()
        {
            Task<int> result = SumAsync(1, 2);
            result.ContinueWith(t => Console.WriteLine($"异步操作结果为：{result.Result}"));
            for (int i = 0; i < 10; i++)
            {
                Thread.Sleep(1000);
                Console.WriteLine(i);
            }
            Console.ReadKey();
        }


        //2.调用异步方法SumAsync
        //不需要对返回结果操作，则可以接收Task类型的返回值，用于查看异步操作的状态等
        private static void FirstAsyncWithTaskReturn()
        {
            Task result = SumAsync(1, 2);//调用异步方法，但是不需要对其返回值进行操作，所以定义接收类型为Task
            result.ContinueWith(t => Console.WriteLine(result.Status));//查看异步操作的status
            for (int i = 0; i < 10; i++)
            {
                Thread.Sleep(1000);
                Console.WriteLine(i);
            }
            Console.ReadKey();
        }

        //3.同时调用多个异步方法,且第二个异步方法不依赖第一个异步方法的结果
        //这里没有使用await等待，而是使用了延续任务，从而也避免了async的传染性，所以这个MultipleMethod()方法没有使用async修饰
        private static void MultipleMethod()
        {
            Task<int> result1 = SumAsync(1, 2);
            Task<int> result2 = SumAsync(1, 3);
            result2.ContinueWith(t => Console.WriteLine($"result2:{result2.Result}"));
            for (int i = 0; i < 10; i++)
            {
                Thread.Sleep(1000);
                Console.WriteLine(i);
            }
            Console.ReadKey();
        }



        //4.同时调用多个异步方法,且第二个异步方法依赖第一个异步方法的结果
        //若是此时和3中的调用方法一样则，在出现result1.Result的地方会出现线程阻塞
        //所以使用await关键字
        private static async void MultipleMethod2()
        {
            int result1 = await SumAsync(1, 2);//这里使用了await 关键字则，调用方法MultipleMethod2()必须使用async修饰（即async传染性）
            int result2 = await SumAsync(1, result1);

            Console.WriteLine(result2);
            for (int i = 0; i < 10; i++)
            {
                Thread.Sleep(1000);
                Console.WriteLine(i);
            }

            Console.ReadKey();
        }

        //定义一个异步方法SumAsync
        //注意：在一个async修饰的异步方法中，
        //await表达式之前的操作在异步方法调用后就立即执行了
        //执行到await表达式，则开始异步操作，（若是此时await表达式没有完成运算，则回到调用方法中，继续调用方法中的后续代码）
        //一旦await表达式在后台完成运算后，则开始执行异步方法中await表达式的后面的操作
        //await表达式的后面的操作完成后，则把await表达式的返回结果传递到调用方法中，执行在调用方法中的异步方法延续任务
        private static async Task<int> SumAsync(int num1, int num2)
        {
            int sum = await Task.Run(() => { Thread.Sleep(3000); return num1 + num2; });
            return sum;
        }

        private static Task<int> SumAsync2(int num1, int num2)//该方法非异步方法，只是返回值类型是Task<T>
        {
            if (num1 == 0 && num2 == 0)
            {
                return Task.FromResult(0);//返回一个Task<int>类型，其Result属性为0
            }
            else
            {
                return Task.Run(() => num1 + num2);//返回一个Task<int>
            }
        }
    }


}
