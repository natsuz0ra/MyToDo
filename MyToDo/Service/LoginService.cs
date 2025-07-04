using MyToDo.Shared;
using MyToDo.Shared.Dtos;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyToDo.Service
{
    public class LoginService : ILoginService
    {
        private readonly HttpRestClient client;
        private readonly string serviceName = "Login";

        public LoginService(HttpRestClient client)
        {
            this.client = client;
        }

        public async Task<ApiResponse<UserDto>> LoginAsync(UserDto user)
        {
            BaseRequest request = new BaseRequest();
            request.Method = Method.Post;
            request.Parameter = user;
            request.Route = $"api/{serviceName}/Login";

            return await client.ExecuteAsync<UserDto>(request);
        }

        public async Task<ApiResponse<UserDto>> RegisterAsync(UserDto user)
        {
            BaseRequest request = new BaseRequest();
            request.Method = Method.Post;
            request.Parameter = user;
            request.Route = $"api/{serviceName}/Register";

            return await client.ExecuteAsync<UserDto>(request);
        }
    }
}
