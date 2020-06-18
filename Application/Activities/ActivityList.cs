using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using AutoMapper;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities
{
    public class ActivityList
    {
        public class Query : IRequest<ActivitiesEnvelope>
        {
            public int? Limit { get; set; }
            public int? Offset { get; set; }
            public bool IsGoing { get; set; }
            public bool IsHost { get; set; }
            public DateTime? StartDate { get; set; }

            public Query(int? limit, int? offset, bool isGoing, bool isHost, DateTime? startDate)
            {
                this.StartDate = startDate ?? DateTime.Now;
                this.IsHost = isHost;
                this.IsGoing = isGoing;
                this.Limit = limit;
                this.Offset = offset;
            }
        }

        public class Handler : IRequestHandler<Query, ActivitiesEnvelope>
        {
            private readonly DataContext context;
            private readonly IMapper mapper;
            private readonly IUserAccessor userAccessor;

            public Handler(DataContext context, IMapper mapper, IUserAccessor userAccessor)
            {
                this.userAccessor = userAccessor;
                this.mapper = mapper;
                this.context = context;
            }

            public async Task<ActivitiesEnvelope> Handle(Query request, CancellationToken cancellationToken)
            {
                var queryable = context.Activities
                    .Where(x => x.Date >= request.StartDate)
                    .OrderBy(x => x.Date)
                    .AsQueryable();

                if (request.IsGoing && !request.IsHost)
                    queryable = queryable.Where(x => x.UserActivities.Any(a => a.AppUser.UserName == userAccessor.GetCurrentUsername()));

                if (!request.IsGoing && request.IsHost)
                    queryable = queryable.Where(x => x.UserActivities.Any(a => a.AppUser.UserName == userAccessor.GetCurrentUsername() && a.IsHost));

                var activities = await queryable
                    .Skip(request.Offset.GetValueOrDefault(0))
                    .Take(request.Limit.GetValueOrDefault(3))
                    .ToListAsync();

                return new ActivitiesEnvelope
                {
                    Activities = mapper.Map<List<Activity>, List<ActivityDto>>(activities),
                    ActivityCount = queryable.Count()
                };
            }
        }
    }
}