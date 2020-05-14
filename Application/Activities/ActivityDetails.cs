using System;
using System.Threading;
using System.Threading.Tasks;
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

            public async Task<Activity> Handle(Request request, CancellationToken cancellationToken) => await context.Activities.FindAsync(request.Id);
        }
    }
}