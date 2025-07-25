﻿using MyToDo.Common.Events;
using MyToDo.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyToDo.ViewModels
{
    public class NavigationViewModel : BindableBase, INavigationAware
    {
        private readonly IContainerProvider containerProvider;
        public readonly IEventAggregator eventAggregator;

        public NavigationViewModel(IContainerProvider containerProvider)
        {
            this.containerProvider = containerProvider;
            eventAggregator = this.containerProvider.Resolve<IEventAggregator>();
        }

        public virtual bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public virtual void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }

        public virtual void OnNavigatedTo(NavigationContext navigationContext)
        {

        }

        public void UpdateLoading(bool IsOpen)
        {
            eventAggregator.UpdateLoading(new UpdateModel()
            {
                IsOpen = IsOpen
            });
        }
    }
}
