using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterDadosPaginadosComFiltrosQueryHandler: IRequestHandler<ObterDadosPaginadosComFiltrosQuery,IEnumerable<DadosListagemFormacaoComTurma>>
    {
        private readonly IRepositorioInscricao _repositorioInscricao;

        public ObterDadosPaginadosComFiltrosQueryHandler(IRepositorioInscricao repositorioInscricao, IMapper mapper)
        {
            _repositorioInscricao =
                repositorioInscricao ?? throw new ArgumentNullException(nameof(repositorioInscricao));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        private readonly IMapper _mapper;
        public async Task<IEnumerable<DadosListagemFormacaoComTurma>> Handle(ObterDadosPaginadosComFiltrosQuery request, CancellationToken cancellationToken)
        {
            var propostasTurmas = await _repositorioInscricao.ObterDadosPaginadosComFiltros(request.UsuarioId,
                request.CodigoFormacao, request.NomeFormacao, request.NumeroPagina, request.NumeroRegistros);

            var codigosFormacao = propostasTurmas.Select(x => x.PropostaId).ToArray();
            
            return new List<DadosListagemFormacaoComTurma>();
        }
    }
}