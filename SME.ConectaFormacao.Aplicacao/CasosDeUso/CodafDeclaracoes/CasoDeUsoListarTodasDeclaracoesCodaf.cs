using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Interfaces.CodafDeclaracoes;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafDeclaracoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafDeclaracoes
{
    public class CasoDeUsoListarTodasDeclaracoesCodaf(
        IRepositorioCodafDeclaracao repositorioCodafDeclaracao) : ICasoDeUsoListarTodasDeclaracoesCodaf
    {
        public async Task<Resultado<PaginacaoResultadoDto<ListagemDeclaracoesCodafDto>>> ExecutarAsync(FiltroListagemTodasDeclaracoesCodafDto filtro)
        {
            var resultado = await repositorioCodafDeclaracao.ObterTodasDeclaracoesAsync(filtro);
            AplicarMascaraDeDocumento(resultado.Itens);
            var resultadoDto = new PaginacaoResultadoDto<ListagemDeclaracoesCodafDto>(
                resultado.Itens,
                resultado.TotalRegistros,
                resultado.TamanhoPagina);
            return resultadoDto;
        }

        private static void AplicarMascaraDeDocumento(IEnumerable<ListagemDeclaracoesCodafDto> itens)
        {
            if (itens is null || !itens.Any()) return;

            foreach (var item in itens)
            {
                if (!string.IsNullOrWhiteSpace(item.DocumentoCursista))
                {
                    var (documento, tipo) = ResolvedorDocumentoUsuario.Resolver(item.DocumentoCursista, string.Empty);
                    item.DocumentoCursista = ResolvedorDocumentoUsuario.FormatarValor(documento, tipo);
                }

                if (!string.IsNullOrWhiteSpace(item.DocumentoRegente))
                {
                    var (documento, tipo) = ResolvedorDocumentoUsuario.Resolver(item.DocumentoRegente, string.Empty);
                    item.DocumentoRegente = ResolvedorDocumentoUsuario.FormatarValor(documento, tipo);
                }
            }
        }
    }
}