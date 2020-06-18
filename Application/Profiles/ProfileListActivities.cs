using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Profiles
{
    public class ProfileListActivities
    {
        public class Query : IRequest<List<UserActivityDto>>
        {
            public string UserName { get; set; }
            public string Predicate { get; set; }
        }

        public class Handler : IRequestHandler<Query, List<UserActivityDto>>
        {
            private readonly DataContext context;

            public Handler(DataContext context)
            {
                this.context = context;
            }

            public async Task<List<UserActivityDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var user = await context.Users.SingleOrDefaultAsync(x => x.UserName == request.UserName);

                if (user == null)
                    throw new RestException(HttpStatusCode.NotFound, new { User = "Not found" });

                var queryable = user.UserActivities
                    .OrderBy(a => a.Activity.Date)
                    .AsQueryable();

                switch (request.Predicate)
                {
                    case "past":
                        queryable = queryable.Where(a => a.Activity.Date <= DateTime.Now);
                        break;
                    case "hosting":
                        queryable = queryable.Where(a => a.IsHost);
                        break;
                    default:
                        queryable = queryable.Where(a => a.Activity.Date >= DateTime.Now);
                        break;
                }

                var activities = queryable.ToList();
                return activities.Select(x =>
                    new UserActivityDto
                    {
                        Id = x.Activity.Id,
                        Title = x.Activity.Title,
                        Category = x.Activity.Category,
                        Date = x.Activity.Date
                    }
                ).ToList();
            }
        }
    }
}