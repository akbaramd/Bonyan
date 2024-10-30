using Bonyan.Core;
using FluentAssertions;

namespace Bonyan.Tests;

public class UnitTest1
{
  // write xuni test for Check.NotNullOrEmpty
  [Fact]
  public void TestCheckNotNullOrEmpty()
  {
    // Arrange
    string name = "Test";

    // Act
    var result = Check.NotNullOrEmpty(name, nameof(name));

    // Assert
    result.Should().Be(name);
  }

}
