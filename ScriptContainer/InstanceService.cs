namespace Core.ModelSpace
{
  /// <summary>
  /// Singleton wrapper
  /// </summary>
  public class InstanceService<T> where T: new()
  {
    private static readonly T _instance = new();

    /// <summary>
    /// Single instance
    /// </summary>
    public static T Instance => _instance;

    /// <summary>
    /// Constructor
    /// </summary>
    static InstanceService() {}

    /// <summary>
    /// Constructor
    /// </summary>
    private InstanceService() {}
  }
}
