using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ImpersonationAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    [AllowAnonymous]
    public class ImpersonationController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ImpersonationController(IHttpContextAccessor contextAccessor)
        {
            _httpContextAccessor = contextAccessor;
        }

        public class ImpersonationRequest
        {
            public string UserId { get; set; }
        }

        [HttpPost("start")]
        public IActionResult Start([FromBody] string userEmail)
        {
            if (string.IsNullOrEmpty(userEmail))
                return BadRequest("Email is required.");

            var session = _httpContextAccessor.HttpContext.Session;
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            session.SetString("OriginalUserId", currentUserId ?? "unknown");
            session.SetString("ImpersonatedUserEmail", userEmail);

            return Ok(new
            {
                message = $"Impersonating {userEmail}",
                impersonated = userEmail
            });
        }

        [HttpPost("revert")]
        public IActionResult Revert()
        {
            var session = _httpContextAccessor.HttpContext.Session;
            string originalUser = session.GetString("OriginalUserId");
            session.Clear();

            return Ok(new { message = $"Reverted to original user: {originalUser}" });
        }
    }
}
