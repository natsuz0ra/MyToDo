using AutoMapper;
using MyToDo.Api.Context;
using MyToDo.Shared;
using MyToDo.Shared.Dtos;
using MyToDo.Shared.Parameters;
using System.Collections.ObjectModel;

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

        public async Task<ApiResponse> Summary()
        {
            var todoRepo = unitOfWork.GetRepository<ToDo>();
            var memoRepo = unitOfWork.GetRepository<Memo>();

            try
            {
                // 待办事项列表
                var todos = await todoRepo.GetAllAsync(
                    orderBy: source => source.OrderByDescending(t => t.CreateTime));

                // 备忘录列表
                var memos = await memoRepo.GetAllAsync(
                    orderBy: source => source.OrderByDescending(t => t.CreateTime));

                SummaryDto summary = new SummaryDto();
                summary.ToDoList = new ObservableCollection<ToDoDto>(mapper.Map<List<ToDoDto>>(todos.Where(t => t.Status.Equals(0))));
                summary.MemoList = new ObservableCollection<MemoDto>(mapper.Map<List<MemoDto>>(memos.Where(t => true)));
                summary.Sum = todos.Count(); // 待办事项总数
                summary.CompletedCount = todos.Where(t => t.Status.Equals(1)).Count(); // 待办事项完成数
                summary.CompletedRatio = (summary.CompletedCount / (double)summary.Sum).ToString("0%"); // 待办事项完成比例
                summary.MemoCount = memos.Count(); // 备忘录总数

                return new ApiResponse(true, summary);
            }
            catch (Exception e)
            {
                return new ApiResponse(e.Message);
            }
        }
    }
}
