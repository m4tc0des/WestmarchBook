using System.Text.RegularExpressions;
using FluentValidation;
using WestmarchBook.Exception;

namespace WestmarchBook.Application.UseCases.Shared.Validators;

public static partial class ReaderValidatorExtensions
{
    private static readonly HashSet<string> ValidTlds = new(StringComparer.OrdinalIgnoreCase)
    {
        "com", "br", "org", "net", "edu", "gov", "mil", "int",
        "io", "dev", "app", "tech", "ai", "co", "me", "info",
        "biz", "name", "pro", "tv", "cc", "ws", "mobi", "online",
        "site", "store", "xyz", "club", "cloud", "digital", "email",
        "global", "group", "live", "media", "network", "news", "one",
        "page", "plus", "services", "solutions", "space", "studio",
        "support", "systems", "team", "today", "tools", "world", "zone",
        "jus", "leg", "emp", "art", "blog", "wiki", "ind", "inf", "srv",
        "adm", "adv", "arq", "bio", "cnt", "ecn", "eng", "esp", "etc",
        "eti", "far", "fnd", "fot", "fst", "ggf", "imb", "jor", "lel",
        "mat", "med", "mus", "not", "ntr", "odo", "ppg", "psc", "psi",
        "qsl", "radio", "rec", "slg", "tmp", "trd", "tur", "vet", "vlog", "zlg",
        "us", "uk", "pt", "es", "fr", "de", "it", "ca", "au", "jp",
        "ar", "cl", "uy", "py", "mx", "ch", "nl", "be", "se", "no",
        "dk", "fi", "pl", "ie", "nz", "za", "in", "kr", "cn", "tw",
        "hk", "sg", "il", "tr", "gr", "at", "cz", "hu", "ro", "ua"
    };

    [GeneratedRegex(
        @"^(?!\.)(?!.*\.\.)[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+(?<!\.)@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?\.)+[a-zA-Z]{2,24}$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant)]
    private static partial Regex StrictEmailRegex();

    public static IRuleBuilderOptions<T, string> EmailRule<T>(this IRuleBuilderInitial<T, string> ruleBuilder)
    {
        return ruleBuilder
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage(ResourceMessagesException.VALIDATION_EMAIL_REQUIRED)
            .Must(BeAValidEmail).WithMessage(ResourceMessagesException.VALIDATION_EMAIL_INVALID);
    }

    private static bool BeAValidEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        var trimmed = email.Trim();
        if (!StrictEmailRegex().IsMatch(trimmed))
            return false;

        var atIndex = trimmed.LastIndexOf('@');
        if (atIndex <= 0 || atIndex == trimmed.Length - 1)
            return false;

        var domainParts = trimmed[(atIndex + 1)..].Split('.');
        if (domainParts.Length < 2)
            return false;

        var tld = domainParts[^1];
        if (!ValidTlds.Contains(tld))
            return false;

        if (domainParts.Length >= 3 && tld.Equals("br", StringComparison.OrdinalIgnoreCase))
        {
            var sld = domainParts[^2];
            if (!ValidTlds.Contains(sld))
                return false;
        }

        return true;
    }

    public static IRuleBuilderOptions<T, string> PhoneRule<T>(this IRuleBuilderInitial<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage(ResourceMessagesException.VALIDATION_READER_PHONE_REQUIRED)
            .Matches(@"^(\+?55\s?)?(\(?\d{2}\)?\s?)?(9\d{4}|\d{4})-?\d{4}$")
            .WithMessage(ResourceMessagesException.VALIDATION_READER_PHONE_INVALID);
    }

    public static IRuleBuilderOptions<T, string> CpfRule<T>(this IRuleBuilderInitial<T, string> ruleBuilder)
    {
        return ruleBuilder
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage(ResourceMessagesException.VALIDATION_READER_DOCUMENTNUMBER_REQUIRED)
            .Must(BeAValidCpf)
            .WithMessage(ResourceMessagesException.VALIDATION_READER_DOCUMENTNUMBER_INVALID);
    }

    private static bool BeAValidCpf(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return false;

        cpf = new string(cpf.Where(char.IsDigit).ToArray());

        if (cpf.Length != 11 || cpf.Distinct().Count() == 1)
            return false;

        int sum = 0;
        for (int i = 0; i < 9; i++)
            sum += int.Parse(cpf[i].ToString()) * (10 - i);

        int remainder = sum % 11;
        int digit1 = remainder < 2 ? 0 : 11 - remainder;

        if (int.Parse(cpf[9].ToString()) != digit1)
            return false;

        sum = 0;
        for (int i = 0; i < 10; i++)
            sum += int.Parse(cpf[i].ToString()) * (11 - i);

        remainder = sum % 11;
        int digit2 = remainder < 2 ? 0 : 11 - remainder;

        return int.Parse(cpf[10].ToString()) == digit2;
    }
}
