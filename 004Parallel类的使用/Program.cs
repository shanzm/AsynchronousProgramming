using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _004Parallel类的使用
{
    class Program
    {
        static void Main(string[] args)
        {
            //ParallelFor();
            //ParallelForEach();
            ParallelInvoke();
        }
        //使用Parallel.For()对数组中的每一个元素进行并行操作
        //正常的遍历数组是按照索引的顺序执行的
        //但是并行操作，对数组的每一个元素的操作不一定按照索引顺序操作
        private static void ParallelFor()
        {
            int[] intArray = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            Console.WriteLine("------------对数组进行循环遍历------------");
            Array.ForEach(intArray, n => Console.WriteLine($"当前操作的数组元素是{n}"));//注意这里的参数n是元素而不是索引

            Console.WriteLine("------------并行操作 对数组进行循环遍历------------");
            //注意第一个参数是循环开始的索引（包含），第二个参数是循环结束的索引（不含）
            //其实就相当于：for (int i = 0; i < intArray.length; i++),只是在这里是并行操作，所以并不按照0-9的顺序执行
            //注意Parallel.For()的第三个参数是一个有参数无返回值的委托，其参数是数组的索引
            Parallel.For(0, intArray.Length, (i) => Console.WriteLine($"当前循环次数{i},当前操作的数组元素是{intArray[i]}"));
            Console.ReadKey();
        }

        //Parallel.ForEach()用于对泛型集合元素进行并行操作
        //其实就相当于：foreach (var item in collection)
        //注意 Parallel.ForEach(）的第二个参数是一个有参数无返回值的委托，其参数是集合的元素
        private static void ParallelForEach()
        {
            List<int> intList = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            Parallel.ForEach(intList, n => Console.WriteLine(n+100));
            Console.ReadKey();
        }

        //Parallel.Invoke()对指定一系列操作并行运算
        //参数是一个无返回值的Action委托数组
        private static void ParallelInvoke()
        {
            Action action1=() =>
            {
                for (int i = 0; i < 5; i++)
                {
                    Console.WriteLine($"action-1-操作");
                }
            };

            Action action2 = () =>
            {
                for (int i = 0; i < 5; i++)
                {
                    Console.WriteLine($"action-2-操作");
                }
            };
            //Parallel.Invoke(action1, action2);
            Action[] actions = { action1, action2 };
            Parallel.Invoke(actions);
            Console.ReadKey();
        }
    }
}
