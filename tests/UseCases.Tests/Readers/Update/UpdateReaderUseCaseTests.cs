using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using Shouldly;
using System.Net;
using WestmarchBook.Application.UseCases.Readers.Update;
using WestmarchBook.Domain.Entities;
using WestmarchBook.Exception;
using WestmarchBook.Exception.ExceptionsBase;

namespace UseCases.Tests.Readers.Update;

public class UpdateReaderUseCaseTests
{
    [Fact]
    public async Task Success()
    {
        var reader = ReaderBuilder.Build();
        var request = RequestUpdateReaderJsonBuilder.Build();

        var useCase = CreateUseCase(reader);

        await useCase.Execute(reader.Id, request).ShouldNotThrowAsync();

        reader.FullName.ShouldBe(request.FullName);
        reader.Email.ShouldBe(request.Email);
        reader.Phone.ShouldBe(request.Phone);
        reader.DocumentNumber.ShouldBe(request.DocumentNumber);
    }

    [Fact]
    public async Task Validate_ShouldThrowException_When_ReaderNotFound()
    {
        var reader = ReaderBuilder.Build();
        var request = RequestUpdateReaderJsonBuilder.Build();

        var useCase = CreateUseCase(reader);

        var exception = await useCase.Execute(id: 1000, request).ShouldThrowAsync<NotFoundException>();

        exception.GetStatusCode().ShouldBe(HttpStatusCode.NotFound);

        exception.GetErrorMessage().ShouldSatisfyAllConditions(errorMessage =>
        {
            errorMessage.Count.ShouldBe(1);
            errorMessage.ShouldContain(ResourceMessagesException.VALIDATION_READER_NOT_FOUND);
        });
    }

    private static UpdateReaderUseCase CreateUseCase(Reader reader)
    {
        var readerReadOnlyRepository = new IReaderReadOnlyRepositoryBuilder().GetById(reader).Build();
        var readerUpdateOnlyRepository = IReaderUpdateOnlyRepositoryBuilder.Build();
        var unitOfWork = IUnitOfWorkBuilder.Build();

        return new UpdateReaderUseCase(readerReadOnlyRepository, readerUpdateOnlyRepository, unitOfWork);
    }
}
