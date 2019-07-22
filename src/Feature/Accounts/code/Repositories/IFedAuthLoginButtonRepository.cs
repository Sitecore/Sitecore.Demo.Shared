using System.Collections.Generic;
using Sitecore.Demo.Feature.Accounts.Models;      

namespace Sitecore.Demo.Feature.Accounts.Repositories
{
    public interface IFedAuthLoginButtonRepository
    {
        IEnumerable<FedAuthLoginButton> GetAll();
    }
}