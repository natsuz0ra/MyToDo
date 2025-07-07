using MyToDo.Common.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MyToDo.Extensions
{
    public static class JournalExtensions
    {
        /// <summary>
        /// 注册路由对象传递事件
        /// </summary>
        /// <param name="aggregator"></param>
        /// <param name="action"></param>
        public static void RegisterSetNavigationJournal(this IEventAggregator aggregator, Action<IRegionNavigationJournal> action)
        {
            aggregator.GetEvent<SetNavigationJournalEvent>().Subscribe(action);
        }

        /// <summary>
        /// 路由对象传递
        /// </summary>
        /// <param name="aggregator"></param>
        /// <param name="journal"></param>
        public static void SetNavigationJournal(this IEventAggregator aggregator, IRegionNavigationJournal journal)
        {
            aggregator.GetEvent<SetNavigationJournalEvent>().Publish(journal);
        }
    }
}
