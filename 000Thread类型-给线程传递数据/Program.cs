﻿using System;
using System.Threading;

namespace _000Thread类型_给线程传递数据
{
    //在介绍Thread类型中，开启一个新线程并执行某个函数
    //这个函数是无参的，若是这个函数有参数该如何？
    //这就是给线程传递数据的问题了
    //给线程传递数据有两种方式：
    //1.Thread构造函数有一个重载是参数为ParameterizedThreadStart委托类型的，我们可以使用该重载
    //2.创建自定义类，将方法定义为实例中的方法，这样就可以在实例初始化的时候，赋值方法所需的参数
    //3.通过使用Lambda表达式调用函数

    internal class Program
    {
        private static void Main(string[] args)
        {
            //1.方式1：
            //ThreadStartWithParam1();

            //2.方式2：
            //ThreadStartWithParam2();

            //3.方式3：
            //ThreadStartWithParam3();

            //4.说明：
            //TestLambda();

            //5. 获取返回值1
            //GetReturnValue();

            //6. 获取返回值2
            GetReturnValue2();

            Console.ReadKey();
        }

        #region 1. 使用以ParameterizedThreadStart委托类型的为参数的Thread(）构造函数,并使用Thread.Start()来传递参数

        //ParameteriedThreadStart委托，其参数为object类型，且其为无返回值类型的委托
        //注意两点：参数为object类型；无返回值
        //这种方法是最原始的给线程传递参数的方法
        private static void Do(object obj)
        {
            int n = (int)obj;
            Console.WriteLine($"方法1：新开线程执行方法，其参数是{n}");
        }

        private static void ThreadStartWithParam1()
        {
            Thread thread = new Thread(Do);//这里的Do函数就是ParameterizedThreadStart类型的委托
            int n = 999;
            thread.Start(n);//在Start函数中传递参数
        }

        #endregion

        #region 2. 使用自定义类，将方法封装，则在自定义类实例化的时候，传递参数

        //期望在新开线程中执行Do(int n)方法，我们将其封装在MyClass类中，其参数直接定义为一个类的属性
        /// <summary>
        /// 定义一个类，将我们需要在新开线程中执行的方法，封装起来，
        /// 通过类的实例化给该函数中变量赋值（相当于传递参数）
        /// </summary>
        private class MyClass
        {
            public int param { get; set; }

            public MyClass(int n)
            {
                param = n;
            }

            public void Do()
            {
                Console.WriteLine($"方法2：新开线程执行方法，其参数是{param}");
            }
        }

        private static void ThreadStartWithParam2()
        {
            MyClass myClass = new MyClass(999);
            Thread thread = new Thread(myClass.Do);
            thread.Start();
        }

        #endregion

        #region 3. 使用Lambda表达式,在Lambda表达式中调用指定的函数

        //在C#3.0之前没有Lambda表达式，所以C#3.0之前一般只能使用上述两种方法
        private static void Do(int n, int m)
        {
            Console.WriteLine(n * m);
        }

        private static void ThreadStartWithParam3()
        {
            Thread thread1 = new Thread(() => Do(2, 3));//定义一个Lambda表达式，调用Do()函数
                                                        //() => Do(2, 3);
                                                        //是指定义一个匿名函数，这个匿名函数的函数主体就一一行代码，这行代码就是调用Do(2,3)
            thread1.Start();

            //其实这里我们就是可以将所有的函数逻辑直接写在Lambda表达式中，从而更加方便
            Thread thread2 = new Thread(() => { Console.WriteLine(2 * 3); });
            thread2.Start();
        }

        #endregion

        #region 4. 关于使用Lambda表达式给线程传递参数可能出现的问题

        private static void TestLambda()
        {
            //for (int i = 0; i < 10; i++)
            //{
            //    new Thread(() => Console.Write(i)).Start();
            //}
            //打印结果每次都不一样，但是都会出现重复的值
            //和我一开始想像的不一样，我一开始以为会打印0-9，
            //循环中定义了10个线程，每个线程打印一个数字，可能顺序不一样,但是不会出现重复数字

            //为什么会出现重复的数字呢？
            //因为在这个循环中10个线程都是在操作内存中同一个位置上的变量i
            //所以某个循环步骤的中创建的线程在调用Console.WriteLine()打印i的时候，i又被别的线程修改了

            //针对上述的问题，解决方案：为循环变量创建一个临时变量
            for (int i = 0; i < 10; i++)
            {
                int temp = i;
                new Thread(() => Console.Write(temp)).Start();
                //这里打印结果就和我们期待的一样了，打印数字顺序不确定，
                //这里其实就是每次循环都定义了一个变量，也就是10个变量，他们在内存中的地址可不一样
                //我们每次循环创建的线程都是使用该循环中创建的变量，
                //不会出现一个线程中正在使用的变量，被另外一个线程修改了的现象了

                //其实，这就是说使用往线程中传递某个变量，一定注意这个变量与此同时有可能会被其他线程修改
            }
        }

        #endregion

        #region 5. 使用ParameteriedThreadStart委托，实现多参并带有返回值

        //构造辅助类，注意这里不能使用struct结构体，因为struct为值类型
        private class TempClass
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int Ret { get; set; }
        }

        private static void DoWithParam(object obj)
        {
            TempClass tempClass = obj as TempClass;
            tempClass.Ret = tempClass.X + tempClass.Y;
        }

        private static void GetReturnValue()
        {
            Thread thread = new Thread(DoWithParam);
            TempClass tempClass = new TempClass() { X = 1, Y = 2 };
            thread.Start(tempClass);
            thread.Join();//注意这里表示等待tempClass线程执行完成
            Console.WriteLine($"实现获取到返回值：{tempClass.Ret}");
        }

        #endregion

        #region 6. 上述写法可以改为类似 2中的自定义辅助类，将期望在线程中执行的方法也封装起来

        private class TempClass2
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int Ret { get; set; }

            public void DoWithParam()
            {
                Ret = X + Y;
            }
        }

        private static void GetReturnValue2()
        {
            TempClass2 tempClass2 = new TempClass2() { X = 1, Y = 2 };
            Thread thread = new Thread(tempClass2.DoWithParam);
            thread.Start();
            thread.Join();//注意这里表示等待tempClass线程执行完成
            Console.WriteLine($"实现获取到返回值：{tempClass2.Ret}");
        }

        #endregion
    }
}
