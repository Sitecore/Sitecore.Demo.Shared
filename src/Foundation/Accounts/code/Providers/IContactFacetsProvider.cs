using Sitecore.Analytics.Tracking;
using Sitecore.Demo.Shared.Foundation.Accounts.Models.Facets;
using Sitecore.XConnect.Collection.Model;

namespace Sitecore.Demo.Shared.Foundation.Accounts.Providers
{
    public interface IContactFacetsProvider
    {
        Contact Contact { get; }
        XConnect.Collection.Model.Cache.InteractionsCache InteractionsCache { get; }
        PersonalInformation PersonalInfo { get; }
        AddressList Addresses { get; }
        EmailAddressList Emails { get; }
        ConsentInformation CommunicationProfile { get; }
        PhoneNumberList PhoneNumbers { get; }
        Avatar Picture { get; }
        bool IsKnown { get; }
        SportType SportType { get; }
        SportName SportName { get; }
    }
}