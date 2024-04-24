using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarPropostaParecerCommandHandler : IRequestHandler<SalvarPropostaParecerCommand, long>
    {
        private readonly IRepositorioPropostaParecer _repositorioPropostaParecer;
        private readonly IMapper _mapper;

        public SalvarPropostaParecerCommandHandler(IMapper mapper, IRepositorioPropostaParecer repositorioPropostaParecer)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repositorioPropostaParecer = repositorioPropostaParecer ?? throw new ArgumentNullException(nameof(repositorioPropostaParecer));
        }

        public async Task<long> Handle(SalvarPropostaParecerCommand request, CancellationToken cancellationToken)
        {
            if (request.PropostaParecerDTO.Id > 0)
            {
                var alterarPropostaParecer = await _repositorioPropostaParecer.ObterPorId(request.PropostaParecerDTO.Id.Value);
                
                alterarPropostaParecer.Descricao = request.PropostaParecerDTO.Descricao;
            
                await _repositorioPropostaParecer.Atualizar(alterarPropostaParecer);
                
                return alterarPropostaParecer.Id;
            }
            
            var propostaParecer = _mapper.Map<PropostaParecer>(request.PropostaParecerDTO);

            return await _repositorioPropostaParecer.Inserir(propostaParecer);
        }
    }
}