﻿using MyToDo.Common;
using MyToDo.Extensions;
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
using System.Windows.Shapes;

namespace MyToDo.Views
{
    /// <summary>
    /// MainView.xaml 的交互逻辑
    /// </summary>
    public partial class MainView : Window
    {
        public MainView(IEventAggregator aggregator, IDialogHostService dialogHostService)
        {
            InitializeComponent();

            // 注册等待消息窗口
            aggregator.RegisterUpdateLoading(arg =>
            {
                MainDialogHost.IsOpen = arg.IsOpen;
                if (MainDialogHost.IsOpen)
                {
                    MainDialogHost.DialogContent = new ProgressView();
                }
            });

            // 注册提示消息
            aggregator.RegisterMessage(e =>
            {
                if (Snackbar?.MessageQueue != null)
                {
                    Snackbar.MessageQueue.Enqueue(e.Message);
                }
            });

            btnMin.Click += (s, e) => { this.WindowState = WindowState.Minimized; };
            btnMax.Click += (s, e) =>
            {
                if (this.WindowState == WindowState.Maximized)
                {
                    this.WindowState = WindowState.Normal;
                }
                else
                {
                    this.WindowState = WindowState.Maximized;
                }
            };
            btnClose.Click += async (s, e) =>
            {
                var result = await dialogHostService.Question("温馨提示", "确认退出系统？");
                if (result.Result != ButtonResult.OK)
                    return;

                this.Close();
            };

            ColorZone.MouseMove += (s, e) =>
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                    this.DragMove();
            };
            //ColorZone.MouseDoubleClick += (s, e) =>
            //{
            //    if (this.WindowState == WindowState.Maximized)
            //    {
            //        this.WindowState = WindowState.Normal;
            //    }
            //    else
            //    {
            //        this.WindowState = WindowState.Maximized;
            //    }
            //};
        }
    }
}
