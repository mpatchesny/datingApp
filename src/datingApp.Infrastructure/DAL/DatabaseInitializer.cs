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
            var scope = _serviceProdivder.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DatingAppDbContext>();
            await dbContext.Database.MigrateAsync(cancellationToken);

            if (dbContext.Users.Count() == 0)
            {
                List<User> users = new List<User>{
                    new User(Guid.Parse("00000000-0000-0000-0000-000000000001"), "012345678", "test@test.com", "Maciej", new DateOnly(1999,1,1), Sex.Male, null, new UserSettings(Guid.NewGuid(), Sex.Female | Sex.Male, 18, 99, 100, 0.0, 0.0), bio: "Lubie zapach kawy o poranku"),
                    new User(Guid.Parse("00000000-0000-0000-0000-000000000002"), "111111111", "test1@test.com", "Grazyna", new DateOnly(1999,1,1), Sex.Female, null, new UserSettings(Guid.NewGuid(), Sex.Male, 18, 30, 100, 54.543793, 18.454628)),
                    new User(Guid.Parse("00000000-0000-0000-0000-000000000003"), "222222222", "test2@test.com", "Kasia", new DateOnly(1999,1,1), Sex.Female, null, new UserSettings(Guid.NewGuid(), Sex.Male, 18, 30, 100, 54.490013, 18.550562)),
                    new User(Guid.Parse("00000000-0000-0000-0000-000000000004"), "123456789", "dwight@dundermifflin.com", "Dwight Schrute", new DateOnly(1970, 1, 20), Sex.Male, null, new UserSettings(Guid.NewGuid(), Sex.Female, 18, 50, 100, 54.448639, 18.433207), "Assistant manager", "I am Dwight, I am a beet farmer. Beets. Bears. Battlestar Galactica."),
                    new User(Guid.Parse("00000000-0000-0000-0000-000000000005"), "555123456", "jim@dundermifflin.com", "Jim", new DateOnly(1978, 10, 1), Sex.Male, null, new UserSettings(Guid.NewGuid(), Sex.Female, 18, 50, 100, 54.448639, 18.433207), "Salesman", "Just a regular guy who happens to work here."),
                    new User(Guid.Parse("00000000-0000-0000-0000-000000000006"), "555867530", "pam@dundermifflin.com", "Pam", new DateOnly(1979, 3, 25), Sex.Female, null, new UserSettings(Guid.NewGuid(), Sex.Male, 18, 50, 100, 54.327097, 18.634065), "Receptionist", "I'm Pam, I love art and design and I'm engaged to Jim. I'm not looking for anyone!"),
                    new User(Guid.Parse("00000000-0000-0000-0000-000000000007"), "5551357", "michael.scott@dundermifflin.com", "Michael", new DateOnly(1964, 3, 15), Sex.Male, null, new UserSettings(Guid.NewGuid(), Sex.Female, 18, 50, 100, 54.269371, 18.608961), "Regional Manager", "Hi, I'm Michael. I'm the World's Best Boss and I love making people laugh."),
                    new User(Guid.Parse("00000000-0000-0000-0000-000000000008"), "5553698", "ryan.howard@dundermifflin.com", "Ryan", new DateOnly(1979, 5, 5), Sex.Male, null, new UserSettings(Guid.NewGuid(), Sex.Female, 18, 50, 100, 54.394825, 18.603981), "Temp", "Hey, I'm Ryan. I'm a temp at Dunder Mifflin and I'm also working on a startup called WUPHF.com."),
                    new User(Guid.Parse("00000000-0000-0000-0000-000000000009"), "5557890", "kelly.kapoor@dundermifflin.com", "Kelly", new DateOnly(1980, 2, 5), Sex.Female, null, new UserSettings(Guid.NewGuid(), Sex.Male, 18, 50, 100, 54.351261, 18.79027), "Customer Service", "Hi, I'm Kelly. I'm obsessed with celebrity gossip and I love fashion."),
                    new User(Guid.Parse("00000000-0000-0000-0000-000000000010"), "5554321", "stanley.hudson@dundermifflin.com", "Stanley", new DateOnly(1958, 2, 19), Sex.Male, null, new UserSettings(Guid.NewGuid(), Sex.Female, 18, 50, 100, 54.363106, 18.718208), "Sales Representative", "I'm Stanley. I just want to do my job and go home."),
                    new User(Guid.Parse("00000000-0000-0000-0000-000000000011"), "5552468", "angela.martin@dundermifflin.com", "Angela", new DateOnly(1971, 6, 25), Sex.Female, null, new UserSettings(Guid.NewGuid(), Sex.Male, 18, 50, 100, 54.349445, 18.643997), "Accounting", "Hi, I'm Angela. I'm in charge of accounting and I take cats very seriously."),
                    new User(Guid.Parse("00000000-0000-0000-0000-000000000012"), "5558642", "oscar.martinez@dundermifflin.com", "Oscar", new DateOnly(1971, 5, 21), Sex.Male, null, new UserSettings(Guid.NewGuid(), Sex.Male, 18, 50, 100, 54.364494, 18.636314), "Accounting", "Hey, I'm Oscar. I'm an accountant and I'm also openly gay."),
                    new User(Guid.Parse("00000000-0000-0000-0000-000000000013"), "5551234", "kevin@dundermifflin.com", "Kevin", new DateOnly(1978, 6, 1), Sex.Male, null, new UserSettings(Guid.NewGuid(), Sex.Female, 18, 50, 100, 54.4081, 18.638201), "Accountant", "I'm Kevin, and I love my chili. Sometimes I spill it on myself, but it's worth it."),
                    new User(Guid.Parse("00000000-0000-0000-0000-000000000014"), "5554322", "creed@dundermifflin.com", "Creed", new DateOnly(1943, 2, 8), Sex.Male, null, new UserSettings(Guid.NewGuid(), Sex.Female, 18, 50, 100, 54.433857, 18.584745), "Quality Assurance", "I'm Creed, and I'm the real boss around here. Just don't tell anyone."),
                    new User(Guid.Parse("00000000-0000-0000-0000-000000000015"), "5555678", "phyllis@dundermifflin.com", "Phyllis", new DateOnly(1951, 3, 15), Sex.Female, null, new UserSettings(Guid.NewGuid(), Sex.Male, 18, 50, 100, 54.446609, 18.572577), "Sales", "I'm Phyllis, and I'm a saleswoman here at Dunder Mifflin. I love knitting and baking."),
                    new User(Guid.Parse("00000000-0000-0000-0000-000000000016"), "5558765", "meredith@dundermifflin.com", "Meredith", new DateOnly(1966, 5, 12), Sex.Female, null, new UserSettings(Guid.NewGuid(), Sex.Male, 18, 99, 100, 54.450878, 18.556637), "Supplier Relations", "I'm Meredith, and I like to party. Sometimes I bring my own booze to work."),
                    new User(Guid.Parse("00000000-0000-0000-0000-000000000017"), "55555555", "andy@dundermifflin.com", "Andy", new DateOnly(1980, 7, 22), Sex.Male, null, new UserSettings(Guid.NewGuid(), Sex.Female, 18, 50, 100, 54.389275, 18.654627), "Regional Director", "I'm Andy, the Nard Dog! I'm a Cornell grad and an acapella enthusiast. I used to have anger issues, but I've worked hard to control my temper. I'm also a talented musician, and I'm always looking for an opportunity to showcase my singing skills."),
                    new User(Guid.Parse("00000000-0000-0000-0000-000000000018"), "55555556", "erin.hannon@dundermifflin.com", "Erin", new DateOnly(1986, 10, 4), Sex.Female, null, new UserSettings(Guid.NewGuid(), Sex.Male, 18, 50, 100, 54.387718, 18.622404), "Receptionist", "Hi, I'm Erin! I love puppies, romantic comedies, and bubble baths. I'm still trying to figure out the whole receptionist thing, but I'm excited to be here at Dunder Mifflin!")
                };

                dbContext.Users.AddRange(users);
                dbContext.SaveChanges();
                
            };

            if (dbContext.Photos.Count() == 0)
            {
                List<Photo> photos = new List<Photo> {
                    new Photo(Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), "deprecated", "./storage/00000000-0000-0000-0000-000000000001.jpg", 0),
                    new Photo(Guid.Parse("00000000-0000-0000-0000-000000000002"), Guid.Parse("00000000-0000-0000-0000-000000000002"), "deprecated", "./storage/00000000-0000-0000-0000-000000000002.jpg", 0),
                    new Photo(Guid.Parse("00000000-0000-0000-0000-000000000003"), Guid.Parse("00000000-0000-0000-0000-000000000003"), "deprecated", "./storage/00000000-0000-0000-0000-000000000003.jpg", 0),
                    new Photo(Guid.Parse("00000000-0000-0000-0000-000000000004"), Guid.Parse("00000000-0000-0000-0000-000000000004"), "deprecated", "./storage/00000000-0000-0000-0000-000000000004.jpg", 0),
                    new Photo(Guid.Parse("00000000-0000-0000-0000-000000000005"), Guid.Parse("00000000-0000-0000-0000-000000000005"), "deprecated", "./storage/00000000-0000-0000-0000-000000000005.jpg", 0),
                    new Photo(Guid.Parse("00000000-0000-0000-0000-000000000006"), Guid.Parse("00000000-0000-0000-0000-000000000006"), "deprecated", "./storage/00000000-0000-0000-0000-000000000006.jpg", 0),
                    new Photo(Guid.Parse("00000000-0000-0000-0000-000000000007"), Guid.Parse("00000000-0000-0000-0000-000000000007"), "deprecated", "./storage/00000000-0000-0000-0000-000000000007.jpg", 0),
                    new Photo(Guid.Parse("00000000-0000-0000-0000-000000000008"), Guid.Parse("00000000-0000-0000-0000-000000000008"), "deprecated", "./storage/00000000-0000-0000-0000-000000000008.jpg", 0),
                    new Photo(Guid.Parse("00000000-0000-0000-0000-000000000009"), Guid.Parse("00000000-0000-0000-0000-000000000009"), "deprecated", "./storage/00000000-0000-0000-0000-000000000009.jpg", 0),
                    new Photo(Guid.Parse("00000000-0000-0000-0000-000000000010"), Guid.Parse("00000000-0000-0000-0000-000000000010"), "deprecated", "./storage/00000000-0000-0000-0000-000000000010.jpg", 0),
                    new Photo(Guid.Parse("00000000-0000-0000-0000-000000000011"), Guid.Parse("00000000-0000-0000-0000-000000000011"), "deprecated", "./storage/00000000-0000-0000-0000-000000000011.jpg", 0),
                    new Photo(Guid.Parse("00000000-0000-0000-0000-000000000012"), Guid.Parse("00000000-0000-0000-0000-000000000012"), "deprecated", "./storage/00000000-0000-0000-0000-000000000012.jpg", 0),
                    new Photo(Guid.Parse("00000000-0000-0000-0000-000000000013"), Guid.Parse("00000000-0000-0000-0000-000000000013"), "deprecated", "./storage/00000000-0000-0000-0000-000000000013.jpg", 0),
                    new Photo(Guid.Parse("00000000-0000-0000-0000-000000000014"), Guid.Parse("00000000-0000-0000-0000-000000000014"), "deprecated", "./storage/00000000-0000-0000-0000-000000000014.jpg", 0),
                    new Photo(Guid.Parse("00000000-0000-0000-0000-000000000015"), Guid.Parse("00000000-0000-0000-0000-000000000015"), "deprecated", "./storage/00000000-0000-0000-0000-000000000015.jpg", 0),
                    new Photo(Guid.Parse("00000000-0000-0000-0000-000000000016"), Guid.Parse("00000000-0000-0000-0000-000000000016"), "deprecated", "./storage/00000000-0000-0000-0000-000000000016.jpg", 0),
                    new Photo(Guid.Parse("00000000-0000-0000-0000-000000000017"), Guid.Parse("00000000-0000-0000-0000-000000000017"), "deprecated", "./storage/00000000-0000-0000-0000-000000000017.jpg", 0),
                    new Photo(Guid.Parse("00000000-0000-0000-0000-000000000018"), Guid.Parse("00000000-0000-0000-0000-000000000018"), "deprecated", "./storage/00000000-0000-0000-0000-000000000018.jpg", 0),
                };
                await dbContext.Photos.AddRangeAsync(photos);
                await dbContext.SaveChangesAsync();
            };

            if (dbContext.Swipes.Count() == 0)
            {
                var swipe = new Swipe(Guid.NewGuid(), Guid.Parse("00000000-0000-0000-0000-000000000003"), Guid.Parse("00000000-0000-0000-0000-000000000001"), Like.Like, DateTime.UtcNow);
                dbContext.Swipes.Add(swipe);
                await dbContext.SaveChangesAsync();
            };

            if (dbContext.Matches.Count() == 0)
            {
                List<Match> matches = new List<Match> {
                    new Match(Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000002"), false, false, null, DateTime.UtcNow)
                };
                dbContext.Matches.AddRange(matches);
                dbContext.SaveChanges();
            };

            if (dbContext.Messages.Count() == 0)
            {
                List<Message> messages = new List<Message> {
                    new Message(Guid.NewGuid(), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000002"), "Cześć, co słychać?", false, DateTime.UtcNow - TimeSpan.FromMinutes(20)),
                    new Message(Guid.NewGuid(), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), "Wszystko w porzadku, a u Ciebie?", false, DateTime.UtcNow - TimeSpan.FromMinutes(15)),
                    new Message(Guid.NewGuid(), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000002"), "U mnie też wszystko w porządku. Właśnie oglądam seriale.", false, DateTime.UtcNow - TimeSpan.FromMinutes(13)),
                    new Message(Guid.NewGuid(), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), "To super, ja lubie The Office, a Ty?", false, DateTime.UtcNow - TimeSpan.FromMinutes(10)),
                    new Message(Guid.NewGuid(), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000002"), "Ja też! Michael jest najlepszy!.", false, DateTime.UtcNow - TimeSpan.FromMinutes(9)),
                    new Message(Guid.NewGuid(), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000001"), "❤️❤️❤️ chyba jesteśmy sobie przeznaczeni! Wyjdziesz za mnie?", false, DateTime.UtcNow - TimeSpan.FromMinutes(8)),
                    new Message(Guid.NewGuid(), Guid.Parse("00000000-0000-0000-0000-000000000001"), Guid.Parse("00000000-0000-0000-0000-000000000002"), "No pewnie!", false, DateTime.UtcNow - TimeSpan.FromMinutes(7)),
                };
                await dbContext.Messages.AddRangeAsync(messages);
                await dbContext.SaveChangesAsync();
            };
        }
        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}