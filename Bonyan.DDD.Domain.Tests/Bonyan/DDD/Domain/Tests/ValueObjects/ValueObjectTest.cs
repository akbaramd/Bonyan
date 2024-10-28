using Bonyan.Layer.Domain.ValueObjects;
using FluentAssertions;

namespace Bonyan.DDD.Domain.Tests.ValueObjects;

public class ValueObjectTest
{
  [Fact]
  public void Equals_WithNullOther_ReturnsFalse()
  {
      var valueObject = new TestValueObject(1, "Test");
      valueObject.Equals(null).Should().BeFalse();
  }

  [Fact]
  public void Equals_WithDifferentType_ReturnsFalse()
  {
      var valueObject = new TestValueObject(1, "Test");
      var differentObject = new object();
      valueObject.Equals(differentObject).Should().BeFalse();
  }

  [Fact]
  public void Equals_WithSameTypeAndEqualComponents_ReturnsTrue()
  {
      var valueObject1 = new TestValueObject(1, "Test");
      var valueObject2 = new TestValueObject(1, "Test");
      valueObject1.Equals(valueObject2).Should().BeTrue();
  }

  [Fact]
  public void GetHashCode_WithEqualComponents_ReturnsSameHashCode()
  {
      var valueObject1 = new TestValueObject(1, "Test");
      var valueObject2 = new TestValueObject(1, "Test");
      valueObject1.GetHashCode().Should().Be(valueObject2.GetHashCode());
  }

  [Fact]
  public void ToString_ReturnsFormattedString()
  {
    var valueObject = new TestValueObject(1, "Test");
    var result = valueObject.ToString();
    result.Should().Contain("TestValueObject (");
    result.Should().Contain(": ");
  }

  [Fact]
  public void Clone_ReturnsShallowCopy()
  {
      var valueObject = new TestValueObject(1, "Test");
      var clone = valueObject.Clone();
      clone.Should().NotBeSameAs(valueObject);
      clone.Should().BeOfType(valueObject.GetType());
  }

  [Fact]
  public void GetValueComparer_WithEqualObjects_ReturnsTrue()
  {
      var valueObject1 = new TestValueObject(1, "Test");
      var valueObject2 = new TestValueObject(1, "Test");
      var comparer = ValueObject.GetValueComparer<TestValueObject>();
      comparer.Equals(valueObject1, valueObject2).Should().BeTrue();
  }

  [Fact]
  public void GetValueComparer_WithDifferentObjects_ReturnsFalse()
  {
      var valueObject1 = new TestValueObject(1, "Test");
      var valueObject2 = new TestValueObject(2, "Different");
      var comparer = ValueObject.GetValueComparer<TestValueObject>();
      comparer.Equals(valueObject1, valueObject2).Should().BeFalse();
  }
}

public class TestValueObject : ValueObject
{
  public int Id { get; }
  public string Name { get; }

  public TestValueObject(int id, string name)
  {
    Id = id;
    Name = name;
  }

  protected override IEnumerable<object> GetEqualityComponents()
  {
    yield return Id;
    yield return Name;
  }
}