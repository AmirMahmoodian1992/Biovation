using Biovation.Domain;
using RestSharp;

namespace Biovation.Repository.Api.v2
{
    public class UserCardRepository
    {
        private readonly RestClient _restClient;
        public UserCardRepository(RestClient restClient)
        {
            _restClient = restClient;
        }

        public ResultViewModel<PagingResult<UserCard>> GetCardsByFilter(long userId, bool isActive,
            int pageNumber = default, int pageSize = default)
        {
            var restRequest = new RestRequest("Queries/v2/UserCard", Method.GET);
            restRequest.AddQueryParameter("userId", userId.ToString());
            restRequest.AddQueryParameter("isActive", isActive.ToString());
            restRequest.AddQueryParameter("pageNumber", pageNumber.ToString());
            restRequest.AddQueryParameter("pageSize", pageSize.ToString());
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<PagingResult<UserCard>>>(restRequest);
            return requestResult.Result.Data;
        }

        public ResultViewModel<User> FindUserByCardNumber(string cardNumber)
        {
            var restRequest = new RestRequest("Queries/v2/UserCard/UserByCardNumber", Method.GET);
            restRequest.AddQueryParameter("cardNumber", cardNumber);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<User>>(restRequest);
            return requestResult.Result.Data;
        }
        public ResultViewModel ModifyUserCard(UserCard card)
        {
            var restRequest = new RestRequest("Commands/v2/UserCard", Method.PUT);
            restRequest.AddJsonBody(card);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Result.Data;
        }
        public ResultViewModel DeleteUserCard(int id = default)
        {
            var restRequest = new RestRequest("Commands/v2/UserCard/{id}", Method.DELETE);
            restRequest.AddUrlSegment("id", id.ToString());
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Result.Data;
        }

        public int ReadCardNumber(string brandName = default, int deviceId = default)
        {
            //TODO call virdi API
            return 0;
        }
    }
}