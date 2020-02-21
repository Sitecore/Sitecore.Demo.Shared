using System.Collections.Generic;
using Sitecore.Demo.Shared.Feature.Accounts.Models;

namespace Sitecore.Demo.Shared.Feature.Accounts.Repositories
{
    public interface IFedAuthLoginButtonRepository
    {
        IEnumerable<FedAuthLoginButton> GetAll();
    }
}