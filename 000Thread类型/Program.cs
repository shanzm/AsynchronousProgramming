using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _000Thread类型
{
    class Program
    {
        static void Main(string[] args)
        {
            //示例1：
            //StartThread();  
            StartThreadWithLambda();
            Console.ReadKey();
        }


        //示例1：开启一个线程执行一个方法
        static void StartThread()
        {
            Console.WriteLine("这是主线程");
            Thread thread = new Thread(Do);
            thread.Start();//start表示计划执行该线程，并不表示执行该线程，而是表示，此时已经准备好，什么时候可以执行，由CPU执行，我们无法决定
            Console.WriteLine("这是主线程");
         

            #region 最终的打印结果如下，这也说明了，thread.Start()并不代表线程立即就开启
            // 这是主线程
            // 这是主线程
            // 这是开启一个新线程:执行本函数成功！
            #endregion
        }

        //期望开启一个新的线程执行的函数
        static void Do() => Console.WriteLine("这是开启一个新线程:执行本函数成功！");


        //示例2：Lambda表达式和Thread类一起使用
        //使用Lambda表达式作为Thread类构造函数的实参
        static void StartThreadWithLambda()
        {
            Thread thread = new Thread(() => Console.WriteLine("这是开启一个新线程:执行本函数成功！"));
            thread.Start();
        }
    }
}
