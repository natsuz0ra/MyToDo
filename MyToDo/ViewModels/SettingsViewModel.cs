using MaterialDesignThemes.Wpf;
using MyToDo.Common.Models;
using MyToDo.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MyToDo.ViewModels
{
    public class SettingsViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;

        public DelegateCommand<MenuBar> NavigateCommand { get; private set; }

        public SettingsViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            NavigateCommand = new DelegateCommand<MenuBar>(Navigate);
            CreateMenuBar();
        }

        private ObservableCollection<MenuBar> menuBars;

        public ObservableCollection<MenuBar> MenuBars
        {
            get { return menuBars; }
            set { menuBars = value; RaisePropertyChanged(); }
        }

        void CreateMenuBar()
        {
            menuBars = new ObservableCollection<MenuBar>();
            menuBars.Add(new MenuBar() { Icon = "Palette", Title = "个性化", NameSpace = "SkinView" });
            menuBars.Add(new MenuBar() { Icon = "Cog", Title = "系统设置", NameSpace = "" });
            menuBars.Add(new MenuBar() { Icon = "Information", Title = "关于更多", NameSpace = "AboutView" });
        }

        /// <summary>
        /// 导航菜单跳转
        /// </summary>
        /// <param name="obj"></param>
        private void Navigate(MenuBar obj)
        {
            if (obj == null || string.IsNullOrWhiteSpace(obj.NameSpace))
                return;
            _regionManager.Regions[PrismManager.SettingsViewRegionName].RequestNavigate(obj.NameSpace);
        }
    }
}
