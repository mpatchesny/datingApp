using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace datingApp.Core.Entities
{
    public class User
    {
        public long Id { get; }
        
        [Required(ErrorMessage = "phone no is required")]
        [StringLength(15, ErrorMessage = "phone no must be maximum 15 characters long")]
        public string PhoneNo { get; private set; }
        
        [Required(ErrorMessage = "name is required")]
        [StringLength(15, ErrorMessage = "name must be maximum 15 characters long")]
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,40}$", ErrorMessage = "invalid characters in name")]
        public string Name { get; private set; }
        
        [Required(ErrorMessage = "age is required")]
        [Range(18, 100, ErrorMessage = "age must be between 18 and 100")]
        public int Age { get; private set; }

        [Required(ErrorMessage = "sex is required")]
        [RegularExpression(@"[K|M]", ErrorMessage = "sex must be K or M")]
        public string Sex { get; private set; }
        public string Bio { get; private set; }
        public IEnumerable<Match> Matches => _matches;
        private readonly HashSet<Match> _matches = new();

        public User(long id, string phoneNo, string name, int age, string sex, string bio)
        {
            Id = id;
            PhoneNo = phoneNo;
            Name = name;
            Age = age;
            Sex = sex;
            Bio = bio;
        }

        public void AddMatch(Match match)
        {
            if (_matches.Any(x => x.Id == match.Id))
            {
                throw new Exception("match already added to user");
            }
            _matches.Add(match);
        }

        public void RemoveMatch(long matchId)
        {
            _matches.RemoveWhere(x => x.Id == matchId);
        }
    }
}