using Microsoft.Extensions.DependencyInjection;
using WestmarchBook.Application.UseCases.Login.WithEmailAndPassword;
using WestmarchBook.Application.UseCases.User.Register;

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
        }
    }
}
