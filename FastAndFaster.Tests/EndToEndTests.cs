namespace FastAndFaster.Tests;

public class EndToEndTests
{
    [Fact]
    public void ShouldCreateInstanceAndCallMethod()
    {
        // Arrange, Act
        var typeName = typeof(InvocationTarget).AssemblyQualifiedName;
        var methodName = nameof(InvocationTarget.Action);
        var argumentTypes = new[] { typeof(string), typeof(int) };
        var arguments = new object[] { "DuongNT", 1989 };
        var initializer = Initializer.Create(typeName);
        var invocator = Invocator.CreateAction(typeName, methodName, argumentTypes);

        // Act
        var target = initializer(null) as InvocationTarget;
        invocator(target, arguments);

        // Assert
        target.Should().NotBeNull();
        target.ActionData.Should().Be(("DuongNT", 1989));
    }
}
