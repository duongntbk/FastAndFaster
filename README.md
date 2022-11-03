# A quicker way to initialize objects and call methods

The reflection API is the default way to dynamically initialize objects and call their methods at runtime. However, it is quite a bit slower than the equivalent static code. The goal of this library is to speed up such processes by utilizing dynamic code generation. It is inspired by the [FastMember](https://github.com/mgravell/fast-member) library.

For more information about this library, see the [overview](doc/OVERVIEW.md).

## Menu

- [Installation](#installation)
- [The example class](#the-example-class)
- [Initialize objects of a class](#initialize-objects-of-a-class)
- [Call instance methods](#call-instance-methods)
  - [Call methods that return void](#call-methods-that-return-void)
  - [Call methods that return results](#call-methods-that-return-results)
- [Call static methods](#call-static-methods)
- [Call generic methods](#call-generic-methods)
- [Change cache settings](#change-cache-settings)
- [Run the benchmark](#run-the-benchmark)
- [License](#license)

## Installation

This library is available as a Nuget package. Install it with the following command.
```
dotnet add package FastAndFaster
```

## The example class

We will use these classes in our examples.
```csharp
public class Person
{
    private string _name;
    private int _age;

    public Person()
    {
    }

    public Person(string name, int age)
    {
        _name = name;
        _age = age;
    }

    public void PrintInfo() => Console.WriteLine($"{name}:{age}");

    public void SetInfo(string, int age)
    {
        _name = name;
        _age = age;
    }

    public int ReturnOne() => 1;

    public int PlusOne(int i) => i + 1;

    public static void SetInfoStatic(string, int age)
    {
        _name = name;
        _age = age;
    }

    public static int PlusOneStatic(int i) => i + 1;

    public T1 GenericFunc(string s, T2 input)
        where T1: ISkill, new()
    {
        var rs = new T1();
        foreach (var skill in skills)
        {
            rs.Skills.add(skill);
        }
    }
}

public interface ISkill
{
    public List<string> Skills { get; set; }
}

public class Skill : ISkill
{
    public List<string> Skills { get; set; } = new List<string>();
}
```

## Initialize objects of a class

**Note**: the target class must have at least one public constructor in order to use this library.

The simplest use case is to create an instance with a parameterless public constructor.
```csharp
var typeName = typeof(Person).AssemblyQualifiedName; // Must use assembly qualified name here
Func<object[], object> initializer = Initializer.Create(typeName);
object instance = initializer(null); // Because we are calling a parameterless constructor, we need to pass null here
```

To call a constructor which has parameter(s), we must supply a list of types. The arguments can then be passed as an array of objects.
```csharp
var parameterTypes = new[] { typeof(string), typeof(int) };
Func<object[], object> initializer = Initializer.Create(typeName, parameterTypes);

var arguments = new object[] { "Duong", 32 };
object instance = initializer(arguments);
```

## Call instance methods

**Note**: this library can only call public methods.

### Call methods that return void

We use `Invocator.CreateAction` to create the necessary delegate. We can omit the list of argument types when calling a parameterless action.
```csharp
var typeName = typeof(Person).AssemblyQualifiedName;
var methodName = nameof(Person.PrintInfo);
Action<object, object[]> invocator = Invocator.CreateAction(typeName, methodName);
```

Because we are calling an instance method, we need an instance of type `Person`. This instance can be created by `Initializer` class, or by static code. Then we can invoke the delegate created above.
```csharp
var target = new Person();
invocator(target, null); // Because PrintInfo takes no arguments, we pass null here
```

Similar to `Initializer` class, to pass arguments to the called methods, we need to provide a list of argument types when creating the delegate; and the arguments can be passed as an object array.
```csharp
var typeName = typeof(Person).AssemblyQualifiedName;
var methodName = nameof(Person.SetName);
var argumentTypes = new[] { typeof(string), typeof(int) };
Action<object, object[]> invocator = Invocator.CreateAction(typeName, methodName, argumentTypes);

var target = new Person();
var arguments = new object[] { "Duong", 32 };
invocator(target, arguments);
```

### Call methods that return results

In this case, we need to use the `Invocator.CreateFunc` method. This is how we call a parameterless method.
```csharp
var typeName = typeof(Person).AssemblyQualifiedName;
var methodName = nameof(Person.ReturnOne);
Func<object, object[], object> invocator = Invocator.CreateAction(typeName, methodName);
// Or to be explicit: Func<object, object[], object> invocator = Invocator.CreateAction(typeName, methodName, Type.EmptyTypes);
object rs = invocator(target, null);
```

And this is how we call a method that has parameter(s).
```csharp
var typeName = typeof(Person).AssemblyQualifiedName;
var methodName = nameof(Person.PlusOne);
var argumentTypes = new[] { typeof(int) };
Func<object, object[], object> invocator = Invocator.CreateAction(typeName, methodName, argumentTypes);
var arguments = new object[] { 17 };
object rs = invocator(target, arguments);
```

## Call static methods

Calling a static method is almost identical to calling an instance method. The only difference is that we don't need a `target` instance.
```csharp
var typeName = typeof(Person).AssemblyQualifiedName;
var staticActionName = nameof(Person.SetInfoStatic);
var staticFuncName = nameof(Person.PlusOneStatic);

var actionInvocator = Invocator.CreateAction(typeName, methodName, new[] { typeof(string), typeof(int) });
actionInvocator(null, new object[] { "Duong", 32 });

var funcInvocator = Invocator.CreateFunc(typeName, methodName, new[] { typeof(int) });
var rs = funcInvocator(null, new object[] { 17 });
```

Notice that we are using `null` as the target of our delegate. When calling a parameterless static method, even the argument array is not needed.
```csharp
var rs = invocator(null, null);
```

## Call generic methods

Both `Invocator.CreateAction` and `Invocator.CreateFunc` accept an optional argument of type [GenericInfo](/FastAndFaster/Helpers/GenericInfo.cs). This argument enables the invocation of generic methods. We use dynamic code generation to execute the code below.
```csharp
Person person = new Person();
Skill rs = person.GenericFunc<Skill, bool>("James Bond", true);
```

The arguments of `GenericFunc` is `(string s, T2 input)`. Because we only have one generic parameter at index 1 (the second parameter), we set `GenericInfo.GenericTypeIndex = new[] { 1 }`. And because we use `T1 == Skill, T2 == bool`, we set `GenericInfo.GenericType == new[] { typeof(Skill), typeof(bool) }`. The static code above is equivalent to:
```csharp
var typeName = typeof(Person).AssemblyQualifiedName;
var methodName = nameof(Person.GenericFunc);
var argumentTypes = new[] { typeof(string), typeof(bool) };
var arguments = new object[] { "James Bond", true };
var genericInfo = new GenericInfo
{
    GenericTypeIndex = new[] { 1 },
    GenericTypeIndex == new[] { typeof(Skill), typeof(bool) }
};

var invocator = Invocator.CreateFunc(typeName, methodName, argumentTypes, genericInfo);
var target = new Person();

var rs = invocator(target, arguments);
```

The code to call a generic method that returns void is almost identical, we only need to change `Invocator.CreateFunc` to `Invocator.CreateAction`.

## Change cache settings

To improve performance, all delegates created by `Initializer` and `Invocator` are cached. By default, a cache entry will expire 12 hours after the last time it was used. You can check and modify these settings separately for each class.
```csharp
var initCacheSettings = Initializer.SlidingExpirationInSecs; // The default value is 43,200
Initializer.SlidingExpirationInSecs = 3600 // Delegates will now expire 1 hour after their last use
var invocCacheSettings = Invocator.SlidingExpirationInSecs; // The default value is 43,200
Invocator.SlidingExpirationInSecs = 600 // Delegates will now expire 10 minutes after their last use
```

## Run the benchmark

This library comes with a benchmark project, which can be found [here](/FastAndFaster.Benchmark). Run the tests with the following command.
```
dotnet run --configuration Release
```

You should then see the following list of tests.
```
Available Benchmarks:
  #0 TestActionRunner
  #1 TestCreatorRunner
  #2 TestFuncRunner
  #3 TestStaticActionRunner
  #4 TestStaticFuncRunner
```

Then select the test you want to run by typing its name or number.
```
2
```

Or
```
TestFuncRunner
```

## License

[MIT](https://opensource.org/licenses/MIT)
