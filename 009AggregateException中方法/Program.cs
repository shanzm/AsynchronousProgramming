using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _009AggregateException中方法
{
    class Program
    {
        static void Main(string[] args)
        {
            TestAsync();
            Console.ReadKey();
        }

        private static async void TestAsync()
        {
            //通过下面的一个任务，模拟抛出的异常出现嵌套
            //注意Task.Run()无法设置： TaskCreationOptions属性
            var task = Task.Factory.StartNew(() =>
            {
                var child1 = Task.Factory.StartNew(() =>
                {
                    var child2 = Task.Factory.StartNew(() =>
                    {
                        throw new TestException("Attached child2 faulted.");
                    }, TaskCreationOptions.AttachedToParent);//表示创建的Task是当前线程所在Task的子任务

                    throw new TestException("Attached child1 faulted.");
                }, TaskCreationOptions.AttachedToParent);
            });

            try
            {
                await task;
            }
            catch (AggregateException ex)
            {
                foreach (var exception in ex.Flatten().InnerExceptions)
                //使用AggregateException的Flatten()方法，除去异常的嵌套,这里你也可以测试不使用Flatten(),抛出的异常信息为“有一个或多个异常”
                {
                    if (exception is TestException)
                    {

                        Console.WriteLine(exception.Message);
                    }
                    else
                    {
                        throw;
                    }

                }

                Console.ReadKey();
            }

        }


        //定义异常类
        public class TestException : Exception
        {
            public TestException(string message) : base(message)
            {

            }
        }
    }
}
