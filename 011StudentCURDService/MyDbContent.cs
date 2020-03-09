using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _011StudentCURDService
{
    class MyDbContent : DbContext
    {
        public MyDbContent() : base("name=connStr")
        {
        }
        public DbSet<Student> Students { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Student>().ToTable("dbo.Students");
        }
    }
}
