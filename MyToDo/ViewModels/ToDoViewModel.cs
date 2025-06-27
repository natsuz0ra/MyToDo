using Microsoft.Win32;
using MyToDo.Common;
using MyToDo.Extensions;
using MyToDo.Service;
using MyToDo.Shared.Dtos;
using MyToDo.Shared.Parameters;
using Prism.Ioc;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyToDo.ViewModels
{
    public class ToDoViewModel : NavigationViewModel
    {
        private readonly IToDoService service;
        private readonly IDialogHostService dialogHostService;

        public DelegateCommand<string> ExecuteCommand { get; private set; }
        public DelegateCommand<ToDoDto> SelectedCommand { get; private set; }
        public DelegateCommand<ToDoDto> DeleteCommand { get; private set; }

        private bool isRightDrawerOpen;

        /// <summary>
        /// 右侧编辑窗口是否展开
        /// </summary>
        public bool IsRightDrawerOpen
        {
            get { return isRightDrawerOpen; }
            set { isRightDrawerOpen = value; RaisePropertyChanged(); }
        }

        private string search;

        /// <summary>
        /// 搜索条件
        /// </summary>
        public string Search
        {
            get { return search; }
            set { search = value; RaisePropertyChanged(); }
        }


        private ObservableCollection<ToDoDto> toDoDtos;

        public ObservableCollection<ToDoDto> ToDoDtos
        {
            get { return toDoDtos; }
            set { toDoDtos = value; RaisePropertyChanged(); }
        }

        private ToDoDto currentDto;

        /// <summary>
        /// 编辑选中/新增对象
        /// </summary>
        public ToDoDto CurrentDto
        {
            get { return currentDto; }
            set { currentDto = value; RaisePropertyChanged(); }
        }

        private int selectedIndex;

        /// <summary>
        /// 选中的状态/索引
        /// </summary>
        public int SelectedIndex
        {
            get { return selectedIndex; }
            set { selectedIndex = value; RaisePropertyChanged(); }
        }


        public ToDoViewModel(IContainerProvider containerProvider, IToDoService service) : base(containerProvider)
        {
            ExecuteCommand = new DelegateCommand<string>(Execute);
            SelectedCommand = new DelegateCommand<ToDoDto>(Selected);
            DeleteCommand = new DelegateCommand<ToDoDto>(Delete);

            this.service = service;
            this.dialogHostService = containerProvider.Resolve<IDialogHostService>();
            this.toDoDtos = new ObservableCollection<ToDoDto>();
        }

        /// <summary>
        /// 添加待办
        /// </summary>
        private void Execute(string action)
        {
            switch (action)
            {
                case "add": Add(); break;
                case "query": GetDataAsync(); break;
                case "save": Save(); break;
            }
        }

        private void Add()
        {
            CurrentDto = new ToDoDto();
            IsRightDrawerOpen = true;
        }

        /// <summary>
        /// 添加/修改
        /// </summary>
        private async void Save()
        {
            if (string.IsNullOrWhiteSpace(CurrentDto.Title) ||
                string.IsNullOrWhiteSpace(CurrentDto.Content))
                return;

            UpdateLoading(true);

            try
            {
                if (CurrentDto.Id > 0)
                {
                    var updateResult = await service.UpdateAsync(CurrentDto);
                    if (updateResult.Status)
                    {
                        CurrentDto = updateResult.Result;
                    }
                }
                else
                {
                    var addResult = await service.AddAsync(CurrentDto);
                    if (addResult.Status)
                    {
                        GetDataAsync();
                    }
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                UpdateLoading(false);
                IsRightDrawerOpen = false;
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        private async void Delete(ToDoDto obj)
        {
            var result = await dialogHostService.Question("温馨提示", $"确认删除待办事项：{obj.Title}？");
            if (result.Result != ButtonResult.OK)
                return;

            UpdateLoading(true);
            try
            {
                if (obj.Id > 0)
                {
                    var deleteResponse = await service.DeleteAsync(obj.Id);
                    if (deleteResponse.Status)
                    {
                        GetDataAsync();
                    }
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                UpdateLoading(false);
            }
        }

        private async void Selected(ToDoDto obj)
        {
            try
            {
                UpdateLoading(true);
                var todoResult = await service.GetFirstOrDefaultAsync(obj.Id);
                if (todoResult.Status)
                {
                    CurrentDto = obj;
                    IsRightDrawerOpen = true;
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                UpdateLoading(false);
            }
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        async void GetDataAsync()
        {
            try
            {
                UpdateLoading(true);

                int? status = SelectedIndex == 0 ? null : SelectedIndex == 1 ? 0 : 1;

                var todoResult = await service.GetAllFilterAsync(new ToDoParameter()
                {
                    Search = Search,
                    Status = status,
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
            }
            catch (Exception ex)
            {

            }
            finally
            {
                UpdateLoading(false);
            }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
            GetDataAsync();
        }
    }
}
