using System.Collections.Generic;

namespace Sitecore.Demo.Shared.Feature.Accounts.Models
{
    public class FedAuthLoginInfo
    {
        public IEnumerable<FedAuthLoginButton> LoginButtons { get; set; }
    }
}