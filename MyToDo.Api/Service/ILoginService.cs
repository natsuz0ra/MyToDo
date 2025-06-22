using MyToDo.Shared;
using MyToDo.Shared.Dtos;

namespace MyToDo.Api.Service
{
    public interface ILoginService
    {
        Task<ApiResponse> LoginAsync(string Account, string Password);

        Task<ApiResponse> RegisterAsync(UserDto user);
    }
}
