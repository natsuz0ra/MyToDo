using AutoMapper;
using MyToDo.Api.Context;
using MyToDo.Shared;
using MyToDo.Shared.Dtos;
using MyToDo.Shared.Parameters;

namespace MyToDo.Api.Service
{
    /// <summary>
    /// 备忘录的实现
    /// </summary>
    public class MemoService : IMemoService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public MemoService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<ApiResponse> AddAsync(MemoDto model)
        {
            var repo = unitOfWork.GetRepository<Memo>();
            try
            {
                var memo = mapper.Map<Memo>(model);

                memo.CreateTime = DateTime.Now;
                memo.UpdateTime = DateTime.Now;

                await repo.InsertAsync(memo);
                if (await unitOfWork.SaveChangesAsync() > 0)
                    return new ApiResponse(true, memo);
                return new ApiResponse("添加数据失败");
            }
            catch (Exception e)
            {
                return new ApiResponse(e.Message);
            }
        }

        public async Task<ApiResponse> DeleteAsync(int id)
        {
            var repo = unitOfWork.GetRepository<Memo>();
            try
            {
                var memo = await repo.GetFirstOrDefaultAsync(predicate: t => t.Id.Equals(id));
                repo.Delete(memo);
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
            var repo = unitOfWork.GetRepository<Memo>();
            try
            {
                var memos = await repo.GetPagedListAsync(predicate:
                    t => string.IsNullOrEmpty(query.Search) ? true : t.Title.Contains(query.Search),
                    pageIndex: query.Page,
                    pageSize: query.Size,
                    orderBy: source => source.OrderByDescending(t => t.CreateTime));
                return new ApiResponse(true, memos);
            }
            catch (Exception e)
            {
                return new ApiResponse(e.Message);
            }
        }

        public async Task<ApiResponse> GetSingleAsync(int id)
        {
            var repo = unitOfWork.GetRepository<Memo>();
            try
            {
                var memo = await repo.GetFirstOrDefaultAsync(predicate: t => t.Id.Equals(id));
                return new ApiResponse(true, memo);
            }
            catch (Exception e)
            {
                return new ApiResponse(e.Message);
            }
        }

        public async Task<ApiResponse> UpdateAsync(MemoDto model)
        {
            var repo = unitOfWork.GetRepository<Memo>();
            try
            {
                var dbTodo = mapper.Map<Memo>(model);

                var memo = await repo.GetFirstOrDefaultAsync(predicate: t => t.Id.Equals(dbTodo.Id));

                memo.Title = dbTodo.Title;
                memo.Content = dbTodo.Content;
                memo.UpdateTime = DateTime.Now;

                repo.Update(memo);

                if (await unitOfWork.SaveChangesAsync() > 0)
                    return new ApiResponse(true, memo);
                return new ApiResponse("更新数据失败");
            }
            catch (Exception e)
            {
                return new ApiResponse(e.Message);
            }
        }
    }
}
