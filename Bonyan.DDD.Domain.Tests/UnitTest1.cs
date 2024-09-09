using Bonyan.DomainDrivenDesign.Domain.Entities;
using FluentAssertions;

public class UnitTest1
{
  // create test to test Entity and generic implementation
  [Fact]
  public void TestEntityTest()
  {
    // Arrange
    var entity = new TestEntity { Id = 1, Name = "Test" };

    // Act
    var keys = entity.GetKeys();

    // Assert
    keys.Should().NotBeNull();
    keys.Should().HaveCount(1);
    keys[0].Should().Be(entity.Id);
  }


  // generate TestEntity
  class TestEntity : IEntity<int>
  {
    public int Id { get; set; }
    public required string Name { get; set; }

    public object[] GetKeys()
    {
      return new object[] { Id };
    }
  }
}