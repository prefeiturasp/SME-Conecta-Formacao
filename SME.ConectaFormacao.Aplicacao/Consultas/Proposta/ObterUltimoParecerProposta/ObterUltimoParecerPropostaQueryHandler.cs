using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterUltimoParecerPropostaQueryHandler : IRequestHandler<ObterUltimoParecerPropostaQuery, PropostaMovimentacaoDTO>
    {
        private readonly IMapper _mapper;
        private readonly IRepositorioPropostaMovimentacao _repositorioPropostaMovimentacao;

        public ObterUltimoParecerPropostaQueryHandler(IMapper mapper, IRepositorioPropostaMovimentacao repositorioPropostaMovimentacao)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repositorioPropostaMovimentacao = repositorioPropostaMovimentacao ?? throw new ArgumentNullException(nameof(repositorioPropostaMovimentacao));
        }

        public async Task<PropostaMovimentacaoDTO> Handle(ObterUltimoParecerPropostaQuery request, CancellationToken cancellationToken)
        {
            var parecer = await _repositorioPropostaMovimentacao.ObterUltimoParecerPropostaId(request.PropostaId);
            return _mapper.Map<PropostaMovimentacaoDTO>(parecer);
        }
    }
}
