using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace datingApp.Core.Entities
{
    public class User
    {
        public long Id { get; }
        public string PhoneNo { get; private set; }
        public string Name { get; private set; }
        public int Age { get; private set; }
        public int Sex { get; private set; }
        public string Bio { get; private set; }

        public User(long id, string phoneNo, string name, int age, int sex, string bio)
        {
            Id = id;
            PhoneNo = phoneNo;
            Name = name;
            Age = age;
            Sex = sex;
            Bio = bio;
        }
    }
}