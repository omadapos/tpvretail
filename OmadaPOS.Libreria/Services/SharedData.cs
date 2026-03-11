namespace OmadaPOS.Libreria.Services;

public static class SharedData
{
    private static volatile string? weightUnit;
    public static event Action<string>? WeightUnitChanged;

    public static string? WeightUnit
    {
        get => weightUnit;
        set
        {
            if (weightUnit != value)
            {
                weightUnit = value;
                WeightUnitChanged?.Invoke(weightUnit);
            }
        }
    }
}
