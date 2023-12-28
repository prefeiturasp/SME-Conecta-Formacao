using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPropostaTurmasComVagasPorIdQueryHandler : IRequestHandler<ObterPropostaTurmasComVagasPorIdQuery, IEnumerable<RetornoListagemDTO>>
    {
        private readonly IRepositorioProposta _repositorioProposta;
        private readonly IMapper _mapper;

        public ObterPropostaTurmasComVagasPorIdQueryHandler(IRepositorioProposta repositorioProposta, IMapper mapper)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<RetornoListagemDTO>> Handle(ObterPropostaTurmasComVagasPorIdQuery request, CancellationToken cancellationToken)
        {
            var proposta = await _repositorioProposta.ObterPorId(request.PropostaId) ??
                throw new NegocioException(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA, System.Net.HttpStatusCode.NotFound);

            IEnumerable<PropostaTurma> turmas;
            if (proposta.Situacao == Dominio.Enumerados.SituacaoProposta.Publicada && proposta.FormacaoHomologada == Dominio.Enumerados.FormacaoHomologada.Sim)
                turmas = await _repositorioProposta.ObterTurmasPorId(proposta.Id);
            else
            {
                turmas = await _repositorioProposta.ObterTurmasComVagaPorId(request.PropostaId);
                if (turmas.NaoPossuiElementos())
                    throw new NegocioException(MensagemNegocio.NENHUMA_TURMA_COM_VAGA_DISPONIVEL, System.Net.HttpStatusCode.NotFound);
            }

            return _mapper.Map<IEnumerable<RetornoListagemDTO>>(turmas);
        }
    }
}
