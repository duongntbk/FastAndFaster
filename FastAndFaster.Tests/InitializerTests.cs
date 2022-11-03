namespace FastAndFaster.Tests;

public class InitializerTests
{
    [Fact]
    public void ShouldCreateObject_ParameterlessConstructor()
    {
        // Arrange
        var assemblyQualifiedName = typeof(MultipleConstructors).AssemblyQualifiedName;
        var initializer = Initializer.Create(assemblyQualifiedName);

        // Act
        var cls = initializer(null) as MultipleConstructors;

        // Assert
        cls.Should().NotBeNull();
    }

    [Fact]
    public void ShouldCreateObject_ConstructorWithParameter()
    {
        // Arrange
        var assemblyQualifiedName = typeof(MultipleConstructors).AssemblyQualifiedName;
        var argumentTypes = new[] { typeof(int), typeof(string), typeof(List<bool>) };
        var age = 32;
        var name = "Duong";
        var bools = new List<bool> { true, false, false, true };
        var arguments = new object[] { age, name, bools };
        var initializer = Initializer.Create(assemblyQualifiedName, argumentTypes);

        // Act
        var cls = initializer(arguments) as MultipleConstructors;

        // Assert
        cls.Should().NotBeNull();
        cls.Age.Should().Be(age);
        cls.Name.Should().Be(name);
        cls.Bools.Should().BeSameAs(bools);
    }

    [Fact]
    public void ShouldCreateIndividualObjects()
    {
        // Arrange
        var assemblyQualifiedName = typeof(MultipleConstructors).AssemblyQualifiedName;
        var initializer = Initializer.Create(assemblyQualifiedName);

        // Act
        var cls1 = initializer(null) as MultipleConstructors;
        var cls2 = initializer(null) as MultipleConstructors;

        // Assert
        cls1.Should().NotBeNull();
        cls2.Should().NotBeNull();
        cls1.Should().NotBe(cls2);
    }

    [Fact]
    public void ShouldThrowException_TypeNameNotFound()
    {
        // Act
        var fakeName = "FastAndFaster.Tests.DummyClasses.FakeFake, " +
            "FastAndFaster.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";

        // Arrange
        Func<object> action = () => Initializer.Create(fakeName);

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage($"Cannot find any class with Assembly Qualified Name: {fakeName}");
    }

    [Fact]
    public void ShouldThrowException_WrongConstructorSignature()
    {
        // Arrange
        var assemblyQualifiedName = typeof(MultipleConstructors).AssemblyQualifiedName;

        // Act
        Func<object> action = () => Initializer.Create(assemblyQualifiedName, new[] { typeof(string) });

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage($"Cannot find constructor with given signature for class: {assemblyQualifiedName}");
    }

    [Fact]
    public void ShouldThrowException_NonVisibleConstructor()
    {
        // Arrange
        var assemblyQualifiedName = typeof(MultipleConstructors).AssemblyQualifiedName;
        var argumentTypes = new[] { typeof(int) };

        // Act
        Func<object> action = () => Initializer.Create(assemblyQualifiedName, argumentTypes);

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage($"Cannot find constructor with given signature for class: {assemblyQualifiedName}");
    }
}
