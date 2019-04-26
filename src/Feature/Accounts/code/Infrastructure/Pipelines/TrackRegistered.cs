using Sitecore.Demo.Feature.Accounts.Services;
using Sitecore.Demo.Foundation.Accounts.Pipelines;
using Sitecore.Demo.Foundation.Accounts.Services;

namespace Sitecore.Demo.Feature.Accounts.Infrastructure.Pipelines
{            
    public class TrackRegistered
    {
        private readonly IAccountTrackerService _accountTrackerService;
        private readonly IUserProfileService _userProfileService;

        public TrackRegistered(IAccountTrackerService accountTrackerService, IUserProfileService userProfileService)
        {
            _accountTrackerService = accountTrackerService;
            _userProfileService = userProfileService;
        }

        public void Process(AccountsPipelineArgs args)
        {
            _userProfileService.UpdateContactFacetData(args.User.Profile);
            _accountTrackerService.TrackRegistration();
        }             
    }
}