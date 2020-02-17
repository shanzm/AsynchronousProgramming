using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#region 说明
//下面的代码展示了如何使用CancellationTokenSource和CancellationToken来实现取消某个异步操作。
//注意，该过程是协同的。即调用CancellationTokenSource的Cancel时，它本身并不会执行取消操作。
//而是会将CancellationToken的IsCancellationRequested属性设置为true。
//包含CancellationToken的代码负责检查该属性，并判断是否需要停止执行并返回。
#endregion

namespace _0030取消一个异步操作
{
    class Program
    {
        static void Main(string[] args)
        {
            CancellTask();

            CancellTask2();
        }


        //示例1：简单的演示使用CancellationTokenSource和CancellationToken对象取消一个异步程序
        private static void CancellTask()
        {
            CancellationTokenSource cts = new CancellationTokenSource();//生成一个CancellationTokenSource对象，该对象可以创建CancellationToken
            CancellationToken ct = cts.Token;//获取一个令牌（token)可以将令牌传递给多个任务，这样可以同时取消多个任务。
            Task.Run(() =>
            {
                for (int i = 0; i < 20; i++)
                {
                    if (ct.IsCancellationRequested)
                    {
                        return;
                    }
                    Thread.Sleep(1000);
                    Console.WriteLine($"异步程序的的循环：{i}");
                }
            }, ct);//注意Run()的第二个参数就是终止令牌token
            for (int i = 0; i < 4; i++)
            {

                Thread.Sleep(1000);
                Console.WriteLine($"主线程中循环：{i}");
            }
            Console.WriteLine("马上sts.Cancel(),即将要终止异步程序");
            cts.Cancel();//一旦运行了cts.Cancel(),则不论异步运行到什么状况，含有该CancellationTokenSource的token的异步程序，终止！
            Console.ReadKey();
        }


        //示例2：取消一个异步程序，其中该异步方法是一个使用async和await关键字实现的
        private static void CancellTask2()
        {
            CancellationTokenSource cts = new CancellationTokenSource();//生成一个CancellationTokenSource对象，该对象可以创建CancellationToken
            CancellationToken ct = cts.Token;//获取一个令牌（token)
            Task result = DoAsync(ct, 50);
            for (int i = 0; i <= 5; i++)//主线程中的循环（模拟在异步方法声明之后的工作）
            {
                Thread.Sleep(1000);
                Console.WriteLine("主线程中的循环次数：" + i);
            }
            cts.Cancel();//注意在主线程中的循环结束后（5s左右），运行到此处，
                         //则此时CancellationTokenSource对象中的所有的token.IsCancellationRequested==true
                         //则在异步操作DoAsync()中根据此判断，则取消异步操作
            Console.ReadKey();
        }

        //示例2中调用的异步方法
        static async Task DoAsync(CancellationToken ct, int Max)
        {
            await Task.Run(() =>
            {
                for (int i = 0; i <= Max; i++)
                {
                    if (ct.IsCancellationRequested)//一旦CancellationToken对象的源CancellationTokenSource运行了Cancel();
                                                   //则此时CancellationToken.IsCancellationRequested==ture
                    {
                        return;
                    }
                    Thread.Sleep(1000);
                    Console.WriteLine("次线程中的循环次数：" + i);
                }
            });
        }
    }
}
