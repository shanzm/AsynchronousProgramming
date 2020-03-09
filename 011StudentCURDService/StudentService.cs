using _011StudentCURDDTO;
using _011StudentCURDIService;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//这里的使用的Linq to EF都是异步版本
//即对数据库的所有增删改查都是使用异步版本

namespace _011StudentCURDService
{
    public class StudentService : IStudentService
    {
        public async Task<long> AddAsync(string name, int age)
        {
            using (MyDbContent cxt = new MyDbContent())
            {
                Student st = new Student() { Name = name, Age = age, StuNo = "11111111" };
                cxt.Students.Add(st);
                //cxt.SaveChanges();
                await cxt.SaveChangesAsync();//使用异步版本
                return st.Id;
            }
        }

        public async Task DeleteByIdAsync(long id)
        {
            using (MyDbContent cxt = new MyDbContent())
            {
                Student st = await cxt.Students.FirstAsync(s => s.Id == id);
                cxt.Entry(st).State = EntityState.Deleted;
                await cxt.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<StudentDTO>> GetAllAsync()
        {
            using (MyDbContent cxt = new MyDbContent())
            {
                List<StudentDTO> students = new List<StudentDTO>();
                //foreach (var student in cxt.Students)
                //{
                //    StudentDTO dto = new StudentDTO() { Id = student.Id, Name = student.Name, Age = student.Age };
                //    students.Add(dto);
                //}
                await cxt.Students.ForEachAsync<Student>(student => { StudentDTO dto = new StudentDTO() { Id = student.Id, Name = student.Name, Age = student.Age }; students.Add(dto); });
                return students;
            }
        }


        public async Task<StudentDTO> GetByIdAsync(long id)
        {
            using (MyDbContent cxt = new MyDbContent())
            {
                Student st = await cxt.Students.FirstAsync(s => s.Id == id);
                if (st != null)
                {
                    StudentDTO dto = new StudentDTO() { Id = st.Id, Name = st.Name, Age = st.Age };
                    return dto;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}