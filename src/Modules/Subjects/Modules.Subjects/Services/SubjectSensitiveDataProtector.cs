using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.DataProtection;

namespace FSH.Modules.Subjects.Services;

public interface ISubjectSensitiveDataProtector
{
    string? Protect(string? plaintext);
    string? ComputeHash(string? plaintext);
}

public sealed class SubjectSensitiveDataProtector
    : ISubjectSensitiveDataProtector
{
    private readonly IDataProtector _protector;

    public SubjectSensitiveDataProtector(IDataProtectionProvider provider)
    {
        ArgumentNullException.ThrowIfNull(provider);
        _protector = provider.CreateProtector("DNationalSystem.Subjects.SensitiveIdentifiers.v1");
    }

    public string? Protect(string? plaintext) =>
        string.IsNullOrWhiteSpace(plaintext) ? null : _protector.Protect(plaintext.Trim());

    public string? ComputeHash(string? plaintext) =>
        string.IsNullOrWhiteSpace(plaintext)
            ? null
            : Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(plaintext.Trim())));
}
