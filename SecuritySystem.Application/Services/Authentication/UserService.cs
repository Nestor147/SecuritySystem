using SecuritySystem.Application.Interfaces.Authentication;
using SecuritySystem.Core.Entities;
using SecuritySystem.Core.Entities.core.CustomEntities.ResponseApi;
using SecuritySystem.Core.Entities.core.CustomEntities.ResponseApi.Details;
using SecuritySystem.Core.Entities.SealedAuthentication;
using SecuritySystem.Core.Enums;
using SecuritySystem.Core.Interfaces.Core;
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
                                Tipo = TypeMessage.error.ToString(),
                                Descripcion = "Request body cannot be null."
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
                                Tipo = TypeMessage.warning.ToString(),
                                Descripcion = "Email and password are required."
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
                                Tipo = TypeMessage.information.ToString(),
                                Descripcion = $"User with email '{request.Email}' already exists."
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
                            Tipo = TypeMessage.success.ToString(),
                            Descripcion = "User created successfully."
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
                            Tipo = TypeMessage.error.ToString(),
                            Descripcion = $"An unexpected error occurred while creating the user: {ex.Message}"
                        }
                    }
                };
            }
        }
    }
}
