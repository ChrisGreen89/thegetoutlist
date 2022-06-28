using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheGetOutList.Core.Requests;
using TheGetOutList.Data.Models;
using TheGetOutList.Services;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TheGetOutList.Controllers
{
    [ApiController]   
    public class AuthenticationController : BaseController
    {
        private readonly IAuthenticationService authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService) : base(authenticationService)
        {
            this.authenticationService = authenticationService;
        }

        [HttpPost]
        [Route("/users")]
        public async Task<User> CreateNewUser([FromBody] CreateUserRequest request)
        {
            return await authenticationService.CreateUser(request);
        }
    }
}

