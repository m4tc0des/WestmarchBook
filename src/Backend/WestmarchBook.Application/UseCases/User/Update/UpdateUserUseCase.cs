using FluentValidation.Results;
using WestmarchBook.Communication.Requests;
using WestmarchBook.Domain.Identity;
using WestmarchBook.Domain.Repositories;
using WestmarchBook.Domain.Repositories.User;
using WestmarchBook.Exception;
using WestmarchBook.Exception.ExceptionsBase;

namespace WestmarchBook.Application.UseCases.User.Update;

public class UpdateUserUseCase : IUpdateUserUseCase
{
    private readonly ILoggedUser _loggedUser;
    private readonly IUserReadOnlyRepository _userReadRepository;
    private readonly IUserUpdateOnlyRepository _userUpdateRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserUseCase(ILoggedUser loggedUser, IUserReadOnlyRepository userReadRepository, IUserUpdateOnlyRepository userUpdateRepository, IUnitOfWork unitOfWork)
    {
        _loggedUser = loggedUser;
        _userReadRepository = userReadRepository;
        _userUpdateRepository = userUpdateRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Execute(RequestUpdateUserJson request)
    {
        var loggedUser = await _loggedUser.GetProfile();

        await Validate(request, loggedUser);

        loggedUser.UserName = request.UserName;
        loggedUser.Email = request.Email;

        _userUpdateRepository.UpdateProfile(loggedUser);

        await _unitOfWork.Commit();
    }

    private async Task Validate(RequestUpdateUserJson request, Domain.Entities.User loggedUser)
    {
        var validator = new UpdateUserValidator();

        var result = validator.Validate(request);

        if (loggedUser.Email.Equals(request.Email) == false)
        {
            var userExists = await _userReadRepository.ExisteActiveUserWithEmail(request.Email);

            if (userExists)
            {
                result.Errors.Add(new ValidationFailure("email", ResourceMessagesException.VALIDATION_EMAIL_ALREADY_EXISTS));    
            }
        }

        if (result.IsValid == false)
        {
            var errorMessage = result.Errors.Select(error => error.ErrorMessage).ToList();

            throw new ErrorOnValidationException(errorMessage);
        }
    }
}
