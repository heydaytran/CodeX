# Nexus API

The purpose of this API is to enable our external application to communicate with the front-end application.

This document outlines the naming conventions to be followed when defining endpoints in this project.

## Project Setup 📦

To set up and run the project, follow these steps:

1. Ensure you have Docker and latest .Net SDK installed and running.
2. Start the required services (RabbitMQ, PostgreSQL, Redis) using Docker
   Compose:

   ```sh
   docker-compose up -d
   ```

3. Run the .NET application:

   ```sh
   dotnet run
   ```

## Branching (Trunk-Based Development) 🔥

Trunk-Based Development (TBD) is a software development practice where developers work in a single main branch (trunk) and commit small, incremental changes frequently. This approach minimizes merge conflicts, encourages
continuous integration, and ensures a stable codebase.

### Key Principles

- Single Main Branch – Developers commit directly to main or short-lived feature branches.
- Frequent Commits – Small, incremental changes are merged often to reduce conflicts.
- Feature Flags – Incomplete features are hidden using feature toggles instead of long-lived branches.
- Continuous Integration (CI) – Automated tests and validation ensure code quality.

By following TBD, teams can deliver features faster, improve code stability, and simplify collaboration. 🚀

- ✅ [Endpoint Guidelines](/docs/endpoint-guidelines.md)
- ✅ [Folder Structure](/docs/folder-structure.md)
- ✅ [First-Commit Guidelines](/docs/first-commit-guidelines.md)

## Architecture Decision ♻️

We deliberately chose a Module Monolith with Clean Architecture instead of adopting microservices right away. While microservices offer scalability and independent deployments, they also introduce significant operational complexity, requiring distributed system management, inter-service communication, and eventual consistency handling.

By starting with a well-structured module monolith, we ensure:

- 👉 Simplified Development & Deployment: A single deployable unit reduces operational overhead.

- 👉 Clear Module Boundaries: The modular structure allows for independent feature development while keeping internal service communication simple.

- 👉 Gradual Evolution to Microservices: If necessary, modules can be extracted into separate services with minimal refactoring.

- 👉 Faster Development Cycles: Avoiding the complexity of microservices enables quicker iterations and feature delivery.

This approach provides the best balance between modularity and simplicity while leaving room for future microservices adoption as the system grows.
