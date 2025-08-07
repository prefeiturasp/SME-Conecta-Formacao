using MediatR;
using SME.ConectaFormacao.Aplicacao.Comandos.Inscricao.ReativarInscricao;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricao
{
    public class CasoDeUsoReativarInscricoes : CasoDeUsoAbstrato, ICasoDeUsoReativarInscricoes
    {
        public CasoDeUsoReativarInscricoes(IMediator mediator) : base(mediator)
        {
        }

        public async Task<RetornoDTO> Executar(long[] ids)
        {
            if (ids == null || !ids.Any())
                throw new NegocioException(MensagemNegocio.INSCRICOES_REATIVADAS_COM_PROBLEMA);

            var erros = new List<string>();
            foreach (long id in ids.OrderBy(o => o))
            {
                try
                {
                    await mediator.Send(new ReativarInscricaoCommand(id));
                }
                catch (NegocioException ex)
                {
                    erros.AddRange(ex.Mensagens);
                }
            }

            if (ids.Count() == 1 && erros.Any())
                throw new NegocioException(erros);

            if (erros.Any())
                return RetornoDTO.RetornarSucesso(MensagemNegocio.INSCRICOES_REATIVADAS_COM_INCONSISTENCIAS);

            return RetornoDTO.RetornarSucesso(MensagemNegocio.INSCRICOES_REATIVACAO_CONFIRMADAS_COM_SUCESSO);
        }
    }
}
