# CODELSOFT USER SERVICE

The User Service API is a GRPC & Restful service designed to provide a robust platform for managing user data. Built with modern development practices, it offers essentials functionalities while adhering the REST combined with GRPC principles, ensuring a scalable and maintainable solution.

<p align="center">
  <img src="https://www.techmeet360.com/wp-content/uploads/2018/11/ASP.NET-Core-Logo.png" alt="Logo" height="300">

## Features

- Get Profile: Allows the authenticated user to view its profile, with the fields: Id, Name, First Last Name, Second Last Name, Rut, Email and Career Name.
- Update Profile: Allows the authenticated user to edit its profile, with the fields: Name, First Last Name and Second Last Name.
- Get My-Progress: Allows the authenticated user to view its progress in the curricular matrix, with the fields: Id and Code of all subjects that the user has approved.
- Update My-Progress: Allows the authenticated user to update its progress, with two lists: The first list is the code of the subjects that the user wants to mark as approved, and the second list is the code of the subjects that the user wants to remove from their approved subjects.
- Register: Allows the authenticated user to create an account by providing the following fields: Name, First Last Name, Second Last Name, Rut, Email, Career Name, and Password. The system validates the provided information before creating the user profile.
- Update Password: Allows the authenticated user to update their password. The user must provide the current password and the new password to successfully update it. The system ensures that the new password meets the required security criteria.

## Technologies

- ASP.NET Core 8.0
- Entity Framework Core
- PostgreSQL
- JWT for token-based authentication

## Requirements

- [.NET 8.0.302 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Visual Studio Code](https://code.visualstudio.com/) or [Visual Studio](https://visualstudio.microsoft.com/)
- [PostgreSQL](https://www.postgresql.org/download/)

## Installation

1. Clone the repository from GitHub and navigate to its directory by running the following commands in your terminal:

   ```bash
   git clone https://github.com/funktasthic/CodelsoftUserService.git
   cd CodelsoftUserService
   code .
   ```

2. Configure the `appsettings.json` file with your PostgreSQL database details & add the JWT Secret key on it.

3. Restore the project dependencies:

   ```bash
   dotnet restore
   ```

4. Create and apply Entity Framework Core migrations to set up the database:

   ```bash
   dotnet ef database drop --project src/Api
   dotnet ef migrations add InitMigration --project src/Api --output-dir Data/Migrations
   dotnet ef database update --project src/Api
   ```

5. Run the application locally:

   ```bash
   dotnet run --project src/api
   ```

6. (Optional step) If you have problems running the project you just have to use

   ```bash
   dotnet help
   ```

7. (Optional step) If you have problems running the proto files you have to use

   ```bash
   dotnet clean
   ```

## Author

- [@funktasthic](https://github.com/funktasthic)
