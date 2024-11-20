using System;
using Common.Domain;
using FluentAssertions;

namespace Common.UnitTests;

[Trait("Category", "Unit")]
public sealed class AggregateRootTests
{
    [Fact]
    [Trait("Category", "Unit")]
    public void Constructor_WithValidId_ShouldInitializeId()
    {
        // Arrange
        TestAggregateId id = new(Guid.NewGuid());

        // Act
        TestAggregate aggregate = new(id);

        // Assert
        aggregate.Id.Should().Be(id);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Constructor_Default_ShouldNotSetId()
    {
        // Act
        TestAggregate aggregate = new();

        // Assert
        Action action = () =>
        {
            TestAggregateId _ = aggregate.Id;
        };
        action.Should().Throw<InvalidOperationException>().WithMessage("Id is not set");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void DomainEvents_WhenCreated_ShouldBeEmpty()
    {
        // Arrange
        TestAggregate aggregate = new(new TestAggregateId(Guid.NewGuid()));

        // Assert
        aggregate.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void AddDomainEvent_WhenEventAdded_ShouldContainEvent()
    {
        // Arrange
        TestAggregate aggregate = new(new TestAggregateId(Guid.NewGuid()));
        TestDomainEvent domainEvent = new();

        // Act
        aggregate.AddDomainEvent(domainEvent);

        // Assert
        aggregate.DomainEvents.Should().ContainSingle().Which.Should().Be(domainEvent);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void AddDomainEvent_MultipleEvents_ShouldMaintainOrder()
    {
        // Arrange
        TestAggregate aggregate = new(new TestAggregateId(Guid.NewGuid()));
        TestDomainEvent event1 = new();
        TestDomainEvent event2 = new();
        TestDomainEvent event3 = new();

        // Act
        aggregate.AddDomainEvent(event1);
        aggregate.AddDomainEvent(event2);
        aggregate.AddDomainEvent(event3);

        // Assert
        aggregate.DomainEvents.Should().HaveCount(3);
        aggregate.DomainEvents.Should().ContainInOrder(event1, event2, event3);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ClearDomainEvents_WhenCalled_ShouldRemoveAllEvents()
    {
        // Arrange
        TestAggregate aggregate = new(new TestAggregateId(Guid.NewGuid()));
        aggregate.AddDomainEvent(new TestDomainEvent());
        aggregate.AddDomainEvent(new TestDomainEvent());

        // Act
        aggregate.ClearDomainEvents();

        // Assert
        aggregate.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void DomainEvents_ShouldBeReadOnly()
    {
        // Arrange
        TestAggregate aggregate = new(new TestAggregateId(Guid.NewGuid()));
        TestDomainEvent domainEvent = new();

        // Act
        aggregate.AddDomainEvent(domainEvent);

        // Assert
        aggregate
            .DomainEvents.Should()
            .BeOfType<System.Collections.ObjectModel.ReadOnlyCollection<IDomainEvent>>();
    }

    private sealed record TestAggregateId : AggregateRootId<Guid>
    {
        public TestAggregateId(Guid value)
            : base(value) { }
    }

    private sealed class TestAggregate : AggregateRoot<TestAggregateId, Guid>
    {
        public TestAggregate(TestAggregateId id)
            : base(id) { }

        public TestAggregate() { }
    }

    private sealed class TestDomainEvent : IDomainEvent { }
}
