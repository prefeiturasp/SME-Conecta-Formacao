using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Consultas.Proposta.ObterRegentePorId;
using SME.ConectaFormacao.Aplicacao.Consultas.Proposta.ObterRegenteTurmaPorRegenteId;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Excecoes;
using System.Net;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoObterPropostaRegentePorId : CasoDeUsoAbstrato, ICasoDeUsoObterPropostaRegentePorId
    {
        private readonly IMapper _mapper;

        public CasoDeUsoObterPropostaRegentePorId(IMediator mediator, IMapper mapper) : base(mediator)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<PropostaRegenteDTO> Executar(long regenteId)
        {
            var regente = await mediator.Send(new ObterRegentePorIdQuery(regenteId)) ??
                throw new NegocioException("Registro não encontrado", HttpStatusCode.NoContent);

            var turmas = await mediator.Send(new ObterRegenteTurmaPorRegenteIdQuery(regenteId));

            var regenteDto = _mapper.Map<PropostaRegenteDTO>(regente);
            regenteDto.Turmas = _mapper.Map<IEnumerable<PropostaRegenteTurmaDTO>>(turmas);

            return regenteDto;
        }
    }
}