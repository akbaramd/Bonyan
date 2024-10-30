using Bonyan.Collections;

namespace Bonyan.Validation;

public class BonyanValidationOptions
{
    public List<Type> IgnoredTypes { get; }

    public ITypeList<IObjectValidationContributor> ObjectValidationContributors { get; set; }

    public BonyanValidationOptions()
    {
        IgnoredTypes = new List<Type>();
        ObjectValidationContributors = new TypeList<IObjectValidationContributor>();
    }
}
