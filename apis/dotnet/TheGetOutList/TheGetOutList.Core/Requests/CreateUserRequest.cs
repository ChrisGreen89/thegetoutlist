using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TheGetOutList.Core.Requests
{
	public class CreateUserRequest
	{
        [Required]
        [JsonPropertyName("emailAddress")]
        public string EmailAddress { get; set; }
        [JsonPropertyName("firstName")]
        [Required]
        public string FirstName { get; set; }
        [JsonPropertyName("lastName")]

        [Required]
        public string LastName { get; set; }
        [JsonPropertyName("password")]

        [Required]
        public string Password { get; set; }
        [JsonPropertyName("confirmPassword")]

        [Required]
        public string ConfirmPassword { get; set; }
    }
}

