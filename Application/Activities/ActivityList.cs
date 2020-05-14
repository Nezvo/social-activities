using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities
{
    public class ActivityList
    {
        public class Request : IRequest<List<Activity>> { }

        public class Handler : IRequestHandler<Request, List<Activity>>
        {
            private readonly DataContext context;
            public Handler(DataContext context)
            {
                this.context = context;
            }

            public async Task<List<Activity>> Handle(Request request, CancellationToken cancellationToken)
            {
                var activities = await context.Activities.ToListAsync();

                return activities;
            }
        }
    }
}