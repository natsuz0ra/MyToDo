using MyToDo.Common;
using MyToDo.Common.Models;
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
        private readonly IToDoService toDoService;
        private readonly IMemoService memoService;
        private readonly IDialogHostService dialogService;

        public DelegateCommand<string> ExecuteCommand { get; private set; }
        public DelegateCommand<ToDoDto> AddToDoCommand { get; private set; }
        public DelegateCommand<MemoDto> AddMemoCommand { get; private set; }
        public DelegateCommand<ToDoDto> ToDoCompletedCommand { get; private set; }

        #region 属性

        private ObservableCollection<TaskBar> taskBars;

        public ObservableCollection<TaskBar> TaskBars
        {
            get { return taskBars; }
            set { taskBars = value; RaisePropertyChanged(); }
        }

        private ObservableCollection<ToDoDto> toDoDtos;

        public ObservableCollection<ToDoDto> ToDoDtos
        {
            get { return toDoDtos; }
            set { toDoDtos = value; RaisePropertyChanged(); }
        }

        private ObservableCollection<MemoDto> memoDtos;

        public ObservableCollection<MemoDto> MemoDtos
        {
            get { return memoDtos; }
            set { memoDtos = value; RaisePropertyChanged(); }
        }

        #endregion

        public IndexViewModel(IContainerProvider containerProvider, IDialogHostService dialogService) : base(containerProvider)
        {
            CreateTaskBars();

            ToDoDtos = new ObservableCollection<ToDoDto>();
            MemoDtos = new ObservableCollection<MemoDto>();

            ExecuteCommand = new DelegateCommand<string>(Execute);
            AddToDoCommand = new DelegateCommand<ToDoDto>(AddToDo);
            AddMemoCommand = new DelegateCommand<MemoDto>(AddMemo);
            ToDoCompletedCommand

            this.toDoService = containerProvider.Resolve<IToDoService>();
            this.memoService = containerProvider.Resolve<IMemoService>();
            this.dialogService = dialogService;

            InitData();
        }

        private void CreateTaskBars()
        {
            TaskBars = new ObservableCollection<TaskBar>();
            TaskBars.Add(new TaskBar() { Icon = "ClockFast", Title = "汇总", Content = "9", Color = "#FF0CA0FF", Target = "" });
            TaskBars.Add(new TaskBar() { Icon = "ClockCheckOutline", Title = "已完成", Content = "8", Color = "#FF1ECA3A", Target = "" });
            TaskBars.Add(new TaskBar() { Icon = "ChartLineVariant", Title = "完成比例", Content = "89%", Color = "#FF02C6DC", Target = "" });
            TaskBars.Add(new TaskBar() { Icon = "PlaylistStar", Title = "备忘录", Content = "4", Color = "#FFFFA000", Target = "" });
        }

        async void InitData()
        {
            UpdateLoading(true);

            var todoResult = await toDoService.GetAllFilterAsync(new ToDoParameter()
            {
                Page = 0,
                Size = 100,
                Status = 0,
            });

            if (todoResult.Status)
            {
                toDoDtos.Clear();
                foreach (var item in todoResult.Result.Items)
                {
                    toDoDtos.Add(item);
                }
            }

            var memoResult = await memoService.GetAllAsync(new QueryParameter()
            {
                Page = 0,
                Size = 100,
            });

            memoDtos.Clear();
            if (memoResult.Status)
            {
                foreach (var memo in memoResult.Result.Items)
                {
                    memoDtos.Add(memo);
                }
            }

            UpdateLoading(false);
        }

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
        /// 新增待办
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
            }
            InitData();
        }

        /// <summary>
        /// 新增备忘录
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
            }
            InitData();
        }

        async void ToDoCompleted(ToDoDto model)
        {
            model.Status = 1;

        }
    }
}
