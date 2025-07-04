using MyToDo.Common;
using MyToDo.Extensions;
using MyToDo.Service;
using MyToDo.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyToDo.ViewModels.Dialogs
{
    public class LoginViewModel : BindableBase, IDialogAware
    {
        #region 配置属性，命令等
        private readonly ILoginService loginService;
        private readonly IEventAggregator aggregator;

        public string Title { get; set; } = "MyToDo";
        public DialogCloseListener RequestClose { get; set; }

        public DelegateCommand<string> ExecuteCommand { get; private set; }
        #endregion

        #region 属性
        private RegisterUserDto userDto;

        public RegisterUserDto UserDto
        {
            get { return userDto; }
            set { userDto = value; RaisePropertyChanged(); }
        }

        private int selectedIndex;

        public int SelectedIndex
        {
            get { return selectedIndex; }
            set { selectedIndex = value; RaisePropertyChanged(); }
        }
        #endregion

        public LoginViewModel(IContainerProvider containerProvider, ILoginService loginService)
        {
            ExecuteCommand = new DelegateCommand<string>(Execute);
            this.loginService = loginService;
            this.UserDto = new RegisterUserDto();
            aggregator = containerProvider.Resolve<IEventAggregator>();
        }

        private void Execute(string arg)
        {
            switch (arg)
            {
                case "login": // 登录
                    Login();
                    break;
                case "loginOut": // 退出登录
                    LoginOut();
                    break;
                case "goRegister": // 跳转注册页
                    SelectedIndex = 1;
                    Clean();
                    break;
                case "return": // 返回登录页
                    SelectedIndex = 0;
                    Clean();
                    break;
                case "register": // 注册
                    Register();
                    break;
                default:
                    break;
            }
        }

        private async void Login()
        {
            if (string.IsNullOrEmpty(UserDto.Account) || string.IsNullOrEmpty(UserDto.Password))
                return;

            var result = await loginService.LoginAsync(new UserDto()
            {
                Account = this.UserDto.Account,
                Password = this.UserDto.Password,
            });

            if (!result.Status)
            {
                return;
            }

            RequestClose.Invoke(new DialogResult(ButtonResult.OK));
        }

        private async void Register()
        {
            if (string.IsNullOrEmpty(UserDto.Account) || string.IsNullOrEmpty(UserDto.Password) || string.IsNullOrEmpty(UserDto.NewPassword) || string.IsNullOrEmpty(UserDto.Username))
                return;

            if (!UserDto.Password.Equals(UserDto.NewPassword))
                return;

            var result = await loginService.RegisterAsync(new UserDto()
            {
                Account = this.UserDto.Account,
                UserName = this.UserDto.Username,
                Password = this.UserDto.Password
            });

            if (!result.Status)
            {
                Clean();
                return;
            }

            SelectedIndex = 0;
        }

        private void LoginOut()
        {
            RequestClose.Invoke(new DialogResult(ButtonResult.No));
        }

        private void Clean()
        {
            UserDto.Account = "";
            UserDto.Username = "";
            UserDto.Password = "";
            UserDto.NewPassword = "";
        }

        #region 接口方法
        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            //LoginOut();
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {

        }
        #endregion
    }
}
