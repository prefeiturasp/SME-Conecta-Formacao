using Bogus;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.TesteIntegracao.Mocks
{
    public class NotificacaoMock : BaseMock
    {

        public static IEnumerable<Notificacao> GerarNotificacoes(int quantidade)
        {
            var faker = new Faker<Notificacao>();
            faker.RuleFor(dest => dest.Titulo, f => f.Lorem.Sentence());
            faker.RuleFor(dest => dest.Mensagem, f => f.Lorem.Sentence());
            faker.RuleFor(dest => dest.Tipo, f => f.PickRandom<NotificacaoTipo>());
            faker.RuleFor(dest => dest.Categoria, f => f.PickRandom<NotificacaoCategoria>());
            faker.RuleFor(dest => dest.TipoEnvio, f => f.PickRandom<NotificacaoTipoEnvio>());

            AuditoriaFaker(faker);

            return faker.Generate(quantidade);
        }

        public static IEnumerable<NotificacaoUsuario> GetNotificacaoUsuarios(IEnumerable<Notificacao> notificacaos, string login)
        {
            var usuarios = new List<NotificacaoUsuario>();

            foreach (var notificacao in notificacaos)
            {
                var notificacaoUsuario = new NotificacaoUsuario
                {
                    NotificacaoId = notificacao.Id,
                    Login = login
                };

                Auditoria(notificacaoUsuario);

                usuarios.Add(notificacaoUsuario);
            }

            return usuarios;
        }
    }
}
