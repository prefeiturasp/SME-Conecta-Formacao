using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Consultas.Propostas.ObterEncontrosPaginado
{
    public class ObterEncontrosPaginadoQueryHandler(IMapper mapper, IRepositorioProposta repositorioProposta) : 
        IRequestHandler<ObterEncontrosPaginadoQuery, PaginacaoResultadoDto<PropostaEncontroDto>>
    {
        public async Task<PaginacaoResultadoDto<PropostaEncontroDto>> Handle(ObterEncontrosPaginadoQuery request, CancellationToken cancellationToken)
        {
            var totalRegistros = await repositorioProposta.ObterTotalEncontros(request.PropostaId);

            IEnumerable<PropostaEncontro> encontros = new List<PropostaEncontro>();
            if (totalRegistros > 0)
            {
                encontros = await repositorioProposta.ObterEncontrosPaginados(request.NumeroPagina, request.NumeroRegistros, request.PropostaId);

                var ids = encontros.Select(t => t.Id).ToArray();
                var datas = await repositorioProposta.ObterEncontroDatasPorEncontroId(ids);
                var turmas = await repositorioProposta.ObterEncontroTurmasPorEncontroId(ids);

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
