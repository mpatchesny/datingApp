using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace datingApp.Infrastructure.DAL
{
    internal sealed class DatabaseInitializer : IHostedService
    {
        private readonly IServiceProvider _serviceProdivder;
        public DatabaseInitializer(IServiceProvider serviceProvider)
        {
            _serviceProdivder = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProdivder.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DatingAppDbContext>();
            await dbContext.Database.MigrateAsync(cancellationToken);

            if (dbContext.Users.Count() == 0)
            {
                List<User> users = new List<User>{
                    new User(Guid.Parse("00000000-0000-0000-0000-000000000001"), "012345678", "test@test.com", "Maciej", new DateOnly(1999,1,1), Sex.Male, null, new UserSettings(Guid.NewGuid(), Sex.Female & Sex.Male, 18, 99, 100, 0.0, 0.0), bio: "Lubie zapach kawy o poranku"),
                    new User(Guid.Parse("00000000-0000-0000-0000-000000000002"), "111111111", "test1@test.com", "Grazyna", new DateOnly(1999,1,1), Sex.Female, null, new UserSettings(Guid.NewGuid(), Sex.Male, 18, 30, 100, 0.0, 0.0)),
                    new User(Guid.Parse("00000000-0000-0000-0000-000000000003"), "222222222", "test2@test.com", "Karyna", new DateOnly(1999,1,1), Sex.Female, null, new UserSettings(Guid.NewGuid(), Sex.Male, 18, 30, 100, 0.0, 0.0)),
                    new User(Guid.NewGuid(), "123456789", "dwight@dundermifflin.com", "Dwight Schrute", new DateOnly(1970, 1, 20), Sex.Male, null, new UserSettings(Guid.NewGuid(), Sex.Female, 18, 50, 100, 0.0, 0.0), "Assistant manager", "I am Dwight, I am a beet farmer. Beets. Bears. Battlestar Galactica."),
                    new User(Guid.NewGuid(), "555123456", "jim@dundermifflin.com", "Jim", new DateOnly(1978, 10, 1), Sex.Male, null, new UserSettings(Guid.NewGuid(), Sex.Female, 18, 50, 100, 0.0, 0.0), "Salesman", "Just a regular guy who happens to work here."),
                    new User(Guid.NewGuid(), "555867530", "pam@dundermifflin.com", "Pam", new DateOnly(1979, 3, 25), Sex.Female, null, new UserSettings(Guid.NewGuid(), Sex.Male, 18, 50, 100, 0.0, 0.0), "Receptionist", "I'm Pam, I love art and design and I'm engaged to Jim."),
                    new User(Guid.NewGuid(), "5551357", "michael.scott@dundermifflin.com", "Michael", new DateOnly(1964, 3, 15), Sex.Male, null, new UserSettings(Guid.NewGuid(), Sex.Female, 18, 50, 100, 0.0, 0.0), "Regional Manager", "Hi, I'm Michael. I'm the World's Best Boss and I love making people laugh."),
                    new User(Guid.NewGuid(), "5553698", "ryan.howard@dundermifflin.com", "Ryan", new DateOnly(1979, 5, 5), Sex.Male, null, new UserSettings(Guid.NewGuid(), Sex.Female, 18, 50, 100, 0.0, 0.0), "Temp", "Hey, I'm Ryan. I'm a temp at Dunder Mifflin and I'm also working on a startup called WUPHF.com."),
                    new User(Guid.NewGuid(), "5557890", "kelly.kapoor@dundermifflin.com", "Kelly", new DateOnly(1980, 2, 5), Sex.Female, null, new UserSettings(Guid.NewGuid(), Sex.Male, 18, 50, 100, 0.0, 0.0), "Customer Service", "Hi, I'm Kelly. I'm obsessed with celebrity gossip and I love fashion."),
                    new User(Guid.NewGuid(), "5554321", "stanley.hudson@dundermifflin.com", "Stanley", new DateOnly(1958, 2, 19), Sex.Male, null, new UserSettings(Guid.NewGuid(), Sex.Female, 18, 50, 100, 0.0, 0.0), "Sales Representative", "I'm Stanley. I just want to do my job and go home."),
                    new User(Guid.NewGuid(), "5552468", "angela.martin@dundermifflin.com", "Angela", new DateOnly(1971, 6, 25), Sex.Female, null, new UserSettings(Guid.NewGuid(), Sex.Male, 18, 50, 100, 0.0, 0.0), "Accounting", "Hi, I'm Angela. I'm in charge of accounting and I take cats very seriously."),
                    new User(Guid.NewGuid(), "5558642", "oscar.martinez@dundermifflin.com", "Oscar", new DateOnly(1971, 5, 21), Sex.Male, null, new UserSettings(Guid.NewGuid(), Sex.Male, 18, 50, 100, 0.0, 0.0), "Accounting", "Hey, I'm Oscar. I'm an accountant and I'm also openly gay."),
                    new User(Guid.NewGuid(), "5551234", "kevin@dundermifflin.com", "Kevin", new DateOnly(1978, 6, 1), Sex.Male, null, new UserSettings(Guid.NewGuid(), Sex.Female, 18, 50, 100, 0.0, 0.0), "Accountant", "I'm Kevin, and I love my chili. Sometimes I spill it on myself, but it's worth it."),
                    new User(Guid.NewGuid(), "5554322", "creed@dundermifflin.com", "Creed", new DateOnly(1943, 2, 8), Sex.Male, null, new UserSettings(Guid.NewGuid(), Sex.Female, 18, 50, 100, 0.0, 0.0), "Quality Assurance", "I'm Creed, and I'm the real boss around here. Just don't tell anyone."),
                    new User(Guid.NewGuid(), "5555678", "phyllis@dundermifflin.com", "Phyllis", new DateOnly(1951, 3, 15), Sex.Female, null, new UserSettings(Guid.NewGuid(), Sex.Male, 18, 50, 100, 0.0, 0.0), "Sales", "I'm Phyllis, and I'm a saleswoman here at Dunder Mifflin. I love knitting and baking."),
                    new User(Guid.NewGuid(), "5558765", "meredith@dundermifflin.com", "Meredith", new DateOnly(1966, 5, 12), Sex.Female, null, new UserSettings(Guid.NewGuid(), Sex.Male, 18, 99, 100, 0.0, 0.0), "Supplier Relations", "I'm Meredith, and I like to party. Sometimes I bring my own booze to work."),
                    new User(Guid.NewGuid(),"55555555", "andy@dundermifflin.com", "Andy", new DateOnly(1980, 7, 22), Sex.Male, null, new UserSettings(Guid.NewGuid(), Sex.Female, 18, 50, 100, 0.0, 0.0), "Regional Director", "I'm Andy, the Nard Dog! I'm a Cornell grad and an acapella enthusiast. I used to have anger issues, but I've worked hard to control my temper. I'm also a talented musician, and I'm always looking for an opportunity to showcase my singing skills."),
                    new User(Guid.NewGuid(), "555555556", "erin.hannon@dundermifflin.com", "Erin Hannon", new DateOnly(1986, 10, 4), Sex.Female, null, new UserSettings(Guid.NewGuid(), Sex.Male, 18, 50, 100, 0.0, 0.0), "Receptionist", "Hi, I'm Erin! I love puppies, romantic comedies, and bubble baths. I'm still trying to figure out the whole receptionist thing, but I'm excited to be here at Dunder Mifflin!")
                };

                dbContext.Users.AddRange(users);
                await dbContext.SaveChangesAsync();
            };

            if (dbContext.Photos.Count() == 0)
            {
                var photo = new Photo(Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), "abc", "abc", 0);
                dbContext.Photos.Add(photo);
                await dbContext.SaveChangesAsync();
            };

            if (dbContext.Swipes.Count() == 0)
            {
                var swipe = new Swipe(Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000003"), Guid.Parse("00000000-0000-0000-0000-000000000001"), Like.Like, DateTime.UtcNow);
                dbContext.Swipes.Add(swipe);
                await dbContext.SaveChangesAsync();
            };

            if (dbContext.Matches.Count() == 0)
            {
                var match = new Match(Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000002"), false, false, null, DateTime.UtcNow);
                dbContext.Matches.Add(match);
                await dbContext.SaveChangesAsync();
            };

            if (dbContext.Messages.Count() == 0)
            {
                List<Message> messages = new List<Message> {
                    new Message(Guid.NewGuid(), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000002"), "Cześć, co słychać?", false, DateTime.UtcNow - TimeSpan.FromMinutes(20)),
                    new Message(Guid.NewGuid(), Guid.Parse("00000000-0000-0000-0000-000000000002"), Guid.Parse("00000000-0000-0000-0000-000000000001"), "Wszystko w porzadku, a u Ciebie?", false, DateTime.UtcNow - TimeSpan.FromMinutes(15)),
                    new Message(Guid.NewGuid(), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000002"), "U mnie też wszystko w porządku. Właśnie oglądam seriale.", false, DateTime.UtcNow - TimeSpan.FromMinutes(13)),
                    new Message(Guid.NewGuid(), Guid.Parse("00000000-0000-0000-0000-000000000002"), Guid.Parse("00000000-0000-0000-0000-000000000001"), "To super, ja lubie The Office, a Ty?", false, DateTime.UtcNow - TimeSpan.FromMinutes(10)),
                    new Message(Guid.NewGuid(), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000002"), "Ja też! Michael jest najlepszy!.", false, DateTime.UtcNow - TimeSpan.FromMinutes(9)),
                    new Message(Guid.NewGuid(), Guid.Parse("00000000-0000-0000-0000-000000000002"), Guid.Parse("00000000-0000-0000-0000-000000000001"), "❤️❤️❤️ chyba jesteśmy sobie przeznaczeni! Wyjdziesz za mnie?", false, DateTime.UtcNow - TimeSpan.FromMinutes(8)),
                    new Message(Guid.NewGuid(), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000002"), "No pewnie!", false, DateTime.UtcNow - TimeSpan.FromMinutes(7)),
                };
                dbContext.Messages.AddRange(messages);
                await dbContext.SaveChangesAsync();
            };
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}