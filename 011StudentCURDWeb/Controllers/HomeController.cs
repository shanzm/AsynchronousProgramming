using _011StudentCURDDTO;
using _011StudentCURDService;
using _011StudentCURDWeb.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace _011StudentCURDWeb.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public async Task<ActionResult> Index()
        {
            StudentService stuService = new StudentService();
            IEnumerable<StudentDTO> students = await stuService.GetAllAsync();
            return View(students);
        }

        [HttpGet]
        public ActionResult AddNew()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> AddNew(AddNewStudent stu)
        {
            StudentService stuService = new StudentService();
            long id = await stuService.AddAsync(stu.Name, stu.Age);
            return Redirect("~/Home/Index");
        }

        public async Task<ActionResult> Export()
        {
            StudentService stuService = new StudentService();
            IEnumerable<StudentDTO> students = await stuService.GetAllAsync();
            this.Response.AddHeader("Content-Disposition", "attachment;filename=1.txt");//弹出下载窗口
            MemoryStream ms = new MemoryStream();
            StreamWriter writer = new StreamWriter(ms);
            {
                foreach (var s in students)
                {
                    await writer.WriteLineAsync(s.Name + s.Age);
                }
            }
            writer.Flush();
            ms.Position = 0;
            return File(ms, "text/plain");//返回File()即下载文件
        }
    }
}