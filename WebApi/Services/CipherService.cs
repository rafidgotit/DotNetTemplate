using Microsoft.AspNetCore.DataProtection;

namespace WebApi.Services;

public interface ICipherService
{
    string Encrypt(string input);
    string Decrypt(string cipherText);
}

public class CipherService : ICipherService
{
    private readonly IDataProtectionProvider _dataProtectionProvider;
    private const string Key = "C1B174C5-D18A-487F-A698-714AD1E2CE15";

    public CipherService(IDataProtectionProvider dataProtectionProvider)
    {
        _dataProtectionProvider = dataProtectionProvider;
    }

    public string Encrypt(string input)
    {
        var protector = _dataProtectionProvider.CreateProtector(Key);
        return protector.Protect(input);
    }

    public string Decrypt(string cipherText)
    {
        var protector = _dataProtectionProvider.CreateProtector(Key);
        return protector.Unprotect(cipherText);
    }
}