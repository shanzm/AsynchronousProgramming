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
            //TestAsync1();
            TestAsync2();
            Console.ReadKey();
        }


        //Flatten()--解除异常的嵌套，使之在一个平面中
        private static async void TestAsync1()
        {
            try
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

                await task;

            }

            catch (AggregateException ae)//AggregateException类型异常的错误信息是“发生一个或多个异常”
            {
                foreach (var exception in ae.Flatten().InnerExceptions)
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

                #region 遍历嵌套
                //如果不使用Flatten(),若还想打印所有的异常信息则非常的麻烦
                //foreach (Exception ex in ae.InnerExceptions)
                //{
                //    if (ex is AggregateException)
                //    {
                //        foreach (Exception item in (ex as AggregateException).InnerExceptions)
                //        {

                //            if (item is TestException)
                //            {
                //                Console.WriteLine(item.Message);
                //            }
                //            else
                //            {
                //                foreach (var i in (item as AggregateException).InnerExceptions)
                //                {
                //                    Console.WriteLine(i.Message);
                //                }
                //            }

                //        }
                //    }
                //    else
                //    {
                //        throw;
                //    }
            }
            #endregion
            Console.ReadKey();
        }




        //使用 Handle() 方法筛选内部异常--配合await表达式
        private static async void TestAsync2()
        {
            Task t = null;
            try
            {
                await (t = Task.Run(() => { throw new TestException("异常信息：shanzm"); }));
            }
            catch (Exception)
            {
                t.Exception.Handle(e =>
                {
                    if (e is TestException)
                    {
                        Console.WriteLine(e.Message);
                    }
                    return e is TestException;
                });
            }

        }

        //使用 Handle() 方法筛选内部异常--配合Wait()方法
        private static void TestAsync3()
        {
            try
            {
                Task t = Task.Run(() => { throw new TestException("异常信息：shanzm"); });
                t.Wait();
            }
            catch (AggregateException ex)
            {
                ex.Handle(e =>
                {
                    if (e is TestException)
                    {
                        Console.WriteLine(e.Message);
                    }
                    return e is TestException;
                });
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
