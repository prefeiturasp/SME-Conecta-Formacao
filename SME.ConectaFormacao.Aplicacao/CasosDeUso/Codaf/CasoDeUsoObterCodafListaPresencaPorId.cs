using AutoMapper;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Aplicacao.Interfaces.Codaf;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Armazenamento.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf
{
    public class CasoDeUsoObterCodafListaPresencaPorId(
        IRepositorioCodafListaPresenca repositorioCodafListaPresenca,
        IServicoArmazenamento servicoArmazenamento,
        IMapper mapper) : ICasoDeUsoObterCodafListaPresencaPorId
    {
        public async Task<Resultado<CodafListaPresencaDto>> ExecutarAsync(long listaPresencaId)
        {
            var listaPresenca = await repositorioCodafListaPresenca.ObterPorIdDetalhadoAsync(listaPresencaId);
            if (listaPresenca == null)
                return Erro.NaoEncontrado("Lista de presença não encontrada.");

            var listaPresencaDto = mapper.Map<CodafListaPresencaDto>(listaPresenca);

            if (listaPresencaDto.Anexos != null)
            {
                foreach (var anexo in listaPresencaDto.Anexos)
                {
                    anexo.UrlDownload = await servicoArmazenamento.ObterUrlPorGuidAsync(anexo.ArquivoCodigo);
                }
            }
            return listaPresencaDto;
        }
    }
}
