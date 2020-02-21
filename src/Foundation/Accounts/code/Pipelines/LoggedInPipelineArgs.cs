using System;

namespace Sitecore.Demo.Shared.Foundation.Accounts.Pipelines
{
    public class LoggedInPipelineArgs : AccountsPipelineArgs
    {
        public string Source { get; set; }

        public Guid? PreviousContactId
        {
            get
            {
                return (Guid)this.CustomData["PreviousContactId"];
            }
            set
            {
                this.CustomData["PreviousContactId"] = value;
            }
        }
    }
}