using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using PicoShelter_ApiServer.BLL.Interfaces;

namespace PicoShelter_ApiServer.Hubs
{
    public class CommentHub : Hub, ICommentNotifier
    {
        private readonly IHubContext<CommentHub> _hubContext;

        public CommentHub(IHubContext<CommentHub> hubContext)
        {
            _hubContext = hubContext;
        }
        
        public async Task OnCommentAddedAsync(string imageCode)
        {
            await _hubContext.Clients.Group($"image-{imageCode}")
                .SendAsync("comment-added");
        }

        public async Task OnCommentDeletedAsync(string imageCode, int commentId)
        {
            await _hubContext.Clients.Group($"image-{imageCode}")
                .SendAsync("comment-deleted", commentId);
        }
        
        public async Task SubscribeToImageAsync(string imageCode)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"image-{imageCode}");
        }
    }
}