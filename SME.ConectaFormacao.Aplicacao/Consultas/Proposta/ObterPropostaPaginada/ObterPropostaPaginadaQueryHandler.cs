using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Infra.Dados.Dtos.Propostas;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Consultas.Proposta.ObterPropostaPaginada
{
    public class ObterPropostaPaginadaQueryHandler(
        IMapper mapper, 
        IRepositorioProposta repositorioProposta,
        IContextoAplicacao contextoAplicacao) :
        IRequestHandler<ObterPropostaPaginadaQuery, PaginacaoResultadoDto<PropostaPaginadaDTO>>
    {
        public async Task<PaginacaoResultadoDto<PropostaPaginadaDTO>> Handle(ObterPropostaPaginadaQuery request, CancellationToken cancellationToken)
        {
            var filtroRepositorio = new FiltroListagemPropostaDto
            {
                Pagina = request.NumeroPagina,
                TamanhoPagina = request.NumeroRegistros,
                AreaPromotoraIdUsuarioLogado = request.AreaPromotoraIdUsuarioLogado,
                PropostaId = request.PropostaFiltrosDTO.Id,
                AreaPromotoraId = request.PropostaFiltrosDTO.AreaPromotoraId,
                Formato = request.PropostaFiltrosDTO.Formato,
                PublicoAlvoIds = request.PropostaFiltrosDTO.PublicoAlvoIds,
                NomeFormacao = request.PropostaFiltrosDTO.NomeFormacao,
                NumeroHomologacao = request.PropostaFiltrosDTO.NumeroHomologacao,
                PeriodoRealizacaoInicio = request.PropostaFiltrosDTO.PeriodoRealizacaoInicio,
                PeriodoRealizacaoFim = request.PropostaFiltrosDTO.PeriodoRealizacaoFim,
                Situacao = request.PropostaFiltrosDTO.Situacao,
                FormacaoHomologada = request.PropostaFiltrosDTO.FormacaoHomologada,
                LoginUsuarioLogado = contextoAplicacao.UsuarioLogado,
                PerfilUsuarioLogado = contextoAplicacao.IdPerfilUsuario ?? Guid.NewGuid(),
                Revalidacao = request.PropostaFiltrosDTO.Revalidacao
            };

            var resultado = await repositorioProposta.ObterPropostaPorFiltroAsync(filtroRepositorio);
            var resultadoDto = new PaginacaoResultadoDto<PropostaPaginadaDTO>(
                mapper.Map<List<PropostaPaginadaDTO>>(resultado.Itens),
                resultado.TotalRegistros,
                resultado.TamanhoPagina);
            return resultadoDto;
        }
    }
}
