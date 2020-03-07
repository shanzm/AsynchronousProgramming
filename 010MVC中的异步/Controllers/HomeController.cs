using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace _010MVC中的异步.Controllers
{
    public class HomeController : Controller
    {
        //同步操作
        public ActionResult Index()
        {
            string msg = "";
            using (StreamReader sr = new StreamReader(@"C:\Users\shanzm\Desktop\1.txt", Encoding.Default))
            {
                while (!sr.EndOfStream)
                {
                    msg = sr.ReadToEnd();
                }

            }
            return Content(msg);
        }

        //异步操作
        public async Task<ActionResult> Index2()
        {
            string msg = "";
            using (StreamReader sr = new StreamReader(@"C:\Users\shanzm\Desktop\1.txt", Encoding.Default))
            {
                while (!sr.EndOfStream)
                {
                    msg = await sr.ReadToEndAsync();//使用异步版本的方法
                }
            }
            return Content(msg);
        }
    }
}