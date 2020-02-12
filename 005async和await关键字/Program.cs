using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _005async和await关键字
{
    class Program
    {
        static void Main(string[] args)
        {
            //FirstAsyncWithTReturn();

            //FirstAsyncWithTaskReturn();
        }


        //接收异步操作的返回值Task<T>
        private static void FirstAsyncWithTReturn()
        {
            Task<int> result = SumAsync(1, 2);
            result.ContinueWith(t => Console.WriteLine("异步操作结果为：" + result.Result));
            for (int i = 0; i < 100; i++)
            {
                Console.WriteLine(i);
            }
            Console.ReadKey();
        }

        //不需要对返回结果操作，则可以接收Task类型的返回值，用于查看异步操作的状态等
        private static void FirstAsyncWithTaskReturn()
        {
            Task result = SumAsync(1, 2);
            result.ContinueWith(t => Console.WriteLine(result.Status));
            for (int i = 0; i < 100; i++)
            {
                Console.WriteLine(i);
            }
            Console.ReadKey();
        }


        private static async Task<int> SumAsync(int num1, int num2)
        {
            int sum = await Task.Run(() =>num1+num2);
            return sum;
        }
    }
}
