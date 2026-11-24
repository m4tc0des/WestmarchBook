using WestmarchBook.Communication.Requests;
using WestmarchBook.Communication.Responses;
using WestmarchBook.Domain.Repositories.User;
using WestmarchBook.Domain.Repositories.VerificationCode;
using WestmarchBook.Domain.Security.Tokens;
using WestmarchBook.Domain.Services.StorageService;
using WestmarchBook.Exception;
using WestmarchBook.Exception.ExceptionsBase;

namespace WestmarchBook.Application.UseCases.Login.WithExternalProvider;

public class ExchangeExternalLoginCodeUseCase : IExchangeExternalLoginCodeUseCase
{
    private const int ExpirationTimeSeconds = 30;
    private readonly IVerificationCodeReadOnlyRepository _verificationCodeReadOnlyRepository;
    private readonly IVerificationCodeWriteOnlyRepository _verificationCodeWriteOnlyRepository;
    private readonly IUserReadOnlyRepository _userReadOnlyRepository;
    private readonly IAccessTokenGenerator _accessTokenGenerator;
    private readonly IStorageService _storageService;

    public ExchangeExternalLoginCodeUseCase(IVerificationCodeReadOnlyRepository verificationCodeReadOnlyRepository,
        IVerificationCodeWriteOnlyRepository verificationCodeWriteOnlyRepository,
        IUserReadOnlyRepository userReadOnlyRepository,
        IAccessTokenGenerator accessTokenGenerator,
        IStorageService storageService)
    {
        _verificationCodeReadOnlyRepository = verificationCodeReadOnlyRepository;
        _verificationCodeWriteOnlyRepository = verificationCodeWriteOnlyRepository;
        _userReadOnlyRepository = userReadOnlyRepository;
        _accessTokenGenerator = accessTokenGenerator;
        _storageService = storageService;
    }

    public async Task<ResponseRegisterUserJson> Execute(RequestExternalLoginJson request)
    {
        var verificationCode = await _verificationCodeReadOnlyRepository.GetExternalLoginCode(request.Code);
        if (verificationCode is null) 
            throw new ErrorOnValidationException([ResourceMessagesException.VERIFICATION_CODE_INVALID]);

        var isCodeValid = verificationCode.CreatedAt.AddSeconds(ExpirationTimeSeconds) >= DateTime.UtcNow;
        if (isCodeValid == false) 
            throw new ErrorOnValidationException([ResourceMessagesException.VERIFICATION_CODE_INVALID]);

        await _verificationCodeWriteOnlyRepository.Delete(verificationCode);

        var user = await _userReadOnlyRepository.GetById(verificationCode.UserId);

        return new ResponseRegisterUserJson
        {
            UserName = user.UserName,
            ImageUrl = user.HasImage ? _storageService.GetProfilePictureUrl(user.Id) : string.Empty,
            Tokens = new ResponseTokensJson
            {
                AccessToken = _accessTokenGenerator.Generate(user)
            }
        };
    }
}
