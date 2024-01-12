using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterDadosPaginadosComFiltrosQueryHandler : IRequestHandler<ObterDadosPaginadosComFiltrosQuery, PaginacaoResultadoDTO<DadosListagemFormacaoComTurmaDTO>>
    {
        private readonly IRepositorioInscricao _repositorioInscricao;

        public ObterDadosPaginadosComFiltrosQueryHandler(IRepositorioInscricao repositorioInscricao, IMapper mapper)
        {
            _repositorioInscricao =
                repositorioInscricao ?? throw new ArgumentNullException(nameof(repositorioInscricao));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        private readonly IMapper _mapper;

        public async Task<PaginacaoResultadoDTO<DadosListagemFormacaoComTurmaDTO>> Handle(ObterDadosPaginadosComFiltrosQuery request, CancellationToken cancellationToken)
        {
            var retornoComTurmas = new List<DadosListagemFormacaoComTurmaDTO>();
            var totalRegistrosFiltro = await _repositorioInscricao.ObterDadosPaginadosComFiltrosTotalRegistros(request.CodigoFormacao, request.NomeFormacao);
            if (totalRegistrosFiltro > 0)
            {
                var propostasTurmas = await _repositorioInscricao.ObterDadosPaginadosComFiltros(request.CodigoFormacao, request.NomeFormacao, request.NumeroPagina, request.NumeroRegistros);

                var mapeamentoDto = _mapper.Map<IEnumerable<DadosListagemFormacaoComTurmaDTO>>(propostasTurmas);
                var codigosFormacao = propostasTurmas.Select(x => x.Id).ToArray();
                var turmas = await _repositorioInscricao.DadosListagemFormacaoComTurma(codigosFormacao);
                retornoComTurmas.AddRange(ObterTurmas(turmas, mapeamentoDto)); 
            }
            return new PaginacaoResultadoDTO<DadosListagemFormacaoComTurmaDTO>(retornoComTurmas, totalRegistrosFiltro, request.NumeroRegistros);
        }

        private IEnumerable<DadosListagemFormacaoComTurmaDTO> ObterTurmas(IEnumerable<Inscricao> inscricoes,IEnumerable<DadosListagemFormacaoComTurmaDTO> propostas)
        {

            var retorno = propostas;
            foreach (var proposta in propostas)
            {
                var inscricao = inscricoes.Where(x => x.PropostaTurma.PropostaId == proposta.Id);
                var turmas = inscricao.Select(i => new DadosListagemFormacaoTurma
                {
                    NomeTurma = i.PropostaTurma.Nome,
                    QuantidadeVagas = i.PropostaTurma.Proposta.QuantidadeVagasTurma,
                    QuantidadeInscricoes = inscricao.Where(x => x.PropostaTurma.Nome == i.PropostaTurma.Nome && i.Id > 0).Count(),
                    Data = $"{i.PropostaTurma.Proposta?.DataRealizacaoInicio!.Value:dd/MM/yyyy} até {i.PropostaTurma.Proposta?.DataRealizacaoFim!.Value:dd/MM/yyyy}"
                }).DistinctBy(x => x.NomeTurma);
                retorno.FirstOrDefault(x => x.Id == proposta.Id)!.Turmas = turmas;
            }

            return retorno;
        }
    }
}