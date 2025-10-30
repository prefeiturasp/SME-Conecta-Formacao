using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterInscricaoPorIdQueryHandler : IRequestHandler<ObterInscricaoPorIdQuery, PaginacaoResultadoDTO<DadosListagemInscricaoDTO>>
    {
        private readonly IRepositorioInscricao _repositorioInscricao;
        private readonly IMapper _mapper;

        public ObterInscricaoPorIdQueryHandler(IRepositorioInscricao repositorioInscricao, IMapper mapper)
        {
            _repositorioInscricao = repositorioInscricao ?? throw new ArgumentNullException(nameof(repositorioInscricao));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<PaginacaoResultadoDTO<DadosListagemInscricaoDTO>> Handle(ObterInscricaoPorIdQuery request, CancellationToken cancellationToken)
        {
            var mapeamento = new List<DadosListagemInscricaoDTO>();

            var totalDeRegistros = await _repositorioInscricao.ObterInscricaoPorIdComFiltrosTotalRegistros(
                request.PropostaId, request.filtros.RegistroFuncional, request.filtros.Cpf,
                request.filtros.NomeCursista, request.filtros.TurmasId, request.filtros.OcultarCancelada,

                request.filtros.OcultarTransferida);


            if (totalDeRegistros <= 0)
                return new PaginacaoResultadoDTO<DadosListagemInscricaoDTO>(mapeamento, totalDeRegistros, request.NumeroRegistros);

            var inscricoes = await _repositorioInscricao.ObterInscricaoPorIdComFiltros(request.PropostaId,
                request.filtros.RegistroFuncional, request.filtros.Cpf, request.filtros.NomeCursista,
                request.filtros.TurmasId, request.NumeroPagina, request.NumeroRegistros, request.filtros.OcultarCancelada,
                request.filtros.OcultarTransferida);

            var propostaPossuiAnexo = await _repositorioInscricao.ObterSeInscricaoPossuiAnexoPorPropostasIds(inscricoes.Select(x => x.Id).ToArray());

            mapeamento = _mapper.Map<IEnumerable<DadosListagemInscricaoDTO>>(inscricoes).ToList();

            mapeamento.ForEach(item =>
            {
                var anexos = propostaPossuiAnexo
                    .Where(x => x.InscricaoId == item.InscricaoId && !string.IsNullOrEmpty(x.NomeArquivo))
                    .Select(anexo => new DadosAnexosInscricao(anexo.NomeArquivo, anexo.Codigo))
                    .ToList();

                item.Anexos.AddRange(anexos);
            });

            return new PaginacaoResultadoDTO<DadosListagemInscricaoDTO>(mapeamento, totalDeRegistros, request.NumeroRegistros);
        }
    }
}