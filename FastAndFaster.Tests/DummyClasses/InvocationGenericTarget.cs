namespace FastAndFaster.Tests.DummyClasses;

public interface IPerson
{
    public List<string> Skills { get; set; }
}

public class Person : IPerson
{
    public List<string> Skills { get; set; } = new List<string>();
}

public class InvocationGenericTarget
{
    public string ActionGenericArgumentsData { get; private set; }

    public static string ActionStaticGenericData { get; private set; }

    public void ActionGenericArguments<T>(int i, T input)
    {
    }

    public void ActionGenericArguments<T1, T2>(string s, T1 input) =>
        ActionGenericArgumentsData = $"{s}_{input}_{default(T2)}";

    public static void ActionStaticGeneric<T1, T2>(T1 input1, T2 input2) =>
        ActionStaticGenericData = $"{input1}_{input2}";

    public T FuncGenericArguments<T>(int i, T input) => input;

    public string FuncGenericArguments<T1, T2>(int i, T1 input) => $"{i}:{input}_{default(T2)}";

    public T FuncReturnGeneric<T>(IEnumerable<string> skills)
        where T : IPerson, new()
    {
        var rs = new T();
        foreach (var skill in skills)
        {
            rs.Skills.Add(skill);
        }
        return rs;
    }

    public static T1 FuncStaticGeneric<T1, T2>(T2 input)
        where T1 : IPerson, new() => new T1
        {
            Skills = new List<string> { input.ToString() }
        };
}
