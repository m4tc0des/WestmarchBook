namespace WestmarchBook.Desktop.Services;

public interface IPhotoPickerService
{
    Task<Stream?> PickPhotoAsync();
}
