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
            var ehAlteracao = request.PropostaParecerDTO.Id > 0;
            
            var propostaParecer = ehAlteracao ? await _repositorioPropostaParecer.ObterPorId(request.PropostaParecerDTO.Id.Value) : new PropostaParecer();
            
            propostaParecer = _mapper.Map<PropostaParecer>(request.PropostaParecerDTO);

            if (!ehAlteracao) 
                return await _repositorioPropostaParecer.Inserir(propostaParecer);
            
            await _repositorioPropostaParecer.Atualizar(propostaParecer);
            return propostaParecer.Id;
        }
    }
}