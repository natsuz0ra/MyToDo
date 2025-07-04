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
        private string account;

        public string Account
        {
            get { return account; }
            set { account = value; RaisePropertyChanged(); }
        }

        private string username;

        public string Username
        {
            get { return username; }
            set { username = value; RaisePropertyChanged(); }
        }

        private string password;

        public string Password
        {
            get { return password; }
            set { password = value; RaisePropertyChanged(); }
        }

        private string repectPassword;

        public string RepectPassword
        {
            get { return repectPassword; }
            set { repectPassword = value; RaisePropertyChanged(); }
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
            aggregator = containerProvider.Resolve<IEventAggregator>();
        }

        private void Execute(string arg)
        {
            switch (arg)
            {
                case "login":
                    Login();
                    break;
                case "loginOut":
                    LoginOut();
                    break;
                case "goRegister":
                    SelectedIndex = 1;
                    break;
                case "return":
                    SelectedIndex = 0;
                    break;
                case "register":
                    Register();
                    break;
                default:
                    break;
            }
        }

        private async void Login()
        {
            if (string.IsNullOrEmpty(Account) || string.IsNullOrEmpty(Password))
                return;

            var result = await loginService.LoginAsync(new UserDto()
            {
                Account = this.Account,
                Password = this.Password
            });

            if (!result.Status)
            {
                return;
            }

            RequestClose.Invoke(new DialogResult(ButtonResult.OK));
        }

        private async void Register()
        {
            if (string.IsNullOrEmpty(Account) || string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(RepectPassword) || string.IsNullOrEmpty(Username))
                return;

            if (!Password.Equals(RepectPassword))
                return;

            var result = await loginService.RegisterAsync(new UserDto()
            {
                Account = this.Account,
                UserName = this.Username,
                Password = this.Password
            });

            if (!result.Status)
            {
                return;
            }

            SelectedIndex = 0;
        }

        private void LoginOut()
        {
            RequestClose.Invoke(new DialogResult(ButtonResult.No));
        }

        #region 接口方法
        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            LoginOut();
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {

        }
        #endregion
    }
}
