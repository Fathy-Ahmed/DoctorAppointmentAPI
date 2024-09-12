
using Entities.DTO;
using Entities.Models;

namespace Entities.Services;

public interface IAuthenticationServices
{
    Task<AuthenticationModel> RegisterAsync(RegisterDTO model);
    Task<AuthenticationModel> GetTokenAsync(TokenRequstModel model);
    //Task<string> AddRoleAsync(AddRoleModel model);
}
