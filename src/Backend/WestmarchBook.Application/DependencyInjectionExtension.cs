using Microsoft.Extensions.DependencyInjection;
using WestmarchBook.Application.UseCases.Login.WithEmailAndPassword;
using WestmarchBook.Application.UseCases.User.ChangePassword;
using WestmarchBook.Application.UseCases.User.Profile;
using WestmarchBook.Application.UseCases.User.Register;
using WestmarchBook.Application.UseCases.User.Update;
using WestmarchBook.Domain.Identity;

namespace WestmarchBook.Application;

public static class DependencyInjectionExtension
{
    extension(IServiceCollection services)
    {
        public void AddApplication()
        {
            services.AddUseCases();
        }
        private void AddUseCases()
        {
            services.AddScoped<IRegisterUserAccountUseCase, RegisterUserAccountUseCase>();
            services.AddScoped<ILoginWithEmailAndPasswordUseCase, LoginWithEmailAndPasswordUseCase>();
            services.AddScoped<IGetUserProfileUseCase, GetUserProfileUseCase>();
            services.AddScoped<IChangePasswordUseCase, ChangePasswordUseCase>();
            services.AddScoped<IUpdateUserUseCase, UpdateUserUseCase>();
        }
    }
}
