using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.Application.DTO;
using datingApp.Application.Security;
using Microsoft.EntityFrameworkCore;

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
            var existingCode = _dbContext.AccessCodes.FirstOrDefault(x => x.EmailOrPhone == code.EmailOrPhone);
            if (existingCode != null)
            {
                _dbContext.Entry(existingCode).CurrentValues.SetValues(code);
            }
            else
            {
                _dbContext.AccessCodes.Add(code);
            }
            _dbContext.SaveChanges();
        }
    }
}