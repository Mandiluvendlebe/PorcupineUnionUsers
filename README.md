#PorcupineUnionUsers API

A simple .NET Core Web API with a basic UI for user, group, and permissions management.
This project demonstrates clean architecture, Entity Framework Core Code First, Seed Data, unit & integration testing, and a minimal web interface.

#How to Run Locally 🛠️
1. Clone the Repository
git clone https://github.com/Mandiluvendlebe/PorcupineUnionUsers.git
cd PorcupineUnionUsers

2. Open the Project in Visual Studio

Open PorcupineUnionUsers.sln

Set the Startup Project → PU.Users.Api

Confirm the launch URL:

https://localhost:44320/

3. Set Up the Database

#Note: You do NOT need to create initial migrations.
Migrations and seed data are already included.

But, if you want to reset the database:

dotnet ef database drop
dotnet ef database update


This will:

Apply all migrations ✅

Insert seed data automatically ✅

4. Run the Project
dotnet build
dotnet run --project PU.Users.Api


Or simply press F5 in Visual Studio.

Your API will now be available at:

https://localhost:44320/

5. Seed Data Info

The project already includes default seed data for:

Users

Groups

Permissions

You can modify this in:

PU.Users.Api/Data/SeedData.cs

6. API Endpoints
Users

GET /api/users → Get all users

GET /api/users/{id} → Get user by ID

POST /api/users → Create a user

PUT /api/users/{id} → Update a user

DELETE /api/users/{id} → Delete a user

Groups

GET /api/groups → Get all groups

POST /api/groups → Create a group

GET /api/groups/{id}/users → Get users in a group

Statistics

GET /api/users/count → Get total user count

GET /api/groups/users-count → Get number of users per group

7. Tests
Run Unit & Integration Tests
dotnet test


✅ All tests pass locally.

8. Continuous Integration (CI)

⚠ Note:
CI builds may fail on GitHub Actions because the workflow uses an Ubuntu-based runner, and LocalDB storage used in integration tests is Windows-only.
Locally, all tests work perfectly. ✅

9. Simple Static UI

A minimal static UI is included for basic user management.

Location:

PU.Users.Api/wwwroot/index.html


You can open this page directly in the browser or access it via:

https://localhost:44320/

10. Known Issues

CI integration tests fail on GitHub Linux runners due to LocalDB storage.

To fix this in the future, we can:

Switch to SQLite or InMemory for CI

Or configure SQL Server container in GitHub Actions.
