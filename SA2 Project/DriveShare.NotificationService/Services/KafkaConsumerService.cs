using Confluent.Kafka;
using DriveShare.Shared.Events;
using DriveShare.NotificationService.Services.Interfaces;
using System.Text.Json;

namespace DriveShare.NotificationService.Services
{
    public class KafkaConsumerService : BackgroundService
    {
        private readonly IConfiguration _config;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<KafkaConsumerService> _logger;

        public KafkaConsumerService(IConfiguration config, IServiceProvider serviceProvider, ILogger<KafkaConsumerService> logger)
        {
            _config = config;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = _config["Kafka:BootstrapServers"],
                GroupId = "notification-service-group",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            consumer.Subscribe(new[] { "user-registered-topic", "booking-confirmed-topic", "new-car-listing-topic" });

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = consumer.Consume(stoppingToken);
                    if (consumeResult != null)
                    {
                        using var scope = _serviceProvider.CreateScope();
                        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

                        if (consumeResult.Topic == "user-registered-topic")
                        {
                            var userEvent = JsonSerializer.Deserialize<UserRegisteredEvent>(consumeResult.Message.Value);
                            if (userEvent != null)
                            {
                                await notificationService.SendNotificationAsync(int.Parse(userEvent.UserId), 
                                    $"Welcome to DriveShare, {userEvent.FullName}! We're glad to have you.");
                                
                                await notificationService.SendNotificationToAdminsAsync(
                                    $"New user registered: {userEvent.FullName} ({userEvent.Email})");
                            }
                        }
                        else if (consumeResult.Topic == "booking-confirmed-topic")
                        {
                            var bookingEvent = JsonSerializer.Deserialize<BookingConfirmedEvent>(consumeResult.Message.Value);
                            if (bookingEvent != null)
                            {
                                await notificationService.SendNotificationAsync(int.Parse(bookingEvent.UserId), 
                                    $"Your booking #{bookingEvent.BookingId} has been confirmed! Enjoy your ride.");
                            }
                        }
                        else if (consumeResult.Topic == "new-car-listing-topic")
                        {
                            var carData = JsonSerializer.Deserialize<JsonElement>(consumeResult.Message.Value);
                            var brand = carData.GetProperty("Brand").GetString();
                            var model = carData.GetProperty("Model").GetString();
                            
                            await notificationService.SendNotificationToAdminsAsync(
                                $"New car listing submitted: {brand} {model}. Please review it.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error consuming Kafka message");
                    await Task.Delay(5000, stoppingToken);
                }
            }
        }
    }
}
