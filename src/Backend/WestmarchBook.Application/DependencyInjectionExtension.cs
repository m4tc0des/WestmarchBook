using Microsoft.Extensions.DependencyInjection;
using WestmarchBook.Application.Mappings;
using WestmarchBook.Application.UseCases.Author.GetAll;
using WestmarchBook.Application.UseCases.Author.GetById;
using WestmarchBook.Application.UseCases.Author.Register;
using WestmarchBook.Application.UseCases.Book.ActiveById;
using WestmarchBook.Application.UseCases.Book.DeleteById;
using WestmarchBook.Application.UseCases.Book.Filter;
using WestmarchBook.Application.UseCases.Book.GetAll;
using WestmarchBook.Application.UseCases.Book.GetById;
using WestmarchBook.Application.UseCases.Book.InactiveById;
using WestmarchBook.Application.UseCases.Book.Register;
using WestmarchBook.Application.UseCases.Book.Update;
using WestmarchBook.Application.UseCases.Login.WithEmailAndPassword;
using WestmarchBook.Application.UseCases.Publisher.GetAll;
using WestmarchBook.Application.UseCases.Publisher.Register;
using WestmarchBook.Application.UseCases.Readers.GetAll;
using WestmarchBook.Application.UseCases.Readers.GetById;
using WestmarchBook.Application.UseCases.Readers.Register;
using WestmarchBook.Application.UseCases.Readers.Update;
using WestmarchBook.Application.UseCases.Rent.Register;
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

            services.AddScoped<IGetAuthorByIdUseCase, GetAuthorByIdUseCase>();

            services.AddScoped<IGetAllAuthorsUseCase, GetAllAuthorsUseCase>();

            services.AddScoped<IRegisterPublisherUseCase, RegisterPublisherUseCase>();

            services.AddScoped<IGetAllPublishersUseCase, GetAllPublishersUseCase>();

            services.AddScoped<IRegisterBookUseCase, RegisterBookUseCase>();

            services.AddScoped<IUpdateBookUseCase, UpdateBookUseCase>();

            services.AddScoped<IGetBookByIdUseCase, GetBookByIdUseCase>();

            services.AddScoped<IGetAllBooksUseCase, GetAllBooksUseCase>();

            services.AddScoped<IFilterBookUseCase, FilterBookUseCase>();

            services.AddScoped<IActiveBookByIdUseCase, ActiveBookByIdUseCase>();

            services.AddScoped<IInactiveBookByIdUseCase, InactiveBookByIdUseCase>();

            services.AddScoped<IDeleteBookByIdUseCase, DeleteBookByIdUseCase>();

            services.AddScoped<IRegisterReaderAccountUseCase, RegisterReaderAccountUseCase>();

            services.AddScoped<IUpdateReaderUseCase, UpdateReaderUseCase>();

            services.AddScoped<IGetAllReadersUseCase, GetAllReadersUseCase>();

            services.AddScoped<IGetReaderByIdUseCase, GetReaderByIdUseCase>();

            services.AddScoped<IRegisterRentUseCase, RegisterRentUseCase>();
        }
    }
}
