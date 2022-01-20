﻿using System;
using System.Threading;

namespace _000Thread类型
{
    class Program
    {
        //Thread 类中的一些常用方法和属性
        //主要用于线程的创建，挂起，停止，销毁线程
        static void Main(string[] args)
        {
            //示例1：
            //StartThread();  
            //示例2
            //StartThreadWithLambda();
            //示例3
            //ThreadDemo();
            //示例4
            //GetCurrentThread();
            //示例5
            //ThreadSleep();
            //示例6
            //JoinThread();
            ThreadSuspend();
            Console.ReadKey();
        }


        //示例1——Start()
        //开启一个线程执行一个方法
        static void StartThread()
        {
            Console.WriteLine("这是主线程");
            Thread thread = new Thread(Do);
            thread.Start();//start表示通知CLR尽快执行本线程，计划执行该线程，并不表示执行该线程，而是表示，此时已经准备好，什么时候可以执行，由CPU执行，我们无法决定
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


        //示例3——Name属性
        //通过Name属性给线程设置友好的名称
        static void ThreadDemo()
        {
            Thread thread = new Thread(() => { Console.WriteLine("test"); });
            thread.Name = "测试线程";//定义线程名称，只能线程的name 属性只能定义一次，定义后就不能修改了
            Console.WriteLine($"线程当前的状态{ thread.ThreadState}");//获取线程状态
            thread.Start();//执行Start方法
            Console.WriteLine($"线程当前的状态{ thread.ThreadState}");

            //打印结果：
            //线程当前状态：Unstarted
            //线程当前状态：Running
            //test
        }


        //示例4——CurrentThread只读属性
        //获取当前正在执行的线程ID和name属性
        static void GetCurrentThread()
        {
            Console.WriteLine($"当前运行的主线程,id:{Thread.CurrentThread.ManagedThreadId},name:{Thread.CurrentThread.Name}");
            Thread thread = new Thread(
                () => Console.WriteLine($"当前运行的新创建的线程，id:{Thread.CurrentThread.ManagedThreadId},name:{Thread.CurrentThread.Name}")
                );
            thread.Name = "myThread";
            thread.Start();
            Console.WriteLine($"当前运行的主线程{Thread.CurrentThread.ManagedThreadId}");
        }


        //示例5——Join()
        //等待线程执行结束
        //首先我们要明确线程什么时候结束，一个线程结束的条件是其构造函数传入的委托执行完毕
        //同时：一个线程一旦执行完毕就无法再次重启该线程
        static void JoinThread()
        {
            //Thread thread = new Thread(() => { Thread.Sleep(2000); Console.WriteLine("waited 2s ,this my thread"); });
            //thread.Start();
            //Console.WriteLine("this main thread");
            //以上程序打印结果：
            //this main thread 
            //waited 2s ,this my thread


            //若是我们期望等待次线程thread执行完毕再执行主线程，则次线程可以调用Join方法，将次线程加入到当前，将主线程设置为等待状态
            Thread thread2 = new Thread(() => { Thread.Sleep(2000); Console.WriteLine("waited 2s ,this my thread"); });
            thread2.Start();
            thread2.Join();//将线程thread2加入此处，将本来要运行的主线程设置为等待状态
            Console.WriteLine("this main thread");
            //打印结果：
            //waited 2s ,this my thread
            //this main thread 

        }

        #region 挂起线程


        //示例5——Sleep()
        //线程休眠,线程挂起指定的时间
        static void ThreadSleep()
        {
            Console.WriteLine($"this is main thread");
            Console.WriteLine(Thread.CurrentThread.ThreadState);//Running
            Thread.Sleep(5000);

            Console.WriteLine(Thread.CurrentThread.ThreadState);//Running
            Thread thread = new Thread(() => { Console.WriteLine($"this my thread"); });
            thread.Start();
        }

        //示例6——Suspend()和Resume()
        //已经弃用的方法
        //Suspend:挂起当前线程，如果线程已经挂起，则Suspend不起作用
        //Resume:使已经挂起的线程继续执行
        static void ThreadSuspend()
        {
            int currentThreadId = Thread.CurrentThread.ManagedThreadId;
            Console.WriteLine($"this is main thread:{currentThreadId}");

            for (int i = 0; i < 10; i++)
            {
                if (i == 5)
                {
                    Thread thread = Thread.CurrentThread;
                    thread.Suspend();

                    thread.Resume();

                }

                Console.WriteLine(i);
            }
            Console.ReadKey();
        }
        #endregion

    }
}
