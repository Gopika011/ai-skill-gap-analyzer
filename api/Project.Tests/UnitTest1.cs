namespace Project.Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        // Arrange
        int a = 5;
        int b = 5;

        // Act
        int result = a + b;

        // Assert
        Assert.Equal(10, result);
    }
}
