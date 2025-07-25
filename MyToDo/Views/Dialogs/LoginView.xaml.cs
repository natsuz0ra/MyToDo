﻿using MyToDo.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyToDo.Views.Dialog
{
    /// <summary>
    /// LoginView.xaml 的交互逻辑
    /// </summary>
    public partial class LoginView : UserControl
    {
        public LoginView(IEventAggregator aggregator)
        {
            if (aggregator == null)
                throw new ArgumentNullException(nameof(aggregator));

            aggregator.RegisterMessage(e =>
            {
                if (LoginSnackbar?.MessageQueue != null)
                {
                    LoginSnackbar.MessageQueue.Enqueue(e.Message);
                }
            }, "Login");
            InitializeComponent();
        }
    }
}
