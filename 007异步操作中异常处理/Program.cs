using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _007异步操作中异常处理
{
    class Program
    {
        static void Main(string[] args)
        {
            //NewMethod1();
            NewMethod2();
            Console.ReadKey();
        }


        //演示1：无法捕获异常
        //多打断点,就可以发现为何捕捉不到异常了，
        //因为执行到ThrowEx(2000, "异常信息")，开始异步方法中的await表达式，
        //即创建一个新的线程，在后台执行await表达式
        //主线程中继续ThrowEx(2000, "异常信息"); 后的代码，此时，异步方法中还在等待await表达式的执行，还没有抛出我们自己定义的异常
        // 所以此时就没有异常抛出，所以catch语句也就捕获不到异常
        // 而当异步方法抛出异常，此时主线程中catch语句已经执行完毕了！
        private static void NewMethod1()
        {
            try
            {
                ThrowEx(2000, "异常信息");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.ReadKey(); Console.ReadKey();
        }


        //演示2：捕获异常
        private static async void NewMethod2()
        {
            try
            {
                await ThrowEx(2000, "这是异常信息");
                //Console.WriteLine(t.IsFaulted);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.ReadKey();
        }
        private static async Task<int> ThrowEx(int ms, string message)
        {
            //await Task.Delay(ms).ContinueWith(t => { Console.WriteLine("hello world"); return 2; });
            await Task.Run(() => { Thread.Sleep(2000); Console.WriteLine("hello world"); return 2; });
            throw new Exception(message);
        }
    }
}
