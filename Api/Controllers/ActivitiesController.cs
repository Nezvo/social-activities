using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Activities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public class ActivitiesController : BaseController
    {
        [HttpGet]
        public async Task<ActionResult<List<ActivityDto>>> List() => await Mediator.Send(new ActivityList.Request());

        [HttpGet("{id}")]
        public async Task<ActionResult<ActivityDto>> Details(Guid id) => await Mediator.Send(new ActivityDetails.Request { Id = id });

        [HttpPost]
        public async Task<ActionResult<Unit>> Create(ActivityCreate.Request request) => await Mediator.Send(request);

        [HttpPut("{id}")]
        public async Task<ActionResult<Unit>> Edit(Guid id, ActivityEdit.Request request)
        {
            request.Id = id;
            return await Mediator.Send(request);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Unit>> Delete(Guid id) => await Mediator.Send(new ActivityDelete.Request { Id = id });

        [HttpPost("{id}/attend")]
        public async Task<ActionResult<Unit>> Attend(Guid id) => await Mediator.Send(new ActivityAttend.Request { Id = id });

        [HttpDelete("{id}/attend")]
        public async Task<ActionResult<Unit>> Unattend(Guid id) => await Mediator.Send(new ActivityUnattend.Request { Id = id });
    }
}