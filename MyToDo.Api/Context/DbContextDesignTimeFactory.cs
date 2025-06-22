using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using MyToDo.Api.Context;

namespace MyToDo.Api
{
    public class DbContextDesignTimeFactory : IDesignTimeDbContextFactory<MyToDoContext>
    {
        public MyToDoContext CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder<MyToDoContext> builder = new DbContextOptionsBuilder<MyToDoContext>();
            builder.UseSqlite("Data Source=to.db");
            return new MyToDoContext(builder.Options);
        }
    }
}