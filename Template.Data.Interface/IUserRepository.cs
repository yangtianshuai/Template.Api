using Template.Model;
using DotNetCore.Repository;

namespace Template.Data.Interface
{
    public interface IUserRepository: IRepository<UserInfo>
    {
        
    }
}