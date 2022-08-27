using Microsoft.AspNetCore.Authorization;
using Abmes.DataCollector.Vault.Configuration;

namespace Abmes.DataCollector.Vault.WebAPI.Authorization
{
    public class UserAllowedDataCollectionHandler : AuthorizationHandler<UserAllowedDataCollectionRequirement>
    {
        private readonly IUsersProvider _usersProvider;
        private readonly IDataCollectionNameProvider _dataCollectionNameProvider;

        public UserAllowedDataCollectionHandler(
            IUsersProvider usersProvider,
            IDataCollectionNameProvider dataCollectionNameProvider)
        {
            _usersProvider = usersProvider;
            _dataCollectionNameProvider = dataCollectionNameProvider;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, UserAllowedDataCollectionRequirement requirement)
        {
            var dataCollectionName = _dataCollectionNameProvider.GetDataCollectionName();

            var identityUserId = context.User.Claims.Where(x => x.Type.Equals("sub")).Select(x => x.Value).FirstOrDefault();

            if (string.IsNullOrEmpty(identityUserId))
            {
                context.Fail();
            }
            else
            {
                var users = await _usersProvider.GetUsersAsync(CancellationToken.None);

                var user = users.Where(x => x.IdentityUserId == identityUserId).FirstOrDefault();

                if ((user == null) || (!user.DataCollectionNames.Contains(dataCollectionName)))
                {
                    context.Fail();
                }
                else
                {
                    context.Succeed(requirement);
                }
            }
        }
    }
}