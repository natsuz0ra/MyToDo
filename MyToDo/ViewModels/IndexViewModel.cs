using MyToDo.Common;
using MyToDo.Common.Events;
using MyToDo.Common.Models;
using MyToDo.Extensions;
using MyToDo.Service;
using MyToDo.Shared.Dtos;
using MyToDo.Shared.Parameters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyToDo.ViewModels
{
    public class IndexViewModel : NavigationViewModel
    {
        #region 命令和服务

        private readonly IToDoService toDoService;
        private readonly IMemoService memoService;
        private readonly IDialogHostService dialogService;
        private readonly IRegionManager regionManager;
        private readonly IEventAggregator aggregator;

        public DelegateCommand<string> ExecuteCommand { get; private set; }
        public DelegateCommand<ToDoDto> EditToDoCommand { get; private set; }
        public DelegateCommand<MemoDto> EditMemoCommand { get; private set; }
        public DelegateCommand<ToDoDto> ToDoCompletedCommand { get; private set; }
        public DelegateCommand<TaskBar> NavigateCommand { get; private set; }

        #endregion

        #region 属性

        private ObservableCollection<TaskBar> taskBars;

        public ObservableCollection<TaskBar> TaskBars
        {
            get { return taskBars; }
            set { taskBars = value; RaisePropertyChanged(); }
        }

        private SummaryDto summary;

        public SummaryDto Summary
        {
            get { return summary; }
            set { summary = value; RaisePropertyChanged(); }
        }

        private string title;

        public string Title
        {
            get { return title; }
            set { title = value; RaisePropertyChanged(); }
        }


        #endregion

        public IndexViewModel(IContainerProvider containerProvider, IDialogHostService dialogService) : base(containerProvider)
        {
            CreateTaskBars();

            ExecuteCommand = new DelegateCommand<string>(Execute);
            EditToDoCommand = new DelegateCommand<ToDoDto>(AddToDo);
            EditMemoCommand = new DelegateCommand<MemoDto>(AddMemo);
            ToDoCompletedCommand = new DelegateCommand<ToDoDto>(Completed);
            NavigateCommand = new DelegateCommand<TaskBar>(Navigate);

            this.toDoService = containerProvider.Resolve<IToDoService>();
            this.memoService = containerProvider.Resolve<IMemoService>();
            this.regionManager = containerProvider.Resolve<IRegionManager>();
            this.aggregator = containerProvider.Resolve<IEventAggregator>();
            this.dialogService = dialogService;
        }

        #region 方法

        /// <summary>
        /// 创建汇总数据菜单
        /// </summary>
        private void CreateTaskBars()
        {
            TaskBars = new ObservableCollection<TaskBar>();
            TaskBars.Add(new TaskBar() { Icon = "ClockFast", Title = "汇总", Content = "-", Color = "#FF0CA0FF", Target = "ToDoView" });
            TaskBars.Add(new TaskBar() { Icon = "ClockCheckOutline", Title = "已完成", Content = "-", Color = "#FF1ECA3A", Target = "ToDoView" });
            TaskBars.Add(new TaskBar() { Icon = "ChartLineVariant", Title = "完成比例", Content = "-", Color = "#FF02C6DC", Target = "" });
            TaskBars.Add(new TaskBar() { Icon = "PlaylistStar", Title = "备忘录", Content = "-", Color = "#FFFFA000", Target = "MemoView" });
        }

        /// <summary>
        /// 刷新首页数据
        /// </summary>
        async void RefreshData()
        {
            UpdateLoading(true);

            var summaryResult = await toDoService.SummaryAsync();

            if (summaryResult.Status)
            {
                Summary = summaryResult.Result;

                TaskBars[0].Content = Summary.Sum.ToString();
                TaskBars[1].Content = Summary.CompletedCount.ToString();
                TaskBars[2].Content = Summary.CompletedRatio;
                TaskBars[3].Content = Summary.MemoCount.ToString();
            }

            UpdateLoading(false);
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="arg"></param>
        private void Execute(string arg)
        {
            switch (arg)
            {
                case "AddToDo":
                    AddToDo(null);
                    break;
                case "AddMemo":
                    AddMemo(null);
                    break;
            }
        }

        /// <summary>
        /// 新增/编辑待办
        /// </summary>
        async void AddToDo(ToDoDto model)
        {
            DialogParameters param = new DialogParameters();
            if (model != null)
                param.Add("value", model);

            var result = await dialogService.ShowDialog("AddToDoView", param);
            if (result.Result == ButtonResult.OK)
            {
                var todo = result.Parameters.GetValue<ToDoDto>("value");

                if (todo.Id > 0)
                {
                    var updateResult = await toDoService.UpdateAsync(todo);
                }
                else
                {
                    var addResult = await toDoService.AddAsync(todo);
                }
                RefreshData();
            }
        }

        /// <summary>
        /// 新增/编辑备忘录
        /// </summary>
        async void AddMemo(MemoDto model)
        {
            DialogParameters param = new DialogParameters();
            if (model != null)
                param.Add("value", model);

            var result = await dialogService.ShowDialog("AddMemoView", param);
            if (result.Result == ButtonResult.OK)
            {
                var memo = result.Parameters.GetValue<MemoDto>("value");

                if (memo.Id > 0)
                {
                    var updateResult = await memoService.UpdateAsync(memo);
                }
                else
                {
                    var addResult = await memoService.AddAsync(memo);
                }
                RefreshData();
            }
        }

        /// <summary>
        /// 完成待办
        /// </summary>
        /// <param name="model"></param>
        async void Completed(ToDoDto model)
        {
            UpdateLoading(true);

            try
            {
                var updateResult = await toDoService.UpdateAsync(model);
            }
            catch (Exception ex)
            {
            }
            finally
            {
                UpdateLoading(false);
                aggregator.SendMessage("已完成");
                RefreshData();
            }
        }

        void Navigate(TaskBar taskBar)
        {
            if (taskBar.Target == null) return;

            NavigationParameters param = new NavigationParameters();

            if (taskBar.Title == "已完成")
            {
                param.Add("selectedStatus", 2);
            }
            regionManager.Regions[PrismManager.MainViewRegionName].RequestNavigate(taskBar.Target, param);
        }

        #endregion

        /// <summary>
        /// 导航进入页面初始化事件
        /// </summary>
        /// <param name="navigationContext"></param>
        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            Title = $"你好，{AppSession.User.UserName}！今天是 {DateTime.Now.GetDateTimeFormats('D')[1]}";
            RefreshData();
            base.OnNavigatedTo(navigationContext);
        }
    }
}
