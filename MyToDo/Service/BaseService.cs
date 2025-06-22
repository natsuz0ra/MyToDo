using MyToDo.Shared;
using MyToDo.Shared.Parameters;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyToDo.Service
{
    public class BaseService<TEntity> : IBaseService<TEntity> where TEntity : class
    {
        protected readonly HttpRestClient client;
        protected readonly string serviceName;

        public BaseService(HttpRestClient client, string serviceName)
        {
            this.client = client;
            this.serviceName = serviceName;
        }

        public async Task<ApiResponse<TEntity>> AddAsync(TEntity entity)
        {
            BaseRequest request = new BaseRequest();
            request.Method = Method.Post;
            request.Parameter = entity;
            request.Route = $"api/{serviceName}/Add";

            return await client.ExecuteAsync<TEntity>(request);
        }

        public async Task<ApiResponse<TEntity>> UpdateAsync(TEntity entity)
        {

            BaseRequest request = new BaseRequest();
            request.Method = Method.Post;
            request.Parameter = entity;
            request.Route = $"api/{serviceName}/Update";

            return await client.ExecuteAsync<TEntity>(request);
        }

        public async Task<ApiResponse> DeleteAsync(int id)
        {
            BaseRequest request = new BaseRequest();
            request.Method = Method.Delete;
            request.Route = $"api/{serviceName}/Delete?id={id}";

            return await client.ExecuteAsync(request);
        }

        public async Task<ApiResponse<PagedList<TEntity>>> GetAllAsync(Shared.Parameters.QueryParameter param)
        {
            BaseRequest request = new BaseRequest();
            request.Method = Method.Get;
            request.Route = $"api/{serviceName}/GetAll?page={param.Page}&size={param.Size}&search={param.Search}";

            return await client.ExecuteAsync<PagedList<TEntity>>(request);
        }

        public async Task<ApiResponse<TEntity>> GetFirstOrDefaultAsync(int id)
        {
            BaseRequest request = new BaseRequest();
            request.Method = Method.Get;
            request.Route = $"api/{serviceName}/Get?id={id}";

            return await client.ExecuteAsync<TEntity>(request);
        }
    }
}
