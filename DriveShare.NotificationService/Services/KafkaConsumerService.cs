using Confluent.Kafka;
using DriveShare.NotificationService.Hubs;
using DriveShare.NotificationService.Services.Interfaces;
using DriveShare.Shared.Events;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

namespace DriveShare.NotificationService.Services
{
    public class KafkaConsumerService : BackgroundService
    {
        private readonly IConfiguration _config;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<KafkaConsumerService> _logger;

        public KafkaConsumerService(
            IConfiguration config, 
            IServiceProvider serviceProvider,
            ILogger<KafkaConsumerService> logger)
        {
            _config = config;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Wait for the application to fully start to prevent blocking SignalR/Swagger
            await Task.Yield();

            var config = new ConsumerConfig
            {
                BootstrapServers = _config["Kafka:BootstrapServers"],
                GroupId = "notification-group",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                AllowAutoCreateTopics = true // Prevents crash if topic doesn't exist yet
            };

            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            
            // Subscribe to all relevant topics
            consumer.Subscribe(new[] { 
                "license-uploaded-topic", 
                "new-car-listing-topic", 
                "booking-confirmed-topic",
                "license-status-topic",
                "user-targeted-topic"
            });

            _logger.LogInformation("[KAFKA] Started consuming from: license-uploaded, new-car-listing, booking-confirmed, license-status, user-targeted topics.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Use a short timeout to allow cancellation check
                    var consumeResult = consumer.Consume(TimeSpan.FromSeconds(1));
                    if (consumeResult == null) continue;

                    _logger.LogInformation("[KAFKA] Received message from {Topic}", consumeResult.Topic);

                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

                        if (consumeResult.Topic == "new-car-listing-topic")
                        {
                            var carData = JsonSerializer.Deserialize<JsonElement>(consumeResult.Message.Value);
                            var message = carData.GetProperty("message").GetString() ?? "A new car has been listed for review.";
                            var type = carData.GetProperty("type").GetString() ?? "NewCarListing";
                            
                            _logger.LogInformation("[SIGNALR] Broadcasting New Car Listing: {Message}", message);
                            await notificationService.SendNotificationToAdminsAsync(message, type);
                        }
                        else if (consumeResult.Topic == "booking-confirmed-topic")
                        {
                            // Logic for booking confirmation if needed
                            _logger.LogInformation("[KAFKA] Processing Booking Confirmation...");
                        }
                        else if (consumeResult.Topic == "license-status-topic")
                        {
                            var statusEvent = JsonSerializer.Deserialize<LicenseStatusUpdatedEvent>(consumeResult.Message.Value);
                            if (statusEvent != null)
                            {
                                var type = statusEvent.Status == "Approved" ? "LicenseApproved" : "LicenseRejected";
                                _logger.LogInformation("[SIGNALR] Broadcasting License Status {Status} to User: {UserId}", statusEvent.Status, statusEvent.UserId);
                                await notificationService.SendNotificationAsync(statusEvent.UserId, statusEvent.Message, type);
                            }
                        }
                        else if (consumeResult.Topic == "user-targeted-topic")
                        {
                            var targetEvent = JsonSerializer.Deserialize<UserTargetedEvent>(consumeResult.Message.Value);
                            if (targetEvent != null)
                            {
                                // Switch-case strategy for targeted events
                                switch (targetEvent.Type)
                                {
                                    case "AccountApproved":
                                    case "AccountRejected":
                                    case "CarApproved":
                                    case "CarRejected":
                                    case "NewBookingRequest":
                                    case "BookingAccepted":
                                    case "BookingRejected":
                                        _logger.LogInformation("[SIGNALR] Routing {Type} to User: {UserId}", targetEvent.Type, targetEvent.UserId);
                                        await notificationService.SendNotificationAsync(targetEvent.UserId, targetEvent.Message, targetEvent.Type);
                                        break;
                                    default:
                                        _logger.LogWarning("[SIGNALR] Unknown Targeted Event Type: {Type}", targetEvent.Type);
                                        await notificationService.SendNotificationAsync(targetEvent.UserId, targetEvent.Message, targetEvent.Type);
                                        break;
                                }
                            }
                        }
                    }
                }
                catch (ConsumeException ex)
                {
                    // Log the error but keep the loop running
                    _logger.LogError(ex, "[ERROR] Kafka Consumption failed for topic/partition. Reason: {Reason}", ex.Error.Reason);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[ERROR] General error in KafkaConsumerService: {Message}", ex.Message);
                }
            }

            consumer.Close();
        }
    }
}
