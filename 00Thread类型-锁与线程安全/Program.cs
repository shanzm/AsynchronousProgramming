using System;
using System.Threading;

namespace _000Thread类型_锁与线程安全
{
    internal class Program
    {
        //什么是线程安全，什么是非线程安全
        //非线程安全：在运行中不提供数据访问保护，这就可能导致多个线程先后更改数据，最后所得的数据是脏数据。
        //线程安全：多线程访问时，采用了加锁机制，当一个线程访问该类的某个数据时，进行保护，其他线程不能进行访问直到该线程读取完，其他线程才可使用。不会出现数据不一致或者数据污染
        //单线程则同一时间内只有一个线程操作数据，故单线程是线程安全的。
        private static void Main(string[] args)
        {
            //因为Go中有加锁，所以任何情形下都不会出现打印两次的情形，一定是打印一次
            //new Thread(Go).Start();
            //Go();

            //测试死锁
            //TestDeadLock();

            TestMonitorTryEnter();

            Console.ReadKey();
        }

        #region 加锁-lock关键字

        ///1. lock关键字，将一段代码定义为互斥段，互斥段在一个时刻内只允许一个线程进入执行，而其他线程必须等待。
        ///lock区域内的代码标识临界区，实现同一时间只有一个线程能够进入lock所包含的代码块中
        ///实现原子操作，保护同一资源只有一个线程进行修改，实现不同线程中的数据的同步
        ///未进入Lock的线程被阻塞，处于阻塞状态，直到Lock锁被打开才唤醒其中一个线程进入，并进行上锁
        ///这样会造成严重的性能问题！
        ///
        ///2. 创建的对象锁_locker 应该是不同线程能够访问的同一个对象，因此至少应该是类中的全局变量而不是类中的局部变量
        ///注意lock锁的对象应该是静态对象
        ///
        ///3. lock关键字是Monitor的语法糖
        ///
        private static bool _done;

        //创建对象锁
        private static readonly object _locker = new object();

        private static void Go()
        {
            //注意锁的对象是我们定义任意引用类型的对象（若是值类型则会装箱，而每次装箱产生的引用类型都是一个新的引用对象）
            //当两个线程竞争一个锁的时候，一个线程会进行等待（阻塞），直到锁被释放，这样来保证一次只有一个线程能够进入这个代码块
            //在不确定多线程上下文的情形下，采用这种方式进行保护的代码称为线程安全的代码
            //锁并非解决线程安全的银弹，开发的时候容易忘记在访问字段的时候加锁，而锁本身也存在一些问题（如死锁）
            lock (_locker)
            {
                if (!_done)
                {
                    Console.WriteLine("Done");
                    _done = true;
                }
            }
        }

        #endregion

        #region 测试死锁-案例1——C#7.0核心技术指南

        //死锁是什么?两个线程互相等待对方占用的资源就会使双方都无法继续执行，从而形成死锁。

        private static readonly object locker1 = new object();

        private static readonly object locker2 = new object();

        private static void LockTooMuch(object lockerA, object lockerB)
        {
            Console.WriteLine(Thread.CurrentThread.Name + "开始执行");
            lock (lockerA)
            {
                Console.WriteLine(Thread.CurrentThread.Name + "锁住lockerA（注：此时：若是线程1则锁住locker1,若是线程2锁住locker2）");
                Thread.Sleep(1000);
                lock (lockerB)
                {
                    Console.WriteLine("success");
                };
            }
        }

        private static void TestDeadLock()
        {
            //创建两个线程thread1和thread2,当一个线程thread1占用资源lockerA时，再去请求另外一个资源lockerB时
            //而此时的thread2已经抢占了lockerB,并要请求lockerA的资源，但是locker又被thread1占用,也只能进行等待
            //此时两个线程都只能一直等待对方释放资源，这就形成了死锁。

            //保持对locker1对象的锁定，等待直到locker2对象被释放（此时locker2已被thread2锁定）
            Thread thread1 = new Thread(() => LockTooMuch(locker1, locker2)); thread1.Name = "thread1";
            //保持对locker2对象的锁定，等待直到locker1对象被释放 (此时locker1已经thread1锁定)
            Thread thread2 = new Thread(() => LockTooMuch(locker2, locker1)); thread2.Name = "thread2";

            //多线程避免死锁关键，锁定对象顺序要相同。
            thread1.Start();
            thread2.Start();
        }

        #endregion

        #region 测试死锁-案例2-C#高级编程第九版21.7.2

        private static readonly StateObject t1;
        private static readonly StateObject t2;

        public void Deadlock()
        {
            int i = 0;
            while (true)
            {
                lock
            }
        }

        #endregion

        //2. 使用Monitor.TryEnter()方法。可以解决死锁问题，但是最会避免出现死锁的情形
        private static void TestMonitorTryEnter()
        {
            Thread thread1 = new Thread(() => LockTooMuch(locker1, locker2)); thread1.Name = "thread1";
            Thread thread2 = new Thread(() => TestMonitor(locker2, locker1)); thread2.Name = "thread2";
            thread1.Start();
            thread2.Start();
        }

        private static void TestMonitor(object lockerA, object lockerB)
        {
            lock (lockerB)
            {
                Thread.Sleep(1000);
                //在指定的时间内尝试获取对象的排他锁，如果当前线程获取该锁，则为true,否则为false
                //简而言之：Monitor.TryEnter是用于检查锁对象在指定的时间内是否释放子夜，如果释放则为true
                if (Monitor.TryEnter(lockerA, 1000))
                {
                    Console.WriteLine(Thread.CurrentThread.Name + ":获取一个受保护的资源成功");
                }
                else
                {
                    Console.WriteLine("获取资源超时");
                }
            }
        }
    }
}
