using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyToDo.Shared.Dtos
{
    public class UserDto : BaseDto
    {
        private string userName;
        private string password;
        private string account;
        private string avatar;

        public string UserName
        {
            get { return userName; }
            set { userName = value; OnPropertyChanged(); }
        }

        public string Account
        {
            get { return account; }
            set { account = value; OnPropertyChanged(); }
        }

        public string Password
        {
            get { return password; }
            set { password = value; OnPropertyChanged(); }
        }

        public string Avatar
        {
            get { return avatar; }
            set { avatar = value; OnPropertyChanged(); }
        }

    }
}
