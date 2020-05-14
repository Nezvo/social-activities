using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Activities;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivitiesController : ControllerBase
    {
        private readonly IMediator mediator;
        public ActivitiesController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<Activity>> List() => await mediator.Send(new ActivityDetails.Request());

        [HttpGet("{id}")]
        public async Task<ActionResult<Activity>> Details(Guid id) => await mediator.Send(new ActivityDetails.Request { Id = id });
    }
}