using System.Security.Cryptography;
using WestmarchBook.Communication.Requests;
using WestmarchBook.Domain.Entities;
using WestmarchBook.Domain.Repositories;
using WestmarchBook.Domain.Repositories.User;
using WestmarchBook.Domain.Repositories.VerificationCode;
using WestmarchBook.Domain.Security.EmailService;

namespace WestmarchBook.Application.UseCases.PasswordRecovery.RequestCode;

public class RequestPasswordRecoveryCodeUseCase : IRequestPasswordRecoveryCodeUseCase
{
    private readonly IUserReadOnlyRepository userReadOnlyRepository;
    private readonly IVerificationCodeWriteOnlyRepository _verificationCodeRepository;
    private readonly ISendEmailService _sendEmailService;
    private readonly IUnitOfWork _unitOfWork;
    public RequestPasswordRecoveryCodeUseCase(IUserReadOnlyRepository userReadOnlyRepository,
        IVerificationCodeWriteOnlyRepository verificationCodeRepository,
        ISendEmailService sendEmailService,
        IUnitOfWork unitOfWork)
    {
        this.userReadOnlyRepository = userReadOnlyRepository;
        _verificationCodeRepository = verificationCodeRepository;
        _sendEmailService = sendEmailService;
        _unitOfWork = unitOfWork;
    }

    public async Task Execute(RequestPasswordRecoveryCodeJson request)
    {
        var user = await userReadOnlyRepository.GetByEmail(request.Email);

        if (user is null)
        {
            return;
        }

        var code = RandomNumberGenerator.GetInt32(1, 1_000_000);

        var verificationCode = new VerificationCode
        {
            Code = code.ToString("D6"),
            Type = Domain.Enums.VerificationCodeType.PasswordRecovery,
            UserId = user.Id
        };

        await _verificationCodeRepository.Replace(verificationCode);

        await _unitOfWork.Commit();

        await _sendEmailService.SendPasswordRecoveryCode(user.Email, user.UserName, verificationCode.Code);
    }
}
