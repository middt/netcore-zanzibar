# Google Zanzibar Implementation in .NET Core

This project implements a simplified version of Google's Zanzibar authorization system using .NET Core. Zanzibar is a global authorization system that provides consistent, real-time access control for millions of users and resources.

## Features

- **Fine-grained Access Control**: Define permissions at the namespace, object, and relation level
- **Hierarchical Permissions**: Support for parent-child relationships in permission inheritance
- **Group-based Access**: Manage permissions through user groups
- **Real-time Permission Checks**: Fast and consistent permission evaluation
- **RESTful API**: Easy integration with any client application

## Getting Started

### Prerequisites

- .NET 8.0 SDK or later
- Your favorite IDE (Visual Studio, VS Code, Rider, etc.)

### Installation

1. Clone the repository:
```bash
git clone https://github.com/yourusername/google-zanzibar.git
cd google-zanzibar
```

2. Restore dependencies:
```bash
dotnet restore
```

3. Build the solution:
```bash
dotnet build
```

4. Run the application:
```bash
dotnet run --project Zanzibar.Api
```

The API will be available at `http://localhost:5027`

## API Usage

### Basic Document Access

```http
# Add direct viewer permission for user1 on doc1
POST http://localhost:5027/api/permission/add
Content-Type: application/json

{
    "namespace": "documents",
    "object": "doc1",
    "relation": "viewer",
    "subject": "user1"
}

# Check permission
POST http://localhost:5027/api/permission/check
Content-Type: application/json

{
    "namespace": "documents",
    "object": "doc1",
    "relation": "viewer",
    "subject": "user1"
}
```

### Group-based Permissions

```http
# Create a group
POST http://localhost:5027/api/permission/add
Content-Type: application/json

{
    "namespace": "groups",
    "object": "engineering",
    "relation": "group",
    "subject": "group:engineering"
}

# Add user to group
POST http://localhost:5027/api/permission/add
Content-Type: application/json

{
    "namespace": "groups",
    "object": "engineering",
    "relation": "member",
    "subject": "user2"
}

# Give group access to document
POST http://localhost:5027/api/permission/add
Content-Type: application/json

{
    "namespace": "documents",
    "object": "doc2",
    "relation": "viewer",
    "subject": "group:engineering"
}
```

### Hierarchical Permissions

```http
# Create parent document permission
POST http://localhost:5027/api/permission/add
Content-Type: application/json

{
    "namespace": "documents",
    "object": "parent-doc",
    "relation": "viewer",
    "subject": "user3"
}

# Add child document relationship
POST http://localhost:5027/api/permission/add
Content-Type: application/json

{
    "namespace": "documents",
    "object": "child-doc",
    "relation": "parent",
    "subject": "parent-doc"
}
```

## Project Structure

```
google-zanzibar/
├── Zanzibar.Api/           # Web API project
├── Zanzibar.Core/          # Core business logic
│   ├── Interfaces/         # Service interfaces
│   ├── Models/            # Data models
│   └── Services/          # Service implementations
└── Zanzibar.Tests/        # Unit tests
```

## Testing

Run the test suite:

```bash
dotnet test
```

## Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

- Inspired by Google's Zanzibar paper: [Zanzibar: Google's Consistent, Global Authorization System](https://research.google/pubs/pub48190/)
- .NET Core community for the excellent framework and tools 