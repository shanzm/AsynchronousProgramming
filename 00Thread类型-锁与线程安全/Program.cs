using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _000Thread类型_锁与线程安全
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //因为Go中有加锁，所以任何情形下都不会出现打印两次的情形，一定是打印一次
            new Thread(Go).Start();
            Go();

            Console.ReadKey();
        }

        #region lock关键字

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
    }
}
