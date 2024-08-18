
using Bonyan.DDD.Domain.Enumerations;

namespace Bonyan.DDD.Domain.Tests.Enumerations;

public class EnumerationTests
{
  private class TestEnumeration : Enumeration
  {
    public static readonly TestEnumeration One = new(1, "One");
    public static readonly TestEnumeration Two = new(2, "Two");

    public TestEnumeration(int id, string name) : base(id, name) { }
  }

  [Fact]
  public void CompareTo_SameId_ReturnsZero()
  {
    var one = TestEnumeration.One;
    var anotherOne = new TestEnumeration(1, "AnotherOne");

    Assert.Equal(0, one.CompareTo(anotherOne));
  }

  [Fact]
  public void ToString_ReturnsName()
  {
    var one = TestEnumeration.One;

    Assert.Equal("One", one.ToString());
  }

  [Fact]
  public void Equals_SameIdAndType_ReturnsTrue()
  {
    var one = TestEnumeration.One;
    var anotherOne = new TestEnumeration(1, "AnotherOne");

    Assert.True(one.Equals(anotherOne));
  }

  [Fact]
  public void GetHashCode_SameIdAndType_ReturnsSameHashCode()
  {
    var one = TestEnumeration.One;
    var anotherOne = new TestEnumeration(1, "AnotherOne");

    Assert.Equal(one.GetHashCode(), anotherOne.GetHashCode());
  }

  [Fact]
  public void ImplicitConversionToInt_ReturnsId()
  {
    var one = TestEnumeration.One;

    int id = one;

    Assert.Equal(1, id);
  }

  [Fact]
  public void ImplicitConversionToString_ReturnsName()
  {
    var one = TestEnumeration.One;

    string name = one;

    Assert.Equal("One", name);
  }

  [Fact]
  public void GetAll_ReturnsAllInstances()
  {
    var all = Enumeration.GetAll<TestEnumeration>();

    Assert.Contains(TestEnumeration.One, all);
    Assert.Contains(TestEnumeration.Two, all);
  }

  [Fact]
  public void FromId_ValidId_ReturnsInstance()
  {
    var one = Enumeration.FromId<TestEnumeration>(1);

    Assert.Equal(TestEnumeration.One, one);
  }

  [Fact]
  public void FromName_ValidName_ReturnsInstance()
  {
    var one = Enumeration.FromName<TestEnumeration>("One");

    Assert.Equal(TestEnumeration.One, one);
  }

  [Fact]
  public void TryParse_ValidId_ReturnsTrueAndInstance()
  {
    var result = Enumeration.TryParse<TestEnumeration>(1, out var one);

    Assert.True(result);
    Assert.Equal(TestEnumeration.One, one);
  }

  [Fact]
  public void TryParse_ValidName_ReturnsTrueAndInstance()
  {
    var result = Enumeration.TryParse<TestEnumeration>("One", out var one);

    Assert.True(result);
    Assert.Equal(TestEnumeration.One, one);
  }

  [Fact]
  public void EqualityOperator_SameIdAndType_ReturnsTrue()
  {
    var one = TestEnumeration.One;
    var anotherOne = new TestEnumeration(1, "AnotherOne");

    Assert.True(one == anotherOne);
  }

  [Fact]
  public void InequalityOperator_DifferentId_ReturnsTrue()
  {
    var one = TestEnumeration.One;
    var two = TestEnumeration.Two;

    Assert.True(one != two);
  }

  [Fact]
  public void LessThanOperator_ValidComparison_ReturnsTrue()
  {
    var one = TestEnumeration.One;
    var two = TestEnumeration.Two;

    Assert.True(one < two);
  }

  [Fact]
  public void GreaterThanOperator_ValidComparison_ReturnsTrue()
  {
    var one = TestEnumeration.One;
    var two = TestEnumeration.Two;

    Assert.True(two > one);
  }

  [Fact]
  public void LessThanOrEqualOperator_ValidComparison_ReturnsTrue()
  {
    var one = TestEnumeration.One;
    var two = TestEnumeration.Two;

    Assert.True(one <= two);
  }

  [Fact]
  public void GreaterThanOrEqualOperator_ValidComparison_ReturnsTrue()
  {
    var one = TestEnumeration.One;
    var two = TestEnumeration.Two;

    Assert.True(two >= one);
  }
}