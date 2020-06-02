using System.Threading.Tasks;
using Application.Followers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/profiles")]
    public class FollowersController : BaseController
    {
        [HttpPost("{username}/follow")]
        public async Task<ActionResult<Unit>> Follow(string userName) => await Mediator.Send(new FollowerAdd.Command { UserName = userName });

        [HttpDelete("{username}/follow")]
        public async Task<ActionResult<Unit>> Unfollow(string userName) => await Mediator.Send(new FollowerDelete.Command { UserName = userName });
    }
}