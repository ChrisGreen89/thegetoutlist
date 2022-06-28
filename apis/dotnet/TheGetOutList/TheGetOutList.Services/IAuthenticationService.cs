using System;
using System.Security;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Nodes;
using Auth0.ManagementApi;
using Microsoft.Extensions.Configuration;
using TheGetOutList.Core.Requests;
using TheGetOutList.Data;
using TheGetOutList.Data.Models;

namespace TheGetOutList.Services
{
    public interface IAuthenticationAdministrationService
    {
        Task<string> CreateUser(CreateUserRequest request);
    }

    public class AuthenticationAdministrationService : IAuthenticationAdministrationService
    {
        private readonly IConfiguration config;
        private string _clientId;
        private string _clietnSecret;
        private string _audience;
        private string _tokenAddress;
        private string _domain; 
        public AuthenticationAdministrationService(IConfiguration config)
        {
            this.config = config;

            _clientId = config["Auth0:ManagementApiId"];
            _clietnSecret = config["Auth0:ManagementApiSecret"];
            _audience = config["Auth0:ManagementApiAudience"];
            _domain = config["Auth0:Domain"];
            _tokenAddress = $"https://{_domain}/oauth/token";

        }

        public async Task<string> CreateUser(CreateUserRequest request)
        {
            string authId = null;
            try
            {
                var token = await GetManagementAPIToken();
                var managementClient = new ManagementApiClient(token, _domain);

                var newUser = await managementClient.Users.CreateAsync(new Auth0.ManagementApi.Models.UserCreateRequest()
                {
                    Email = request.EmailAddress,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Password = request.Password,
                    Connection = config["Auth0:Connection"]

                });

                authId = newUser.UserId;
            }
            catch (Exception ex)
            {
                
            }

            return authId;
        }

        private async Task<string> GetManagementAPIToken()
        {
            // Get a management API Token
            var client = new HttpClient();

            var request = new HttpRequestMessage(HttpMethod.Post, _tokenAddress);

            var requestBody = new Dictionary<string, string>();
            requestBody.Add("grant_type", "client_credentials");
            requestBody.Add("client_id", _clientId);
            requestBody.Add("client_secret", _clietnSecret);
            requestBody.Add("audience", _audience);

            request.Content = new StringContent(JsonSerializer.Serialize(requestBody), System.Text.Encoding.UTF8, "application/json");
            var response = await client.SendAsync(request);

            var responseContent = await response.Content.ReadAsStringAsync();

            var joResponse = JsonObject.Parse(responseContent);
            string token = ((JsonValue)joResponse["access_token"]).GetValue<string>();
            return token;
        }
    }
	public interface IAuthenticationService
	{
        /// <summary>
        /// Validates and returns a user based on the authentication ID
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
		Task<User> ValidateAuth(ClaimsPrincipal user);

        /// <summary>
        /// Creates a new user
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<User> CreateUser(CreateUserRequest request);
	}

    public class AuthenticationService : IAuthenticationService
    {
        private readonly IDataRepository repo;
        private readonly IAuthenticationAdministrationService authenticationAdministrationService;

        public AuthenticationService(IDataRepository repo, IAuthenticationAdministrationService authenticationAdministrationService)
        {
            this.repo = repo;
            this.authenticationAdministrationService = authenticationAdministrationService;
        }

        /// <inheritdoc />
        public async Task<User> CreateUser(CreateUserRequest request)
        {
            var newUser = await authenticationAdministrationService.CreateUser(request);
            User user = null;
            if (!string.IsNullOrEmpty(newUser))
            {
                user = await repo.UpsertItem<User>(new User()
                {
                    AuthId = newUser,
                    EmailAddress = request.EmailAddress,
                    FirstName = request.FirstName,
                    LastName = request.LastName
                });
            }

            return user;
        }

        /// <inheritdoc />
        public Task<User> ValidateAuth(ClaimsPrincipal auth)
        {
            var identifier = ((ClaimsIdentity)auth.Identity).Claims.FirstOrDefault(x =>
                x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");

            if (identifier == null)
            {
                string claims = "NULL";
                if (auth.Identity != null && ((ClaimsIdentity)auth.Identity).Claims != null)
                    claims = System.Text.Json.JsonSerializer.Serialize(((ClaimsIdentity)auth.Identity).Claims);
                throw new SecurityException($"Claim not found: Claims: {claims}");
            }

            var user = repo.GetItem<User>(x => x.AuthId == identifier.Value);

            return user;
        }
    }
}

