using _011StudentCURDDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//注意定义异步接口，无需使用async和await关键字，只要注意异步方法返回值和异步方法名

namespace _011StudentCURDIService
{
    public interface IStudentService
    {
        Task<long> AddAsync(string name, int age);
        Task DeleteByIdAsync(long id);
        Task<StudentDTO> GetByIdAsync(long id);
        Task<IEnumerable<StudentDTO>> GetAllAsync();
    }
}
