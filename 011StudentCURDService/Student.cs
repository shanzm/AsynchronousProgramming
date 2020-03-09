using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _011StudentCURDService
{
    public class Student
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string StuNo { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime CreateDateTime { get; set; } = DateTime.Now;
    }
}