using MediatR;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoEnviarPropostaParecer : CasoDeUsoAbstrato, ICasoDeUsoEnviarPropostaParecer
    {
        private long _propostaId;
        public CasoDeUsoEnviarPropostaParecer(IMediator mediator) : base(mediator)
        {
        }

        public async Task<bool> Executar(long propostaId)
        {
            _propostaId = propostaId;

            var perfilLogado = await mediator.Send(new ObterGrupoUsuarioLogadoQuery());
            var funcaoPerfil = new Dictionary<Guid, Func<Task>>()
            {
                { Perfis.PARECERISTA, EnviaParecerPareceristas },
                { Perfis.ADMIN_DF, EnviaParecerAdminDF },
            };

            if (funcaoPerfil.ContainsKey(perfilLogado))
                await funcaoPerfil[perfilLogado]();

            return true;
        }

        private Task EnviaParecerPareceristas()
        {
            return mediator.Send(new EnviarParecerPareceristaCommand(_propostaId));
        }

        private Task EnviaParecerAdminDF() 
        {
            return mediator.Send(new EnviarParecerAdminDFCommand(_propostaId));
        }
    }
}
