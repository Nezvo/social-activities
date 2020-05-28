using System.Threading.Tasks;
using Application.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public class UserController : BaseController
    {
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<User>> Login(Login.Query request) => await Mediator.Send(request);

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(Register.Command request) => await Mediator.Send(request);

        [HttpGet]
        public async Task<ActionResult<User>> CurrentUser() => await Mediator.Send(new CurrentUser.Query());
    }
}