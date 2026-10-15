using Microsoft.Extensions.DependencyInjection;
using WestmarchBook.Application.Mappings;
using WestmarchBook.Application.UseCases.Author.Register;
using WestmarchBook.Application.UseCases.Book.Register;
using WestmarchBook.Application.UseCases.Login.WithEmailAndPassword;
using WestmarchBook.Application.UseCases.Publisher.Register;
using WestmarchBook.Application.UseCases.User.ChangePassword;
using WestmarchBook.Application.UseCases.User.Profile;
using WestmarchBook.Application.UseCases.User.Register;
using WestmarchBook.Application.UseCases.User.Update;

namespace WestmarchBook.Application;

public static class DependencyInjectionExtension
{
    extension(IServiceCollection services)
    {
        public void AddApplication()
        {
            services.AddUseCases();
            MapsterConfiguration.Configure();
        }
        private void AddUseCases()
        {
            services.AddScoped<IRegisterUserAccountUseCase, RegisterUserAccountUseCase>();

            services.AddScoped<ILoginWithEmailAndPasswordUseCase, LoginWithEmailAndPasswordUseCase>();
            services.AddScoped<IGetUserProfileUseCase, GetUserProfileUseCase>();

            services.AddScoped<IChangePasswordUseCase, ChangePasswordUseCase>();

            services.AddScoped<IUpdateUserUseCase, UpdateUserUseCase>();

            services.AddScoped<IRegisterAuthorUseCase, RegisterAuthorUseCase>();

            services.AddScoped<IRegisterPublisherUseCase, RegisterPublisherUseCase>();

            services.AddScoped<IRegisterBookUseCase, RegisterBookUseCase>();
        }
    }
}
