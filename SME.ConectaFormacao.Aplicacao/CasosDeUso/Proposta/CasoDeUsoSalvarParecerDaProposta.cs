using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoSalvarParecerDaProposta : CasoDeUsoAbstrato, ICasoDeUsoSalvarParecerDaProposta
    {
        private readonly IMapper _mapper;

        public CasoDeUsoSalvarParecerDaProposta(IMediator mediator, IMapper mapper) : base(mediator)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<bool> Executar(long propostaId, PropostaMovimentacaoDTO propostaMovimentacaoDto)
        {
            var proposta = await mediator.Send(new ObterPropostaPorIdQuery(propostaId));

            if (proposta.EhNulo() || proposta.Excluido)
                throw new NegocioException(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA);

            await mediator.Send(new AlterarSituacaoDaPropostaCommand(propostaId, propostaMovimentacaoDto.Situacao));

            return await mediator.Send(new SalvarPropostaMovimentacaoCommand(propostaId, propostaMovimentacaoDto.Situacao, propostaMovimentacaoDto.Justificativa));
        }
    }
}