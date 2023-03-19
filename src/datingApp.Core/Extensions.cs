using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace datingApp.Core
{
    public static class Extensions
    {
        public static IServiceCollection AddCore(this IServiceCollection services)
        {
            return services;
        }

        public static int ComputeAge(this DateOnly olderDate, DateOnly newerDate)
        {
            var age = newerDate.Year - olderDate.Year;
            switch (newerDate.Month - olderDate.Month)
            {
                case < 0:
                    age -= 1;
                    break;
                case 0:
                    if ((newerDate.Day - olderDate.Day) < 0)
                    {
                        age -= 1;
                    }
                    break;
            }
            return age;
        }
    }
}