using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;


namespace Application.Followers
{
    public class FollowerDelete
    {
        public class Command : IRequest
        {
            public string UserName { get; set; }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly DataContext context;
            private readonly IUserAccessor userAccessor;

            public Handler(DataContext context, IUserAccessor userAccessor)
            {
                this.userAccessor = userAccessor;
                this.context = context;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var observer = await context.Users.SingleOrDefaultAsync(x => x.UserName == userAccessor.GetCurrentUsername());

                var target = await context.Users.SingleOrDefaultAsync(x => x.UserName == request.UserName);

                if (target == null)
                    throw new RestException(HttpStatusCode.NotFound, new { User = "Not found" });

                var following = await context.Followings.SingleOrDefaultAsync(x => x.ObserverId == observer.Id && x.TargetId == target.Id);

                if (following == null)
                    throw new RestException(HttpStatusCode.BadRequest, new { User = "You are not following this user" });

                context.Followings.Remove(following);

                var success = await context.SaveChangesAsync() > 0;

                if (success)
                    return Unit.Value;

                throw new Exception("Problem saving changes");
            }
        }
    }
}