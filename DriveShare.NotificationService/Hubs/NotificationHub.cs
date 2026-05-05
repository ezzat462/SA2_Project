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
            
            // Log all claims to debug JWT mapping
            var claims = Context.User?.Claims.Select(c => $"{c.Type}: {c.Value}").ToList() ?? new List<string>();
            Console.WriteLine($"[SIGNALR] Claims for connection {Context.ConnectionId}: {string.Join(", ", claims)}");

            // Robust role check (handles default URI schemas and raw string "role")
            var isAdmin = Context.User?.IsInRole("Admin") == true || 
                          Context.User?.HasClaim(c => c.Type.EndsWith("role", StringComparison.OrdinalIgnoreCase) && c.Value.Equals("Admin", StringComparison.OrdinalIgnoreCase)) == true;
            
            Console.WriteLine($"[SIGNALR] User {userId} connected. Is Admin: {isAdmin}");

            if (!string.IsNullOrEmpty(userId))
            {
                // Private group for targeted user notifications (1-to-1)
                await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");

                // Role-based groups for broadcast-style targeted notifications
                if (isAdmin)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
                    Console.WriteLine($"[SIGNALR] Connection {Context.ConnectionId} joined Admins group.");
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

                var isAdmin = Context.User?.IsInRole("Admin") == true || 
                              Context.User?.HasClaim(c => c.Type.EndsWith("role", StringComparison.OrdinalIgnoreCase) && c.Value.Equals("Admin", StringComparison.OrdinalIgnoreCase)) == true;

                if (isAdmin)
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
