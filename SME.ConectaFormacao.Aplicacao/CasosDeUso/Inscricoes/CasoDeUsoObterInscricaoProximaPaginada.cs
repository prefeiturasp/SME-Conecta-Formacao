using MediatR;
using Microsoft.AspNetCore.Http;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricoes;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Infra.Dados.Dtos.Inscricoes;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricoes
{
    public class CasoDeUsoObterInscricaoProximaPaginada : CasoDeUsoAbstratoPaginado, ICasoDeUsoObterInscricaoProximaPaginada
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        public CasoDeUsoObterInscricaoProximaPaginada(IMediator mediator, IContextoAplicacao contextoAplicacao, IHttpContextAccessor httpContextAccessor) : base(mediator, contextoAplicacao)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<PaginacaoResultadoDto<InscricaoPaginadaDTO>> Executar(InscricaoProximaFiltroDTO inscricaoDTO)
        {
            var usuarioLogado = await mediator.Send(new ObterUsuarioLogadoQuery());

            var filtro = MapearParaFiltroDominio(inscricaoDTO);

            return await mediator.Send(new ObterInscricaoProximaPaginadaQuery(usuarioLogado.Id, NumeroPagina, NumeroRegistros, filtro));
        }

        private InscricaoProximaFiltro MapearParaFiltroDominio(InscricaoProximaFiltroDTO dto)
        {
            if (dto == null)
                return new InscricaoProximaFiltro();

            return new InscricaoProximaFiltro
            {
                CodigoFormacao = dto.CodigoFormacao,
                NomeFormacao = dto.NomeFormacao,
                NomeTurma = dto.NomeTurma,
                Situacao = dto.Situacao,
                DataInscricao = dto.DataInscricao,
                DataInicial = dto.DataInicial,
                DataFinal = dto.DataFinal
            };
        }
    }
}
