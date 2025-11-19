using MediatR;
using SME.ConectaFormacao.Aplicacao.Comandos.Inscricoes.CancelarInscricao;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricao
{
    public class CasoDeUsoCancelarInscricoes(IMediator mediator) : CasoDeUsoAbstrato(mediator), ICasoDeUsoCancelarInscricoes
    {
        public async Task<RetornoDTO> Executar(long[] ids, string? motivo)
        {
            var erros = new List<string>();
            foreach (long id in ids.OrderBy(o => o))
            {
                try
                {
                    await mediator.Send(new CancelarInscricaoCommand(id, motivo));
                }
                catch (NegocioException ex)
                {
                    erros.AddRange(ex.Mensagens);
                }
            }

            if (ids.Length == 1 && erros.Count != 0)
                throw new NegocioException(erros);

            if (erros.Count != 0)
                return RetornoDTO.RetornarSucesso(MensagemNegocio.INSCRICOES_CANCELADAS_COM_INCONSISTENCIAS);

            return RetornoDTO.RetornarSucesso(MensagemNegocio.INSCRICOES_CANCELADAS_COM_SUCESSO);
        }
    }
}
