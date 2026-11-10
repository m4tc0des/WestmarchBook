using WestmarchBook.Communication.Requests;
using WestmarchBook.Domain.Entities;
using WestmarchBook.Domain.Enums;
using WestmarchBook.Domain.Repositories;
using WestmarchBook.Domain.Repositories.User;
using WestmarchBook.Domain.Repositories.VerificationCode;
using WestmarchBook.Domain.Security.PasswordHashing;
using WestmarchBook.Exception;
using WestmarchBook.Exception.ExceptionsBase;

namespace WestmarchBook.Application.UseCases.PasswordRecovery.ResetPassword;

public class ResetPasswordUseCase : IResetPasswordUseCase
{
    private const int EXPIRATION_TIME_IN_MINUTES = 10;

    private readonly IUserReadOnlyRepository _userReadRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IVerificationCodeWriteOnlyRepository _verificationCodeWriteRepository; 
    private readonly IVerificationCodeReadOnlyRepository _verificationCodeReadRepository;
    private readonly IUserUpdateOnlyRepository _userUpdateRepository;

    public ResetPasswordUseCase(IUserReadOnlyRepository userReadRepository,
        IPasswordHasher passwordHasher,
        IVerificationCodeWriteOnlyRepository verificationCodeWriteRepository,
        IVerificationCodeReadOnlyRepository verificationCodeReadRepository,
        IUserUpdateOnlyRepository userUpdateRepository)
    {
        _userReadRepository = userReadRepository;
        _passwordHasher = passwordHasher;
        _verificationCodeWriteRepository = verificationCodeWriteRepository;
        _verificationCodeReadRepository = verificationCodeReadRepository;
        _userUpdateRepository = userUpdateRepository;
    }

    public async Task Execute(RequestResetPasswordJson request)
    {
        var user = await _userReadRepository.GetByEmail(request.Email);

        if (user is null)
        {
            throw new ErrorOnValidationException([ResourceMessagesException.VERIFICATION_CODE_INVALID]);
        }

        var verificationCode = await _verificationCodeReadRepository.GetCode(user.Id, request.Code, VerificationCodeType.PasswordRecovery);

        if (verificationCode is null)
        {
            throw new ErrorOnValidationException([ResourceMessagesException.VERIFICATION_CODE_INVALID]);
        }

        ValidateAndThrowOnFailures(request, verificationCode);

        await _userUpdateRepository.UpdatePassword(user.Id, _passwordHasher.HashPassword(request.NewPassword));

        await _verificationCodeWriteRepository.Delete(verificationCode);
    }

    private void ValidateAndThrowOnFailures(RequestResetPasswordJson request, VerificationCode verificationCode)
    {
        var isCodeValid  = verificationCode.CreatedAt.AddMinutes(EXPIRATION_TIME_IN_MINUTES) >= DateTime.UtcNow;

        if (isCodeValid == false)
        {
            throw new ErrorOnValidationException([ResourceMessagesException.VERIFICATION_CODE_INVALID]);
        }

        var result = new ResetPasswordValidator().Validate(request);

        if (result.IsValid == false)
        {
            throw new ErrorOnValidationException(result.Errors.Select(e => e.ErrorMessage).ToList());
        }
    }
}
