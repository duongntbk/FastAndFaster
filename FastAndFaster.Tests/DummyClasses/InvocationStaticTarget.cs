namespace FastAndFaster.Tests.DummyClasses;

public static class InvocationStaticTarget
{
    public static bool ActionParameterlessCalled { get; set; }

    public static int ActionHandleValData { get; set; }

    public static List<string> ActionHandleRefData { get; set; }

    public static int FuncParameterlessData { get; set; } = 1989;

    public static int InterfaceTestData { get; private set; }

    public static void ActionParameterless() => ActionParameterlessCalled = true;

    public static void ActionHandleVal(int first, int second) => ActionHandleValData = first + second;

    public static void ActionHandleRef(List<string> names, string name) =>
        ActionHandleRefData = new List<string>(names)
        {
            name
        };

    public static int FuncParameterless() => FuncParameterlessData;

    public static int FuncHandleVal(int first, int second) => first + second;

    public static List<string> FuncHandleRef(List<string> names, string name)
    {
        var newList = new List<string>(names)
        {
            name
        };
        return newList;
    }

    public static int FuncWithInterface(IEnumerable<string> names) => names.Count();

    public static void ActionWithInterface(IEnumerable<int> ages) => InterfaceTestData = ages.Count();
}
