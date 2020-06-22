using System;
using System.IO;
using System.Net;
using System.Text;
using Sitecore.Configuration;
using Sitecore.SecurityModel;

namespace Sitecore.Demo.Shared.Website.Utilities
{
    public partial class DeployMarketingDefinitions : System.Web.UI.Page
    {
        private string GetApiKey()
        {
            return Settings.GetSetting("MarketingDefinitions.ApiKey");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            string apiKey = Request.QueryString["apiKey"];
            if (!string.Equals(apiKey, GetApiKey()))
            {
                Response.Write("Invalid API Key");
                Sitecore.Diagnostics.Log.Warn("DeployMarketingDefinitions utility: Invalid API key", this);
                Response.End();
            }

            try
            {
                using (new SecurityDisabler())
                {
                    // Simulate the request made by the Sitecore Control Panel "Deploy Marketing Definitions" dialog.
                    // This starts an asynchronous Sitecore job that takes about 30 minutes to complete.
                    var url = new Uri(new Uri(Request.Url.GetLeftPart(UriPartial.Authority)), "/api/sitecore/DeployMarketingDefinitions/DeployDefinitions");
                    HttpWebRequest request = WebRequest.CreateHttp(url);
                    request.Method = "POST";
                    request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";

                    // Deploy all the marketing definitions
                    string requestContent = "definitionTypes=%5B%22automationplans%22%2C%22campaigns%22%2C%22events%22%2C%22funnels%22%2C%22goals%22%2C%22marketingassets%22%2C%22pageevents%22%2C%22outcomes%22%2C%22profiles%22%2C%22segments%22%5D&publishTaxonomies=true";
                    byte[] requestContentBytes = Encoding.UTF8.GetBytes(requestContent);
                    request.ContentLength = requestContentBytes.Length;

                    using (Stream requestStream = request.GetRequestStream())
                    {
                        requestStream.Write(requestContentBytes, 0, requestContentBytes.Length);
                        requestStream.Close();
                    }

                    using (HttpWebResponse response = (HttpWebResponse) request.GetResponse())
                    {
                        using (Stream responseStream = response.GetResponseStream())
                        {
                            using (StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8))
                            {
                                string responseContent = streamReader.ReadToEnd();
                                Response.Write(responseContent);
                            }
                        }
                    }

                    Sitecore.Diagnostics.Log.Info("Deploying Marketing Definitions", this);
                }
            }
            catch(Exception ex)
            {
                Sitecore.Diagnostics.Log.Error(string.Format("Deploying Marketing Definitions failed: {0}", ex.Message), this);
            }
        }
    }
}