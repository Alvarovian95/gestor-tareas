using FluentAssertions;
using GestorTareas.Domain.Common;
using Xunit;

namespace GestorTareas.Tests.Domain.Common;
public class EntityTests
{
    private sealed class TestEntity : Entity
    {
        public TestEntity() : base() { }
        public TestEntity(Guid id) : base(id) { }
    }

    private sealed class AnotherTestEntity : Entity
    {
        public AnotherTestEntity(Guid id) : base(id) { }
    }


    // CREACIÓN
    [Fact]
    public void Constructor_WithoutId_ShouldGenerateNewGuid()
    {
        var entity = new TestEntity();
        entity.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void Constructor_WithoutId_TwoInstances_ShouldHaveDifferentIds()
    {
        var entity1 = new TestEntity();
        var entity2 = new TestEntity();
 
        entity1.Id.Should().NotBe(entity2.Id);
    }

    [Fact]
    public void Constructor_WithSpecificId_ShouldUseIt()
    {
        var expectedId = Guid.NewGuid();
        var entity = new TestEntity(expectedId);
        entity.Id.Should().Be(expectedId);
    }

    [Fact]
    public void Constructor_WithEmptyGuid_ShouldThrowArgumentException()
    {
        var action = () => new TestEntity(Guid.Empty);
        action.Should().Throw<ArgumentException>().WithMessage("*vacío*");
    }

    // IGUALDAD
    [Fact]
    public void Equals_WithSameId_ShouldReturnTrue()
    {
        var id = Guid.NewGuid();
        var entity1 = new TestEntity(id);
        var entity2 = new TestEntity(id);
  
        entity1.Equals(entity2).Should().BeTrue();
        (entity1 == entity2).Should().BeTrue();
        (entity1 != entity2).Should().BeFalse();
    }

    [Fact]
    public void Equals_WithDifferentIds_ShouldReturnFalse()
    {
        var entity1 = new TestEntity(Guid.NewGuid());
        var entity2 = new TestEntity(Guid.NewGuid());

        entity1.Equals(entity2).Should().BeFalse();
        (entity1 == entity2).Should().BeFalse();
        (entity1 != entity2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithDifferentTypes_SameId_ShouldReturnFalse()
    {
        var id = Guid.NewGuid();
        var entity1 = new TestEntity(id);
        var entity2 = new AnotherTestEntity(id);

        entity1.Equals(entity2).Should().BeFalse();
    }

    [Fact]
    public void Equals_WithNull_ShouldReturnFalse()
    {
        var entity = new TestEntity();

        entity.Equals(null).Should().BeFalse();
    }

    [Fact]
    public void Equals_WithSameReference_ShouldReturnTrue()
    {
        var entity = new TestEntity();
        var sameReference = entity;

        entity.Equals(sameReference).Should().BeTrue();
    }


    // GETHASHCODE
    [Fact]
    public void GetHashCode_TwoEntitiesWithSameId_ShouldReturnSameHashCode()
    {
        var id = Guid.NewGuid();
        var entity1 = new TestEntity(id);
        var entity2 = new TestEntity(id);

        entity1.GetHashCode().Should().Be(entity2.GetHashCode());
    }


    // OPERADORES CON NULL
    [Fact]
    public void EqualsOperator_BothNull_ShouldReturnTrue()
    {
        TestEntity? entity1 = null;
        TestEntity? entity2 = null;

        (entity1 == entity2).Should().BeTrue();
    }

    [Fact]
    public void EqualsOperator_OneNull_ShouldReturnFalse()
    {
        var entity = new TestEntity();
        TestEntity? nullEntity = null;

        (entity == nullEntity).Should().BeFalse();
        (nullEntity == entity).Should().BeFalse();
    }
}