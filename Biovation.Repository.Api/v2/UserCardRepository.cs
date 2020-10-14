using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using RestSharp;

namespace Biovation.Repository.Api.v2
{
    public class UserCardRepository
    {
        private readonly RestClient _restClient;
        private readonly BiovationConfigurationManager _biovationConfigurationManager;
        public UserCardRepository(RestClient restClient, BiovationConfigurationManager biovationConfigurationManager)
        {
            _restClient = restClient;
            _biovationConfigurationManager = biovationConfigurationManager;
        }

        public ResultViewModel<PagingResult<UserCard>> GetCardsByFilter(long userId, bool isActive,
            int pageNumber = default, int pageSize = default, string token = default)
        {
            var restRequest = new RestRequest("Queries/v2/UserCard", Method.GET);
            restRequest.AddQueryParameter("userId", userId.ToString());
            restRequest.AddQueryParameter("isActive", isActive.ToString());
            restRequest.AddQueryParameter("pageNumber", pageNumber.ToString());
            restRequest.AddQueryParameter("pageSize", pageSize.ToString());
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<PagingResult<UserCard>>>(restRequest);
            return requestResult.Result.Data;
        }

        public ResultViewModel<User> FindUserByCardNumber(string cardNumber, string token = default)
        {
            var restRequest = new RestRequest("Queries/v2/UserCard/UserByCardNumber", Method.GET);
            restRequest.AddQueryParameter("cardNumber", cardNumber);
            token ??= _biovationConfigurationManager.DefaultToken();
            restRequest.AddHeader("Authorization", token);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<User>>(restRequest);
            return requestResult.Result.Data;
        }
        public ResultViewModel ModifyUserCard(UserCard card, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/UserCard", Method.PUT);
            restRequest.AddJsonBody(card);
            token ??= _biovationConfigurationManager.DefaultToken();
            restRequest.AddHeader("Authorization", token);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Result.Data;
        }
        public ResultViewModel DeleteUserCard(int id = default, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/UserCard/{id}", Method.DELETE);
            restRequest.AddUrlSegment("id", id.ToString());
            token ??= _biovationConfigurationManager.DefaultToken();
            restRequest.AddHeader("Authorization", token);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Result.Data;
        }

        public int ReadCardNumber(string brandName = default, int deviceId = default, string token = default)
        {
            //TODO call virdi API
            return 0;
        }
    }
}