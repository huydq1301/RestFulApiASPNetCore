using RestApi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApi.Models
{
    public class UserWithToken : UserModel
    {
        
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }

        public UserWithToken(UserModel user)
        {
            this.UserId = user.UserId;
            this.Email = user.Email;            
            this.Username = user.Username;
            this.Password = user.Password;
            this.Phone = user.Phone;
            this.Address = user.Address;
            this.RoleId = user.RoleId;
        }
    }
}
