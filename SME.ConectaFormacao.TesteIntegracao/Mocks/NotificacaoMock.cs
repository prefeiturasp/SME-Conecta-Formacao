using Bogus;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.TesteIntegracao.Mocks
{
    public class NotificacaoMock : BaseMock
    {

        private static Faker<Notificacao> Gerador()
        {
            var faker = new Faker<Notificacao>();
            faker.RuleFor(dest => dest.Titulo, f => f.Lorem.Sentence());
            faker.RuleFor(dest => dest.Mensagem, f => f.Lorem.Sentence());
            faker.RuleFor(dest => dest.Tipo, f => f.PickRandom<NotificacaoTipo>());
            faker.RuleFor(dest => dest.Categoria, f => f.PickRandom<NotificacaoCategoria>());
            faker.RuleFor(dest => dest.TipoEnvio, f => f.PickRandom<NotificacaoTipoEnvio>());

            AuditoriaFaker(faker);

            return faker;
        }

        public static IEnumerable<Notificacao> GerarNotificacoes(int quantidade)
        {
            return Gerador().Generate(quantidade);
        }

        public static Notificacao GerarNotificacao()
        {
            return Gerador().Generate();
        }

        public static NotificacaoUsuario GerarNotificacaoUsuario(string login, Notificacao notificacao)
        {
            var notificacaoUsuario = new NotificacaoUsuario
            {
                NotificacaoId = notificacao.Id,
                Login = login
            };

            Auditoria(notificacaoUsuario);

            return notificacaoUsuario;
        }

        public static IEnumerable<NotificacaoUsuario> GerarNotificacaoUsuarios(string login, IEnumerable<Notificacao> notificacaos)
        {
            var usuarios = new List<NotificacaoUsuario>();

            foreach (var notificacao in notificacaos)
            {
                usuarios.Add(GerarNotificacaoUsuario(login, notificacao));
            }

            return usuarios;
        }
    }
}
