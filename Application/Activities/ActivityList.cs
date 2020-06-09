using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
            public Query(int? limit, int? offset)
            {
                this.Limit = limit;
                this.Offset = offset;

            }

            public int? Limit { get; set; }
            public int? Offset { get; set; }
        }

        public class Handler : IRequestHandler<Query, ActivitiesEnvelope>
        {
            private readonly DataContext context;
            private readonly IMapper mapper;
            public Handler(DataContext context, IMapper mapper)
            {
                this.mapper = mapper;
                this.context = context;
            }

            public async Task<ActivitiesEnvelope> Handle(Query request, CancellationToken cancellationToken)
            {
                var queryable = context.Activities.AsQueryable();

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