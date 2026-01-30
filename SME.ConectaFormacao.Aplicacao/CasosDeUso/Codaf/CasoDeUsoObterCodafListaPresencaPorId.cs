using AutoMapper;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Aplicacao.Interfaces.Codaf;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Armazenamento.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf
{
    public class CasoDeUsoObterCodafListaPresencaPorId(
        IRepositorioCodafListaPresenca repositorioCodafListaPresenca,
        IServicoArmazenamento servicoArmazenamento,
        IRepositorioCodafComentarioListaPresenca repositorioCodafComentarioListaPresenca,
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
                    anexo.UrlDownload = await servicoArmazenamento.ObterUrlPorChaveObjetoAsync(anexo.ArquivoCodigo.ToString());
                }
            }
            await ObterComentarioDfAsync(listaPresencaDto);
            return listaPresencaDto;
        }

        private async Task ObterComentarioDfAsync(CodafListaPresencaDto listaPresencaDto)
        {
            if (listaPresencaDto == null) return;
            if (listaPresencaDto.Status != StatusCodafListaPresenca.DevolvidoParaCorrecao) return;
            listaPresencaDto.Comentario = await repositorioCodafComentarioListaPresenca.ObterUltimoComentarioDevolucaoPorUsuarioAsync(
                listaPresencaDto.Id, StatusCodafListaPresenca.DevolvidoParaCorrecao, StatusCodafListaPresenca.AguardandoDf);
        }
    }
}
