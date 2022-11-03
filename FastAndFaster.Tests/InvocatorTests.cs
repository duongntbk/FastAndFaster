namespace FastAndFaster.Tests;

public class InvocatorTests
{
    public static IEnumerable<object[]> CallParameterlessActionData()
    {
        var target = new InvocationTarget();
        var childTarget = new ChildInvocationTarget();

        return new List<object[]>
        {
            new object[] { target, nameof(InvocationTarget.ParameterlessAction) },
            // Call a method defined in parent class
            new object[] { childTarget, nameof(ChildInvocationTarget.ParameterlessAction) }
        };
    }

    [Theory]
    [MemberData(nameof(CallParameterlessActionData))]
    public void ShouldCallAction_Parameterless(dynamic target, string methodName)
    {
        // Arrange
        var typeName = target.GetType().AssemblyQualifiedName;
        var invocator = Invocator.CreateAction(typeName, methodName);

        // Act
        invocator(target, null);

        // Assert
        Assert.True(target.ParameterlessActionCalled);
    }

    public static IEnumerable<object[]> CallActionWithParameterData()
    {
        var target = new InvocationTarget();
        var childTarget = new ChildInvocationTarget();

        return new List<object[]>
        {
            new object[]
            {
                target, target.GetType().AssemblyQualifiedName, nameof(InvocationTarget.Action)
            },
            new object[]
            {
                // Call the action via interface
                target, typeof(IInvocationTarget).AssemblyQualifiedName, nameof(InvocationTarget.Action)
            },
            new object[]
            {
                // Call a method defined in parent class
                childTarget, target.GetType().AssemblyQualifiedName, nameof(ChildInvocationTarget.Action)
            }
        };
    }

    [Theory]
    [MemberData(nameof(CallActionWithParameterData))]
    public void ShouldCallAction_HasParameter(dynamic target, string typeName, string methodName)
    {
        // Arrange
        var name = "Duong";
        var age = 32;
        var parameters = new object[] { name, age };
        var parameterTypes = new[] { typeof(string), typeof(int) };
        var invocator = Invocator.CreateAction(typeName, methodName, parameterTypes);

        // Act
        invocator(target, parameters);

        // Assert
        Assert.Equal(target.ActionData, (name, age));
    }

    public static IEnumerable<object[]> CallParameterlessFuncData()
    {
        var refExpectedData = "thai duong";
        var refTarget = new InvocationTarget { ParameterlessFuncReturnRefData = refExpectedData };
        var childRefTarget = new ChildInvocationTarget { ParameterlessFuncReturnRefData = refExpectedData };
        var valExpectedData = 17;
        var valTarget = new InvocationTarget { ParameterlessFuncReturnValData = valExpectedData };
        var childValTarget = new ChildInvocationTarget { ParameterlessFuncReturnValData = valExpectedData };

        return new List<object[]>
        {
            new object[]
            {
                refTarget, nameof(InvocationTarget.ParameterlessFuncReturnRef), refExpectedData
            },
            new object[]
            {
                valTarget, nameof(InvocationTarget.ParameterlessFuncReturnVal), valExpectedData
            },
            // In the next 2 cases, call methods defined in parent class
            new object[]
            {
                childRefTarget, nameof(ChildInvocationTarget.ParameterlessFuncReturnRef), refExpectedData
            },
            new object[]
            {
                childValTarget, nameof(ChildInvocationTarget.ParameterlessFuncReturnVal), valExpectedData
            },
        };
    }

    [Theory]
    [MemberData(nameof(CallParameterlessFuncData))]
    public void ShouldCallFunc_Parameterless(object target, string methodName, object expectedResult)
    {
        // Arrange
        var typeName = target.GetType().AssemblyQualifiedName;
        var invocator = Invocator.CreateFunc(typeName, methodName);

        // Act
        var rs = invocator(target, null);

        // Assert
        rs.Should().Be(expectedResult);
    }

    public static List<object[]> CallFuncWithParameterData()
    {
        var names = new List<string> { "Duong", "Luy" };
        var name = "Nana";
        var refArguments = new object[] { names, name };
        var refArgumentTypes = new[] { typeof(List<string>), typeof(string) };
        var refExpectedResult = new List<string> { "Duong", "Luy", "Nana" };

        var firstNumber = 17;
        var secondNumber = 10;
        var valArgumentTypes = new[] { typeof(int), typeof(int) };
        var valArguments = new object[] { firstNumber, secondNumber };
        var valExpectedResult = firstNumber + secondNumber;

        return new List<object[]>
        {
            new object[]
            {
                typeof(InvocationTarget).AssemblyQualifiedName, nameof(InvocationTarget.FuncHandleRef),
                refArgumentTypes, refArguments, refExpectedResult
            },
            new object[]
            {
                typeof(InvocationTarget).AssemblyQualifiedName, nameof(ChildInvocationTarget.FuncHandleVal),
                valArgumentTypes, valArguments, valExpectedResult
            },
            // In the next 2 cases, the functions are called via interface
            new object[]
            {
                typeof(IInvocationTarget).AssemblyQualifiedName, nameof(InvocationTarget.FuncHandleRef),
                refArgumentTypes, refArguments, refExpectedResult
            },
            new object[]
            {
                typeof(IInvocationTarget).AssemblyQualifiedName, nameof(ChildInvocationTarget.FuncHandleVal),
                valArgumentTypes, valArguments, valExpectedResult
            }
        };
    }

    [Theory]
    [MemberData(nameof(CallFuncWithParameterData))]
    public void ShouldCallFunc_WithParameter(
        string typeName, string methodName, Type[] argumentTypes, object[] arguments, object expectedResult)
    {
        // Arrange
        var target = new InvocationTarget();
        var invocator = Invocator.CreateFunc(typeName, methodName, argumentTypes);

        // Act
        var rs = invocator(target, arguments);

        // Assert
        rs.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public void ShouldCallAction_Overload()
    {
        // Arrange
        var target = new InvocationTarget();
        var typeName = target.GetType().AssemblyQualifiedName;
        var methodName = nameof(InvocationTarget.OverloadAction);
        var invocator = Invocator.CreateAction(typeName, methodName);

        // Act
        invocator(target, null);

        // Assert
        target.OverloadActionCalled.Should().BeTrue();
    }

    public static IEnumerable<object[]> CallFuncOverloadData()
    {
        var name = "Duong";
        var singleArgumentType = new[] { typeof(string) };
        var singleArgument = new object[] { name };
        var expectedResultForSingle = name;

        var firstNumber = 17;
        var secondNumber = 10;
        var multiArgumentTypes = new[] { typeof(int), typeof(int) };
        var multiArguments = new object[] { firstNumber, secondNumber };
        var expectedResultForMulti = firstNumber + secondNumber;

        return new List<object[]>
        {
            new object[]
            {
                singleArgumentType, singleArgument, expectedResultForSingle
            },
            new object[]
            {
                multiArgumentTypes, multiArguments, expectedResultForMulti
            },
        };
    }

    [Theory]
    [MemberData(nameof(CallFuncOverloadData))]
    public void ShouldCallFunc_Overload(Type[] argumentTypes, object[] arguments, object expectedResult)
    {
        // Arrange
        var target = new InvocationTarget();
        var typeName = target.GetType().AssemblyQualifiedName;
        var methodName = nameof(InvocationTarget.OverloadFunc);
        var invocator = Invocator.CreateFunc(typeName, methodName, argumentTypes);

        // Act
        var rs = invocator(target, arguments);

        // Assert
        rs.Should().Be(expectedResult);
    }

    [Fact]
    public void ShouldCallAction_Virtual()
    {
        // Arrange
        var target = new ChildInvocationTarget();
        var typeName = typeof(InvocationTarget).AssemblyQualifiedName;
        var methodName = nameof(InvocationTarget.VirtualAction);
        var argumentTypes = new[] { typeof(int), typeof(int) };
        var arguments = new object[] { 19, 89 };
        var invocator = Invocator.CreateAction(typeName, methodName, argumentTypes);

        // Act
        invocator(target, arguments);

        // Assert
        // The overrided version ChildInvocationTarget.VirtualAction should be called
        // even though the target is used as an InvocationTarget.
        target.VirtualActionCalled.Should().BeFalse();
        target.ChildVirtualActionCalled.Should().BeTrue();
    }

    public static IEnumerable<object[]> CallVirtualFuncData()
    {
        var expectedResult = "Virtual function from parent";
        var target = new InvocationTarget { VirtualFuncData = expectedResult };
        var childExpectedResult = "Virtual function from child";
        var childTarget = new ChildInvocationTarget { ChildVirtualFuncData = childExpectedResult };

        return new List<object[]>
        {
            new object[]
            {
                target, expectedResult
            },
            new object[]
            {
                // The overrided version ChildInvocationTarget.VirtualFunc should be called
                // even though the target is used as an InvocationTarget.
                childTarget, childExpectedResult
            }
        };
    }

    [Theory]
    [MemberData(nameof(CallVirtualFuncData))]
    public void ShouldCallFunc_Virtual(object target, string expectedResult)
    {
        // Arrange
        var typeName = typeof(InvocationTarget).AssemblyQualifiedName;
        var methodName = nameof(InvocationTarget.VirtualFunc);
        var invocator = Invocator.CreateFunc(typeName, methodName, Type.EmptyTypes);

        // Act
        var rs = invocator(target, null);

        // Assert
        rs.Should().Be(expectedResult);
    }

    [Fact]
    public void ShouldCallAction_Abstract()
    {
        // Arrange
        var target = new InvocationConcreteTarget();
        var typeName = typeof(InvocationAbstractTarget).AssemblyQualifiedName;
        var methodName = nameof(InvocationAbstractTarget.ActionMultipleParams);
        var parameterTypes = new[] { typeof(string), typeof(int) };
        var invocator = Invocator.CreateAction(typeName, methodName, parameterTypes);
        var arguments = new object[] { "Duong", 17 };
        var expectedResult = 22;

        // Act
        invocator(target, arguments);

        // Assert
        target.ActionMultipleParamsCalled.Should().Be(expectedResult);
    }

    [Fact]
    public void ShouldCallFunc_Abstract()
    {
        // Arrange
        InvocationAbstractTarget target = new InvocationConcreteTarget();
        var typeName = typeof(InvocationAbstractTarget).AssemblyQualifiedName;
        var methodName = nameof(InvocationAbstractTarget.FuncMultipleParams);
        var parameterTypes = new[] { typeof(string), typeof(int) };
        var invocator = Invocator.CreateFunc(typeName, methodName, parameterTypes);
        var arguments = new object[] { "Duong", 17 };
        var expectedResult = "Duong:17";

        // Act
        var rs = invocator(target, arguments);

        // Assert
        rs.Should().Be(expectedResult);
    }

    [Fact]
    public void ShouldThrowException_CreateFunc_TypeNameNotFound()
    {
        // Arrange
        var fakeTypeName = "dummy";
        var fakeMethodName = "dummy";

        // Act
        Func<object> action = () => Invocator.CreateFunc(fakeTypeName, fakeMethodName);

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage($"Cannot find any class with Assembly Qualified Name: {fakeTypeName}");
    }

    public static IEnumerable<object[]> CreateFuncIncorrectSignatureData() =>
        new List<object[]>
        {
            new object[] { "fakename", Type.EmptyTypes },
            new object[]
            {
                nameof(InvocationTarget.FuncHandleRef),
                new[] { typeof(double), typeof(long) } // Wrong types
            }
        };

    [Theory]
    [MemberData(nameof(CreateFuncIncorrectSignatureData))]
    public void ShouldThrowException_CreateFunc_IncorrectSignature(string methodName, Type[] argumentTypes)
    {
        // Arrange
        var typeName = typeof(InvocationTarget).AssemblyQualifiedName;

        // Act
        Func<object> action = () => Invocator.CreateFunc(typeName, methodName, argumentTypes);

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage($"Cannot find method [{methodName}] with the given signature");
    }

    [Fact]
    public void ShouldThrowException_CreateAction_TypeNameNotFound()
    {
        // Arrange
        var fakeTypeName = "dummy";
        var fakeMethodName = "dummy";

        // Act
        Func<object> action = () => Invocator.CreateAction(fakeTypeName, fakeMethodName);

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage($"Cannot find any class with Assembly Qualified Name: {fakeTypeName}");
    }

    public static IEnumerable<object[]> CreateActionIncorrectSignatureData() =>
        new List<object[]>
        {
            new object[] { "fakename", Type.EmptyTypes },
            new object[]
            {
                nameof(InvocationTarget.Action),
                new[] { typeof(double), typeof(long) } // Wrong type
            }
        };

    [Theory]
    [MemberData(nameof(CreateFuncIncorrectSignatureData))]
    public void ShouldThrowException_CreateAction_IncorrectSignature(string methodName, Type[] argumentTypes)
    {
        // Arrange
        var typeName = typeof(InvocationTarget).AssemblyQualifiedName;

        // Act
        Func<object> action = () => Invocator.CreateAction(typeName, methodName, argumentTypes);

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage($"Cannot find method [{methodName}] with the given signature");
    }

    [Fact]
    public void ShouldNotCallNonVisibleAction()
    {
        // Arrange
        var typeName = typeof(InvocationTarget).AssemblyQualifiedName;
        var methodName = "PrivateAction";

        // Act
        Func<object> action = () => Invocator.CreateAction(typeName, methodName);

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage($"Cannot find method [{methodName}] with the given signature");
    }

    [Fact]
    public void ShouldNotCallNonVisibleFunc()
    {
        // Arrange
        var typeName = typeof(InvocationTarget).AssemblyQualifiedName;
        var methodName = "PrivateFunc";

        // Act
        Func<object> action = () => Invocator.CreateFunc(typeName, methodName);

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage($"Cannot find method [{methodName}] with the given signature");
    }

    public static IEnumerable<object[]> InterfaceData() =>
        new List<object[]>
        {
            new object[]
            {
                nameof(InvocationTarget.InterfaceInput),
                new[] { typeof(IEnumerable<string>) },
                new object[] { new List<string> { "Duong", "Luy", "Nana" } },
                3
            },
            new object[]
            {
                nameof(InvocationTarget.InterfaceOutput),
                new[] { typeof(int) },
                new object[] { 3 },
                new List<int> { 0, 1, 2 }
            }
        };

    [Theory]
    [MemberData(nameof(InterfaceData))]
    public void ShouldCallMethod_InterfaceInputOrOutput(
        string methodName, Type[] argumentTypes, object[] arguments, object expectedRs)
    {
        // Arrange
        var typeName = typeof(InvocationTarget).AssemblyQualifiedName;
        var invocator = Invocator.CreateFunc(typeName, methodName, argumentTypes);
        var target = new InvocationTarget();

        // Act
        var rs = invocator(target, arguments);

        // Assert
        rs.Should().BeEquivalentTo(expectedRs);
    }
}
