using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Application.Comments;
using System.Linq;
using System.Security.Claims;

namespace Api.SignalR
{
    public class ChatHub : Hub
    {
        private readonly IMediator mediator;

        public ChatHub(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task SendComment(CommentCreate.Command command)
        {
            var userName = Context.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            command.UserName = userName;

            var comment = await mediator.Send(command);

            await Clients.All.SendAsync("RecieveComment", comment);
        }
    }
}