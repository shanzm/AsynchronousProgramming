using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _000Thread类型_捕获异常
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            #region 线程执行和线程创建时所处的try catch语句无关

            try
            {
                new Thread(() => Do(1)).Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            #endregion

            #region 线程中方法的异常的捕获解决方案：将try catch写在需要新开线程执行的方法的方法体中

            //比如这里我们可以将try catch 语句写在方法Do()中，见Do2()函数
            //这里我们就可以捕获到Do2()中的除0异常了
            new Thread(() => Do2(1)).Start();

            #endregion

            Console.ReadKey();
        }

        private static double Do(int n)
        {
            return n / 0;//这里会抛出除0异常
        }

        private static double Do2(int n)
        {
            try
            {
                return n / 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }
    }
}
