using AutoMapper;
using SME.ConectaFormacao.Aplicacao.Dtos.CodafSuplementares;
using SME.ConectaFormacao.Aplicacao.Interfaces.CodafSuplementares;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Armazenamento.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafSuplementares
{
    public class CasoDeUsoObterCodafSuplementarPorId(
        IRepositorioCodafSuplementar repositorioCodafSuplementar,
        IServicoArmazenamento servicoArmazenamento,
        IMapper mapper) : ICasoDeUsoObterCodafSuplementarPorId
    {
        public async Task<Resultado<CodafSuplementarDetalhadoDto>> ExecutarAsync(long codafSuplementarId)
        {
            var codafSuplementar = await repositorioCodafSuplementar.ObterPorIdDetalhadoAsync(codafSuplementarId);
            if (codafSuplementar == null)
                return Erro.NaoEncontrado("Codaf Suplementar não encontrado.");

            var codafSuplementarDto = mapper.Map<CodafSuplementarDetalhadoDto>(codafSuplementar);

            codafSuplementarDto.CertificadoEmitido = codafSuplementar.CodafCertificados != null && codafSuplementar.CodafCertificados.Any();

            if (codafSuplementarDto.Anexos != null && codafSuplementarDto.Anexos.Count > 0)
            {
                foreach (var anexo in codafSuplementarDto.Anexos)
                {
                    anexo.UrlDownload = await servicoArmazenamento.ObterUrlPorChaveObjetoAsync(anexo.ArquivoCodigo.ToString());
                }
            }
            return codafSuplementarDto;
        }
    }
}
