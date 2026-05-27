using FluentAssertions;
using GestorTareas.Domain.Exceptions;
using GestorTareas.Domain.ValueObjects;
using Xunit;

namespace GestorTareas.Tests.Domain.ValueObjects
{
    public class EmailTests
    {   
        // CASOS DE ÉXITO

        [Fact]
        public void Create_WithValidEmail_ShouldReturnEmailWithNormalizedValue()
        {       
            var input = "Juan.Perez@Correo.COM";
            var email = Email.Create(input);
            email.Value.Should().Be("juan.perez@correo.com");
        }

        [Fact]
        public void Create_WithEmailContainingSpaces_ShouldTrimAndNormalize()
        {
            var input = "  juan@correo.com  ";
            var email = Email.Create(input);
            email.Value.Should().Be("juan@correo.com");
        }

        [Theory]
        [InlineData("juan@correo.com")]
        [InlineData("juan.perez@correo.com")]
        [InlineData("juan+filtro@correo.es")]
        [InlineData("usuario_123@dominio.org")]
        [InlineData("a@b.co")]
        public void Create_WithValidEmailFormats_ShouldSucceed(string input)
        {
            var action = () => Email.Create(input);
            action.Should().NotThrow();
        }

        [Fact]
        public void TwoEmails_WithSameNormalizedValue_ShouldBeEqual()
        {
            var email1 = Email.Create("Juan@Correo.com");
            var email2 = Email.Create("juan@correo.com");

            email1.Should().Be(email2);
            (email1 == email2).Should().BeTrue();
        }


        // CASOS DE ERROR
        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Create_WithEmptyOrNullValue_ShouldThrowInvalidEmailException(string? input)
        {
            var action = () => Email.Create(input!);
            action.Should().Throw<InvalidEmailException>().WithMessage("*vacío*");
        }

        [Theory]
        [InlineData("sin-arroba")]
        [InlineData("doble@@arroba.com")]
        [InlineData("sin-dominio@")]
        [InlineData("@sin-local.com")]
        [InlineData("espacios en@correo.com")]
        [InlineData("sin-tld@correo")]
        public void Create_WithInvalidFormat_ShouldThrowInvalidEmailException(string input)
        {
            var action = () => Email.Create(input);
            action.Should().Throw<InvalidEmailException>().WithMessage("*formato válido*");
        }

        [Fact]
        public void Create_WithEmailExceedingMaxLength_ShouldThrowInvalidEmailException()
        {
            var longLocalPart = new string('a', 250);
            var input = $"{longLocalPart}@correo.com";

            var action = () => Email.Create(input);
            action.Should().Throw<InvalidEmailException>();
        }


        // CONVERSIONES
        [Fact]
        public void ImplicitConversion_ToString_ShouldReturnValue()
        {
            var email = Email.Create("juan@correo.com");
            string raw = email;
            raw.Should().Be("juan@correo.com");
        }

        [Fact]
        public void ToString_ShouldReturnValue()
        {
            var email = Email.Create("juan@correo.com");
            var result = email.ToString();
            result.Should().Be("juan@correo.com");
        }
    }
}
