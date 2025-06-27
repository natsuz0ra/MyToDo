using MaterialDesignThemes.Wpf;
using MyToDo.Common;
using MyToDo.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyToDo.ViewModels
{
    public class AddToDoViewModel : BindableBase, IDialogHostAware
    {
        public string DialogHostName { get; set; }
        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }

        public AddToDoViewModel()
        {
            SaveCommand = new DelegateCommand(Save);
            CancelCommand = new DelegateCommand(Cancel);
        }

        private ToDoDto model;

        public ToDoDto Model
        {
            get { return model; }
            set { model = value; RaisePropertyChanged(); }
        }

        private void Save()
        {
            if (DialogHost.IsDialogOpen(DialogHostName))
            {
                DialogParameters param = new DialogParameters();
                DialogResult result = new DialogResult(ButtonResult.OK);
                param.Add("value", Model);
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
            if (parametres.ContainsKey("value"))
            {
                model = parametres.GetValue<ToDoDto>("value");
            }
            else
            {
                model = new ToDoDto();
            }
        }
    }
}
