//Originally posted in github under MIT license
//https://github.com/bradirby/AzureTextModerationServices

using System.Collections.Generic;
using System.Threading.Tasks;

namespace TextModeration
{

    public class AzureTermListFactory : IAzureTermListFactory
    {
        private IAzureTermListApi API { get; }

        public AzureTermListFactory(IAzureTermListApi api)
        {
            API = api;
        }

        /// <summary>
        /// Creates a new text moderation list.  The Name does not have to be unique,
        /// so two lists with the same name can be created
        /// </summary>
        public async Task<AzureTermList> CreateNewAsync(string name, string desc)
        {
            var lst = await API.CreateTermListAsync(name, desc);
            return new AzureTermList(lst, API);
        }

        public async Task<IList<AzureTermList>> GetAllExistingAsync()
        {
            var lst = await API.GetAllTermListsAsync();
            var newLst = new List<AzureTermList>();
            foreach (var termList in lst)
            {
                newLst.Add(new AzureTermList(termList, API));
            }

            return newLst;
        }
    }

    public interface IAzureTermListFactory
    {
        Task<AzureTermList> CreateNewAsync(string name, string desc);
        Task<IList<AzureTermList>> GetAllExistingAsync();
    }

}
