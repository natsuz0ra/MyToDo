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
    public class IndexViewModel : BindableBase
    {
        private readonly IToDoService toDoService;
        private readonly IMemoService memoService;

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


        public IndexViewModel(IToDoService toDoService, IMemoService memoService)
        {
            TaskBars = new ObservableCollection<TaskBar>();
            CreateTaskBars();

            ToDoDtos = new ObservableCollection<ToDoDto>();
            MemoDtos = new ObservableCollection<MemoDto>();

            this.toDoService = toDoService;
            this.memoService = memoService;

            InitData();
        }

        private void CreateTaskBars()
        {
            TaskBars.Add(new TaskBar() { Icon = "ClockFast", Title = "汇总", Content = "9", Color = "#FF0CA0FF", Target = "" });
            TaskBars.Add(new TaskBar() { Icon = "ClockCheckOutline", Title = "已完成", Content = "8", Color = "#FF1ECA3A", Target = "" });
            TaskBars.Add(new TaskBar() { Icon = "ChartLineVariant", Title = "完成比例", Content = "89%", Color = "#FF02C6DC", Target = "" });
            TaskBars.Add(new TaskBar() { Icon = "PlaylistStar", Title = "备忘录", Content = "4", Color = "#FFFFA000", Target = "" });
        }

        async void InitData()
        {
            var todoResult = await toDoService.GetAllAsync(new QueryParameter()
            {
                Page = 0,
                Size = 100,
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
        }
    }
}
