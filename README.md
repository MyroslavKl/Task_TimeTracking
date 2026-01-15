# TimeTracking API

A RESTful API for logging employee work hours. Built with **.NET 8**, **Dapper**, and **PostgreSQL**.

## How to Run
### Steps

1. **Configuration**

   Fill appseting.json and docker-compose.yml with your_username and your_password
3. **Run docker-compose.yml**
 
   Run 'docker-compose up -d'
4. **Run project**
   
   Run 'dotnet run', so the database will be seeded by data.
Then you can run Swagger to test the API.

## What Works (Implemented Features)

I have implemented all core requirements plus the bonus tasks.

* **Core Architecture**: Layered architecture (Controllers -> Repositories -> Database) with Dependency Injection.
* **Data Access**: High-performance data access using **Dapper** with raw SQL queries (constants).
* **Database**: **PostgreSQL** integration via Docker Compose.
* **Automation**: Automatic table creation and data seeding on application startup (`DbInitializer`).
* **Validation**:
  * Input validation (Data Annotations).
  * **Business Rule**: Validates that total work hours do not exceed 24 hours per day (implemented for both Create and Update operations).
* **Error Handling**: Global Exception Handling Middleware returning standardized `ProblemDetails` responses.
* **Testing**: Unit tests using **xUnit** and **Moq** covering successful scenarios and validation failures.
