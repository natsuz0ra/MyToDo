using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyToDo.Common
{
    public interface IDialogHostAware
    {
        /// <summary>
        /// DialogHost名称
        /// </summary>
        public string DialogHostName { get; set; }

        /// <summary>
        /// 打开过程中执行
        /// </summary>
        /// <param name="parametres"></param>
        void OnDialogOpend(IDialogParameters parametres);

        /// <summary>
        /// 保存命令
        /// </summary>
        DelegateCommand SaveCommand { get; set; }

        /// <summary>
        /// 取消命令
        /// </summary>
        DelegateCommand CancelCommand { get; set; }
    }
}
