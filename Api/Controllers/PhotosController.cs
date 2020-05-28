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
        public async Task<ActionResult<Photo>> Add([FromForm] PhotoAdd.Request command) => await Mediator.Send(command);
    }
}