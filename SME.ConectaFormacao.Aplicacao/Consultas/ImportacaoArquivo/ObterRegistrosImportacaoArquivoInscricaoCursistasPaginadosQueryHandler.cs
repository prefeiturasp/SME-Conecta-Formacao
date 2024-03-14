using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.ImportacaoArquivo;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterRegistrosImportacaoArquivoInscricaoCursistasPaginadosQueryHandler : IRequestHandler<ObterRegistrosImportacaoArquivoInscricaoCursistasPaginadosQuery, PaginacaoResultadoDTO<ImportacaoArquivoRegistroDTO>>
    {
        private readonly IRepositorioImportacaoArquivoRegistro _repositorioInscricaoImportacaoArquivoRegistro;
        private readonly IMapper _mapper;

        public ObterRegistrosImportacaoArquivoInscricaoCursistasPaginadosQueryHandler(IRepositorioImportacaoArquivoRegistro repositorioInscricaoImportacaoArquivoRegistro, IMapper mapper)
        {
            _repositorioInscricaoImportacaoArquivoRegistro = repositorioInscricaoImportacaoArquivoRegistro ?? throw new ArgumentNullException(nameof(repositorioInscricaoImportacaoArquivoRegistro));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<PaginacaoResultadoDTO<ImportacaoArquivoRegistroDTO>> Handle(ObterRegistrosImportacaoArquivoInscricaoCursistasPaginadosQuery request, CancellationToken cancellationToken)
        {
            var registros = new List<ImportacaoArquivoRegistroDTO>();
            var totalRegistrosFiltro = await _repositorioInscricaoImportacaoArquivoRegistro.ObterRegistrosImportacaoArquivoInscricaoCursistasPaginados(request.ImportacaoArquivoId, request.NumeroRegistros, request.NumeroPagina);
            if (totalRegistrosFiltro > 0)
            {
                var propostasTurmas = await _repositorioInscricao.ObterDadosPaginadosComFiltros(request.AreaPromotoraIdUsuarioLogado, request.CodigoFormacao, request.NomeFormacao, request.NumeroPagina, request.NumeroRegistros);

                var formacao = _mapper.Map<IEnumerable<DadosListagemFormacaoComTurmaDTO>>(propostasTurmas);
                var codigosFormacao = propostasTurmas.Select(x => x.Id).ToArray();
                var turmas = await _repositorioInscricao.DadosListagemFormacaoComTurma(codigosFormacao);
                registros.AddRange(ObterTurmas(turmas, formacao));
            }
            return new PaginacaoResultadoDTO<ImportacaoArquivoRegistroDTO>(registros, totalRegistrosFiltro, request.NumeroRegistros);
        }

        private static IEnumerable<DadosListagemFormacaoComTurmaDTO> ObterTurmas(IEnumerable<ListagemFormacaoComTurmaDTO>? turmasFormacao, IEnumerable<DadosListagemFormacaoComTurmaDTO> formacoes)
        {
            var retorno = formacoes;
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
                retorno.FirstOrDefault(x => x.Id == proposta.Id)!.Turmas = turmas;
            }

            return retorno;
        }
    }
}