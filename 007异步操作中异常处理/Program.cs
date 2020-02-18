using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _007异步操作中异常处理
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ThrowEx(2000, "异常信息");
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
            Console.ReadKey();
        }
        private async static void ThrowEx(int ms, string message)
        {
            await Task.Delay(ms).ContinueWith(t => Console.WriteLine("hello word"));
            throw new Exception(message);
        }
    }
}
