using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Notificacao;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterNotificacaoPaginadaQueryHandler : IRequestHandler<ObterNotificacaoPaginadaQuery, PaginacaoResultadoDTO<NotificacaoPaginadoDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositorioNotificacao _repositorioNotificacao;

        public ObterNotificacaoPaginadaQueryHandler(IMapper mapper, IRepositorioNotificacao repositorioNotificacao)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repositorioNotificacao = repositorioNotificacao ?? throw new ArgumentNullException(nameof(repositorioNotificacao));
        }

        public async Task<PaginacaoResultadoDTO<NotificacaoPaginadoDTO>> Handle(ObterNotificacaoPaginadaQuery request, CancellationToken cancellationToken)
        {
            int total = await _repositorioNotificacao.ObterTotalNotificacao(
                request.Login,
                request.Filtro.Id,
                request.Filtro.Titulo,
                request.Filtro.Categoria,
                request.Filtro.Tipo,
                request.Filtro.Situacao
                );

            var items = Enumerable.Empty<NotificacaoPaginadoDTO>();
            if (total > 0)
            {
                var notificacoes = await _repositorioNotificacao.ObterNotificacaoPaginada(
                request.Login,
                request.Filtro.Id,
                request.Filtro.Titulo,
                request.Filtro.Categoria,
                request.Filtro.Tipo,
                request.Filtro.Situacao,
                request.NumeroRegistros,
                request.QuantidadeRegistrosIgnorados
                );

                items = _mapper.Map<IEnumerable<NotificacaoPaginadoDTO>>(notificacoes);
            }

            return new PaginacaoResultadoDTO<NotificacaoPaginadoDTO>(items, total, request.NumeroRegistros);
        }
    }
}
