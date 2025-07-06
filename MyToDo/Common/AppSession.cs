using MyToDo.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyToDo.Common
{
    public class AppSession
    {
        public static UserDto User { get; set; } = new UserDto();
    }
}
