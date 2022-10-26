using FastAndFaster.Helpers;
using FastAndFaster.Tests.DummyClasses;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace FastAndFaster.Tests
{
    public class InvocatorGenericTests
    {
        public static IEnumerable<object[]> GenericActionData() =>
            new List<object[]>
            {
                new object[]
                {
                    new[] { typeof(string), typeof(bool) },
                    new object[] {  "JamesBond", true },
                    new GenericInfo
                    {
                        GenericTypeIndex = new[] { 1 },
                        GenericType = new[] { typeof(bool), typeof(int) }
                    },
                    "JamesBond_True_0"
                },
                new object[]
                {
                    new[] { typeof(string), typeof(double) },
                    new object[] {  "JamesBond", 17.3 },
                    new GenericInfo
                    {
                        GenericTypeIndex = new[] { 1 },
                        GenericType = new[] { typeof(double), typeof(byte) }
                    },
                    "JamesBond_17.3_0"
                },
            };

        [Theory]
        [MemberData(nameof(GenericActionData))]
        public void ShouldCallGenericAction(
            Type[] argumentTypes, object[] arguments, GenericInfo genericInfo, string expectedRs)
        {
            // Arrange
            var typeName = typeof(InvocationGenericTarget).AssemblyQualifiedName;
            var methodName = nameof(InvocationGenericTarget.ActionGenericArguments);
            var invocator = Invocator.CreateAction(typeName, methodName, argumentTypes, genericInfo);
            var target = new InvocationGenericTarget();

            // Act
            invocator(target, arguments);

            // Assert
            target.ActionGenericArgumentsData.Should().Be(expectedRs);
        }

        [Fact]
        public void ShouldCallStaticGenericAction()
        {
            // Arrange
            var typeName = typeof(InvocationGenericTarget).AssemblyQualifiedName;
            var methodName = nameof(InvocationGenericTarget.ActionStaticGeneric);
            var argumentTypes = new[] { typeof(string), typeof(bool) };
            var arguments = new object[] { "JamesBond", false };
            var genericInfo = new GenericInfo
            {
                GenericTypeIndex = new[] { 0, 1 },
                GenericType = new[] { typeof(string), typeof(bool) }
            };
            var invocator = Invocator.CreateAction(typeName, methodName, argumentTypes, genericInfo);

            // Action
            invocator(null, arguments);

            // Assert
            InvocationGenericTarget.ActionStaticGenericData.Should().Be($"JamesBond_False");
        }

        public static IEnumerable<object[]> ActionWrongGenericInfoData() =>
            new List<object[]>
            {
                // Wrong generic type index
                new object[]
                {
                    new[] { typeof(string), typeof(bool) },
                    new GenericInfo
                    {
                        GenericTypeIndex = new[] { 0 },
                        GenericType = new[] { typeof(bool), typeof(int) }
                    }
                },
                // Wrong number of generic type index
                new object[]
                {
                    new[] { typeof(string), typeof(bool) },
                    new GenericInfo
                    {
                        GenericTypeIndex = new[] { 0, 1, 2 },
                        GenericType = new[] { typeof(bool), typeof(int) }
                    }
                },
                // Wrong number of generic type
                new object[]
                {
                    new[] { typeof(string), typeof(double) },
                    new GenericInfo
                    {
                        GenericTypeIndex = new[] { 1 },
                        GenericType = new[] { typeof(double), typeof(byte), typeof(bool) }
                    }
                },
                // Wrong concrete type
                new object[]
                {
                    new[] { typeof(bool), typeof(bool) },
                    new GenericInfo
                    {
                        GenericTypeIndex = new[] { 1 },
                        GenericType = new[] { typeof(bool), typeof(int) }
                    }
                }
            };

        [Theory]
        [MemberData(nameof(ActionWrongGenericInfoData))]
        public void ShouldThrowException_ActionWrongGenericInfo(Type[] argumentTypes, GenericInfo genericInfo)
        {
            // Arrange
            var typeName = typeof(InvocationGenericTarget).AssemblyQualifiedName;
            var methodName = nameof(InvocationGenericTarget.ActionGenericArguments);

            // Act
            Func<object> action = () => Invocator.CreateAction(typeName, methodName, argumentTypes, genericInfo);

            // Assert
            action.Should().Throw<ArgumentException>()
                .WithMessage($"Cannot find method [{methodName}] with the given signature");

        }

        public static IEnumerable<object[]> GenericFuncData() =>
            new List<object[]>
            {
                new object[]
                {
                    new[] { typeof(int), typeof(string) },
                    new object[] { 7, "JamesBond" },
                    new GenericInfo
                    {
                        GenericTypeIndex = new[] { 1 },
                        GenericType = new[] { typeof(string), typeof(bool) }
                    },
                    "7:JamesBond_False"
                },
                new object[]
                {
                    new[] { typeof(int), typeof(double) },
                    new object[] { 17, 19.89 },
                    new GenericInfo
                    {
                        GenericTypeIndex = new[] { 1 },
                        GenericType = new[] { typeof(double), typeof(byte) }
                    },
                    "17:19.89_0"
                }
            };

        [Theory]
        [MemberData(nameof(GenericFuncData))]
        public void ShouldCallGenericFunc(
            Type[] argumentTypes, object[] arguments, GenericInfo genericInfo, string expectedRs)
        {
            // Arrange
            var typeName = typeof(InvocationGenericTarget).AssemblyQualifiedName;
            var methodName = nameof(InvocationGenericTarget.FuncGenericArguments);
            var invocator = Invocator.CreateFunc(typeName, methodName, argumentTypes, genericInfo);
            var target = new InvocationGenericTarget();

            // Act
            var rs = invocator(target, arguments) as string;

            // Assert
            rs.Should().NotBeNull();
            rs.Should().BeEquivalentTo(expectedRs);
        }

        [Fact]
        public void ShouldCallGenericFunc_ReturnTypeGeneric()
        {
            // Arrange
            var typeName = typeof(InvocationGenericTarget).AssemblyQualifiedName;
            var methodName = nameof(InvocationGenericTarget.FuncReturnGeneric);
            var argumentTypes = new[] { typeof(IEnumerable<string>) };
            var skills = new List<string> { "C#", "Python" };
            var arguments = new object[] { skills };
            var genericInfo = new GenericInfo
            {
                GenericType = new[] { typeof(Person) }
            };
            var invocator = Invocator.CreateFunc(typeName, methodName, argumentTypes, genericInfo);
            var target = new InvocationGenericTarget();

            // Act
            var rs = invocator(target, arguments) as Person;

            // Assert
            rs.Should().NotBeNull();
            rs.Skills.Should().BeEquivalentTo(skills);
        }

        [Fact]
        public void ShouldCallStaticGenericFunc()
        {
            // Arrange
            var typeName = typeof(InvocationGenericTarget).AssemblyQualifiedName;
            var methodName = nameof(InvocationGenericTarget.FuncStaticGeneric);
            var argumentTypes = new[] { typeof(int) };
            var arguments = new object[] { 1989 };
            var genericInfo = new GenericInfo
            {
                GenericTypeIndex = new[] { 0 },
                GenericType = new[] { typeof(Person), typeof(int) }
            };
            var invocator = Invocator.CreateFunc(typeName, methodName, argumentTypes, genericInfo);

            // Act
            var rs = invocator(null, arguments) as Person;

            // Assert
            rs.Should().NotBeNull();
            rs.Skills.Single().Should().Be("1989");
        }

        public static IEnumerable<object[]> FuncWrongGenericInfoData() =>
            new List<object[]>
            {
                // Wrong generic type index
                new object[]
                {
                    new[] {  typeof(int), typeof(string) },
                    new GenericInfo
                    {
                        GenericTypeIndex = new[] { 0 },
                        GenericType = new[] { typeof(string), typeof(bool) }
                    },
                },
                // Wrong number of generic type index
                new object[]
                {
                    new[] { typeof(int), typeof(string) },
                    new GenericInfo
                    {
                        GenericTypeIndex = new[] { 0, 1 },
                        GenericType = new[] { typeof(string), typeof(bool) }
                    }
                },
                // Wrong number of generic type
                new object[]
                {
                    new[] {  typeof(int), typeof(string)  },
                    new GenericInfo
                    {
                        GenericTypeIndex = new[] { 0 },
                        GenericType = new[] { typeof(string), typeof(bool), typeof(float) }
                    },
                },
                // Wrong concrete type
                new object[]
                {
                    new[] { typeof(bool), typeof(string) },
                    new GenericInfo
                    {
                        GenericTypeIndex = new[] { 1 },
                        GenericType = new[] { typeof(string), typeof(bool) }
                    }
                }
            };

        [Theory]
        [MemberData(nameof(FuncWrongGenericInfoData))]
        public void ShouldThrowException_FuncWrongGenericInfo(Type[] argumentTypes, GenericInfo genericInfo)
        {
            // Arrange
            var typeName = typeof(InvocationGenericTarget).AssemblyQualifiedName;
            var methodName = nameof(InvocationGenericTarget.FuncGenericArguments);

            // Act
            Func<object> action = () => Invocator.CreateFunc(typeName, methodName, argumentTypes, genericInfo);

            // Assert
            action.Should().Throw<ArgumentException>()
                .WithMessage($"Cannot find method [{methodName}] with the given signature");
        }
    }
}
