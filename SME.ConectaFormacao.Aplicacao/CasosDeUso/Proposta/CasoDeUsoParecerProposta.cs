using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoParecerProposta : CasoDeUsoAbstrato, ICasoDeUsoParecerProposta
    {
        private readonly IMapper _mapper;
        
        public CasoDeUsoParecerProposta(IMediator mediator,IMapper mapper) : base(mediator)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<bool> Executar(long propostaId, ParecerPropostaDTO parecerPropostaDto)
        {
            var proposta = await mediator.Send(new ObterPropostaPorIdQuery(propostaId));

            if (proposta.EhNulo() || proposta.Excluido)
                throw new NegocioException(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA);

            proposta.Situacao = parecerPropostaDto.Situacao;
            await mediator.Send(new AlterarPropostaCommand(propostaId, _mapper.Map<PropostaDTO>(proposta)));

            return await mediator.Send(new ParecerPropostaCommand(propostaId,parecerPropostaDto));
        }
    }
}