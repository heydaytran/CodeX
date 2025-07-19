# Folder Structure

This project follows a Module Monolith architecture with Clean Architecture principles to ensure maintainability, scalability, and separation of concerns.

## Overview

Module Monolith: The project is structured as a monolithic application but organized into modules to enhance modularity and separation of concerns.

Clean Architecture: The code is structured to follow the Clean Architecture approach, ensuring a clear separation of business logic, application logic, and infrastructure concerns.

## Architecture layers

1. Domain Layer (Core Business Logic)

   - Contains entities, value objects, domain services, and business rules.

   - Independent of external dependencies.

2. Application Layer (Use Cases)

   - Implements use cases and orchestrates the flow of data between the domain and infrastructure layers.

   - Defines interfaces for repositories, services, and external interactions.

3. Infrastructure Layer (External Implementations)

   - Implements database repositories, message brokers, external services, and APIs.

   - Handles dependency injection and third-party integrations.

4. Presentation Layer (Endpoints)

   - Exposes REST API endpoints.

   - Uses controllers to handle HTTP requests and interact with the application layer.

## Module organization

Each module is self-contained and consists of the following structure:

```plaintext
/modules
  ├── Orders
  │   ├── Modules.Orders.Domain
  │   ├── Modules.Orders.Application
  │   ├── Modules.Orders.Infrastructure
  │   ├── Modules.Orders.Endpoints
  │   └── Modules.Orders.Migrator
  │   └── Modules.Orders.Persistence
  ├── Users
  │   ├── Modules.Users.Domain
  │   ├── Modules.Users.Application
  │   ├── Modules.Users.Infrastructure
  │   ├── Modules.Users.Endpoints
  │   └── Modules.Users.Persistence
```

## Benefits of this approach

- Scalability: Modules can be extended or extracted into microservices if needed.

- Maintainability: Clear separation of concerns makes it easier to update and refactor.

- Testability: Each layer can be independently tested.

- Flexibility: Modules are loosely coupled, making dependency management easier.
