using AutoMapper;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricoes;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricoes
{
    public class CasoDeUsoPesquisarCursistaPorPropostaTurmaId(
        IRepositorioInscricao repositorioInscricao,
        IMapper mapper) : ICasoDeUsoPesquisarCursistaPorPropostaTurmaId
    {
        public async Task<Resultado<PaginacaoResultadoDto<DadosInscricaoCursistaRetornoDto>>> ExecutarAsync(long propostaTurmaId, string termoBusca, int numeroPagina = 1, int numeroRegistros = 10)
        {
            var resultado = await repositorioInscricao.PesquisarCursistaPorPropostaTurmaIdAsync(propostaTurmaId, termoBusca, numeroPagina, numeroRegistros);

            var resultadoPaginado = new PaginacaoResultadoDto<DadosInscricaoCursistaRetornoDto>(
                mapper.Map<IEnumerable<DadosInscricaoCursistaRetornoDto>>(resultado.Itens),
                resultado.TotalRegistros,
                resultado.TamanhoPagina);

            return resultadoPaginado;
        }
    }
}
