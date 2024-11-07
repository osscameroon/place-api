using System;
using System.Collections.Generic;
using FluentAssertions;
using Place.Api.Profile.Shared.Domain;

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
        Guid id = Guid.NewGuid();

        // Act
        TestEntity entity = new(id);

        // Assert
        entity.Id.Should().Be(id);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Constructor_WithEmptyGuid_ShouldThrowArgumentException()
    {
        // Act
        Action action = () => new TestEntity(Guid.Empty);

        // Assert
        action.Should().Throw<ArgumentException>().WithParameterName("id").WithMessage("*empty*");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ParameterlessConstructor_ShouldInitializeWithEmptyGuid()
    {
        // Act
        TestEntity entity = new TestEntity();

        // Assert
        entity.Id.Should().Be(Guid.Empty);
    }

    [Theory]
    [Trait("Category", "Unit")]
    [MemberData(nameof(GetOperatorTestCases))]
    public void OperatorEquals_ShouldHandleAllCases(
        Entity? first,
        Entity? second,
        bool expectedResult,
        string reason
    )
    {
        // Act
        bool actualResult = first == second;
        bool notEqualResult = first! != second!;

        // Assert
        actualResult.Should().Be(expectedResult, reason);
        notEqualResult.Should().Be(!expectedResult, reason);
    }

    public static IEnumerable<object[]> GetOperatorTestCases()
    {
        Guid id1 = Guid.NewGuid();
        Guid id2 = Guid.NewGuid();

        TestEntity entity1 = new TestEntity(id1);
        TestEntity entity2 = new TestEntity(id1);
        TestEntity entity3 = new TestEntity(id2);

        return new List<object[]>
        {
            new object[] { null!, null!, true, "Two nulls should be equal" },
            new object[] { entity1, null!, false, "Entity and null should not be equal" },
            new object[] { null!, entity1, false, "Null and entity should not be equal" },
            new object[] { entity1, entity2, true, "Same ids should be equal" },
            new object[] { entity1, entity3, false, "Different ids should not be equal" },
        };
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void GetHashCode_ShouldBeConsistentWithEquality()
    {
        // Arrange
        Guid id = Guid.NewGuid();
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
        Guid id = Guid.NewGuid();
        TestEntity entity = new TestEntity(id);
        TestEntity sameEntity = new TestEntity(id);
        TestEntity differentEntity = new TestEntity(Guid.NewGuid());
        OtherTestEntity differentTypeEntity = new OtherTestEntity(id);
        TestEntity emptyIdEntity = new TestEntity();

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

    private sealed class TestEntity : Entity
    {
        public TestEntity(Guid id)
            : base(id) { }

        public TestEntity()
            : base() { }
    }

    private sealed class OtherTestEntity(Guid id) : Entity(id);
}
