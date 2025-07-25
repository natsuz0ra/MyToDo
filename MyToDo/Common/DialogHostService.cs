﻿using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyToDo.Common
{
    /// <summary>
    /// 自定义对话主机服务
    /// </summary>
    public class DialogHostService : DialogService, IDialogHostService
    {
        private readonly IContainerExtension containerExtension;

        public DialogHostService(IContainerExtension containerExtension) : base(containerExtension)
        {
            this.containerExtension = containerExtension;
        }

        public async Task<IDialogResult> ShowDialog(string name, IDialogParameters parameters, string dialogHostName = "Root")
        {
            if (parameters == null)
                parameters = new DialogParameters();

            // 从容器中取出弹出窗口的实例
            var content = containerExtension.Resolve<object>(name);

            // 验证实例的有效性
            if (!(content is FrameworkElement dialogContent))
                throw new NullReferenceException("A dialog's contain must be a FrameworkElement");

            if (dialogContent is FrameworkElement view && view.DataContext is null && ViewModelLocator.GetAutoWireViewModel(view) is null)
                ViewModelLocator.SetAutoWireViewModel(view, true);

            if (!(dialogContent.DataContext is IDialogHostAware viewModel))
                throw new NullReferenceException("A dialog's contain must implement the IDialogHostService interface");

            viewModel.DialogHostName = dialogHostName;

            DialogOpenedEventHandler eventHandler = (sender, args) =>
            {
                if (viewModel is IDialogHostAware aware)
                {
                    aware.OnDialogOpend(parameters);
                }
                args.Session.UpdateContent(content);
            };

            return (IDialogResult)await DialogHost.Show(dialogContent, viewModel.DialogHostName, eventHandler);
        }

        public new void ShowDialog(string name, IDialogParameters parameters, DialogCallback callback)
        {
            base.ShowDialog(name, parameters, callback);
        }
    }
}
