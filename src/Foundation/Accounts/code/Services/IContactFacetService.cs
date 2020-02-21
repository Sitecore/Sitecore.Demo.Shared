using Sitecore.Demo.Shared.Foundation.Accounts.Models;

namespace Sitecore.Demo.Shared.Foundation.Accounts.Services
{
    public interface IContactFacetService
    {
        ContactFacetData GetContactData();

        void UpdateContactFacets(ContactFacetData data);

        string ExportContactData();

        bool DeleteContact();
    }
}
