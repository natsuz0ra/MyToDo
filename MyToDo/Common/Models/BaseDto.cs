using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyToDo.Common.Models
{
    public class BaseDto
    {
        private int id;
        private DateTime createTime;
        private DateTime updateTime;

        /// <summary>
        /// ID
        /// </summary>
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        /// <summary>
        /// 创建事时间
        /// </summary>
        public DateTime CreateTime
        {
            get { return createTime; }
            set { createTime = value; }
        }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime
        {
            get { return updateTime; }
            set { updateTime = value; }
        }


    }
}
