using AutoMapper;
using MyToDo.Api.Context;
using MyToDo.Shared;
using MyToDo.Shared.Dtos;
using MyToDo.Shared.Parameters;

namespace MyToDo.Api.Service
{

    /// <summary>
    /// 待办事项的实现
    /// </summary>
    public class ToDoService : IToDoService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public ToDoService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<ApiResponse> AddAsync(ToDoDto model)
        {
            var repo = unitOfWork.GetRepository<ToDo>();
            try
            {
                var todo = mapper.Map<ToDo>(model);

                todo.CreateTime = DateTime.Now;
                todo.UpdateTime = DateTime.Now;

                await repo.InsertAsync(todo);
                if (await unitOfWork.SaveChangesAsync() > 0)
                    return new ApiResponse(true, todo);
                return new ApiResponse("添加数据失败");
            }
            catch (Exception e)
            {
                return new ApiResponse(e.Message);
            }
        }

        public async Task<ApiResponse> DeleteAsync(int id)
        {
            var repo = unitOfWork.GetRepository<ToDo>();
            try
            {
                var todo = await repo.GetFirstOrDefaultAsync(predicate: t => t.Id.Equals(id));
                repo.Delete(todo);
                if (await unitOfWork.SaveChangesAsync() > 0)
                    return new ApiResponse("", true);
                return new ApiResponse("删除数据失败");
            }
            catch (Exception e)
            {
                return new ApiResponse(e.Message);
            }
        }

        public async Task<ApiResponse> GetAllAsync(QueryParameter query)
        {
            var repo = unitOfWork.GetRepository<ToDo>();
            try
            {
                var todos = await repo.GetPagedListAsync(predicate:
                    t => string.IsNullOrEmpty(query.Search) ? true : t.Title.Contains(query.Search),
                    pageIndex: query.Page,
                    pageSize: query.Size,
                    orderBy: source => source.OrderByDescending(t => t.CreateTime));
                return new ApiResponse(true, todos);
            }
            catch (Exception e)
            {
                return new ApiResponse(e.Message);
            }
        }

        public async Task<ApiResponse> GetAllAsync(ToDoParameter query)
        {
            var repo = unitOfWork.GetRepository<ToDo>();
            try
            {
                var todos = await repo.GetPagedListAsync(predicate:
                    t => (string.IsNullOrEmpty(query.Search) ? true : t.Title.Contains(query.Search))
                    && (query == null || query.Status == null ? true : t.Status.Equals(query.Status.Value)),
                    pageIndex: query.Page,
                    pageSize: query.Size,
                    orderBy: source => source.OrderByDescending(t => t.CreateTime));
                return new ApiResponse(true, todos);
            }
            catch (Exception e)
            {
                return new ApiResponse(e.Message);
            }
        }

        public async Task<ApiResponse> GetSingleAsync(int id)
        {
            var repo = unitOfWork.GetRepository<ToDo>();
            try
            {
                var todo = await repo.GetFirstOrDefaultAsync(predicate: t => t.Id.Equals(id));
                return new ApiResponse(true, todo);
            }
            catch (Exception e)
            {
                return new ApiResponse(e.Message);
            }
        }

        public async Task<ApiResponse> UpdateAsync(ToDoDto model)
        {
            var repo = unitOfWork.GetRepository<ToDo>();
            try
            {
                var dbTodo = mapper.Map<ToDo>(model);

                var todo = await repo.GetFirstOrDefaultAsync(predicate: t => t.Id.Equals(dbTodo.Id));

                todo.Title = dbTodo.Title;
                todo.Content = dbTodo.Content;
                todo.Status = dbTodo.Status;
                todo.UpdateTime = DateTime.Now;

                repo.Update(todo);

                if (await unitOfWork.SaveChangesAsync() > 0)
                    return new ApiResponse(true, todo);
                return new ApiResponse("更新数据失败");
            }
            catch (Exception e)
            {
                return new ApiResponse(e.Message);
            }
        }
    }
}
