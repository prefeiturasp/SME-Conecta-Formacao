using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterEncontrosPaginadoQueryHandler : IRequestHandler<ObterEncontrosPaginadoQuery, PaginacaoResultadoDTO<PropostaEncontroDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositorioProposta _repositorioProposta;

        public ObterEncontrosPaginadoQueryHandler(IMapper mapper, IRepositorioProposta repositorioProposta)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<PaginacaoResultadoDTO<PropostaEncontroDTO>> Handle(ObterEncontrosPaginadoQuery request, CancellationToken cancellationToken)
        {
            var totalRegistros = await _repositorioProposta.ObterTotalEncontros(request.PropostaId);

            IEnumerable<PropostaEncontro> encontros = new List<PropostaEncontro>();
            if (totalRegistros > 0)
            {
                encontros = await _repositorioProposta.ObterEncontrosPaginados(request.NumeroPagina, request.NumeroRegistros, request.PropostaId);

                var ids = encontros.Select(t => t.Id).ToArray();
                var datas = await _repositorioProposta.ObterEncontroDatasPorEncontroId(ids);
                var turmas = await _repositorioProposta.ObterEncontroTurmasPorEncontroId(ids);

                foreach (var encontro in encontros)
                {
                    encontro.Datas = datas.Where(x => x.PropostaEncontroId == encontro.Id);
                    encontro.Turmas = turmas.Where(x => x.PropostaEncontroId == encontro.Id);
                }
            }

            var items = _mapper.Map<IEnumerable<PropostaEncontroDTO>>(encontros);
            return new PaginacaoResultadoDTO<PropostaEncontroDTO>(items, totalRegistros, request.NumeroRegistros);
        }
    }
}
