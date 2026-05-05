import * as signalR from '@microsoft/signalr';

/**
 * SignalR Service for managing real-time notifications.
 */
class SignalRService {
  constructor() {
    this.connection = null;
    this.url = "http://localhost:5003/notificationHub";
  }

  /**
   * Initializes and starts the SignalR connection.
   * @param {Function} onMessageReceived - Callback function when a notification is received.
   */
  async startConnection(onMessageReceived) {
    // Avoid creating multiple connections
    if (this.connection && this.connection.state !== signalR.HubConnectionState.Disconnected) {
      return;
    }

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(this.url, {
        // Pass the JWT token as a query parameter 'access_token' automatically
        accessTokenFactory: () => localStorage.getItem('token'),
      })
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Information)
      .build();

    // Listen for events from the server
    // The server-side method name must match (e.g., 'ReceiveNotification')
    this.connection.on('ReceiveNotification', (payload) => {
      console.log('New notification received:', payload);
      if (onMessageReceived) {
        onMessageReceived(payload);
      }
    });

    try {
      await this.connection.start();
      console.log('SignalR Connection Started successfully.');
    } catch (err) {
      console.error('SignalR Connection Error: ', err);
      // Retry connection after 5 seconds if it fails initially
      setTimeout(() => this.startConnection(onMessageReceived), 5000);
    }
  }

  /**
   * Stops the SignalR connection.
   */
  async stopConnection() {
    if (this.connection) {
      await this.connection.stop();
      this.connection = null;
      console.log('SignalR Connection Stopped.');
    }
  }
}

const signalRService = new SignalRService();
export default signalRService;
