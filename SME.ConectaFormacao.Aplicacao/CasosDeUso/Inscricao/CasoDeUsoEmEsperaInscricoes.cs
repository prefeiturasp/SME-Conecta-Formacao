using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricao
{
    public class CasoDeUsoEmEsperaInscricoes : CasoDeUsoAbstrato, ICasoDeUsoEmEsperaInscricoes
    {
        public CasoDeUsoEmEsperaInscricoes(IMediator mediator) : base(mediator)
        {
        }

        public async Task<RetornoDTO> Executar(long[] ids)
        {
            var erros = new List<string>();
            foreach (long id in ids.OrderBy(o => o))
            {
                try
                {
                    await mediator.Send(new EmEsperaInscricaoCommand(id));
                }
                catch(NegocioException ex)
                {
                    erros.AddRange(ex.Mensagens);
                }
            }

            if (ids.Count() == 1 && erros.Any())
                throw new NegocioException(erros);

            if (erros.Any())
                return RetornoDTO.RetornarSucesso(MensagemNegocio.INSCRICOES_EM_ESPERA_COM_INCONSISTENCIAS);

            return RetornoDTO.RetornarSucesso(MensagemNegocio.INSCRICOES_EM_ESPERA_COM_SUCESSO);
        }
    }
}
