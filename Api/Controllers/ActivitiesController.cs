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
        public async Task<ActionResult<List<Activity>>> List() => await mediator.Send(new ActivityList.Request());

        [HttpGet("{id}")]
        public async Task<ActionResult<Activity>> Details(Guid id) => await mediator.Send(new ActivityDetails.Request { Id = id });

        [HttpPost]
        public async Task<ActionResult<Unit>> Create(ActivityCreate.Request request) => await mediator.Send(request);

        [HttpPut("{id}")]
        public async Task<ActionResult<Unit>> Edit(Guid id, ActivityEdit.Request request)
        {
            request.Id = id;
            return await mediator.Send(request);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Unit>> Delete(Guid id) => await mediator.Send(new ActivityDelete.Request { Id = id });
    }
}