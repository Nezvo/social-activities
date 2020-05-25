using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using Application.Interfaces;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.User
{
    public class Register
    {
        public class Request : IRequest<User>
        {
            public string DisplayName { get; set; }
            public string UserName { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class RequestValidator : AbstractValidator<Request>
        {
            public RequestValidator()
            {
                RuleFor(x => x.DisplayName).NotEmpty();
                RuleFor(x => x.UserName).NotEmpty();
                RuleFor(x => x.Email).NotEmpty();
                RuleFor(x => x.Password).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Request, User>
        {
            private readonly DataContext context;
            private readonly UserManager<AppUser> userManager;
            private readonly IJwtGenerator jwtGenerator;

            public Handler(DataContext context, UserManager<AppUser> userManager, IJwtGenerator jwtGenerator)
            {
                this.context = context;
                this.userManager = userManager;
                this.jwtGenerator = jwtGenerator;
            }

            public async Task<User> Handle(Request request, CancellationToken cancellationToken)
            {
                if (await context.Users.AnyAsync(x => x.Email == request.Email))
                    throw new RestException(HttpStatusCode.BadRequest, new { Email = "Email already exists" });

                if (await context.Users.AnyAsync(x => x.UserName == request.UserName))
                    throw new RestException(HttpStatusCode.BadRequest, new { UserName = "UserName already exists" });

                var user = new AppUser
                {
                    DisplayName = request.DisplayName,
                    Email = request.Email,
                    UserName = request.UserName
                };

                var result = await userManager.CreateAsync(user, request.Password);

                if (result.Succeeded)
                    return new User
                    {
                        DisplayName = user.DisplayName,
                        Token = jwtGenerator.CreateToken(user),
                        UserName = user.UserName,
                        Image = null
                    };

                throw new Exception("Problem creating user");
            }
        }
    }
}