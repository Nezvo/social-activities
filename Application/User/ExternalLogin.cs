using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.User
{
    public class ExternalLogin
    {
        public class Query : IRequest<User>
        {
            public string AccessToken { get; set; }
        }

        public class Handler : IRequestHandler<Query, User>
        {
            private readonly UserManager<AppUser> userManager;
            private readonly IFacebookAccessor facebookAcessor;
            private readonly IJwtGenerator jwtGenerator;

            public Handler(UserManager<AppUser> userManager, IFacebookAccessor facebookAcessor, IJwtGenerator jwtGenerator)
            {
                this.jwtGenerator = jwtGenerator;
                this.facebookAcessor = facebookAcessor;
                this.userManager = userManager;
            }

            public async Task<User> Handle(Query request, CancellationToken cancellationToken)
            {
                var userInfo = await facebookAcessor.FacebookLogin(request.AccessToken);

                if (userInfo == null)
                    throw new RestException(HttpStatusCode.BadRequest, new { User = "Problem validating token" });

                var user = await userManager.FindByEmailAsync(userInfo.Email);

                if (user == null)
                {
                    user = new AppUser
                    {
                        DisplayName = userInfo.Name,
                        Id = userInfo.Id,
                        Email = userInfo.Email,
                        UserName = $"fb_{userInfo.Id}",
                        RefreshToken = jwtGenerator.GenerateRefreshToken(),
                        RefreshTokenExpiry = DateTime.Now.AddDays(30),
                        EmailConfirmed = true
                    };

                    var photo = new Photo
                    {
                        Id = $"fb_{userInfo.Id}",
                        Url = userInfo.Picture.Data.Url,
                        IsMain = true
                    };

                    user.Photos.Add(photo);

                    var result = await userManager.CreateAsync(user);

                    if (!result.Succeeded)
                        throw new RestException(HttpStatusCode.BadRequest, new { User = "Problem creating user" });
                }

                return new User
                {
                    DisplayName = user.DisplayName,
                    Token = jwtGenerator.CreateToken(user),
                    RefreshToken = user.RefreshToken,
                    UserName = user.UserName,
                    Image = user.Photos.FirstOrDefault(x => x.IsMain)?.Url
                };
            }
        }
    }
}