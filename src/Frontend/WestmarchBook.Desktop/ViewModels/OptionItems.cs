using WestmarchBook.Communication.Enums;

namespace WestmarchBook.Desktop.ViewModels;

public sealed record EntityOptionItem(long Id, string Name)
{
    public override string ToString() => $"{Id} - {Name}";
}

public sealed record GenreOptionItem(Genre Value, string Name)
{
    public override string ToString() => Name;
}

public sealed record LanguageOptionItem(Language Value, string Name)
{
    public override string ToString() => Name;
}
