using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _000Thread类型_给线程传递数据
{
    //在介绍Thread类型中，开启一个新线程并执行某个函数
    //这个函数是无参的，若是这个函数有参数该如何？
    //这就是给线程传递数据的问题了
    //给线程传递数据有两种方式：
    //1.Thread构造函数有一个重载是参数为ParameterizedThreadStart委托类型的，我们可以使用该重载
    //2.创建自定义类，将方法定义为实例中的方法，这样就可以在实例初始化的时候，赋值方法所需的参数


    class Program
    {
        static void Main(string[] args)
        {
            //方式1：
            //ThreadStartWithParam1();

            //方式2：
            ThreadStartWithParam2();

            Console.ReadKey();
        }

        #region 1. 使用以ParameteriedThreadStart委托类型的为参数的Thread(）构造函数
        //ParameteriedThreadStart委托，其参数为object类型，且其为无返回值类型的委托
        //重点：参数为object类型；无返回值
        static void Do(object obj)
        {
            int n = (int)obj;
            Console.WriteLine($"方法1：新开线程执行方法，其参数是{n}");
        }

        static void ThreadStartWithParam1()
        {
            Thread thread = new Thread(Do);//这里的Do函数就是ParameteriedThreadStart类型的委托
            int n = 999;
            thread.Start(n);//在Start函数中传递参数
        }
        #endregion

        #region 1. 使用自定义类，将方法封装，则在自定义类实例化的时候，传递参数
        //期望在新开线程中执行Do(int n)方法，我们将其封装在MyClass类中，其参数直接定义为一个类的属性

        static void ThreadStartWithParam2()
        {
            MyClass myClass = new MyClass(999);
            Thread thread = new Thread(myClass.Do);
            thread.Start();
        }
        #endregion
    }

    class MyClass
    {
        public int param { get; set; }

        public MyClass(int n)
        {
            this.param = n;
        }
        public void Do()
        {
            Console.WriteLine($"方法2：新开线程执行方法，其参数是{param}");
        }
    }

}
