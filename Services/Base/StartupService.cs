namespace QR_Generator.Services.Base;

public abstract class StartupService
{
    public static List<Type> GetChildTypes() => AppDomain.CurrentDomain.GetAssemblies()
                      .SelectMany(assembly => assembly.GetTypes())
                      .Where(type => type.IsSubclassOf(typeof(StartupService)) && !type.IsAbstract)
                      .ToList();

    public abstract void Initialize();
    public async Task InitializeAsync()
    {
        await Task.Run(Initialize);
        Console.WriteLine($"Init async: {GetType()}");
    }
}
