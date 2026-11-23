# DriveShare Microservices

An event-driven microservices implementation of the DriveShare platform, decomposing monolithic workflows into modular, containerized backend services communicating via Apache Kafka.

---

## Architecture & Services

- **DriveShare.UserService (Port 5001):** Manages user authentication, profile data, license uploads, and identity credentials, backed by a dedicated SQL Server database (`DriveShare_UserDb`).
- **DriveShare.RentalService (Port 5002):** Handles vehicle catalog, availability checking, and rental booking transactions, backed by a dedicated SQL Server database (`DriveShare_RentalDb`).
- **DriveShare.NotificationService (Port 5003):** Processes real-time event notifications and client messaging, backed by a dedicated SQL Server database (`DriveShare_NotificationDb`).
- **Apache Kafka & Zookeeper (Port 9092):** Event bus facilitating asynchronous messaging across topics:
  - `license-uploaded-topic`
  - `license-status-topic`
  - `booking-confirmed-topic`
  - `new-car-listing-topic`
  - `user-targeted-topic`
- **DriveShare.Shared:** Common contracts, DTOs, and cross-cutting utility libraries.
- **Frontend Container (Port 3002):** React web client integrated with the microservice endpoints.

---

## Getting Started with Docker Compose

### Prerequisites
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) installed and running.

### Setup & Run
1. Clone the repository:
   ```bash
   git clone https://github.com/ezzat462/SA2_Project.git
   cd SA2_Project
   ```
2. Build and launch all containers (databases, Kafka message broker, microservices, and frontend):
   ```bash
   docker compose up --build
   ```

3. Service Access:
   - User Service: `http://localhost:5001`
   - Rental Service: `http://localhost:5002`
   - Notification Service: `http://localhost:5003`
   - Frontend Client: `http://localhost:3002`

---

## Author

**Ezzat Sobhi**  
Computer Science Student (Class of 2027) | Aspiring Backend Developer  
- GitHub: [github.com/ezzat462](https://github.com/ezzat462)  
- LinkedIn: [linkedin.com/in/ezzat-sobhi-b277372a4/](https://www.linkedin.com/in/ezzat-sobhi-b277372a4/)
