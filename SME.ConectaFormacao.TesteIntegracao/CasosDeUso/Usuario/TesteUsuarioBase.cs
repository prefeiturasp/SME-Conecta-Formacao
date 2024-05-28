using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Autenticacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Autenticacao.ServicosFakes;
using SME.ConectaFormacao.TesteIntegracao.Setup;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta
{
    public abstract class TesteUsuarioBase : TesteBase
    {
        private const string DOMINIO_EMAIL = "@edu.sme.prefeitura.sp.gov.br";
        
        protected TesteUsuarioBase(CollectionFixture collectionFixture) : base(collectionFixture)
        { }
        
        protected override void RegistrarQueryFakes(IServiceCollection services)
        {
            base.RegistrarQueryFakes(services);
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<ObterUsuarioServicoAcessosPorLoginSenhaQuery, UsuarioAutenticacaoRetornoDTO>), typeof(ObterUsuarioServicoAcessosPorLoginSenhaQueryHandlerFake), ServiceLifetime.Scoped));
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<ObterPerfisUsuarioServicoAcessosPorLoginQuery, UsuarioPerfisRetornoDTO>), typeof(ObterPerfisUsuarioServicoAcessosPorLoginQueryHandlerFake), ServiceLifetime.Scoped));
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<VincularPerfilExternoCoreSSOServicoAcessosCommand, bool>), typeof(VincularPerfilExternoCoreSSOServicoAcessosCommandHandlerFake), ServiceLifetime.Scoped));
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<ObterNomeServidorPorRfEolQuery, string>), typeof(ObterNomeServidorPorRfEolQueryHandlerFake), ServiceLifetime.Scoped));
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<ObterUsuarioLogadoQuery, Dominio.Entidades.Usuario>), typeof(ObterUsuarioLogadoQueryFake), ServiceLifetime.Scoped));
        }
        
        protected string ObterEmailEdu(UsuarioPerfisRetornoDTO retorno)
        {
            var partesNome = retorno.UsuarioNome.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var primeiroNome = partesNome.FirstOrDefault();
            var ultimoNome = partesNome.Length > 1 ? partesNome.LastOrDefault() : String.Empty;
            return (ultimoNome.EstaPreenchido()
                ? $"{primeiroNome}{ultimoNome}.{retorno.UsuarioLogin}{DOMINIO_EMAIL}"
                : $"{primeiroNome}.{retorno.UsuarioLogin}{DOMINIO_EMAIL}").ToLower();
        }
    }
}