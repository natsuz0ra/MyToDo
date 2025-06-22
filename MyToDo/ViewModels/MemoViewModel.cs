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
    public class MemoViewModel : NavigationViewModel
    {
        private readonly IMemoService service;
        public DelegateCommand<string> ExecuteCommand { get; private set; }
        public DelegateCommand<MemoDto> SelectedCommand { get; private set; }
        public DelegateCommand<MemoDto> DeleteCommand { get; private set; }

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


        private ObservableCollection<MemoDto> memoDtos;

        public ObservableCollection<MemoDto> MemoDtos
        {
            get { return memoDtos; }
            set { memoDtos = value; RaisePropertyChanged(); }
        }

        private MemoDto currentDto;

        /// <summary>
        /// 编辑选中/新增对象
        /// </summary>
        public MemoDto CurrentDto
        {
            get { return currentDto; }
            set { currentDto = value; RaisePropertyChanged(); }
        }

        public MemoViewModel(IContainerProvider containerProvider, IMemoService service) : base(containerProvider)
        {
            ExecuteCommand = new DelegateCommand<string>(Execute);
            SelectedCommand = new DelegateCommand<MemoDto>(Selected);
            DeleteCommand = new DelegateCommand<MemoDto>(Delete);

            this.service = service;
            this.memoDtos = new ObservableCollection<MemoDto>();
        }

        /// <summary>
        /// 添加备忘录
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
            CurrentDto = new MemoDto();
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
        private async void Delete(MemoDto obj)
        {
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

        private async void Selected(MemoDto obj)
        {
            try
            {
                UpdateLoading(true);
                var memoResult = await service.GetFirstOrDefaultAsync(obj.Id);
                if (memoResult.Status)
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

                var memoResult = await service.GetAllAsync(new QueryParameter()
                {
                    Search = Search,
                    Page = 0,
                    Size = 100,
                });

                if (memoResult.Status)
                {
                    memoDtos.Clear();
                    foreach (var item in memoResult.Result.Items)
                    {
                        MemoDtos.Add(item);
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
