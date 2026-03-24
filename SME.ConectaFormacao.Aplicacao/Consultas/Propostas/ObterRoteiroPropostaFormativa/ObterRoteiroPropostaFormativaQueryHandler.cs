using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterRoteiroPropostaFormativaQueryHandler : IRequestHandler<ObterRoteiroPropostaFormativaQuery, RoteiroPropostaFormativaDTO>
    {
        private readonly IMapper _mapper;
        private readonly IRepositorioRoteiroPropostaFormativa _repositorioRoteiroPropostaFormativa;

        public ObterRoteiroPropostaFormativaQueryHandler(IMapper mapper, IRepositorioRoteiroPropostaFormativa repositorioRoteiroPropostaFormativa)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repositorioRoteiroPropostaFormativa = repositorioRoteiroPropostaFormativa ?? throw new ArgumentNullException(nameof(repositorioRoteiroPropostaFormativa));
        }

        public async Task<RoteiroPropostaFormativaDTO> Handle(ObterRoteiroPropostaFormativaQuery request, CancellationToken cancellationToken)
        {
            var roteiroPropostaFormativa = await _repositorioRoteiroPropostaFormativa.ObterUltimoRoteiroAtivo();
            return _mapper.Map<RoteiroPropostaFormativaDTO>(roteiroPropostaFormativa);
        }
    }
}
