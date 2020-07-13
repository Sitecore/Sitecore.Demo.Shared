using Sitecore.Analytics;
using Sitecore.Framework.Conditions;
using Sitecore.Pipelines;
using Sitecore.Analytics.Pipelines.CommitSession;
using Sitecore.Analytics.Tracking;
using Sitecore.Analytics.Model;

namespace Sitecore.Demo.Shared.Foundation.SitecoreExtensions.Services
{
    public class UpdateXdbContactFromSessionService
    {
        public void SaveContactToCollectionDb(Contact contact)
        {
            var manager = Configuration.Factory.CreateObject("tracking/contactManager", true) as Analytics.Tracking.ContactManager;

            CommitSessionPipelineArgs args = new CommitSessionPipelineArgs()
            {
                Session = Tracker.Current.Session
            };
            Condition.Requires(args, nameof(args)).IsNotNull();
            CorePipeline.Run("commitSession", args);

            Tracker.Current.Contact.ContactSaveMode = ContactSaveMode.AlwaysSave;
            manager.SaveContactToCollectionDb(Tracker.Current.Contact);
        }
    }
}