using BankMore.Account.Domain.Utils;

namespace BankMore.Account.Tests.Domain.Utils
{
    [TestClass]
    public class CpfUtilsTests
    {
        [TestMethod]
        [DataRow("529.982.247-25", true)]  // CPF válido formatado
        [DataRow("52998224725", true)]     // CPF válido sem formatação
        [DataRow("111.111.111-11", false)] // Todos os dígitos iguais
        [DataRow("123.456.789-00", false)] // CPF inválido
        [DataRow("", false)]                // CPF vazio
        [DataRow(null, false)]              // CPF nulo
        [DataRow("123", false)]             // CPF com menos de 11 dígitos
        public void IsValid_Should_Return_Expected_Result(string cpf, bool expected)
        {
            // Act
            var result = CpfUtils.IsValid(cpf, out var formattedCpf);

            // Assert
            Assert.AreEqual(expected, result);

            if (result)
            {
                // Quando válido, o formattedCpf deve conter apenas números
                Assert.AreEqual(11, formattedCpf.Length);
                Assert.IsTrue(long.TryParse(formattedCpf, out _));
            }
            else
            {
                // Quando inválido, formattedCpf deve ser string vazia
                Assert.AreEqual(string.Empty, formattedCpf);
            }
        }

        [TestMethod]
        public void IsValid_Should_Format_Cpf_Correctly()
        {
            // Arrange
            string cpf = "529.982.247-25";

            // Act
            var isValid = CpfUtils.IsValid(cpf, out var formattedCpf);

            // Assert
            Assert.IsTrue(isValid);
            Assert.AreEqual("52998224725", formattedCpf); // Formatação removida
        }
    }
}