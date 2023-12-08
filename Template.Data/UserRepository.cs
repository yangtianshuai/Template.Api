using DotNetCore.Repository.Cache;
using Microsoft.Extensions.Caching.Distributed;
using Template.Data.DB;
using Template.Data.Interface;
using Template.Model;

namespace Template.Data
{
    public class UserRepository : RedisCacheRepository<UserInfo>, IUserRepository
    {
        public UserRepository(SampleContext dbContext, IDistributedCache cache) : base(dbContext, cache)
        {

        }            

    }
}