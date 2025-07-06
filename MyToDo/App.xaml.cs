using MyToDo.Extensions;
using MyToDo.Service;
using MyToDo.ViewModels;
using MyToDo.Views;
using System.Configuration;
using System.Data;
using System.Windows;
using DryIoc;
using MyToDo.Common;
using MyToDo.Views.Dialog;
using MyToDo.ViewModels.Dialogs;

namespace MyToDo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainView>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.GetContainer()
                .Register<HttpRestClient>(made: Parameters.Of.Type<string>(serviceKey: "webUrl"));
            containerRegistry.GetContainer()
                .RegisterInstance(@"http://localhost:25190/", serviceKey: "webUrl");
            containerRegistry.Register<ILoginService, LoginService>();
            containerRegistry.Register<IToDoService, ToDoService>();
            containerRegistry.Register<IMemoService, MemoService>();

            containerRegistry.Register<IDialogHostService, DialogHostService>();

            containerRegistry.RegisterForNavigation<IndexView, IndexViewModel>();
            containerRegistry.RegisterForNavigation<ToDoView, ToDoViewModel>();
            containerRegistry.RegisterForNavigation<MemoView, MemoViewModel>();
            containerRegistry.RegisterForNavigation<SettingsView, SettingsViewModel>();
            containerRegistry.RegisterForNavigation<SkinView, SkinViewModel>();
            containerRegistry.RegisterForNavigation<AboutView, AboutViewModel>();

            containerRegistry.RegisterDialog<LoginView, LoginViewModel>();

            containerRegistry.RegisterForNavigation<AddToDoView, AddToDoViewModel>();
            containerRegistry.RegisterForNavigation<AddMemoView, AddMemoViewModel>();
            containerRegistry.RegisterForNavigation<MsgView, MsgViewModel>();
        }

        protected override void OnInitialized()
        {
            var dialogService = Container.Resolve<IDialogService>();
            dialogService.ShowDialog("LoginView", callback =>
            {
                if (callback != null && callback.Result != ButtonResult.OK)
                {
                    App.Current.Shutdown();
                    return;
                }

                var service = App.Current.MainWindow.DataContext as IConfigureService;
                if (service != null)
                    service.Configure();
                base.OnInitialized();
            });
        }

        /// <summary>
        /// 处理注销的页面逻辑
        /// </summary>
        public void LoginOut()
        {
            // 隐藏主窗口
            Current.MainWindow.Hide();

            // 弹出登录窗口
            var dialogService = Container.Resolve<IDialogService>();
            dialogService.ShowDialog("LoginView", callback =>
            {
                if (callback != null && callback.Result != ButtonResult.OK)
                {
                    Environment.Exit(0);
                    return;
                }

                var service = App.Current.MainWindow.DataContext as IConfigureService;
                if (service != null)
                    service.Configure();
                Current.MainWindow.Show();
            });
        }
    }

}
