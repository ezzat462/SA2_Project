using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace DriveShare.NotificationService.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;

            if (!string.IsNullOrEmpty(userId))
            {
                // Private group for targeted user notifications (1-to-1)
                await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");

                // Role-based groups for broadcast-style targeted notifications
                if (Context.User?.IsInRole("Admin") == true)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
                }

                if (Context.User?.IsInRole("CarOwner") == true)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, "Owners");
                }
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.UserIdentifier;

            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"User_{userId}");

                if (Context.User?.IsInRole("Admin") == true)
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Admins");
                }

                if (Context.User?.IsInRole("CarOwner") == true)
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Owners");
                }
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
