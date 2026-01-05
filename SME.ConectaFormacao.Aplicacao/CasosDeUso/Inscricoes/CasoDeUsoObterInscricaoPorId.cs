using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricoes;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Infra.Dados.Dtos.Inscricoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricoes
{
    public class CasoDeUsoObterInscricaoPorId(
        IMediator mediator, IContextoAplicacao contextoAplicacao,
        IRepositorioInscricao repositorioInscricao, IMapper mapper) :
        CasoDeUsoAbstratoPaginado(mediator, contextoAplicacao), ICasoDeUsoObterInscricaoPorId
    {
        public async Task<PaginacaoResultadoDto<DadosListagemInscricaoDto>> ExecutarAsync(FiltroListagemInscricaoDto filtro)
        {
            filtro.NumeroPagina = NumeroPagina;
            filtro.NumeroRegistros = NumeroRegistros;
            var mapeamento = new List<DadosListagemInscricaoDto>();

            var retorno = await repositorioInscricao.ObterInscricoesPorPropostaPaginadasAsync(filtro);

            if (retorno.TotalRegistros <= 0)
                return new PaginacaoResultadoDto<DadosListagemInscricaoDto>(mapeamento, retorno.TotalRegistros, NumeroRegistros);

            var inscricoes = retorno.Itens;

            var propostaPossuiAnexo = await repositorioInscricao.ObterSeInscricaoPossuiAnexoPorPropostasIds([.. inscricoes.Select(x => x.Id)]);

            mapeamento = [.. mapper.Map<IEnumerable<DadosListagemInscricaoDto>>(inscricoes)];

            mapeamento.ForEach(item =>
            {
                var anexos = propostaPossuiAnexo
                    .Where(x => x.InscricaoId == item.InscricaoId && !string.IsNullOrEmpty(x.NomeArquivo))
                    .Select(anexo => new DadosAnexosInscricao(anexo.NomeArquivo, anexo.Codigo))
                    .ToList();

                item.Anexos.AddRange(anexos);
            });

            return new PaginacaoResultadoDto<DadosListagemInscricaoDto>(mapeamento, retorno.TotalRegistros, NumeroRegistros);
        }
    }
}