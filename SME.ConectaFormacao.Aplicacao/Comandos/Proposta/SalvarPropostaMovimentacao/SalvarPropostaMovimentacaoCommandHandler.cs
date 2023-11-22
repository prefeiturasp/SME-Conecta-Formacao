using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarPropostaMovimentacaoCommandHandler : IRequestHandler<SalvarPropostaMovimentacaoCommand, bool>
    {
        private readonly IRepositorioPropostaMovimentacao _repositorioPropostaMovimentacao;
        private readonly IMapper _mapper;
        
        public SalvarPropostaMovimentacaoCommandHandler(IRepositorioPropostaMovimentacao repositorioPropostaMovimentacao,IMapper mapper)
        {
            _repositorioPropostaMovimentacao = repositorioPropostaMovimentacao ?? throw new ArgumentNullException(nameof(repositorioPropostaMovimentacao));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<bool> Handle(SalvarPropostaMovimentacaoCommand request, CancellationToken cancellationToken)
        {
            var propostaParecer = _mapper.Map<PropostaMovimentacao>(request.PropostaMovimentacaoDto);
            propostaParecer.PropostaId = request.PropostaId;
            
            await _repositorioPropostaMovimentacao.Inserir(propostaParecer);
            return true;
        }
    }
}