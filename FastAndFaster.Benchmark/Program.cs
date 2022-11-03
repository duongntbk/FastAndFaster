using System;
using System.Reflection;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using FastAndFaster;
using FastAndFaster.Tests.DummyClasses;

BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);

[MemoryDiagnoser]
public class TestCreatorRunner
{
    private Type _type;
    private string _typeName;
    Func<object[], object> _delegate;

    [GlobalSetup]
    public void Setup()
    {
        _type = typeof(MultipleConstructors);
        _typeName = _type.AssemblyQualifiedName;
        _delegate = Initializer.Create(_typeName);
    }

    [Benchmark]
    public object Reflection_CreateInstance() => Activator.CreateInstance(_type);

    [Benchmark]
    public object DynamicGenerator_CreateInstance() => _delegate(null);

    [Benchmark]
    public object Static_CreateInstance() => new MultipleConstructors();
}

public class AbstractTestMethodRunner
{
    protected Type _type;
    protected string _typeName;
    protected InvocationTarget _target;
    protected string _methodName;
    protected MethodInfo _methodInfo;
    protected Type[] _argumentTypes;
    protected object[] _arguments;

    public virtual void Setup()
    {
        _type = typeof(InvocationTarget);
        _typeName = _type.AssemblyQualifiedName;
        _target = new InvocationTarget();
    }
}

[MemoryDiagnoser]
public class TestActionRunner : AbstractTestMethodRunner
{
    private Action<object, object[]> _delegate;

    [GlobalSetup]
    public override void Setup()
    {
        base.Setup();
        _methodName = nameof(InvocationTarget.Action);
        _methodInfo = _type.GetMethod(_methodName);
        _argumentTypes = new[] { typeof(string), typeof(int) };
        _arguments = new object[] { "Duong", 32 };
        _delegate = Invocator.CreateAction(_typeName, _methodName, _argumentTypes);
    }

    [Benchmark]
    public void Reflection_CallAction() => _methodInfo.Invoke(_target, _arguments);

    [Benchmark]
    public void DynamicGenerator_CallAction() => _delegate(_target, _arguments);

    [Benchmark]
    public void Static_CallAction() => _target.Action("Duong", 32);
}


[MemoryDiagnoser]
public class TestFuncRunner : AbstractTestMethodRunner
{
    private Func<object, object[], object> _delegate;

    [GlobalSetup]
    public override void Setup()
    {
        base.Setup();
        _methodName = nameof(InvocationTarget.FuncHandleVal);
        _methodInfo = _type.GetMethod(_methodName);
        _argumentTypes = new[] { typeof(int), typeof(int) };
        _arguments = new object[] { 17, 10 };
        _delegate = Invocator.CreateFunc(_typeName, _methodName, _argumentTypes);
    }

    [Benchmark]
    public object Reflection_CallFunc() => _methodInfo.Invoke(_target, _arguments);

    [Benchmark]
    public object DynamicGenerator_CallFunc() => _delegate(_target, _arguments);

    [Benchmark]
    public object Static_CallFunc() => _target.FuncHandleVal(17, 10);
}

public class AbstractTestStaticMethodRunner
{
    protected Type _type;
    protected string _typeName;
    protected string _methodName;
    protected MethodInfo _methodInfo;
    protected Type[] _argumentTypes;
    protected object[] _arguments;

    public virtual void Setup()
    {
        _type = typeof(InvocationStaticTarget);
        _typeName = _type.AssemblyQualifiedName;
    }
}

[MemoryDiagnoser]
public class TestStaticActionRunner : AbstractTestStaticMethodRunner
{
    private Action<object, object[]> _delegate;

    [GlobalSetup]
    public override void Setup()
    {
        base.Setup();
        _methodName = nameof(InvocationStaticTarget.ActionHandleVal);
        _methodInfo = _type.GetMethod(_methodName);
        _argumentTypes = new[] { typeof(int), typeof(int) };
        _arguments = new object[] { 17, 10 };
        _delegate = Invocator.CreateAction(_typeName, _methodName, _argumentTypes);
    }

    [Benchmark]
    public void Reflection_CallStaticAction() => _methodInfo.Invoke(null, _arguments);

    [Benchmark]
    public void DynamicGenerator_CallStaticAction() => _delegate(null, _arguments);

    [Benchmark]
    public void Static_CallStaticAction() => InvocationStaticTarget.ActionHandleVal(17, 10);
}

[MemoryDiagnoser]
public class TestStaticFuncRunner : AbstractTestStaticMethodRunner
{
    private Func<object, object[], object> _delegate;

    [GlobalSetup]
    public override void Setup()
    {
        base.Setup();
        _methodName = nameof(InvocationStaticTarget.FuncHandleVal);
        _methodInfo = _type.GetMethod(_methodName);
        _argumentTypes = new[] { typeof(int), typeof(int) };
        _arguments = new object[] { 17, 10 };
        _delegate = Invocator.CreateFunc(_typeName, _methodName, _argumentTypes);
    }

    [Benchmark]
    public object Reflection_CallStaticFunc() => _methodInfo.Invoke(null, _arguments);

    [Benchmark]
    public object DynamicGenerator_CallStaticFunc() => _delegate(null, _arguments);

    [Benchmark]
    public object Static_CallStaticFunc() => InvocationStaticTarget.FuncHandleVal(17, 10);
}
