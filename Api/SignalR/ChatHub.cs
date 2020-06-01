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
            var userName = GetUserName();

            command.UserName = userName;

            var comment = await mediator.Send(command);

            await Clients.Group(command.ActivityId.ToString()).SendAsync("RecieveComment", comment);
        }

        public async Task AddToGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            var userName = GetUserName();

            await Clients.Group(groupName).SendAsync("Send", $"{userName} has joined the group");
        }

        public async Task RemoveFromGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

            var userName = GetUserName();

            await Clients.Group(groupName).SendAsync("Send", $"{userName} has left the group");
        }

        private string GetUserName()
        {
            return Context.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        }
    }
}