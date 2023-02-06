using System;
using System.Threading;

namespace _000Thread类型
{
    //msdn:https://learn.microsoft.com/zh-cn/dotnet/api/system.threading.thread.apartmentstate?view=netframework-4.6
    internal class Program
    {
        //Thread 类中的一些常用方法和属性
        //主要用于线程的创建，挂起，停止，销毁线程
        private static void Main(string[] args)
        {
            //ThreadStart();//创建一个线程，并使用Start方法

            ThreadStartWithLambda();//Start方法执行Lambda

            //ThreadState();//ThreadState属性，IsActive属性，Name属性

            //GetCurrentThread();//获取当前运行的线程

            //ThreadSleep();//将当前线程挂起指定的时间

            //ThreadJoin();//在此实例表示的线程终止前，阻止调用线程。

            //ThreadJoin2();

            //ThreadSuspend();//挂起线程，继续已挂起的线程

            Console.ReadKey();
        }

        //示例1——Start()
        //开启一个线程执行一个方法
        private static void ThreadStart()
        {
            Console.WriteLine("这是主线程");
            Thread thread = new Thread(Do);
            thread.Start();//start表示通知CLR尽快执行本线程，计划执行该线程，并不表示执行该线程，而是表示，此时已经准备好，什么时候可以执行，由CPU执行，我们无法决定
            Console.WriteLine("这是主线程");

            //最终的打印结果如下，这也说明了，thread.Start()并不代表线程立即就开启
            // 这是主线程
            // 这是主线程
            // 这是开启一个新线程:执行本函数成功！
        }

        //期望开启一个新的线程执行的函数
        private static void Do() => Console.WriteLine("开启一个新线程:执行本函数成功！");

        //示例2：Lambda表达式和Thread类一起使用
        //使用Lambda表达式作为Thread类构造函数的实参
        private static void ThreadStartWithLambda()
        {
            //Thread的构造函数参数的是一个ThreadStart类型的委托对象或ParameterizedThreadStart类型的委托对象
            //这是最刻板的写法
            Thread thread1 = new Thread(new ThreadStart(() => Do()));
            thread1.Start();

            Thread thread2 = new Thread(() => Do());//若是Do函数有参数，此法是最简单的往线程中执行的方法传递方法的写法
            thread2.Start();

            Thread thread3 = new Thread(Do);
            thread3.Start();

            //可以使用Lambda表达式直接写功能语句
            Thread thread4 = new Thread(() => Console.WriteLine("开启一个新线程:执行本函数成功！"));
            thread4.Start();
        }

        //示例3——Name属性，ThreadState属性，IsActive属性
        //通过Name属性给线程设置友好的名称，
        //Name属性只能设置一次，主要是用于调试使用
        //ThreadState是枚举类型,有许多元素（包括已经快要弃用的Suspend等）一般主要关注于四种状态：UnStarted,Running,WaitSleepJoin,Stopped
        //IsActive:线程一旦启动，该线程的IsActive值为true.
        private static void ThreadState()
        {
            Thread thread = new Thread(() => { Console.WriteLine("线程执行中"); });
            thread.Name = "测试线程";//定义线程名称，只能线程的name 属性只能定义一次，定义后就不能修改了
            Console.WriteLine($"线程当前的状态{ thread.ThreadState}");//线程当前的状态:UnStarted
            Console.WriteLine(thread.IsAlive);//False
            thread.Start();//执行Start方法
            Console.WriteLine(thread.IsAlive);//True
            Console.WriteLine($"线程当前的状态{ thread.ThreadState}");//线程当前的状态：Running
                                                               //打印结果：
                                                               //线程当前状态：Unstarted
                                                               //False
                                                               //True
                                                               //线程当前状态：Running
                                                               //线程执行中

            //注：IsActive:如果此线程已启动并且尚未正常终止或中止，则为 true；否则为 false。
            //注：如果只是想要知道一个线程是否在运行或者是否已经完成了所有的工作的话，可以使用IsAlive。
            //当然更全面的获得线程的详细的信息还是需要通过使用ThreadState来获取。

            //ThreadState是一个枚举类型，大多数值是冗余的，无用的或是废弃的。一般就关注这四种类型：Unstarted、Running、WaitSleepJoin、Stopped
            //注：ThreadState类型一般是用于调试使用，不适用实现同步。因为线程的状态可能在测试ThreadState和获取这个信息的时间段内发生变化
            //简单的说就是要通过修改线程的ThreadState属性来实现某些功能
        }

        public static void ThreadIsBackgroud()
        {
            Console.WriteLine($"{Thread.CurrentThread.Name}");
            Thread thread = new Thread(() => { });
            thread.IsBackground = true;
            thread.Start();
        }

        //示例4——CurrentThread只读属性
        //获取当前正在执行的线程ID和name属性
        private static void GetCurrentThread()
        {
            //这里可以看到Thread.CurrentThread.Name是空，当前运行的主线程是无名称的
            Console.WriteLine($"当前运行的主线程,id:{Thread.CurrentThread.ManagedThreadId},name:{Thread.CurrentThread.Name}");

            Thread thread = new Thread(
                () => Console.WriteLine($"当前运行的新创建的线程，id:{Thread.CurrentThread.ManagedThreadId},name:{Thread.CurrentThread.Name}")
                );
            thread.Name = "myThread";
            thread.Start();
            Console.WriteLine($"当前运行的主线程{Thread.CurrentThread.ManagedThreadId}");
        }

        ///什么是线程的挂起？
        ///线程的挂起操作实质上就是线程进入"非可执行"状态下

        ///在这个状态下CPU不会分给线程时间片，进入这个状态可以用来暂停一个线程的运行。
        ///线程挂起后，可以通过重新唤醒线程来使之恢复运行。
        ///注：当前线程被阻塞或是解除阻塞时，操作系统会进行一次上下文切换，这会导致细小的开销，一般在1到2毫秒左右

        ///注意：
        ///阻塞、挂起、睡眠三者是不一样的
        ///阻塞是被动的，资源被其他线程抢占，一旦一个线程被阻塞，则操作系统主动释放该线程使用的CPU资源，让出资源给其他线程运行
        ///挂起是主动进行挂起的，需要手动恢复，
        ///睡眠也是主动的，但是睡眠是设置睡眠时间，所以睡眠是到了指定的时间后自动恢复的
        ///注：阻塞不是ThreadState值，Suspend是ThreadState,但是随着Suspend()函数的弃用，Suspend状态也是被弃用的

        //示例5——Join()
        //Join被翻译为汇合，其作用是：阻塞当前线程的执行，等到调用Join的线程对象执行完毕值继续执行当前线程
        //简而言之：线程A调用了Join方法，则阻塞当前正在运行的线程，开始运行线程A中的委托对象
        //首先我们要明确线程什么时候结束，一个线程结束的条件是其构造函数传入的委托执行完毕
        //同时：一个线程一旦执行完毕就无法再次重启该线程
        private static void ThreadJoin()
        {
            //Thread thread = new Thread(() => { Thread.Sleep(2000); Console.WriteLine("waited 2s ,this my thread"); });
            //thread.Start();
            //Console.WriteLine("this main thread");
            ////以上程序打印结果：
            ////this main thread
            ////waited 2s ,this my thread

            //若是我们期望等待次线程thread执行完毕再执行主线程，则次线程可以调用Join方法，将次线程加入到当前，将主线程设置为等待状态
            Thread thread2 = new Thread(() => { Thread.Sleep(2000); Console.WriteLine("waited 2s ,this my thread"); });
            thread2.Start();
            thread2.Join();//将线程thread2 join到此处，将本来要运行的主线程设置为等待状态（即阻塞当前的线程，执行thread2线程）
            Console.WriteLine("this main thread");
            //打印结果：
            //waited 2s ,this my thread
            //this main thread

            //当线程thread2.Join(），此时主线程的ThreadState是WaitSleepJoin
        }

        #region 测试Join

        private static Thread thread1, thread2;

        private static void ThreadJoin2()
        {
            thread1 = new Thread(ThreadProc);
            thread1.Name = "Thread1";
            thread1.Start();

            thread2 = new Thread(ThreadProc);
            thread2.Name = "Thread2";
            thread2.Start();
        }

        private static void ThreadProc()
        {
            Console.WriteLine("\nCurrent thread: {0}", Thread.CurrentThread.Name);
            if (Thread.CurrentThread.Name == "Thread1" && thread2.ThreadState != System.Threading.ThreadState.Unstarted)
            {
                thread2.Join();
            }

            Thread.Sleep(4000);
            Console.WriteLine("\nCurrent thread: {0}", Thread.CurrentThread.Name);
            Console.WriteLine("Thread1: {0}", thread1.ThreadState);
            Console.WriteLine("Thread2: {0}\n", thread2.ThreadState);

            // The example displays output like the following:
            //       Current thread: Thread1
            //
            //       Current thread: Thread2
            //
            //       Current thread: Thread2
            //       Thread1: WaitSleepJoin
            //       Thread2: Running
            //
            //
            //       Current thread: Thread1
            //       Thread1: Running
            //       Thread2: Stopped
        }

        #endregion

        //示例5——Sleep()
        //线程休眠,线程挂起指定的时间
        //注：Sleep是Thread类的静态方法，在异步编程中，可以使用Sleep方法来等待其他线程完成某项操作
        //但是，Sleep方法不能准确的知道其他线程所处理时间在什么时间完成，因此Sleep方法仅适用于线程间
        //不需要精确同步的场合。若是需要精确同步，推荐使用等待句柄，例如：AutoResetEvent类
        private static void ThreadSleep()
        {
            Console.WriteLine($"this is main thread");
            Console.WriteLine(Thread.CurrentThread.ThreadState);//Running

            Thread.Sleep(5000);

            Console.WriteLine(Thread.CurrentThread.ThreadState);//Running
            Thread thread = new Thread(() => { Console.WriteLine($"this my thread"); });
            thread.Start();
        }

        //示例6——弃用的挂起方法：Suspend()和Resume()
        //已经弃用的方法
        //Suspend:挂起当前线程，如果线程已经挂起，则Suspend不起作用
        //Resume:使已经挂起的线程继续执行
        private static void ThreadSuspend()
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
    }
}
