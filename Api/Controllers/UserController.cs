using System.Threading.Tasks;
using Application.User;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public class UserController : BaseController
    {
        [HttpPost("login")]
        public async Task<ActionResult<User>> Login(Login.Request request) => await Mediator.Send(request);
    }
}