using System;
using Sitecore.Analytics;
using Sitecore.Demo.Shared.Foundation.SitecoreExtensions.Services;
using Sitecore.Diagnostics;
using Sitecore.ExperienceForms.Models;
using Sitecore.ExperienceForms.Processing;
using Sitecore.ExperienceForms.Processing.Actions;

namespace Sitecore.Demo.Shared.Feature.Forms.SubmitActions
{
    public class PushContactDataFromSessionAction : SubmitActionBase<string>
    {
        private readonly UpdateXdbContactFromSessionService _updateXdbContactService;

        public PushContactDataFromSessionAction(ISubmitActionData submitActionData) : base(submitActionData)
        {
            _updateXdbContactService = new UpdateXdbContactFromSessionService();
        }

        protected override bool TryParse(string value, out string target)
        {
            target = string.Empty;
            return true;
        }

        protected override bool Execute(string data, FormSubmitContext formSubmitContext)
        {
            try
            {
                if (Tracker.Current == null && Tracker.Enabled)
                {
                    Tracker.StartTracking();
                }

                _updateXdbContactService.SaveContactToCollectionDb(Tracker.Current.Contact);
            }
            catch (Exception ex)
            {
                Log.Error(string.Format("Error occured while updating xDB contact data with custom submit action"), ex, this);
                return false;
            }

            return true;
        }
    }
}