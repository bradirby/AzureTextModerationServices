using System;
using System.Collections.Generic;
using System.Text;

//Originally posted in github under MIT license
//https://github.com/bradirby/AzureTextModerationServices

namespace TextModeration
{
    public class AzureTextModerationOptionsDto : IAzureTextModerationOptions
    {
        /// <summary>
        /// Options are 'text/plain', 'text/html', 'text/xml', 'text/markdown'
        /// </summary>
        public string ContentType { get; set; }  

        /// <summary>
        /// Language for this text.  Options available here:  https://docs.microsoft.com/en-us/azure/cognitive-services/content-moderator/language-support
        /// </summary>
        public string Language { get; set; }

        public bool AutoCorrect { get; set; }

        /// <summary>
        /// This is supposed to turn PII detection on and off, but it does not appear to do anything. PII is always detected
        /// </summary>
        public bool DetectPII { get; set; }

        /// <summary>
        /// ID of the term list the API should use when searching for key words
        /// </summary>
        public string KeyWordListId { get; set; }

        public bool Classify { get; set; }

        public AzureTextModerationOptionsDto()
        {
            ContentType = "text/plain";
            Language = "eng";
            AutoCorrect = true;
            DetectPII = true;
            Classify = true;
        }
    }

    public interface IAzureTextModerationOptions
    {
        string ContentType { get; set; }
        string Language { get; set; }
        bool AutoCorrect { get; set; }
        bool DetectPII { get; set; }
        string KeyWordListId { get; set; }
        bool Classify { get; set; }

    }
}