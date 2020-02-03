using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


///Task类的MSDN说明就简单一句：“表示异步操作”，即为能以多线程执行的异步操作。
///另外有一个泛型Task<TResul>类，TResult 表示异步操作执行完成后返回值的类型。
///Task.Run()是一个.net framework4.5及以上定义的一个默认异步操作
///若是低于.net4.5则可以使用Task.Factory.StartNew()，和Task.Run()参数和作用一样
///注意使用Task.Run()是首选方法，但是你想要定义和启动分开，开始使用Task()直接实例化一个对象,使用Start()启动
///
namespace _002Task类的使用
{
    class Program
    {
        static void Main(string[] args)
        {
            //测试使用Task.Run()
            //FirstTaskMethod();

            //测试task.Result
            FirstTaskMethodWithReturn();
        }

        private static void FirstTaskMethodWithReturn()
        {
            #region 说明
            //调试的注意查看，运行到 Console.WriteLine(task.Result)的时候，其中Task任务还是在执行Thread.Sleep(1000)
            //还没有出结果，但是我们希望的异步执行也没有发生，而是一直在等待
            //是为什么呢？
            //是因为一胆执行了task.Result，即使task任务还没有完成，整体线程都会被锁死，直到等待task.Result出结果
            //所以可以这样理解:task.Result可以看作是一个未来
            #endregion

            Console.WriteLine("SomeDoBeforeTask");
            Func<int> Do = () => { Thread.Sleep(1000); Console.WriteLine("Task.Run结束"); return 2; };
            Task<int> task = Task.Run(Do);
            Console.WriteLine(task.Status);//使用task.Status查看当前的Task的状态：当前的状态：WaitingToRun
            Console.WriteLine(task.Result);//使用task.Result操作Task任务的返回值:返回值是：2
            Console.WriteLine(task.Status);//使用task.Status查看当前的Task的状态：当前的状态：RanToComplation
            Console.WriteLine("SomeDoAfterTask");
            Console.ReadKey();
        }

        private static void FirstTaskMethod()
        {
            //1.使用Task构造函数创建,必须显式的使用.Start()才能开始执行
            //Task task = new Task(() => { Thread.Sleep(10); Console.WriteLine("我是task ,我结束了"); });
            //task.Start();

            //2.使用TaskFactory.StartNew（工厂创建） 方法
            //Task task = Task.Factory.StartNew(() => { Thread.Sleep(10); Console.WriteLine("我是task ,我结束了"); });

            //3.使用Task.Run()
            Task task = Task.Run(() => { Thread.Sleep(10); Console.WriteLine("我是Task.Run ,我结束了"); });
            if (!task.IsCompleted)
            {
                Console.WriteLine("当前的Task.Run()尚未执行完,但是因为异步，返回到调用函数，所以可以先执行后续的代码");
            }

            Console.WriteLine("当前Task.Run还没有完成,我们在他之后的代码但是先执行了");
            task.Wait();//等待task完成
            Console.WriteLine("终于Task.Run完成了工作");
            Console.WriteLine($"Task.Run()的返回值result是个啥?是{task}");
            Console.ReadKey();
        }
    }
}
