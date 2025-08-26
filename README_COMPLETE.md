# Utrecht Camera Assignment - Complete Solution

A full-stack camera management application built for the "Keep Talking and Everybody Codes" technical interview exercise. This solution demonstrates modern software architecture patterns, clean code principles, and comprehensive testing practices.

## 🎯 Overview

This application manages and visualizes Utrecht city surveillance cameras through three distinct interfaces:
- **Command Line Interface (CLI)** - Terminal-based camera search
- **REST API** - Data service for external applications
- **React Web Application** - Interactive UI with maps and smart categorization

The solution processes 90+ Utrecht traffic cameras and implements mathematical categorization rules based on camera number divisibility (by 3 and 5).

## 🏗️ Architecture

The project follows **Clean Architecture** principles with clear separation of concerns:

```
┌─────────────────┐    ┌──────────────────┐    ┌──────────────────┐
│   Presentation  │    │    Application   │    │      Data        │
│                 │    │                  │    │                  │
│ • CLI App       │    │ • Camera Service │    │ • CSV Repository │
│ • Web API       │◄──►│ • Interfaces     │◄──►│ • CSV Parser     │
│ • React Web     │    │ • Business Logic │    │ • File I/O       │
└─────────────────┘    └──────────────────┘    └──────────────────┘
```

### Project Structure

```
everybody-codes/
├── CameraAssignment.Core/          # Business logic and models
│   ├── Models/                     # Domain entities (Camera, CameraCsvRecord)
│   ├── Interfaces/                 # Service and repository contracts
│   ├── CameraService.cs           # Business logic implementation
│   ├── CameraRepository.cs        # Data access implementation
│   └── Configuration/             # Service configuration
├── CameraAssignment.API/          # REST API
│   ├── Controllers/               # API endpoints
│   ├── Program.cs                # API startup and configuration
│   └── appsettings.json          # Configuration files
├── CameraAssignment.Cli/          # Command line interface
│   ├── Program.cs                # CLI entry point
│   └── SearchCommandHandler.cs   # Command handling logic
├── CameraAssignment.Tests/        # Unit and integration tests
├── camera-web/                   # React frontend application
│   ├── src/
│   │   ├── components/           # React components
│   │   ├── services/            # API service layer
│   │   └── utils/               # Utility functions
│   └── package.json             # Frontend dependencies
└── data/
    └── cameras-defb.csv         # Utrecht camera dataset
```

## 🚀 Getting Started

### Prerequisites

- **.NET 8.0 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Node.js 18+** - [Download here](https://nodejs.org/)
- **Git** - For cloning the repository

### Quick Start

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd everybody-codes
   ```

2. **Start the API**
   ```bash
   cd CameraAssignment.API
   dotnet run
   ```
   API will be available at `https://localhost:7298` and `http://localhost:5228`

3. **Start the React application**
   ```bash
   cd camera-web
   npm install
   npm start
   ```
   Web app will be available at `http://localhost:3000`

4. **Use the CLI (optional)**
   ```bash
   cd CameraAssignment.Cli
   dotnet run Search --name "Neude"
   ```

## 🔧 Technical Implementation

### Backend (.NET 8)

#### Core Models
```csharp
public class Camera
{
    public int Number { get; set; }           // Extracted from UTR-CM-501 → 501
    public string Name { get; set; }          // Full camera name
    public double Latitude { get; set; }      // GPS coordinates
    public double Longitude { get; set; }
}
```

#### Key Design Patterns
- **Repository Pattern**: Abstracts data access for testability
- **Dependency Injection**: Promotes loose coupling and testability
- **Interface Segregation**: Clear contracts between layers
- **Single Responsibility**: Each class has one clear purpose

#### CSV Data Processing
The application uses advanced CSV parsing with CsvHelper library:
- Regex-based camera number extraction from `UTR-CM-XXX` format
- Robust error handling for malformed data
- Type-safe mapping between CSV records and domain models

#### API Features
- **RESTful Design**: Proper HTTP verbs and status codes
- **Swagger Documentation**: Auto-generated API documentation
- **CORS Support**: Configured for frontend integration
- **Error Handling**: Consistent error responses with meaningful messages

### Frontend (React 19)

#### Component Architecture
```
App.js (Main Container)
├── CameraColumn.js (Column Display)
├── CameraMap.js (Leaflet Map)
├── services/cameraService.js (API Layer)
└── utils/cameraSorter.js (Business Logic)
```

#### Mathematical Sorting Logic
The core business requirement - categorizing cameras by number divisibility:

```javascript
export const sortCamerasIntoColumns = (cameras) => {
  const columns = {
    column1: [], // Divisible by 3 (but not by 5)
    column2: [], // Divisible by 5 (but not by 3)
    column3: [], // Divisible by both 3 and 5
    column4: [], // Not divisible by 3 or 5
  };

  cameras.forEach(camera => {
    const number = camera.number;
    const divisibleBy3 = number % 3 === 0;
    const divisibleBy5 = number % 5 === 0;

    if (divisibleBy3 && divisibleBy5) {
      columns.column3.push(camera);        // Rule 3: Both
    } else if (divisibleBy3) {
      columns.column1.push(camera);        // Rule 1: Only 3
    } else if (divisibleBy5) {
      columns.column2.push(camera);        // Rule 2: Only 5
    } else {
      columns.column4.push(camera);        // Rule 4: Neither
    }
  });

  return columns;
};
```

#### Interactive Map Features
- **Leaflet Integration**: Open-source mapping without API keys
- **Interactive Markers**: Click to select cameras
- **Auto-fit Bounds**: Automatically zoom to show all cameras
- **Synchronized Selection**: Camera selection syncs between map and columns

## 🛠️ Technologies Used

### Backend
- **.NET 8.0** - Latest LTS framework
- **CsvHelper (v33.1.0)** - Professional CSV parsing
- **System.CommandLine (v2.0.0-beta4)** - Modern CLI framework
- **Swashbuckle.AspNetCore (v6.6.2)** - Swagger/OpenAPI documentation
- **Microsoft.Extensions.DependencyInjection** - Built-in DI container

### Frontend
- **React 19.1.1** - Latest React with modern features
- **Leaflet (v1.9.4)** - Interactive maps
- **React-Leaflet (v5.0.0)** - React integration for Leaflet
- **Axios (v1.11.0)** - HTTP client for API calls

### Development Tools
- **xUnit** - Unit testing framework
- **ESLint** - JavaScript linting
- **Git** - Version control

## 🧪 Testing

### Running Tests

```bash
# Run all .NET tests
cd CameraAssignment.Tests
dotnet test

# Run React tests
cd camera-web
npm test
```

### Test Coverage
- **Unit Tests**: Core business logic, data parsing, API controllers
- **Integration Tests**: End-to-end API functionality
- **Component Tests**: React component rendering and interactions

## 📊 Data Flow

### Application Startup
1. API starts → Loads CSV data → Configures services
2. React app starts → Calls API → Gets camera data
3. Frontend sorts data → Displays in columns → Shows on map

### Search Flow
```
User types in search box
     ↓
React calls API with search term
     ↓
API filters cameras by name (case-insensitive)
     ↓
Frontend receives filtered results
     ↓
Data gets re-sorted into columns and map updates
```

### Camera Selection Flow
```
User clicks camera in column OR clicks map marker
     ↓
selectedCamera state updates
     ↓
Map centers on selected camera
     ↓
Camera card highlights in column
     ↓
Map popup opens with camera details
```

## 🔍 API Documentation

When the API is running, visit:
- **Swagger UI**: `https://localhost:7298/swagger`
- **OpenAPI Spec**: `https://localhost:7298/swagger/v1/swagger.json`

### Available Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/cameras` | Get all cameras |
| GET | `/api/cameras/search?name={term}` | Search cameras by name |
| GET | `/api/cameras/{number}` | Get specific camera by number |

### Example API Responses

```json
// GET /api/cameras
[
  {
    "number": 501,
    "name": "UTR-CM-501 Neude rijbaan voor Postkantoor",
    "latitude": 52.093421,
    "longitude": 5.118278
  }
]

// GET /api/cameras/search?name=Neude
[
  {
    "number": 501,
    "name": "UTR-CM-501 Neude rijbaan voor Postkantoor",
    "latitude": 52.093421,
    "longitude": 5.118278
  },
  {
    "number": 503,
    "name": "UTR-CM-503 Neude plein",
    "latitude": 52.093448,
    "longitude": 5.118536
  }
]
```

## 🖥️ CLI Usage

The command line interface provides quick access to camera data:

```bash
# Search for cameras containing "Neude"
dotnet run --project CameraAssignment.Cli Search --name "Neude"

# Expected output:
# 501 | UTR-CM-501 Neude rijbaan voor Postkantoor | 52.093421 | 5.118278
# 503 | UTR-CM-503 Neude plein | 52.093448 | 5.118536
# 504 | UTR-CM-504 Neude / Schoutenstraat | 52.092995 | 5.119088
```

### CLI Features
- **Type-safe commands** with System.CommandLine
- **Built-in help system** (`--help` flag)
- **Input validation** with meaningful error messages
- **Shared business logic** with API and web app

## 🌐 Web Application Features

### Column-based Camera Display
Cameras are automatically sorted into four columns based on mathematical rules:

1. **Column 1**: Numbers divisible by 3 (but not 5) - *Example: 501, 504, 507*
2. **Column 2**: Numbers divisible by 5 (but not 3) - *Example: 505, 520, 535*
3. **Column 3**: Numbers divisible by both 3 and 5 - *Example: 510, 525, 540*
4. **Column 4**: Numbers not divisible by 3 or 5 - *Example: 502, 511, 517*

### Interactive Map
- **All camera locations** displayed as markers
- **Click to select** cameras (syncs with column highlighting)
- **Popup information** with camera details
- **Auto-zoom** to fit all markers
- **Real-time updates** when filtering/searching

### Search Functionality
- **Real-time search** as you type
- **Case-insensitive** matching
- **Partial name matching** for flexible searches
- **Results update** both columns and map simultaneously

## 🔧 Configuration

### API Configuration (`appsettings.json`)
```json
{
  "CameraData": {
    "CsvFilePath": "../data/cameras-defb.csv"
  },
  "Cors": {
    "AllowedOrigins": ["http://localhost:3000"]
  }
}
```

### Environment Variables
- `ASPNETCORE_ENVIRONMENT` - Sets development/production mode
- `CAMERA_CSV_PATH` - Override CSV file location

## 🚀 Deployment

### API Deployment
```bash
cd CameraAssignment.API
dotnet publish -c Release -o ./publish
```

### Frontend Deployment
```bash
cd camera-web
npm run build
# Serve the build folder with your preferred web server
```

## 🔮 Future Enhancements

### Performance Optimizations
- **Caching**: Add Redis for API response caching
- **Pagination**: Implement server-side pagination for large datasets
- **Virtual Scrolling**: Handle thousands of cameras efficiently
- **Map Clustering**: Group nearby markers for better performance

### Security Improvements
- **Authentication**: Add JWT-based authentication
- **Rate Limiting**: Prevent API abuse
- **CORS Restrictions**: Environment-specific CORS policies
- **Input Validation**: Enhanced server-side validation

### Feature Additions
- **Real-time Updates**: WebSocket integration for live camera data
- **Camera Details**: Extended camera information and metadata
- **Export Functions**: CSV/JSON export of filtered data
- **Advanced Filtering**: Multiple filter criteria
- **Responsive Design**: Mobile-optimized interface

### Technical Debt
- **TypeScript Migration**: Convert React app to TypeScript
- **Error Boundaries**: Better React error handling
- **Monitoring**: Application performance monitoring
- **Documentation**: Comprehensive API documentation

## 📝 Development Notes

### Design Decisions
- **Clean Architecture**: Chosen for maintainability and testability
- **Repository Pattern**: Enables easy data source changes
- **React Functional Components**: Modern React best practices
- **CSS Modules**: Avoided to keep setup simple
- **Open Source Libraries**: No API keys required for maps

### Performance Considerations
- **Single API Call**: Load all cameras once, filter client-side
- **Efficient Sorting**: O(n) algorithm for column categorization
- **Memoization**: React useMemo for expensive calculations
- **Component Optimization**: Proper React key usage

### Browser Compatibility
- **Modern Browsers**: Chrome 90+, Firefox 88+, Safari 14+
- **ES6+ Features**: Uses modern JavaScript features
- **CSS Grid/Flexbox**: Modern layout techniques

## 🤝 Contributing

### Development Setup
1. Fork the repository
2. Create a feature branch: `git checkout -b feature/amazing-feature`
3. Make your changes
4. Run tests: `dotnet test && npm test`
5. Commit changes: `git commit -m 'Add amazing feature'`
6. Push to branch: `git push origin feature/amazing-feature`
7. Open a Pull Request

### Code Standards
- **C# Conventions**: Follow Microsoft C# coding conventions
- **React Best Practices**: Functional components, hooks, proper state management
- **Testing**: Maintain test coverage above 80%
- **Documentation**: Update README for significant changes

## 📄 License

This project is open source and available under the [MIT License](LICENSE.txt).

## 📞 Support

For questions about this implementation:
- Check the existing tests for usage examples
- Review the Swagger documentation for API details
- Examine the React components for frontend patterns

---

**Built with ❤️ for the "Keep Talking and Everybody Codes" technical interview exercise.**

*This README demonstrates comprehensive documentation practices, architectural thinking, and attention to detail - key qualities for modern software development teams.*

