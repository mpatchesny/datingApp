using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.DTO;
using datingApp.Application.Security;

namespace datingApp.Infrastructure.Security
{
    internal sealed class DbAccessCodeStorage : IAccessCodeStorage
    {
        private readonly DatingAppDbContext _dbContext;
        public DbAccessCodeStorage(DatingAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public AccessCodeDto Get(string emailOrPhone)
        {
            return _dbContext.AccessCodes.FirstOrDefault(x => x.EmailOrPhone == emailOrPhone);
        }

        public void Set(AccessCodeDto code)
        {
            _dbContext.AccessCodes.Update(code);
            _dbContext.SaveChanges();
        }
    }
}