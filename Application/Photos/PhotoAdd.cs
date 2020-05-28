using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Photos
{
    public class PhotoAdd
    {
        public class Request : IRequest<Photo>
        {
            public IFormFile File { get; set; }
        }

        public class Handler : IRequestHandler<Request, Photo>
        {
            private readonly DataContext context;
            private readonly IUserAccessor userAccessor;
            private readonly IPhotoAccessor photoAccessor;

            public Handler(DataContext context, IUserAccessor userAccessor, IPhotoAccessor photoAccessor)
            {
                this.photoAccessor = photoAccessor;
                this.userAccessor = userAccessor;
                this.context = context;
            }

            public async Task<Photo> Handle(Request request, CancellationToken cancellationToken)
            {
                var photoUploadResult = photoAccessor.AddPhoto(request.File);

                var user = await context.Users.SingleOrDefaultAsync(x => x.UserName == userAccessor.GetCurrentUsername());

                var photo = new Photo
                {
                    Id = photoUploadResult.PublicId,
                    Url = photoUploadResult.Url
                };

                if (!user.Photos.Any(x => x.IsMain))
                    photo.IsMain = true;

                user.Photos.Add(photo);

                var success = await context.SaveChangesAsync() > 0;

                if (success)
                    return photo;

                throw new Exception("Problem saving changes");
            }
        }
    }
}