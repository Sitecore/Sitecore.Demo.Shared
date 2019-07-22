using System.Collections.Generic;
using Sitecore.Demo.Feature.Accounts.Models;
using Sitecore.Demo.Foundation.Accounts.Models;
using Sitecore.Security;
using Sitecore.Security.Accounts;

namespace Sitecore.Demo.Feature.Accounts.Services
{
    public interface IUserProfileService
    {
        string GetUserDefaultProfileId();

        EditProfile GetEmptyProfile();

        EditProfile GetProfile(User user);

        void SaveProfile(UserProfile userProfile, EditProfile model);

        IEnumerable<string> GetInterests();

        string ExportData(UserProfile userProfile);

        void DeleteProfile(UserProfile userProfile);

        void UpdateContactFacetData(UserProfile userProfile);
    }
}