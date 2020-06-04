using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.Profiles;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Followers
{
    public class FollowerList
    {
        public class Query : IRequest<List<Profile>>
        {
            public string UserName { get; set; }
            public string Predicate { get; set; }
        }

        public class Handler : IRequestHandler<Query, List<Profile>>
        {
            private readonly DataContext context;
            private readonly IProfileReader profileReader;

            public Handler(DataContext context, IProfileReader profileReader)
            {
                this.profileReader = profileReader;
                this.context = context;
            }

            public async Task<List<Profile>> Handle(Query request, CancellationToken cancellationToken)
            {
                var queryable = context.Followings.AsQueryable();

                var userFollowings = new List<UserFollowing>();
                var profiles = new List<Profile>();

                switch (request.Predicate)
                {
                    case "followers":
                        {
                            userFollowings = await queryable.Where(x => x.Target.UserName == request.UserName).ToListAsync();

                            foreach (var follower in userFollowings)
                            {
                                profiles.Add(await profileReader.ReadProfile(follower.Observer.UserName));
                            }
                            break;
                        }
                    case "followings":
                        {
                            userFollowings = await queryable.Where(x => x.Observer.UserName == request.UserName).ToListAsync();

                            foreach (var follower in userFollowings)
                            {
                                profiles.Add(await profileReader.ReadProfile(follower.Target.UserName));
                            }
                            break;
                        }
                }

                return profiles;
            }
        }
    }
}