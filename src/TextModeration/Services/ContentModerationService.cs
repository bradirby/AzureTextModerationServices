//Originally posted in github under MIT license
//https://github.com/bradirby/AzureTextModerationServices

using System;
using System.Threading.Tasks;
using TextModeration;
using Microsoft.Azure.CognitiveServices.ContentModerator.Models;

namespace AzureContentModeration
{
    public class ContentModerationService : IContentModerationService
    {
        private IAzureTextModerationAPI AzureAPI { get;  }

        public ContentModerationService(IAzureTextModerationAPI azureApi)
        {
            AzureAPI = azureApi;
        }

        /// <summary>
        /// Moderates the text from a blog post, and allows specifying moderation options
        /// </summary>
        public async Task<BlogPostModerationResult> ModerateBlogPostAsync(BlogPost blogPost, IContentModerationServiceOptions options)
        {
            var azureOptions = new AzureTextModerationOptionsDto();
            azureOptions.DetectPII = options.LookForPII;
            azureOptions.Classify = options.LookForBadWords;
            if (options.BadWords != null) azureOptions.KeyWordListId = options.BadWords.ListID.ToString();
            return await ModerateTextUsingAzureAsync(blogPost.BlogText, azureOptions);
        }

        /// <summary>
        /// Moderates the text from a blog post with the default moderation options
        /// </summary>
        public async Task<BlogPostModerationResult> ModerateBlogPostAsync(BlogPost blogPost)
        {
            //I had intended to compare Azure to AWS, but AWS does not have an offering yet
            var options = new AzureTextModerationOptionsDto();
            return await ModerateTextUsingAzureAsync(blogPost.BlogText, options);
        }

        /// <summary>
        /// Moderates the given text using Azure services
        /// </summary>
        /// <remarks>
        /// https://westus.dev.cognitive.microsoft.com/docs/services/57cf753a3f9b070c105bd2c1/operations/57cf753a3f9b070868a1f66f
        /// </remarks>
        private async Task<BlogPostModerationResult>  ModerateTextUsingAzureAsync(string txt, IAzureTextModerationOptions options)
        {
            if (txt.Length >= 1024) throw new ArgumentException("Text can only be 1024 chars");

            var azureReturnVal = await AzureAPI.ModerateTextAsync(txt, options);
            return InterpretAzureResult(azureReturnVal);
        }

        /// <summary>
        /// Interprets the Azure result into one we have control over
        /// </summary>
        private BlogPostModerationResult InterpretAzureResult(Screen azureReturnVal)
        {
            var returnVal = new BlogPostModerationResult(azureReturnVal);
            returnVal.HasBadWords = TranslateAzureResultHasBadWords(azureReturnVal);
            returnVal.HasPII = (azureReturnVal.PII != null);
            returnVal.HasWordsInCustomList = TranslateAzureResultHasCustomWords(azureReturnVal);
            return returnVal;
        }


        /// <summary>
        /// Interprets the Azure result into one we have control over
        /// </summary>
        private bool TranslateAzureResultHasCustomWords(Screen azureReturnVal)
        {
            if (azureReturnVal.Terms == null) return false;
            if (azureReturnVal.Terms.Count == 0) return false;
            return true;
        }

        /// <summary>
        /// Interprets the moderation response to see if there are bad words we should flag.
        /// </summary>
        /// <remarks>
        /// From the MS Docs:
        /// Category1 refers to potential presence of language that may be considered sexually explicit or adult in certain situations.
        /// Category2 refers to potential presence of language that may be considered sexually suggestive or mature in certain situations.
        /// Category3 refers to potential presence of language that may be considered offensive in certain situations.
        /// Score is between 0 and 1. The higher the score, the higher the model is predicting that the category may be applicable.
        /// This feature relies on a statistical model rather than manually coded outcomes. We recommend testing with your own content
        /// to determine how each category aligns to your requirements.
        /// ReviewRecommended is either true or false depending on the internal score thresholds. Customers should assess whether
        /// to use this value or decide on custom thresholds based on their content policies.
        /// </remarks>
        private bool TranslateAzureResultHasBadWords(Screen azureReturnVal)
        {
            //this should always be there, but just in case
            if (azureReturnVal.Classification == null) return false;

            //do they recommend we review?
            if (azureReturnVal.Classification.ReviewRecommended.HasValue)
                return azureReturnVal.Classification.ReviewRecommended.Value;

            //review recommendation is missing for some reason, so figure it out ourselves.
            var hasBadWords = false;
            if (azureReturnVal.Classification.Category1 != null)
                hasBadWords = (azureReturnVal.Classification.Category1.Score > 0.5);
            if (azureReturnVal.Classification.Category2 != null)
                hasBadWords = hasBadWords || (azureReturnVal.Classification.Category2.Score > 0.5);
            if (azureReturnVal.Classification.Category3 != null)
                hasBadWords = hasBadWords || (azureReturnVal.Classification.Category3.Score > 0.5);

            return hasBadWords;
        }
    }

    public interface IContentModerationService
    {
        Task<BlogPostModerationResult> ModerateBlogPostAsync(BlogPost blogPost);
        Task<BlogPostModerationResult> ModerateBlogPostAsync(BlogPost blogPost, IContentModerationServiceOptions options);
    }
}
