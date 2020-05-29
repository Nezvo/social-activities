using System.Threading.Tasks;
using Application.Profiles;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public class ProfilesController : BaseController
    {
        [HttpGet("{username}")]
        public async Task<ActionResult<Profile>> Get(string userName) => await Mediator.Send(new ProfileDetails.Query { UserName = userName });

        [HttpPut]
        public async Task<ActionResult<Unit>> Edit(ProfileEdit.Command command) => await Mediator.Send(command);
    }
}