using System.Buffers.Text;
using System.Security.Cryptography;
using WestmarchBook.Domain.Dtos;
using WestmarchBook.Domain.Entities;
using WestmarchBook.Domain.Enums;
using WestmarchBook.Domain.Repositories;
using WestmarchBook.Domain.Repositories.User;
using WestmarchBook.Domain.Repositories.VerificationCode;

namespace WestmarchBook.Application.UseCases.Login.WithExternalProvider;

public class LoginWithExternalProviderUseCase : ILoginWithExternalProviderUseCase
{
    private readonly IUserReadOnlyRepository _userReadOnlyRepository;
    private readonly IUserWriteOnlyRepository _userWriteOnlyRepository;
    private readonly IVerificationCodeWriteOnlyRepository _verificationCodeWriteOnlyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public LoginWithExternalProviderUseCase(IUserReadOnlyRepository userReadOnlyRepository,
        IUserWriteOnlyRepository userWriteOnlyRepository,
        IVerificationCodeWriteOnlyRepository verificationCodeWriteOnlyRepository,
         IUnitOfWork unitOfWork)
    {
        _userReadOnlyRepository = userReadOnlyRepository;
        _userWriteOnlyRepository = userWriteOnlyRepository;
        _verificationCodeWriteOnlyRepository = verificationCodeWriteOnlyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<string> Execute(ExternalLoginInfo externalLoginInfo)
    {
        var user = await _userReadOnlyRepository.GetByEmail(externalLoginInfo.Email);

        if (user is null)
        {
            user = new Domain.Entities.User
            {
                Email = externalLoginInfo.Email,
                UserName = externalLoginInfo.Name,
                Password = "-"
            };

            await _userWriteOnlyRepository.Add(user);
            await _unitOfWork.Commit();
        }
        var verificationCode = new VerificationCode
        {
            UserId = user.Id,
            Code = GenerateCode(),
            Type = VerificationCodeType.ExternalLoginExchange
        };

        await _verificationCodeWriteOnlyRepository.Replace(verificationCode);

        await _unitOfWork.Commit();

        return verificationCode.Code;
    }
    private static string GenerateCode()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);

        return Base64Url.EncodeToString(bytes);
    }
}
