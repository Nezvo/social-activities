using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using FluentValidation;
using MediatR;
using Persistence;

namespace Application.Activities
{
    public class ActivityEdit
    {
        public class Request : IRequest
        {
            public Guid Id { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public string Category { get; set; }
            public DateTime? Date { get; set; }
            public string City { get; set; }
            public string Venue { get; set; }
        }

        public class CommandValidator : AbstractValidator<Request>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Title).NotEmpty();
                RuleFor(x => x.Description).NotEmpty();
                RuleFor(x => x.Category).NotEmpty();
                RuleFor(x => x.Date).NotEmpty();
                RuleFor(x => x.City).NotEmpty();
                RuleFor(x => x.Venue).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Request>
        {
            private readonly DataContext context;
            public Handler(DataContext context)
            {
                this.context = context;
            }

            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                var activity = await context.Activities.FindAsync(request.Id);

                if (activity == null)
                    throw new RestException(HttpStatusCode.NotFound, new { Activity = "Not found" });

                activity.Title = request.Title ?? activity.Title;
                activity.Description = request.Description ?? activity.Description;
                activity.Category = request.Category ?? activity.Category;
                activity.Date = request.Date ?? activity.Date;
                activity.City = request.City ?? activity.City;
                activity.Venue = request.Venue ?? activity.Venue;

                await context.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }
}