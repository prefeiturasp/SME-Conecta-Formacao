using SME.ConectaFormacao.Aplicacao.Servicos.Interface;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Infra.Servicos.Acessos;
using SME.ConectaFormacao.Infra.Servicos.Acessos.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Servicos
{
    public class ServicoPerfilUsuario : IServicoPerfilUsuario
    {
        private readonly IServicoAcessos servicoAcessos;

        public ServicoPerfilUsuario(IServicoAcessos servicoAcessos)
        {
            this.servicoAcessos = servicoAcessos ?? throw new ArgumentNullException(nameof(servicoAcessos));
        }

        public async Task<AcessosRetornoPerfilUsuario> ObterPerfisUsuario(string login)
        {
            var retorno = await servicoAcessos.ObterPerfisUsuario(login);

            if (retorno.PerfilUsuario == null)
            {
                await VincularPerfilExternoCoreSSO(login, new Guid(Constantes.PERFIL_EXTERNO_GUID));
                retorno = await ObterPerfisUsuario(login);
            }

            return retorno;
        }

        public Task<bool> VincularPerfilExternoCoreSSO(string login, Guid perfilId)
        {
            var retorno = servicoAcessos.VincularPerfilExternoCoreSSO(login, perfilId);
            return retorno;
        }
    }
}
