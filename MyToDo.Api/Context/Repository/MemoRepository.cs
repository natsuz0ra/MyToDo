using Microsoft.EntityFrameworkCore;

namespace MyToDo.Api.Context.Repository
{
    public class MemoRepository : Repository<Memo>, IRepository<Memo>
    {
        public MemoRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
