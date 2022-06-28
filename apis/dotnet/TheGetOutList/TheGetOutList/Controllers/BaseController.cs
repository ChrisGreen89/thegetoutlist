using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.IO;
using TheGetOutList.Data.Models;
using TheGetOutList.Services;

namespace TheGetOutList.Controllers
{
	public class BaseController : Controller
	{
        private readonly IAuthenticationService authService;

        public BaseController(IAuthenticationService authService)
		{
            this.authService = authService;
        }

        [NonAction]
        public async Task<User> AuthIdClaim()
        {
            var userAuth = await authService.ValidateAuth(this.User);
            return userAuth;
        }
	}
}

