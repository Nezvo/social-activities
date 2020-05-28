using System.Threading.Tasks;
using Api.Controllers;
using Application.Photos;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class PhotosController : BaseController
    {
        [HttpPost]
        public async Task<ActionResult<Photo>> Add([FromForm] PhotoAdd.Command command) => await Mediator.Send(command);

        [HttpDelete("{id}")]
        public async Task<ActionResult<Unit>> Delete(string id) => await Mediator.Send(new PhotoDelete.Command { Id = id });
    }
}