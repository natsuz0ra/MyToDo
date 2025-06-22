using AutoMapper;
using MyToDo.Api.Context;
using MyToDo.Shared;
using MyToDo.Shared.Dtos;

namespace MyToDo.Api.Service
{
    public class LoginService : ILoginService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public LoginService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<ApiResponse> LoginAsync(string account, string password)
        {
            var repo = unitOfWork.GetRepository<User>();
            try
            {
                var user = await repo.GetFirstOrDefaultAsync(predicate:
                    t => (t.Account.Equals(account)) && (t.Password.Equals(password)));

                if (user == null)
                    return new ApiResponse("账号或密码错误，请重试");
                user.Password = "";
                return new ApiResponse(true, user);
            }
            catch (Exception e)
            {
                return new ApiResponse(e.Message);
            }
        }

        public async Task<ApiResponse> RegisterAsync(UserDto model)
        {
            var repo = unitOfWork.GetRepository<User>();
            try
            {
                var user = mapper.Map<User>(model);

                var check = await repo.GetFirstOrDefaultAsync(predicate: t => t.Account.Equals(user.Account));
                if (check != null)
                    return new ApiResponse($"当前帐号{user.Account}已存在，请重新注册");

                user.CreateTime = DateTime.Now;
                user.UpdateTime = DateTime.Now;

                await repo.InsertAsync(user);
                if (await unitOfWork.SaveChangesAsync() > 0)
                {
                    user.Password = "";
                    return new ApiResponse(true, user);
                }
                return new ApiResponse("注册失败，请重试");
            }
            catch (Exception e)
            {
                return new ApiResponse(e.Message);
            }
        }
    }
}
