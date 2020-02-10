using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _0060FileStreamBeginRead
{
    class Program
    {
        static void Main(string[] args)
        {
            //FileStreamReadSync();


            FileStreamReadAsync();

        }

        //使用FileStream.Read()读取C:\Users\shanzm\Desktop\该路径下的1.txt文件
        private static void FileStreamReadSync()
        {
            Console.WriteLine("do something before filestream");

            using (FileStream fsRead = new FileStream(@"C:\Users\shanzm\Desktop\1.txt", FileMode.Open, FileAccess.Read))
            {
                byte[] butter = new byte[1024 * 1024 * 5];
                int byteRead = fsRead.Read(butter, 0, butter.Length);//读取的字节数
                string result = Encoding.Default.GetString(butter, 0, byteRead);
                Console.WriteLine(result);
            }

            Console.WriteLine("do something before filestream");
            Console.ReadKey();
        }

        //异步操作：
        //使用FileStream.BeginRead()
        //这里只是为了演示有这么个接口，你也可以看到MSDC中已经推荐使用ReadAsync()
        private static void FileStreamReadAsync()
        {
            Console.WriteLine($"当前执行主线程，线程ID:{Thread.CurrentThread.ManagedThreadId}:do something before filestream");

            FileStream fsRead = new FileStream(@"C:\Users\shanzm\Desktop\1.txt", FileMode.Open, FileAccess.Read);

            byte[] butter = new byte[1024 * 1024 * 5];
            IAsyncResult asyncResultresult = fsRead.BeginRead(butter, 0, butter.Length, (IAsyncResult iar) =>
            {
                int bytesRead = fsRead.EndRead(iar);
                string result = Encoding.Default.GetString((byte[])butter, 0, bytesRead);
                fsRead.Close(); //此处相当于：fsRead.Dispose();
                Console.WriteLine($"当前执行的新线程，线程ID:{Thread.CurrentThread.ManagedThreadId}:异步操作中...");
                Console.WriteLine(result);
            }, null);

            Console.WriteLine($"当前执行主线程，线程ID:{Thread.CurrentThread.ManagedThreadId}:do something after filestream");
            Console.ReadKey();

            #region 说明
            //你也看到了，很麻烦，其实也没必要，只是为了演示
            //fsRead.BeginRead()是的第三个参数是一个AsyncCallBack回调函数
            //这里我是直接使用了一个Lambda表达式，可以正确的运行
            //Undone：
            //其实一开始我单独的定义一个函数作为回调方法，但是回调方法中要使用fsRead.EndRead()，
            //但是我不知道怎么从回调方法的AsyncResult类型的参数中怎么获取fsRead对象；而且还是使用了函数外的butter对象
            //而且因为回调方法是AsyncCallBack类型的委托，只能有一个IAsyncResult类型的参数，我也无法把fsRead对象单独的做一个参数传入;
            //还有一个问题是FileStream类实现IDisposable，所以要及时释放资源，但是这里你使用using(){……}的方式会出现错误
            //所以我使用了手动的释放资源
            #endregion
        }
    }
}
