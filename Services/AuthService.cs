using PaginationResultWebApi.Data;
using PaginationResultWebApi.Entities;
using PaginationResultWebApi.Repositories.Contracts;
using PaginationResultWebApi.Services.Contracts;
using PaginationResultWebApi.UseCases.Auth.Commands;
using PaginationResultWebApi.UseCases.Auth.Dtos;

namespace PaginationResultWebApi.Services;

public class AuthService(IAuthRepository authRepository) : IAuthService
{
    public async Task<ApiResponse> GoogleAuth(GoogleAuthCommand googleAuthCommand)
    {
        try
        {
            if (googleAuthCommand.UserClaims != null)
            {
                var socialAuthDto = new SocialAuthDto()
                {
                    Action = googleAuthCommand.Action,
                    Succeeded = googleAuthCommand.Succeeded,
                    UserClaims = googleAuthCommand.UserClaims,
                    Provider = Constants.GOOGLE_PROVIDER
                };
            
                var socialAuthUserClaimsDto = SocialAuthUserClaimsDto.FromClaims(googleAuthCommand.UserClaims);

                await ProcessSocialAuthAction(socialAuthDto, socialAuthUserClaimsDto);
            }

            return new ApiResponse(true, "Google Auth Successfully", Constants.REDIRECT_SUCCESS_URL);
        }
        catch (Exception ex)
        {
            return new ApiResponse(false, $"Error Google auth: {ex.Message}", Constants.REDIRECT_ERROR_URL);
        }
    }
    
    private async Task<Customer> ProcessSocialAuthAction(SocialAuthDto socialAuthDto, SocialAuthUserClaimsDto socialAuthUserClaimsDto)
    {
        var existingUser = await authRepository.FindOneAsync(u => u.Email == socialAuthUserClaimsDto.Email);

        if (socialAuthDto.Action == Constants.LOGIN_ACTION)
        {
            if (existingUser == null)
            {
                throw new Exception("User not registered");
            }
            return existingUser;
        }

        if (socialAuthDto.Action == Constants.REGISTER_ACTION)
        {
            if (existingUser != null)
            {
                throw new Exception("User already registered");
            }

            var customer = new Customer
            {
                FirstName = socialAuthUserClaimsDto.GivenName,
                LastName = socialAuthUserClaimsDto.SurName,
                Email = socialAuthUserClaimsDto.Email,
                Photo = socialAuthUserClaimsDto.Picture,
                Provider = socialAuthDto.Provider
            };

            await authRepository.AddAsync(customer);
            return customer;
        }

        throw new Exception("Invalid action");
    }
}