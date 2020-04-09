//Originally posted in github under MIT license
//https://github.com/bradirby/AzureTextModerationServices

using System;

namespace TextModeration
{

    /// <summary>
    /// Class that reads the config from the current environment and provides it to services that need it.
    /// This can load different environments using a well-known naming convention
    /// </summary>
    public class ConfigurationProvider : IConfigurationProvider
    {
        /// <summary>
        /// Your Content Moderator subscription key is found in your Azure portal resource on the 'Keys' page.
        /// </summary>
        /// <remarks>
        /// Add to your environment variables by editing the SetEnvironmentVars.cmd file
        /// </remarks>
        public string AzureContentModerationSubscriptionKey { get; private set; }

        /// <summary>
        /// Base endpoint URL. Add this to your environment variables. Found on 'Overview' page in Azure resource.
        /// </summary>
        /// <remarks>
        /// For example: https://westus.api.cognitive.microsoft.com
        /// Add to your environment variables by editing the SetEnvironmentVars.cmd file
        /// </remarks>
        public string AzureContentModerationEndpoint { get; private set; }

        public ConfigurationProvider()
        {
            //default to loading the prod config which has no environment extension
            LoadNamedConfiguration("");
        }

        /// <summary>
        /// This is for use by tests to get the proper config for the environment
        /// </summary>
        public void LoadNamedConfiguration(string configName)
        {
            //make sure there is a _ separating the config name (if one is specified)
            if (!string.IsNullOrEmpty(configName))
                if (!configName.StartsWith("_"))
                    configName = $"_{configName}";

            // Your Content Moderator subscription key is found in your
            // Azure portal resource on the 'Keys' page. Add to your environment variables.
            AzureContentModerationSubscriptionKey =
                Environment.GetEnvironmentVariable($"CONTENT_MODERATOR_SUBSCRIPTION_KEY{configName}");

            // Base endpoint URL. Add this to your environment variables. Found on
            // 'Overview' page in Azure resource. For example: https://westus.api.cognitive.microsoft.com
            AzureContentModerationEndpoint = Environment.GetEnvironmentVariable($"CONTENT_MODERATOR_ENDPOINT{configName}");

        }
    }
}

public interface IConfigurationProvider
{
    string AzureContentModerationSubscriptionKey { get; }
    string AzureContentModerationEndpoint { get; }
}
