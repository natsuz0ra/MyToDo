﻿using Microsoft.AspNetCore.Mvc;
using MyToDo.Api.Context;
using MyToDo.Api.Service;
using MyToDo.Shared;
using MyToDo.Shared.Dtos;
using MyToDo.Shared.Parameters;

namespace MyToDo.Api.Controllers
{
    /// <summary>
    /// 登录控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService service;

        public LoginController(ILoginService service)
        {
            this.service = service;
        }

        [HttpPost]
        public async Task<ApiResponse> Login([FromBody] UserDto user) => await service.LoginAsync(user.Account, user.Password);

        [HttpPost]
        public async Task<ApiResponse> Register([FromBody] UserDto user) => await service.RegisterAsync(user);
    }
}
