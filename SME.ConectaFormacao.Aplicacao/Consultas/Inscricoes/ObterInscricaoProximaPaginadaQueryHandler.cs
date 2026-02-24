using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterInscricaoProximaPaginadaQueryHandler : IRequestHandler<ObterInscricaoProximaPaginadaQuery, PaginacaoResultadoDto<InscricaoPaginadaDTO>>
    {
        private readonly IRepositorioInscricao _repositorioInscricao;
        private readonly IMapper _mapper;

        public ObterInscricaoProximaPaginadaQueryHandler(IRepositorioInscricao repositorioInscricao, IMapper mapper)
        {
            _repositorioInscricao = repositorioInscricao ?? throw new ArgumentNullException(nameof(repositorioInscricao));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<PaginacaoResultadoDto<InscricaoPaginadaDTO>> Handle(ObterInscricaoProximaPaginadaQuery request, CancellationToken cancellationToken)
        {
            var totalRegistrosFiltro = await _repositorioInscricao.ObterTotalRegistrosPorUsuarioId(request.UsuarioId);

            var registros = Enumerable.Empty<Dominio.Entidades.Inscricao>();
            if (totalRegistrosFiltro > 0)
                registros = await _repositorioInscricao.ObterDadosPaginadosPorUsuarioId(request.UsuarioId, request.NumeroPagina, request.NumeroRegistros);

            var items = _mapper.Map<IEnumerable<InscricaoPaginadaDTO>>(registros);
            foreach (var item in items)
            {
                var cargoFuncao = await _repositorioInscricao.ObterCargoFuncaoPorId(item.Id);
                item.CargoFuncaoCodigo = cargoFuncao.CargoFuncaoCodigo;
                item.CargoFuncao = cargoFuncao.CargoFuncaoNome;
                item.TipoVinculo = cargoFuncao.TipoVinculo;
            }

            return new PaginacaoResultadoDto<InscricaoPaginadaDTO>(items, totalRegistrosFiltro, request.NumeroRegistros);
        }
    }
}
