using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.UsuarioRedeParceria;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterUsuarioRedeParceiraPaginadaQueryHandler : IRequestHandler<ObterUsuarioRedeParceiraPaginadaQuery, PaginacaoResultadoDTO<UsuarioRedeParceriaPaginadoDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositorioUsuario _repositorioUsuario;

        public ObterUsuarioRedeParceiraPaginadaQueryHandler(IMapper mapper, IRepositorioUsuario repositorioUsuario)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repositorioUsuario = repositorioUsuario ?? throw new ArgumentNullException(nameof(repositorioUsuario));
        }

        public async Task<PaginacaoResultadoDTO<UsuarioRedeParceriaPaginadoDTO>> Handle(ObterUsuarioRedeParceiraPaginadaQuery request, CancellationToken cancellationToken)
        {
            var totalRegistrosFiltro = await _repositorioUsuario.ObterTotalUsuarioRedeParceria(request.Filtros.AreaPromotoraIds, request.Filtros.Nome, request.Filtros.Cpf, request.Filtros.Situacao);

            var usuarios = Enumerable.Empty<Usuario>();
            if (totalRegistrosFiltro > 0)
            {
                usuarios = await _repositorioUsuario.ObterUsuarioRedeParceria(request.Filtros.AreaPromotoraIds, request.Filtros.Nome, request.Filtros.Cpf, request.Filtros.Situacao, request.NumeroPagina, request.NumeroRegistros);
            }

            var items = _mapper.Map<IEnumerable<UsuarioRedeParceriaPaginadoDTO>>(usuarios);
            return new PaginacaoResultadoDTO<UsuarioRedeParceriaPaginadoDTO>(items, totalRegistrosFiltro, request.NumeroRegistros);
        }
    }
}
