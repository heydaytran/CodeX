## General Guidelines

- Use **plural nouns** for resource names (e.g., `/users` instead of `/user`).
- Use **kebab-case** for URL paths (e.g., `/order-items` instead of `/orderItems`).
- Use **HTTP methods** correctly to represent the intended operation.
- Use **nested routes** to establish relationships between resources where appropriate.
- Avoid using **verbs in endpoints**, as HTTP methods should convey the action.
- Use **query parameters** for filtering, searching, and sorting.

## Endpoint Naming Standards

### 1. **Standard CRUD Operations**

| Operation      | HTTP Method | Endpoint          | Description                |
| -------------- | ----------- | ----------------- | -------------------------- |
| Get all items  | `GET`       | `/resources`      | Retrieve all records       |
| Get by ID      | `GET`       | `/resources/{id}` | Retrieve a single record   |
| Create         | `POST`      | `/resources`      | Create a new record        |
| Update         | `PUT`       | `/resources/{id}` | Replace an existing record |
| Partial Update | `PATCH`     | `/resources/{id}` | Partially update a record  |
| Delete         | `DELETE`    | `/resources/{id}` | Delete a record            |

#### Example: Users API

| Action         | Endpoint      | HTTP Method |
| -------------- | ------------- | ----------- |
| Get all users  | `/users`      | `GET`       |
| Get user by ID | `/users/{id}` | `GET`       |
| Create user    | `/users`      | `POST`      |
| Update user    | `/users/{id}` | `PUT`       |
| Delete user    | `/users/{id}` | `DELETE`    |

### 2. **Sub-resources (Hierarchical Relationships)**

Use sub-resources for entities that are strongly related to another resource.

#### Example: Orders and Order Items

| Action                       | Endpoint                           | HTTP Method |
| ---------------------------- | ---------------------------------- | ----------- |
| Get all orders               | `/orders`                          | `GET`       |
| Get a specific order         | `/orders/{orderId}`                | `GET`       |
| Get all items in an order    | `/orders/{orderId}/items`          | `GET`       |
| Get a specific order item    | `/orders/{orderId}/items/{itemId}` | `GET`       |
| Add an item to an order      | `/orders/{orderId}/items`          | `POST`      |
| Update an order item         | `/orders/{orderId}/items/{itemId}` | `PUT`       |
| Remove an item from an order | `/orders/{orderId}/items/{itemId}` | `DELETE`    |

### 3. **Filtering, Sorting, and Pagination**

Use query parameters to support filtering, sorting, and pagination.

#### Example: Filtering and Sorting

```
GET /users?role=admin&status=active
GET /products?category=electronics&sort=price_desc
```

#### Example: Pagination

```
GET /users?page=1&limit=10
```

### 4. **Custom Actions**

For non-CRUD actions, use descriptive sub-resources rather than verbs.

#### Example: User Authentication

| Action        | Endpoint              | HTTP Method |
| ------------- | --------------------- | ----------- |
| User login    | `/auth/login`         | `POST`      |
| User logout   | `/auth/logout`        | `POST`      |
| Refresh token | `/auth/refresh-token` | `POST`      |

### 5. **Error Handling**

- Use appropriate HTTP status codes:
  - `200 OK` – Success
  - `201 Created` – Resource created
  - `400 Bad Request` – Invalid request
  - `401 Unauthorized` – Authentication required
  - `403 Forbidden` – Insufficient permissions
  - `404 Not Found` – Resource not found
  - `500 Internal Server Error` – Server failure
- Return consistent error messages in JSON format:

```json
{
  "status": 400,
  "error": "Bad Request",
  "message": "The provided email format is invalid."
}
```

### 6. **Success Response Format**

A consistent success response format should be used:

```json
{
  "success": true,
  "data": {
    "id": "00000000-0000-0000-0000-000000000000",
    "name": "string",
    "value": "string"
  },
  "errors": null
}
```

### 7. **Bulk Operations**

For bulk actions, use appropriate endpoints with batch processing.

#### Example: Bulk Create

```
POST /users/batch
```

Request Body:

```json
{
  "users": [
    { "name": "Alice", "email": "alice@example.com" },
    { "name": "Bob", "email": "bob@example.com" }
  ]
}
```

#### Example: Bulk Update

```
PUT /users/batch
```

Request Body:

```json
{
  "users": [
    { "id": 1, "name": "Alice Updated" },
    { "id": 2, "email": "bob_new@example.com" }
  ]
}
```

#### Example: Bulk Delete

```
DELETE /users/batch
```

Request Body:

```json
{
  "userIds": [1, 2, 3]
}
```
