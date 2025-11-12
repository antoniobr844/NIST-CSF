using Xunit;
using NistXGH;
using System.Runtime;
using System.Reflection;

namespace NistXGH.Tests;
public class MathTest
{
    [Fact]
    public void Soma_DeveretornarResultadoCerot()
    {
        // Given
        int a = 2;
        int b = 3;

        // When
        int resultado = a + b;
        // Then

        Assert.Equal(5, resultado);
    }
}