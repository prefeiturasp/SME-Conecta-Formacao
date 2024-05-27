using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterDadosPaginadosComFiltrosQueryHandler : IRequestHandler<ObterDadosPaginadosComFiltrosQuery, PaginacaoResultadoDTO<DadosListagemFormacaoComTurmaDTO>>
    {
        private readonly IRepositorioInscricao _repositorioInscricao;
        private readonly IMapper _mapper;

        public ObterDadosPaginadosComFiltrosQueryHandler(IRepositorioInscricao repositorioInscricao, IMapper mapper)
        {
            _repositorioInscricao = repositorioInscricao ?? throw new ArgumentNullException(nameof(repositorioInscricao));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<PaginacaoResultadoDTO<DadosListagemFormacaoComTurmaDTO>> Handle(ObterDadosPaginadosComFiltrosQuery request, CancellationToken cancellationToken)
        {
            var retornoComTurmas = new List<DadosListagemFormacaoComTurmaDTO>();
            var propostaPossuiAnexo = Enumerable.Empty<InscricaoPossuiAnexoDTO>();
            var totalRegistrosFiltro = await _repositorioInscricao.ObterDadosPaginadosComFiltrosTotalRegistros(request.AreaPromotoraIdUsuarioLogado, request.CodigoFormacao, request.NomeFormacao, request.NumeroHomologacao);
            if (totalRegistrosFiltro > 0)
            {
                var propostasTurmas = await _repositorioInscricao.ObterDadosPaginadosComFiltros(request.AreaPromotoraIdUsuarioLogado, request.CodigoFormacao, request.NomeFormacao, request.NumeroPagina, request.NumeroRegistros, request.NumeroHomologacao);
                
                propostaPossuiAnexo = await _repositorioInscricao.ObterSeInscricaoPossuiAnexoPorPropostasIds(propostasTurmas.Select(x => x.Id).ToArray());
                
                var formacao = _mapper.Map<IEnumerable<DadosListagemFormacaoComTurmaDTO>>(propostasTurmas);
                var codigosFormacao = propostasTurmas.Select(x => x.Id).ToArray();

                var turmas = await _repositorioInscricao.DadosListagemFormacaoComTurma(codigosFormacao);
                var tiposInscricao = await _repositorioInscricao.ObterTiposInscricaoPorPropostaIds(codigosFormacao);


                    formacao.ToList().ForEach(item =>
                    {
                        var anexos = propostaPossuiAnexo
                            .Where(x => x.PropostaId == item.Id && !string.IsNullOrEmpty(x.NomeArquivo))
                            .Select(anexo => new DadosAnexosInscricao(anexo.NomeArquivo, anexo.Codigo))
                            .ToList();

                        item.Anexos.AddRange(anexos);
                    });

                retornoComTurmas.AddRange(MapearTurmasETipoInscricao(formacao, turmas, tiposInscricao));
            }

            return new PaginacaoResultadoDTO<DadosListagemFormacaoComTurmaDTO>(retornoComTurmas, totalRegistrosFiltro, request.NumeroRegistros);
        }

        private static IEnumerable<DadosListagemFormacaoComTurmaDTO> MapearTurmasETipoInscricao(IEnumerable<DadosListagemFormacaoComTurmaDTO> formacoes, IEnumerable<ListagemFormacaoComTurmaDTO>? turmasFormacao, IEnumerable<PropostaTipoInscricao> tipoInscricaos)
        {
            foreach (var proposta in formacoes)
            {
                var inscricao = turmasFormacao?.Where(x => x.PropostaId == proposta.Id);
                var turmas = inscricao!.Select(i => new DadosListagemFormacaoTurma
                {
                    NomeTurma = i.NomeTurma,
                    QuantidadeVagas = i.QuantidadeVagas,
                    QuantidadeInscricoes = i.TotalInscricoes,
                    Data = inscricao!.Where(x => x.NomeTurma == i.NomeTurma).Where(x => x.Datas != null).Any() ?
                           string.Join(", ", inscricao!.Where(x => x.NomeTurma == i.NomeTurma).Select(x => x.Datas)) : string.Empty
                }).DistinctBy(x => x.NomeTurma);

                proposta.Turmas = turmas;
                proposta.TiposInscricoes = tipoInscricaos.Where(t => t.PropostaId == proposta.Id).Select(s => s.TipoInscricao);
            }

            return formacoes;
        }
    }
}