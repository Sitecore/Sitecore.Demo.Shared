using Sitecore.Demo.Shared.Feature.Accounts.Services;
using Sitecore.Demo.Shared.Foundation.Accounts.Pipelines;

namespace Sitecore.Demo.Shared.Feature.Accounts.Infrastructure.Pipelines
{                                                                   
    public class TrackLoggedOut
    {
        private readonly IAccountTrackerService _accountTrackerService;

        public TrackLoggedOut(IAccountTrackerService accountTrackerService)
        {
            _accountTrackerService = accountTrackerService;
        }

        public void Process(AccountsPipelineArgs args)
        {
            _accountTrackerService.TrackLogout(args.UserName);
        }
    }
}