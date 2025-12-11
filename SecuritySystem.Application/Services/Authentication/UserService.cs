using SecuritySystem.Application.Interfaces.Authentication;
using SecuritySystem.Core.Entities;
using SecuritySystem.Core.Entities.core.CustomEntities.ResponseApi;
using SecuritySystem.Core.Entities.core.CustomEntities.ResponseApi.Details;
using SecuritySystem.Core.Entities.SealedAuthentication;
using SecuritySystem.Core.Enums;
using SecuritySystem.Core.Interfaces.Core;
using SecuritySystem.Core.QueryFilters.Autorization;
using System.Net;

namespace SecuritySystem.Application.Services.Users
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;

        public UserService(
            IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
        }

        public async Task<ResponsePost> CreateUserAsync(CreateUserRequest request, CancellationToken ct)
        {
            try
            {
                #region Basic validation
                if (request == null)
                {
                    return new ResponsePost
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        Mensajes = new[]
                        {
                            new Message
                            {
                                Type = TypeMessage.error.ToString(),
                                Description = "Request body cannot be null."
                            }
                        }
                    };
                }

                if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                {
                    return new ResponsePost
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        Mensajes = new[]
                        {
                            new Message
                            {
                                Type = TypeMessage.warning.ToString(),
                                Description = "Email and password are required."
                            }
                        }
                    };
                }
                #endregion

                #region Check if user already exists
                var existing = await _unitOfWork.UserRepository
                    .FirstOrDefaultAsync(u => u.Email == request.Email && u.RecordStatus == 1, ct);

                if (existing != null)
                {
                    return new ResponsePost
                    {
                        StatusCode = HttpStatusCode.Conflict,
                        Mensajes = new[]
                        {
                            new Message
                            {
                                Type = TypeMessage.information.ToString(),
                                Description = $"User with email '{request.Email}' already exists."
                            }
                        }
                    };
                }
                #endregion

                #region Create user
                var now = DateTime.UtcNow;
                var passwordHash = _passwordHasher.Hash(request.Password);

                var user = new User
                {
                    ExternalUserId = request.ExternalUserId,

                    // We use the external email as both Username and Email
                    Username = request.Email,
                    Email = request.Email,

                    PasswordHash = passwordHash,
                    LastPasswordChange = now,

                    IsLocked = false,
                    LockDate = null,

                    IsNewUser = true,
                    KeepLoggedIn = false,

                    // Audit fields (from AuditFields)
                    RecordStatus = 1,
                    CreatedAt = now,
                    CreatedBy = "SYSTEM" // later you can replace by current user from token
                };

                ResponsePostDetail insertResult = await _unitOfWork.UserRepository.InsertAsync(user, "CREATE_USER");
                await _unitOfWork.SaveChangesAsync();
                #endregion

                return new ResponsePost
                {
                    StatusCode = HttpStatusCode.OK,
                    Mensajes = new[]
                    {
                        new Message
                        {
                            Type = TypeMessage.success.ToString(),
                            Description = "User created successfully."
                        }
                    }
                    // Si tu ResponsePost tiene más campos (Id, Respuesta, etc.),
                    // aquí puedes mapear insertResult.GeneratedId, etc.
                };
            }
            catch (Exception ex)
            {
                // Last-resort error: we never throw, always respond with ResponsePost
                return new ResponsePost
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Mensajes = new[]
                    {
                        new Message
                        {
                            Type = TypeMessage.error.ToString(),
                            Description = $"An unexpected error occurred while creating the user: {ex.Message}"
                        }
                    }
                };
            }
        }

        public async Task<ResponseGet> SearchUsersAsync(string searchCriteria, CancellationToken ct)
        {
            try
            {
                #region Basic validation
                if (string.IsNullOrWhiteSpace(searchCriteria))
                {
                    return new ResponseGet
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        Messages = new[]
                        {
                        new Message
                        {
                            Type = TypeMessage.warning.ToString(),
                            Description = "Search criteria cannot be empty."
                        }
                    }
                    };
                }
                #endregion

                var normalized = searchCriteria.Trim().ToLower();

                // Recupera usuarios activos cuyo Username o Email contengan el criterio
                var users = await _unitOfWork.UserRepository.WhereAsync(
                    u =>
                        u.RecordStatus == 1 &&
                        (
                            (u.Username != null && u.Username.ToLower().Contains(normalized)) ||
                            (u.Email != null && u.Email.ToLower().Contains(normalized))
                        ),
                    ct
                );

                var results = users
                    .Select(u => new UserSearchQueryFilter
                    {
                        Id = u.Id,
                        Username = u.Username,
                        Email = u.Email
                    })
                    .ToList();

                return new ResponseGet
                {
                    StatusCode = HttpStatusCode.OK,
                    // si tu ResponsePost tiene una propiedad Respuesta, la puedes usar:
                    Data = results, // <- aquí devuelves Id, Username y Email
                    Messages = new[]
                    {
                    new Message
                    {
                        Type = TypeMessage.success.ToString(),
                        Description = results.Any()
                            ? "Users retrieved successfully."
                            : "No users found for the given criteria."
                    }
                }
                };
            }
            catch (Exception ex)
            {
                return new ResponseGet
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Messages = new[]
                    {
                    new Message
                    {
                        Type = TypeMessage.error.ToString(),
                        Description = $"An unexpected error occurred while searching users: {ex.Message}"
                    }
                }
                };
            }
        }
    }
}
