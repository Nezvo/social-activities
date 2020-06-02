using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Profiles
{
    public class ProfileDetails
    {
        public class Query : IRequest<Profile>
        {
            public string UserName { get; set; }
        }

        public class Handler : IRequestHandler<Query, Profile>
        {
            private readonly IProfileReader profileReader;

            public Handler(IProfileReader profileReader)
            {
                this.profileReader = profileReader;
            }

            public async Task<Profile> Handle(Query request, CancellationToken cancellationToken) => await profileReader.ReadProfile(request.UserName);
        }
    }
}