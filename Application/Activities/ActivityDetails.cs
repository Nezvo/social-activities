using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using Domain;
using MediatR;
using Persistence;

namespace Application.Activities
{
    public class ActivityDetails
    {
        public class Request : IRequest<Activity>
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Request, Activity>
        {
            private readonly DataContext context;
            public Handler(DataContext context)
            {
                this.context = context;
            }

            public async Task<Activity> Handle(Request request, CancellationToken cancellationToken)
            {
                var activity = await context.Activities.FindAsync(request.Id);

                if (activity == null)
                    throw new RestException(HttpStatusCode.NotFound, new { Activity = "Not found" });

                return activity;
            }
        }
    }
}