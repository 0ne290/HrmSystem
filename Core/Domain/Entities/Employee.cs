using System.Security.Cryptography;
using System.Text;

namespace Domain.Entities;

public class Employee
{
    public Employee()
    {
        DynamicPartOfSalt = RandomNumberGenerator.GetHexString(128);
    }

    // Constructor for EF
    private Employee(string dynamicPartOfSalt, string password, int efficiency, decimal salaryRate, decimal salaryRateCoefficient)
    {
        DynamicPartOfSalt = dynamicPartOfSalt;
        _password = password;
        _efficiency = efficiency;
        _salaryRate = salaryRate;
        _salaryRateCoefficient = salaryRateCoefficient;
    }

    public string DynamicPartOfSalt { get; } = null!;
    
    public string Login { get; set; } = null!;

    public string Password
    {
        get => _password;
        set => _password = Hash(value);
    }

    public string Hash(string value)
    {
        var sha256OfSaltyNewPassword = SHA512.HashData(Salt(value));
            
        var stringBuilder = new StringBuilder();
            
        foreach (var b in sha256OfSaltyNewPassword)
            stringBuilder.Append(b.ToString("x2"));

        return stringBuilder.ToString();
    }

    private byte[] Salt(string value) => Encoding.UTF8
        .GetBytes(value + value + DynamicPartOfSalt + StaticPartOfSalt + Login + Login).Select(b =>
        {
            if (b < 127)
                return (byte)(b + 77);
            return (byte)(b - 13);
        }).ToArray();

    public string Name { get; set; } = null!;

    public string Contact { get; set; } = null!;

    public string CurrentProjectUrl { get; set; } = null!;
    
    public bool ProjectCompleted { get; set; }

    public int Efficiency
    {
        get => _efficiency;
        set
        {
            
        }
    }

    public decimal PremiumRate
    {
        get => _premiumRate;
        set
        {
            
        }
    }

    public decimal SalaryRate
    {
        get => _salaryRate;
        set
        {
            
        }
    }

    public decimal Salary { get; private set; }

    public string PositionGuid { get; set; } = null!;

    public virtual Position? Position { get; set; }

    private string _password = null!;

    private int _efficiency;
    
    private decimal _premiumRate;
    
    private decimal _salaryRate;

    private const string StaticPartOfSalt = "68E76087E32C8849FB0AF7E2C68845D3F770601D72E7F6AC568225709DE19D3C814AFF290F14870982538349224A88EF97C7BF4646336CBFAD906CFA1ADA74A8";
}