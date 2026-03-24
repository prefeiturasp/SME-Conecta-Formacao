using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Consultas.Proposta.ObterSugestoesPareceristas
{
    public class ObterSugestoesPareceristasQueryHandler : IRequestHandler<ObterSugestoesPareceristasQuery, IEnumerable<PropostaPareceristaSugestaoDTO>>
    {
        private readonly IRepositorioProposta _repositorioProposta;
        private readonly IMapper _mapper;

        public ObterSugestoesPareceristasQueryHandler(IRepositorioProposta repositorioProposta, IMapper mapper)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<PropostaPareceristaSugestaoDTO>> Handle(ObterSugestoesPareceristasQuery request, CancellationToken cancellationToken)
        {
            return _mapper.Map<IEnumerable<PropostaPareceristaSugestaoDTO>>(await _repositorioProposta.ObterSugestaoParecerPareceristas(request.PropostaId));
        }
    }
}
