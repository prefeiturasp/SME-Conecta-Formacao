using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Consultas.Propostas.ObterEncontrosPaginado
{
    public class ObterEncontrosPaginadoQueryHandler(
        IMapper mapper,
        IRepositorioPropostaEncontro repositorioPropostaEncontro) : 
        IRequestHandler<ObterEncontrosPaginadoQuery, PaginacaoResultadoDto<PropostaEncontroDto>>
    {
        public async Task<PaginacaoResultadoDto<PropostaEncontroDto>> Handle(ObterEncontrosPaginadoQuery request, CancellationToken cancellationToken)
        {
            var totalRegistros = await repositorioPropostaEncontro.ObterTotalEncontrosAsync(request.PropostaId);

            IEnumerable<PropostaEncontro> encontros = [];
            if (totalRegistros > 0)
            {
                encontros = await repositorioPropostaEncontro.ObterEncontrosPorPropostaAsync(request.PropostaId, request.NumeroPagina, request.NumeroRegistros);

                var ids = encontros.Select(t => t.Id).ToArray();
                var datas = await repositorioPropostaEncontro.ObterEncontroDatasPorEncontroIdAsync(ids);
                var turmas = await repositorioPropostaEncontro.ObterEncontroTurmasPorEncontroIdAsync(ids);

                foreach (var encontro in encontros)
                {
                    encontro.Datas = datas.Where(x => x.PropostaEncontroId == encontro.Id);
                    encontro.Turmas = turmas.Where(x => x.PropostaEncontroId == encontro.Id);
                }
            }

            var items = mapper.Map<IEnumerable<PropostaEncontroDto>>(encontros);
            return new PaginacaoResultadoDto<PropostaEncontroDto>(items, totalRegistros, request.NumeroRegistros);
        }
    }
}
