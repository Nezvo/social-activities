using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Activities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public class ActivitiesController : BaseController
    {
        [HttpGet]
        public async Task<ActionResult<List<ActivityDto>>> List() => await Mediator.Send(new ActivityList.Query());

        [HttpGet("{id}")]
        public async Task<ActionResult<ActivityDto>> Details(Guid id) => await Mediator.Send(new ActivityDetails.Query { Id = id });

        [HttpPost]
        public async Task<ActionResult<Unit>> Create(ActivityCreate.Command request) => await Mediator.Send(request);

        [HttpPut("{id}")]
        [Authorize(Policy = "IsActivityHost")]
        public async Task<ActionResult<Unit>> Edit(Guid id, ActivityEdit.Command request)
        {
            request.Id = id;
            return await Mediator.Send(request);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "IsActivityHost")]
        public async Task<ActionResult<Unit>> Delete(Guid id) => await Mediator.Send(new ActivityDelete.Command { Id = id });

        [HttpPost("{id}/attend")]
        public async Task<ActionResult<Unit>> Attend(Guid id) => await Mediator.Send(new ActivityAttend.Command { Id = id });

        [HttpDelete("{id}/attend")]
        public async Task<ActionResult<Unit>> Unattend(Guid id) => await Mediator.Send(new ActivityUnattend.Command { Id = id });
    }
}