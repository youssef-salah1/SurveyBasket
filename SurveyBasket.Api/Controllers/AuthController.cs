using SurveyBasket.Core.Contracts.Authentication;

namespace SurveyBasket.Api.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [HttpPost("")]
    public async Task<IActionResult> LoginAsync(LoginRequest loginRequest, CancellationToken cancellationToken)
    {
        var authResponse =
            await _authService.GetTokenAsync(loginRequest.Email, loginRequest.Password, cancellationToken);

        return authResponse.IsSuccess
            ? Ok(authResponse.Value)
            : Problem(statusCode: StatusCodes.Status400BadRequest, title: authResponse.Error.Code,
                detail: authResponse.Error.Message);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshAsync([FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        var authResponse =
            await _authService.GetRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);

        return authResponse.IsSuccess
            ? Ok(authResponse.Value)
            : Problem(statusCode: StatusCodes.Status400BadRequest, title: authResponse.Error.Code,
                detail: authResponse.Error.Message);
    }

    [HttpPost("revoke-refresh-token")]
    public async Task<IActionResult> RevokeRefreshTokenAsync([FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        var isRevoked =
            await _authService.RevokeRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);

        return isRevoked.IsSuccess
            ? Ok()
            : Problem(statusCode: StatusCodes.Status400BadRequest, title: isRevoked.Error.Code,
                detail: isRevoked.Error.Message);
        ;
    }
}