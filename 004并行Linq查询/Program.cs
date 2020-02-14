using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _004并行Linq查询
{
    class Program
    {
        static void Main(string[] args)
        {
            //一个简单的小示例，求1到5000中可以整除3的数
            //倒序排序在modThreeIsZero[]中
            int[] intArray = Enumerable.Range(1, 50000000).ToArray();

            Stopwatch sw = new Stopwatch();

            //顺序查询
            sw.Start();
            int[] modThreeIsZero1 = intArray.Select(n => n).Where(n => n % 3 == 0).OrderByDescending(n => n).ToArray();
            sw.Stop();
            Console.WriteLine($"顺序查询，运行时间：{sw.ElapsedMilliseconds}毫秒,可以整除3的个数:{modThreeIsZero1.Count()}");

            //使用AsParallel()实现并行查询
            sw.Restart();
            int[] modThreeIsZero2 = intArray.AsParallel().Select(n => n).Where(n => n % 3 == 0).OrderByDescending(n => n).ToArray();
            sw.Stop();
            Console.WriteLine($"并行查询，运行时间：{sw.ElapsedMilliseconds}毫秒,可以整除3的个数:{modThreeIsZero2.Count()}");

            Console.ReadKey();
        }
    }
}
