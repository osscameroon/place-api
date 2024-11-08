using System;
using System.Collections.Generic;
using FluentAssertions;
using Place.Api.Common.Domain;

namespace Place.Api.Profile.Unit.Tests.Shared;

[Trait("Category", "Unit")]
[Trait("Class", "Entity")]
public sealed class EntityTests
{
    [Fact]
    [Trait("Category", "Unit")]
    public void Constructor_WithValidId_ShouldInitializeId()
    {
        // Arrange
        TestId id = new TestId("test-id");

        // Act
        TestEntity entity = new TestEntity(id);

        // Assert
        entity.Id.Should().Be(id);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Constructor_WithNullId_ShouldNotThrow()
    {
        // Act
        TestEntity entity = new TestEntity();

        // Assert
        Action action = () =>
        {
            TestId _ = entity.Id;
        };
        action.Should().Throw<InvalidOperationException>().WithMessage("Id is not set");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ParameterlessConstructor_ShouldNotSetId()
    {
        // Act
        TestEntity entity = new TestEntity();

        // Assert
        Action action = () =>
        {
            TestId _ = entity.Id;
        };
        action.Should().Throw<InvalidOperationException>().WithMessage("Id is not set");
    }

    [Theory]
    [Trait("Category", "Unit")]
    [MemberData(nameof(GetOperatorTestCases))]
    public void OperatorEquals_ShouldHandleAllCases(
        Entity<TestId>? first,
        Entity<TestId>? second,
        bool expectedResult,
        string reason
    )
    {
        // Act
        bool actualResult = first == second;
        bool notEqualResult = first != second;

        // Assert
        actualResult.Should().Be(expectedResult, reason);
        notEqualResult.Should().Be(!expectedResult, reason);
    }

    public static IEnumerable<object[]> GetOperatorTestCases()
    {
        TestId id1 = new("id1");
        TestId id2 = new("id2");

        TestEntity entity1 = new(id1);
        TestEntity entity2 = new(id1);
        TestEntity entity3 = new(id2);
        TestEntity entityNoId = new();

        return new List<object[]>
        {
            new object[] { null!, null!, true, "Two nulls should be equal" },
            new object[] { entity1, null!, false, "Entity and null should not be equal" },
            new object[] { null!, entity1, false, "Null and entity should not be equal" },
            new object[] { entity1, entity2, true, "Same ids should be equal" },
            new object[] { entity1, entity3, false, "Different ids should not be equal" },
            new object[]
            {
                entityNoId,
                entity1,
                false,
                "Entity with no id should not be equal to entity with id",
            },
            new object[]
            {
                entityNoId,
                entityNoId,
                false,
                "Entities with no ids should not be equal",
            },
        };
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void GetHashCode_ShouldBeConsistentWithEquality()
    {
        // Arrange
        TestId id = new TestId("test-id");
        TestEntity entity1 = new TestEntity(id);
        TestEntity entity2 = new TestEntity(id);
        int expectedHashCode = id.GetHashCode() * 41;

        // Act & Assert
        entity1
            .GetHashCode()
            .Should()
            .Be(entity2.GetHashCode(), "Equal entities should have same hash code");
        entity1.GetHashCode().Should().Be(expectedHashCode, "Hash code should be id hash * 41");
        entity2.GetHashCode().Should().Be(expectedHashCode, "Hash code should be id hash * 41");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Equals_WithObjectParameter_ShouldHandleAllCases()
    {
        // Arrange
        TestId id = new("test-id");
        TestEntity? entity = new(id);
        TestEntity sameEntity = new(id);
        TestEntity differentEntity = new(new TestId("other-id"));
        OtherTestEntity differentTypeEntity = new(id);
        TestEntity emptyIdEntity = new();

        // Act & Assert
        entity.Equals((object)entity).Should().BeTrue("Same reference should be equal");
        entity.Equals((object)sameEntity).Should().BeTrue("Same id should be equal");
        entity
            .Equals((object)differentEntity)
            .Should()
            .BeFalse("Different ids should not be equal");
        entity
            .Equals((object)differentTypeEntity)
            .Should()
            .BeFalse("Different types should not be equal");
        entity.Equals((object)emptyIdEntity).Should().BeFalse("Empty id should not be equal");
        entity.Equals(null).Should().BeFalse("Null should not be equal");
        entity!.Equals(new object()).Should().BeFalse("Different type should not be equal");
    }

    public sealed class TestId
    {
        public string Value { get; }

        public TestId(string value)
        {
            Value = value;
        }

        public override bool Equals(object? obj)
        {
            if (obj is TestId other)
            {
                return Value == other.Value;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }

    private sealed class TestEntity : Entity<TestId>
    {
        public TestEntity(TestId id)
            : base(id) { }

        public TestEntity() { }
    }

    private sealed class OtherTestEntity : Entity<TestId>
    {
        public OtherTestEntity(TestId id)
            : base(id) { }
    }
}
