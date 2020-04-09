//Originally posted in github under MIT license
//https://github.com/bradirby/AzureTextModerationServices

using Microsoft.Azure.CognitiveServices.ContentModerator;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.ContentModerator.Models;

namespace TextModeration
{
    public class AzureTextModerationApi : IAzureTextModerationAPI
    {

        private IConfigurationProvider ConfigProvider { get; }
        public AzureTextModerationApi(IConfigurationProvider configProvider)
        {
            ConfigProvider = configProvider;
        }

        private ContentModeratorClient GetNewClient()
        {
            // Create and initialize an instance of the Content Moderator API wrapper.
            var client = new ContentModeratorClient(new ApiKeyServiceClientCredentials(ConfigProvider.AzureContentModerationSubscriptionKey));

            client.Endpoint = ConfigProvider.AzureContentModerationEndpoint;
            return client;
        }

        public async Task<Screen> ModerateTextAsync(string textToModerate, IAzureTextModerationOptions options)
        {
            // Convert string to a byte[], then into a stream (for parameter in ScreenText()).
            byte[] textBytes = Encoding.UTF8.GetBytes(textToModerate);
            MemoryStream stream = new MemoryStream(textBytes);

            using (var client = GetNewClient())
            {
                return await client.TextModeration.ScreenTextAsync(options.ContentType, stream, options.Language,
                    options.AutoCorrect,options.DetectPII, options.KeyWordListId,options.Classify);
            }
        }

    }

    public interface IAzureTextModerationAPI
    {
        Task<Screen> ModerateTextAsync(string textToModerate, IAzureTextModerationOptions options);
    }
}
