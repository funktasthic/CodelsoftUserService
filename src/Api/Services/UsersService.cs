using Api.Services.Interfaces;
using Google.Protobuf.Collections;
using Grpc.Core;
using UserProto;
using UserService.Api.Exceptions;
using UserService.Api.Models;
using UserService.Api.Repositories.Interfaces;

namespace Api.Services
{
    public class UsersService : UserProto.UsersService.UsersServiceBase, IUsersService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapperService _mapperService;
        private readonly IAuthService _authService;

        public UsersService(IUnitOfWork unitOfWork, IMapperService mapperService, IAuthService authService)
        {
            _unitOfWork = unitOfWork;
            _mapperService = mapperService;
            _authService = authService;
        }

        public override async Task<UserResponse> GetProfile(Empty request, ServerCallContext context)
        {
            var userEmail = _authService.GetUserEmailInToken(context);
            var getByEmail = await GetByEmail(userEmail);
            var response = new UserResponse { User = getByEmail };
            return response;
        }

        public override async Task<UpdateUserProfileResponse> UpdateProfile(UpdateUserProfileDto updateUserProfileDto, ServerCallContext context)
        {
            var userEmail = _authService.GetUserEmailInToken(context);
            var user = await GetUserByEmail(userEmail);

            user.Name = updateUserProfileDto.Name ?? user.Name;
            user.FirstLastName = updateUserProfileDto.FirstLastName ?? user.FirstLastName;
            user.SecondLastName = updateUserProfileDto.SecondLastName ?? user.SecondLastName;

            var updatedUser = await _unitOfWork.UsersRepository.Update(user);
            var updatedUserDto = _mapperService.Map<User, UpdateUserProfileDto>(updatedUser);

            return new UpdateUserProfileResponse { User = updatedUserDto };
        }

        public override async Task<UserProgressResponse> GetUserProgress(Empty request, ServerCallContext context)
        {
            var userId = await GetUserIdByToken(context);
            var userProgress = await _unitOfWork.UsersRepository.GetProgressByUser(userId);

            if (userProgress == null || !userProgress.Any())
                throw new EntityNotFoundException("No progress data found for the user.");

            var progressDtos = _mapperService.Map<IEnumerable<UserProgress>, RepeatedField<UserProgressDto>>(userProgress);

            return new UserProgressResponse { UserProgress = { progressDtos } };
        }

        public override async Task<Empty> SetUserProgress(UpdateUserProgressDto subjects, ServerCallContext context)
        {
            var userId = await GetUserIdByToken(context);

            var subjectsId = await MapAndValidateToSubjectId(subjects);
            var subjectsToAdd = subjectsId.Item1;
            var subjectsToDelete = subjectsId.Item2;

            var userProgress = await _unitOfWork.UsersRepository.GetProgressByUser(userId) ?? new List<UserProgress>();

            var progressToAdd = subjectsToAdd.Select(s =>
            {
                var foundUserProgress = userProgress.FirstOrDefault(up => up.SubjectId == s);

                if (foundUserProgress != null)
                    throw new DuplicateEntityException($"Subject with ID: {foundUserProgress.Subject.Code} already exists");

                return new UserProgress()
                {
                    SubjectId = s,
                    UserId = userId,
                };
            }).ToList();

            var progressToRemove = subjectsToDelete.Select(s =>
            {
                if (userProgress.FirstOrDefault(up => up.SubjectId == s) == null)
                    throw new EntityNotFoundException($"Subject with ID: {s} not found");

                return new UserProgress()
                {
                    SubjectId = s,
                    UserId = userId,
                };
            }).ToList();

            var addResult = await _unitOfWork.UsersRepository.AddProgress(progressToAdd);

            var removeResult = await _unitOfWork.UsersRepository.RemoveProgress(progressToRemove, userId);

            if (!addResult && !removeResult)
                throw new InternalErrorException("Cannot update user progress");

            return new Empty();
        }


        #region PRIVATE_METHODS

        private async Task<Tuple<List<int>, List<int>>> MapAndValidateToSubjectId(UpdateUserProgressDto subjects)
        {
            var allSubjects = await _unitOfWork.SubjectsRepository.Get();
            var subjectsToAdd = subjects.AddSubjects;
            var subjectsToDelete = subjects.DeleteSubjects;

            var mappedSubjectsToAdd = subjectsToAdd.Select(s =>
            {
                s = s.ToLower();
                var foundSubject = allSubjects.FirstOrDefault(sub => sub.Code == s)
                    ?? throw new EntityNotFoundException($"Subject with ID: {s} not found");
                return foundSubject.Id;
            }).ToList();

            var mappedSubjectsToDelete = subjectsToDelete.Select(s =>
            {
                s = s.ToLower();
                var foundSubject = allSubjects.FirstOrDefault(sub => sub.Code == s)
                    ?? throw new EntityNotFoundException($"Subject with ID: {s} not found");
                return foundSubject.Id;
            }).ToList();

            return new Tuple<List<int>, List<int>>(mappedSubjectsToAdd, mappedSubjectsToDelete);

        }

        public async Task<UserDto> GetByEmail(string email)
        {
            var user = await GetUserByEmail(email);
            return _mapperService.Map<User, UserDto>(user);
        }

        private async Task<User> GetUserByEmail(string email)
        {
            var user = await _unitOfWork.UsersRepository.GetByEmail(email)
                        ?? throw new EntityNotFoundException($"User with email: {email} not found");
            return user;
        }

        private async Task<int> GetUserIdByToken(ServerCallContext context)
        {
            var userEmail = _authService.GetUserEmailInToken(context);
            var user = await _unitOfWork.UsersRepository.GetByEmail(userEmail)
                        ?? throw new EntityNotFoundException("User not found");
            return user.Id;
        }

        #endregion
    }
}
