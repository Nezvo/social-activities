using System.Collections.Generic;
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
        public class Request : IRequest<List<ActivityDto>> { }

        public class Handler : IRequestHandler<Request, List<ActivityDto>>
        {
            private readonly DataContext context;
            private readonly IMapper mapper;
            public Handler(DataContext context, IMapper mapper)
            {
                this.mapper = mapper;
                this.context = context;
            }

            public async Task<List<ActivityDto>> Handle(Request request, CancellationToken cancellationToken)
            {
                var activities = await context.Activities.ToListAsync();

                return mapper.Map<List<Activity>, List<ActivityDto>>(activities);
            }
        }
    }
}