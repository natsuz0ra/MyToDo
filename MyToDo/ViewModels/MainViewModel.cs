using MaterialDesignThemes.Wpf;
using MyToDo.Common;
using MyToDo.Common.Models;
using MyToDo.Extensions;
using MyToDo.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MyToDo.ViewModels
{
    public class MainViewModel : BindableBase, IConfigureService
    {
        private readonly IRegionManager _regionManager;
        private IRegionNavigationJournal _navigationJournal;

        public DelegateCommand<MenuBar> NavigateCommand { get; private set; }
        public DelegateCommand GoBackCommand { get; private set; }
        public DelegateCommand GoForwardCommand { get; private set; }
        public DelegateCommand LoginOutCommand { get; private set; }

        public MainViewModel(IContainerProvider containerProvider, IRegionManager regionManager)
        {
            MenuBars = new ObservableCollection<MenuBar>();
            _regionManager = regionManager;
            NavigateCommand = new DelegateCommand<MenuBar>(Navigate);

            // 返回前一页
            GoBackCommand = new DelegateCommand(() =>
            {
                if (_navigationJournal != null && _navigationJournal.CanGoBack)
                    _navigationJournal.GoBack();
            });

            // 返回后一页
            GoForwardCommand = new DelegateCommand(() =>
            {
                if (_navigationJournal != null && _navigationJournal.CanGoForward)
                    _navigationJournal.GoForward();
            });

            // 注销
            LoginOutCommand = new DelegateCommand(() =>
            {
                if (App.Current is App appInstance)
                {
                    appInstance.LoginOut();
                }
            });

            // 注册事件
            var aggregator = containerProvider.Resolve<IEventAggregator>();
            aggregator.RegisterSetNavigationJournal(SetNavigationJournal);
        }

        private ObservableCollection<MenuBar> menuBars;

        public ObservableCollection<MenuBar> MenuBars
        {
            get { return menuBars; }
            set { menuBars = value; RaisePropertyChanged(); }
        }

        private UserDto user;

        public UserDto User
        {
            get { return user; }
            set { user = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 初始化左侧抽屉导航菜单
        /// </summary>
        void CreateMenuBar()
        {
            MenuBars.Add(new MenuBar() { Icon = "Home", Title = "首页", NameSpace = "IndexView" });
            MenuBars.Add(new MenuBar() { Icon = "NotebookOutline", Title = "待办事项", NameSpace = "ToDoView" });
            MenuBars.Add(new MenuBar() { Icon = "NotebookPlus", Title = "备忘录", NameSpace = "MemoView" });
            MenuBars.Add(new MenuBar() { Icon = "Cog", Title = "设置", NameSpace = "SettingsView" });
        }

        /// <summary>
        /// 导航菜单跳转
        /// </summary>
        /// <param name="obj"></param>
        private void Navigate(MenuBar obj)
        {
            if (obj == null || string.IsNullOrWhiteSpace(obj.NameSpace))
                return;
            _regionManager.Regions[PrismManager.MainViewRegionName].RequestNavigate(obj.NameSpace, callback =>
            {
                if (callback.Success && callback.Context != null)
                    _navigationJournal = callback.Context.NavigationService.Journal;
            });
            DrawerHost.CloseDrawerCommand.Execute(Dock.Left, null);
        }

        /// <summary>
        /// 配置首页初始化参数
        /// </summary>
        public void Configure()
        {
            User = AppSession.User;
            CreateMenuBar();
            _regionManager.Regions[PrismManager.MainViewRegionName].RequestNavigate("IndexView");
        }

        /// <summary>
        /// 路由对象设置事件监听回调
        /// </summary>
        /// <param name="navigationJournal"></param>
        public void SetNavigationJournal(IRegionNavigationJournal navigationJournal)
        {
            if (navigationJournal != null)
                _navigationJournal = navigationJournal;
        }
    }
}
