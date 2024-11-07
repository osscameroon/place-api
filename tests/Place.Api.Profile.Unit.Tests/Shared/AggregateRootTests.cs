using System;
using FluentAssertions;
using Place.Api.Profile.Shared.Domain;

namespace Place.Api.Profile.Unit.Tests.Shared;

[Trait("Category", "Unit")]
[Trait("Class", "AggregateRoot")]
public class AggregateRootTests
{
    [Fact]
    [Trait("Category", "Unit")]
    public void Constructor_WithValidId_ShouldCreateInstance()
    {
        // Arrange
        Guid id = Guid.NewGuid();

        // Act
        TestAggregateRoot aggregateRoot = new(id);

        // Assert
        aggregateRoot.Should().NotBeNull();
        aggregateRoot.Id.Should().Be(id);
        aggregateRoot.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Constructor_WithEmptyGuid_ShouldThrowException()
    {
        // Act
        Func<TestAggregateRoot> action = () => new TestAggregateRoot(Guid.Empty);

        // Assert
        action.Should().Throw<ArgumentException>().WithMessage("*empty*");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ParameterlessConstructor_ShouldCreateInstance()
    {
        // Act
        TestAggregateRoot aggregateRoot = new();

        // Assert
        aggregateRoot.Should().NotBeNull();
        aggregateRoot.Id.Should().Be(Guid.Empty);
        aggregateRoot.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void AddDomainEvent_ShouldAddEventToCollection()
    {
        // Arrange
        TestAggregateRoot aggregateRoot = new(Guid.NewGuid());
        TestDomainEvent domainEvent = new();

        // Act
        aggregateRoot.AddDomainEvent(domainEvent);

        // Assert
        aggregateRoot.DomainEvents.Should().HaveCount(1);
        aggregateRoot.DomainEvents.Should().Contain(domainEvent);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void AddDomainEvent_MultipleTimes_ShouldAddAllEvents()
    {
        // Arrange
        TestAggregateRoot aggregateRoot = new(Guid.NewGuid());
        TestDomainEvent domainEvent1 = new();
        TestDomainEvent domainEvent2 = new();
        TestDomainEvent domainEvent3 = new();

        // Act
        aggregateRoot.AddDomainEvent(domainEvent1);
        aggregateRoot.AddDomainEvent(domainEvent2);
        aggregateRoot.AddDomainEvent(domainEvent3);

        // Assert
        aggregateRoot.DomainEvents.Should().HaveCount(3);
        aggregateRoot.DomainEvents.Should().Contain([domainEvent1, domainEvent2, domainEvent3]);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void DomainEvents_ShouldBeReadOnly()
    {
        // Arrange
        TestAggregateRoot aggregateRoot = new(Guid.NewGuid());
        TestDomainEvent domainEvent = new();
        aggregateRoot.AddDomainEvent(domainEvent);

        // Act & Assert
        aggregateRoot
            .DomainEvents.Should()
            .BeOfType<System.Collections.ObjectModel.ReadOnlyCollection<IDomainEvent>>();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ClearDomainEvents_ShouldRemoveAllEvents()
    {
        // Arrange
        TestAggregateRoot aggregateRoot = new(Guid.NewGuid());
        TestDomainEvent domainEvent1 = new();
        TestDomainEvent domainEvent2 = new();
        aggregateRoot.AddDomainEvent(domainEvent1);
        aggregateRoot.AddDomainEvent(domainEvent2);

        // Act
        aggregateRoot.ClearDomainEvents();

        // Assert
        aggregateRoot.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void AddDomainEvent_AfterClear_ShouldAddNewEvents()
    {
        // Arrange
        TestAggregateRoot aggregateRoot = new(Guid.NewGuid());
        TestDomainEvent domainEvent1 = new();
        TestDomainEvent domainEvent2 = new();
        aggregateRoot.AddDomainEvent(domainEvent1);
        aggregateRoot.ClearDomainEvents();

        // Act
        aggregateRoot.AddDomainEvent(domainEvent2);

        // Assert
        aggregateRoot.DomainEvents.Should().HaveCount(1);
        aggregateRoot.DomainEvents.Should().Contain(domainEvent2);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void DomainEvents_ShouldMaintainInsertionOrder()
    {
        // Arrange
        TestAggregateRoot aggregateRoot = new(Guid.NewGuid());
        TestDomainEvent[] events = { new(), new(), new() };

        // Act
        foreach (TestDomainEvent evt in events)
        {
            aggregateRoot.AddDomainEvent(evt);
        }

        // Assert
        aggregateRoot.DomainEvents.Should().ContainInOrder(events);
    }

    private class TestAggregateRoot : AggregateRoot
    {
        public TestAggregateRoot(Guid id)
            : base(id) { }

        public TestAggregateRoot()
            : base() { }
    }

    private class TestDomainEvent : IDomainEvent { }
}
