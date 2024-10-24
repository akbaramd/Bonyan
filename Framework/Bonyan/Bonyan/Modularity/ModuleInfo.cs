using Bonyan.Modularity.Abstractions;

namespace Bonyan.Modularity;

public class ModuleInfo
{
  /// <summary>
  /// نوع ماژول (کلاس پیاده‌سازی کننده IModule)
  /// </summary>
  public Type ModuleType { get; }

  /// <summary>
  /// نمونه‌ی ماژول (IModule)
  /// </summary>
  public IModule? Instance { get; set; }

  /// <summary>
  /// لیست وابستگی‌های این ماژول
  /// </summary>
  public List<ModuleInfo> Dependencies { get; }

  /// <summary>
  /// آیا ماژول به درستی بارگذاری شده است
  /// </summary>
  public bool IsLoaded { get; set; }

  /// <summary>
  /// سازنده‌ی ModuleInfo
  /// </summary>
  /// <param name="moduleType">نوع ماژول</param>
  public ModuleInfo(Type moduleType)
  {
    ModuleType = moduleType ?? throw new ArgumentNullException(nameof(moduleType));
    Dependencies = new List<ModuleInfo>();
    IsLoaded = false;
  }
}
