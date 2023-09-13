using Bogus;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.Mocks
{
    public class PropostaInformacoesCadastranteMock
    {
        public static Guid UsuarioLogadoGrupoId { get; set; }
        public static string UsuarioLogadoNome { get; set; }
        public static string UsuarioLogadoEmail { get; set; }

        public static void Montar()
        {
            UsuarioLogadoGrupoId = Guid.NewGuid();

            var person = new Person("pt_BR");
            UsuarioLogadoNome = person.FullName;
            UsuarioLogadoEmail = person.Email;
        }
    }
}
