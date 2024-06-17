using System.Security.Cryptography;
using System.Text;

namespace Domain.Entities;

public class Employee
{
    public Employee()
    {
        DynamicPartOfSalt = RandomNumberGenerator.GetHexString(128);
    }

    // Copy constructor
    public Employee(Employee copied) : this(copied.DynamicPartOfSalt, copied.Password, copied.Efficiency, copied.PremiumRate, copied.Premium, copied.SalaryRate, copied.Salary)
    {
        Login = copied.Login;
        Name = copied.Name;
        Contact = copied.Contact;
        CurrentProjectUrl = copied.CurrentProjectUrl;
        ProjectCompleted = copied.ProjectCompleted;
        PositionGuid = copied.PositionGuid;
        //PositionNavigation = copied.PositionNavigation;// Почему-то если явно устанавливать в своем коде значения виртуальным навигационным свойствам, то EF будет бросать исключение "The instance of entity type cannot be tracked" даже если получаемые сущности не отслеживаются
    }

    // Constructor for EF
    private Employee(string dynamicPartOfSalt, string password, int efficiency, decimal premiumRate, decimal premium, decimal salaryRate, decimal salary)
    {
        _password = password;
        _efficiency = efficiency;
        _premiumRate = premiumRate;
        _salaryRate = salaryRate;
        
        DynamicPartOfSalt = dynamicPartOfSalt;
        Premium = premium;
        Salary = salary;
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

    public string CurrentProjectUrl { get; set; } = null!;// Логика валидации данных должна содержаться на уровнях Application и Domain в зависимости от типа валидации. Если валидация является частью предметной логики и должна быть применена во всех приложениях, то она должна находится на уровне Domain. Также нужно понимать разницу между валидацией данных и валидацией формата - вся валидация, происходящая на уровне представления, является валидацией формата
    
    public bool ProjectCompleted { get; set; }

    public int Efficiency
    {
        get => _efficiency;
        set
        {
            _efficiency = value;
            
            CalculatePremium();
            CalculateSalary();
        }
    }

    public decimal PremiumRate
    {
        get => _premiumRate;
        set
        {
            _premiumRate = value;

            CalculatePremium();
            CalculateSalary();
        }
    }

    private void CalculatePremium()
    {
        var valueOfScale = _efficiency switch
        {
            < 65 => 65,
            > 135 => 135,
            _ => _efficiency
        };

        var premiumRateCoefficient = ValueOfScaleToCoefficient(valueOfScale);

        Premium = PremiumRate * premiumRateCoefficient;
    }
    
    public decimal Premium { get; private set; }

    public decimal SalaryRate
    {
        get => _salaryRate;
        set
        {
            _salaryRate = value;

            CalculateSalary();
        }
    }

    private void CalculateSalary() => Salary = SalaryRate + Premium;

    public decimal Salary { get; private set; }

    public string PositionGuid { get; set; } = null!;

    public virtual Position? PositionNavigation { get; set; }

    private string _password = null!;

    private int _efficiency;
    
    private decimal _premiumRate;
    
    private decimal _salaryRate;

    private const string StaticPartOfSalt = "68E76087E32C8849FB0AF7E2C68845D3F770601D72E7F6AC568225709DE19D3C814AFF290F14870982538349224A88EF97C7BF4646336CBFAD906CFA1ADA74A8";

    private static readonly Func<decimal, decimal> ValueOfScaleToCoefficient = LinearFunctionCreator.FromTwoPoint((65, 0), (135, 2));
}
