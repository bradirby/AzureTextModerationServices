//Originally posted in github under MIT license
//https://github.com/bradirby/AzureTextModerationServices

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.ContentModerator.Models;

[assembly: InternalsVisibleTo("TextModerationIntgTests")]
[assembly: InternalsVisibleTo("TextModerationUnitTests")]
namespace TextModeration
{
    public class AzureTermList
    {
        private bool CacheEnabled = true;
        public bool IsDeleted { get; private set; }
        public string Language { get; }
        public int ListID { get; internal set; }
        public string Name { get; internal set; }
        private TermList OriginalTermList { get; }
        private IAzureTermListApi API { get; }
        internal List<string> Terms { get; set; } = null;

        internal AzureTermList(TermList azureTermList, IAzureTermListApi api, string language = "eng")
        {
            OriginalTermList = azureTermList;
            if (OriginalTermList != null)
            {
                ListID = OriginalTermList.Id.Value;
                Name = OriginalTermList.Name;
            }
            Language = language;
            API = api;
            IsDeleted = false;
        }

        public void EnableTermCache()
        {
            CacheEnabled = true;
            Terms = null;  //force refresh 
        }

        public void DisableTermCache()
        {
            CacheEnabled = false;
        }

        public async Task RefreshSearchIndexAsync()
        {
            await API.RefreshSearchIndexAsync(ListID.ToString(), Language);
        }

        public async Task UpdateNameAndDescription(string name, string desc)
        {
            await API.UpdateTermListDescriptionAsync(ListID.ToString(), name, desc);
        }

        /// <summary>
        /// Adds a term to the list.  Adding a duplicate term will throw an exception from the API.
        /// If the TermCache is turned on, this error will be handled silently.
        /// </summary>
        public async Task AddTerm(string term)
        {
            if (CacheEnabled)
            {
                var cachedTerms = GetAllTerms(false).Result;
                if (cachedTerms.Contains(term)) return;
            }

            await API.AddTermToTermListAsync(ListID.ToString(), term, Language);

            if (CacheEnabled) Terms.Add(term);
        }

        public async Task DeleteTerm(string term)
        {
            if (CacheEnabled) Terms.Remove(term);
            await API.AddTermToTermListAsync(ListID.ToString(), term, Language);
        }

        public async Task ClearList()
        {
            if (CacheEnabled) Terms.Clear();
            await API.DeleteAllTermsAsync(ListID.ToString(), Language);
        }

        public async Task Delete()
        {
            IsDeleted = true;
            await API.DeleteTermListAsync(ListID.ToString());
        }

        public async Task<List<string>> GetAllTerms(bool resetCache = false)
        {
            if (resetCache || !CacheEnabled) Terms = null;

            if (Terms == null)
                Terms = await API.GetAllTermsInTermListAsStringsAsync(ListID.ToString(), Language);

            return Terms;
        }

    }
}
