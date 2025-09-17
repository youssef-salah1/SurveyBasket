using SurveyBasket.Api.Contracts.Authentication;

namespace SurveyBasket.Api.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [HttpPost("")]
    public async Task<IActionResult> LoginAsync(LoginRequest loginRequest, CancellationToken cancellationToken)
    {
        var authResponse = await _authService.GetTokenAsync(loginRequest.Email, loginRequest.Password, cancellationToken);

        return authResponse is null ? BadRequest("Invalid User") : Ok(authResponse);
    }
}
