using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Application.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Api.Controllers
{
    public class UserController : BaseController
    {
        public IConfiguration Config { get; set; }

        public UserController(IConfiguration config)
        {
            this.Config = config;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<User>> Login(Login.Query request) => await Mediator.Send(request);

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult> Register(Register.Command request)
        {
            request.Origin = Request.Headers["origin"];
            await Mediator.Send(request);

            return Ok("Registration successful - please check your email");
        }

        [HttpGet]
        public async Task<ActionResult<User>> CurrentUser() => await Mediator.Send(new CurrentUser.Query());

        [AllowAnonymous]
        [HttpPost("facebook")]
        public async Task<ActionResult<User>> FacebookLogin(ExternalLogin.Query query) => await Mediator.Send(query);

        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<ActionResult<User>> Refresh(RefreshToken.Query query)
        {
            var principal = GetPrincipalFromExpiredToken(query.Token);
            query.UserName = principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            return await Mediator.Send(query);
        }

        [AllowAnonymous]
        [HttpPost("verifyEmail")]
        public async Task<ActionResult> VerifyEmail(ConfirmEmail.Command command)
        {
            var result = await Mediator.Send(command);

            if (!result.Succeeded)
                return BadRequest("Problem veryfying email address");

            return Ok("Email confirmed - you can now login");
        }

        [AllowAnonymous]
        [HttpGet("resendEmailVerification")]
        public async Task<ActionResult> ResendEmailVerification([FromQuery] ResendEmailVerification.Query query)
        {
            query.Origin = Request.Headers["origin"];
            await Mediator.Send(query);

            return Ok("Email verification link sent - please check email");
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Config["TokenKey"])),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("InvalidToken");

            return principal;
        }
    }
}