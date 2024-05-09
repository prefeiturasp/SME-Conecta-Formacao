using MediatR;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Extensoes;

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

            if (perfilLogado.EhPerfilParecerista())
                await mediator.Send(new EnviarParecerPareceristaCommand(_propostaId));
            
            if (perfilLogado.EhPerfilAdminDF())
                await mediator.Send(new EnviarParecerAdminDFCommand(_propostaId));
                
            if ((await mediator.Send(new ObterPerfilAreaPromotoraQuery(perfilLogado))).NaoEhNulo())
                await mediator.Send(new EnviarParecerAreaPromotoraCommand(_propostaId));

            return true;
        }
    }
}
