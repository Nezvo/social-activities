using System.Threading.Tasks;
using Application.Profiles;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public class ProfilesController : BaseController
    {
        [HttpGet("{username}")]
        public async Task<ActionResult<Profile>> Get(string userName) => await Mediator.Send(new ProfileDetails.Request { UserName = userName });
    }
}