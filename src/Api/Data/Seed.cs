using System.Text.Json;
using UserService.Api.Models;

namespace UserService.Api.Data
{
    public class Seed
    {
        public static void SeedData(DataContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            CallEachSeeder(context, options);
        }

        private static void CallEachSeeder(DataContext context, JsonSerializerOptions options)
        {
            SeedRoles(context, options);
            SeedCareers(context, options);
            SeedFirstOrderTables(context, options);
            SeedSecondOrderTables(context, options);
            SeedUsers(context, options);
            SeedUsersProgress(context, options);
        }


        private static void SeedFirstOrderTables(DataContext context, JsonSerializerOptions options)
        {
            SeedSubjects(context, options);
        }

        private static void SeedSecondOrderTables(DataContext context, JsonSerializerOptions options)
        {
            SeedSubjectsRelationships(context, options);
        }

        private static void SeedRoles(DataContext context, JsonSerializerOptions options)
        {
            var result = context.Roles?.Any();
            if (result is true or null) return;

            var path = "Data/DataSeeders/RolesData.json";
            var rolesData = File.ReadAllText(path);
            var rolesList = JsonSerializer.Deserialize<List<Role>>(rolesData, options) ??
                throw new Exception("RolesData.json is empty");

            rolesList.ForEach(role =>
            {
                role.CreatedAt = role.CreatedAt.ToUniversalTime();
                role.UpdatedAt = role.UpdatedAt.ToUniversalTime();
                if (role.DeletedAt.HasValue)
                {
                    role.DeletedAt = role.DeletedAt.Value.ToUniversalTime();
                }
            });

            context.Roles?.AddRange(rolesList);
            context.SaveChanges();
        }

        private static void SeedUsers(DataContext context, JsonSerializerOptions options)
        {
            var result = context.Users?.Any();
            if (result is true or null) return;

            var path = "Data/DataSeeders/UsersData.json";
            var usersData = File.ReadAllText(path);
            var usersList = JsonSerializer.Deserialize<List<User>>(usersData, options) ??
                throw new Exception("UsersData.json is empty");

            usersList.ForEach(user =>
            {
                user.HashedPassword = BCrypt.Net.BCrypt.HashPassword(user.HashedPassword);

                user.CreatedAt = DateTime.UtcNow;
                user.UpdatedAt = DateTime.UtcNow;
            });

            context.Users?.AddRange(usersList);
            context.SaveChanges();
        }

        private static void SeedSubjects(DataContext context, JsonSerializerOptions options)
        {
            var result = context.Subjects?.Any();
            if (result is true or null) return;

            var path = "Data/DataSeeders/SubjectsData.json";
            var subjectsData = File.ReadAllText(path);
            var subjectsList = JsonSerializer.Deserialize<List<Subject>>(subjectsData, options) ??
                throw new Exception("SubjectsData.json is empty");

            subjectsList.ForEach(s =>
            {
                s.Code = s.Code.ToLower();
                s.Name = s.Name.ToLower();
                s.Department = s.Department.ToLower();
                s.CreatedAt = s.CreatedAt.ToUniversalTime();
                s.UpdatedAt = s.UpdatedAt.ToUniversalTime();
                if (s.DeletedAt.HasValue)
                {
                    s.DeletedAt = s.DeletedAt.Value.ToUniversalTime();
                }
            });

            context.Subjects?.AddRange(subjectsList);
            context.SaveChanges();
        }

        private static void SeedCareers(DataContext context, JsonSerializerOptions options)
        {
            var result = context.Careers?.Any();
            if (result is true or null) return;

            var path = "Data/DataSeeders/CareersData.json";
            var careersData = File.ReadAllText(path);
            var careersList = JsonSerializer.Deserialize<List<Career>>(careersData, options) ??
                throw new Exception("CareersData.json is empty");

            careersList.ForEach(s =>
            {
                s.Name = s.Name.ToLower();
                s.CreatedAt = s.CreatedAt.ToUniversalTime();
                s.UpdatedAt = s.UpdatedAt.ToUniversalTime();
                if (s.DeletedAt.HasValue)
                {
                    s.DeletedAt = s.DeletedAt.Value.ToUniversalTime();
                }
            });

            context.Careers?.AddRange(careersList);
            context.SaveChanges();
        }

        private static void SeedSubjectsRelationships(DataContext context, JsonSerializerOptions options)
        {
            var result = context.SubjectsRelationships?.Any();
            if (result is true or null) return;

            var path = "Data/DataSeeders/SubjectsRelationsData.json";
            var subjectsRelationshipsData = File.ReadAllText(path);
            var subjectsRelationshipsList = JsonSerializer.Deserialize<List<SubjectRelationship>>(subjectsRelationshipsData, options) ??
                throw new Exception("SubjectsRelationsData.json is empty");

            subjectsRelationshipsList.ForEach(s =>
            {
                s.SubjectCode = s.SubjectCode.ToLower();
                s.PreSubjectCode = s.PreSubjectCode.ToLower();
                s.CreatedAt = s.CreatedAt.ToUniversalTime();
                s.UpdatedAt = s.UpdatedAt.ToUniversalTime();
                if (s.DeletedAt.HasValue)
                {
                    s.DeletedAt = s.DeletedAt.Value.ToUniversalTime();
                }
            });

            context.SubjectsRelationships?.AddRange(subjectsRelationshipsList);
            context.SaveChanges();
        }

        private static void SeedUsersProgress(DataContext context, JsonSerializerOptions options)
        {
            var result = context.UsersProgress?.Any();
            if (result is true or null) return;

            var path = "Data/DataSeeders/UsersProgressData.json";
            var usersProgressData = File.ReadAllText(path);
            var usersProgressList = JsonSerializer.Deserialize<List<UserProgress>>(usersProgressData, options) ??
                throw new Exception("UsersProgressData.json is empty");

            usersProgressList.ForEach(up =>
            {
                if (!context.Users!.Any(u => u.Id == up.UserId) ||
                    !context.Subjects!.Any(s => s.Id == up.SubjectId))
                {
                    throw new Exception($"Invalid UserId or SubjectId in UsersProgress: UserId {up.UserId}, SubjectId {up.SubjectId}");
                }

                up.CreatedAt = DateTime.UtcNow;
                up.UpdatedAt = DateTime.UtcNow;
            });

            context.UsersProgress?.AddRange(usersProgressList);
            context.SaveChanges();
        }
    }
}

