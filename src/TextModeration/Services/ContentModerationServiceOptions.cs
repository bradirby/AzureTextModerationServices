//Originally posted in github under MIT license
//https://github.com/bradirby/AzureTextModerationServices



namespace TextModeration
{
    public class ContentModerationServiceOptions : IContentModerationServiceOptions
    {
        /// <summary>
        /// Flag indicating if the service should look for bad words
        /// </summary>
        public bool LookForBadWords { get; }

        /// <summary>
        /// flag indicating if the service should look for Personally Identifiable Information
        /// </summary>
        public bool LookForPII { get; }

        /// <summary>
        /// List of words the API should look for
        /// </summary>
        public AzureTermList BadWords { get;  }

        public ContentModerationServiceOptions(bool lookForBadWords, bool lookForPII, AzureTermList termLst = null)
        {
            LookForBadWords = lookForBadWords;
            LookForPII = lookForPII;
            BadWords = termLst;
        }
    }

    public interface IContentModerationServiceOptions
    {
        bool LookForBadWords { get; }
        bool LookForPII { get; }
        AzureTermList BadWords { get; }
    }
}