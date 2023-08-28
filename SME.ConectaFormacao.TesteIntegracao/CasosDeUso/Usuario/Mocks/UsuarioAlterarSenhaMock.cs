using Bogus;
using SME.ConectaFormacao.Aplicacao.DTOS;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Usuario.Mocks
{
    public class UsuarioAlterarSenhaMock
    {
        public static string Login { get; set; }
        public static AlterarSenhaUsuarioDTO AlterarSenhaUsuarioDTOValido { get; set; }
        public static AlterarSenhaUsuarioDTO AlterarSenhaUsuarioDTOConfirmacaoInvalido { get; set; }
        public static AlterarSenhaUsuarioDTO AlterarSenhaUsuarioDTOSenhaAtualInvalido { get; set; }
        public static AlterarSenhaUsuarioDTO AlterarSenhaUsuarioDTOCriterioSegurancaInvalido { get; set; }

        public static void Montar()
        {
            var faker = new Faker("pt_BR");

            Login = faker.Person.FirstName;

            AlterarSenhaUsuarioDTOValido = new AlterarSenhaUsuarioDTO()
            {
                SenhaAtual = GerarSenhaCriteriosSeguranca(faker.Random, 8, 12),
                SenhaNova = GerarSenhaCriteriosSeguranca(faker.Random, 8, 12),
            };
            AlterarSenhaUsuarioDTOValido.ConfirmarSenha = AlterarSenhaUsuarioDTOValido.SenhaNova;

            AlterarSenhaUsuarioDTOConfirmacaoInvalido = new AlterarSenhaUsuarioDTO()
            {
                SenhaAtual = GerarSenhaCriteriosSeguranca(faker.Random, 8, 12),
                SenhaNova = GerarSenhaCriteriosSeguranca(faker.Random, 8, 12),
                ConfirmarSenha = GerarSenhaCriteriosSeguranca(faker.Random, 8, 12)
            };

            AlterarSenhaUsuarioDTOSenhaAtualInvalido = new AlterarSenhaUsuarioDTO()
            {
                SenhaAtual = GerarSenhaCriteriosSeguranca(faker.Random, 8, 12),
                SenhaNova = GerarSenhaCriteriosSeguranca(faker.Random, 8, 12),
            };
            AlterarSenhaUsuarioDTOSenhaAtualInvalido.ConfirmarSenha = AlterarSenhaUsuarioDTOSenhaAtualInvalido.SenhaNova;

            AlterarSenhaUsuarioDTOCriterioSegurancaInvalido = new AlterarSenhaUsuarioDTO()
            {
                SenhaAtual = GerarSenhaCriteriosSeguranca(faker.Random, 8, 12),
                SenhaNova = faker.Random.AlphaNumeric(12)
            };
            AlterarSenhaUsuarioDTOCriterioSegurancaInvalido.ConfirmarSenha = AlterarSenhaUsuarioDTOCriterioSegurancaInvalido.SenhaNova;
        }

        private static string GerarSenhaCriteriosSeguranca(Randomizer random, int minimo, int maximo)
        {
            var letraMinuscula = random.Char('a', 'z').ToString();
            var letraMaiuscula = random.Char('A', 'Z').ToString();
            var numero = random.Char('0', '9').ToString();

            var aleatorio = random.String2(minimo - 3);
            var aleatorio2 = random.String2(random.Number(0, maximo - minimo));

            var chars = (letraMaiuscula + letraMinuscula + numero + aleatorio + aleatorio2).ToArray();
            var shuffledChars = random.Shuffle(chars).ToArray();

            return new string(shuffledChars);
        }
    }
}
