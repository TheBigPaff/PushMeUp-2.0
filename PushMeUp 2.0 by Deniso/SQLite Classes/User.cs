using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushMeUp_2._0_by_Deniso
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ProPicPath { get; set; }
        public string Birthdate { get; set; }
        public double Weight { get; set; }
        public double Height { get; set; }

        public List<UserStat> UserStats { get; set; }
        public User()
        {
            UserStats = new List<UserStat>();
        }
        //public User(string _name, string _pwd, string _propic, string _birthdate, double _weight, double _height)
        //{
        //    Username = _name;
        //    Password = _pwd;
        //    ProPicPath = _propic;
        //    Birthdate = _birthdate;
        //    Weight = _weight;
        //    Height = _height;
        //}

    }
}
