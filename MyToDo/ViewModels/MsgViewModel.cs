using MaterialDesignThemes.Wpf;
using MyToDo.Common;
using Prism.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyToDo.ViewModels
{
    public class MsgViewModel : BindableBase, IDialogHostAware
    {
        public string DialogHostName { get; set; }
        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }

        private string title;

        public string Title
        {
            get { return title; }
            set { title = value; RaisePropertyChanged(); }
        }

        private string content;

        public string Content
        {
            get { return content; }
            set { content = value; RaisePropertyChanged(); }
        }

        public MsgViewModel()
        {
            SaveCommand = new DelegateCommand(Save);
            CancelCommand = new DelegateCommand(Cancel);
        }

        private void Save()
        {
            if (DialogHost.IsDialogOpen(DialogHostName))
            {
                DialogParameters param = new DialogParameters();
                DialogResult result = new DialogResult(ButtonResult.OK);
                result.Parameters = param;

                DialogHost.Close(DialogHostName, result);
            }
        }

        private void Cancel()
        {
            if (DialogHost.IsDialogOpen(DialogHostName))
            {
                DialogParameters param = new DialogParameters();
                DialogResult result = new DialogResult(ButtonResult.Cancel);
                result.Parameters = param;

                DialogHost.Close(DialogHostName, result);
            }
        }

        public void OnDialogOpend(IDialogParameters parametres)
        {
            if (parametres.ContainsKey("title"))
                Title = parametres.GetValue<string>("title");

            if (parametres.ContainsKey("content"))
                Content = parametres.GetValue<string>("content");

            if (parametres.ContainsKey("dialogHostName"))
                DialogHostName = parametres.GetValue<string>("dialogHostName");
        }
    }
}
