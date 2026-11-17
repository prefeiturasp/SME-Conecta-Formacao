using AutoMapper;
using SME.ConectaFormacao.Aplicacao.Dtos.CodafConferencias;
using SME.ConectaFormacao.Aplicacao.Interfaces.CodafConferencias;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Armazenamento.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafConferencias
{
    public class CasoDeUsoObterCodafConferenciaPorId(
        IRepositorioCodafConferencia repositorioCodafConferencia,
        IServicoArmazenamento servicoArmazenamento,
        IMapper mapper,
        IContextoAplicacao contextoAplicacao) : ICasoDeUsoObterCodafConferenciaPorId
    {
        public async Task<Resultado<CodafConferenciaDetalhadoDto>> ExecutarAsync(long codafConferenciaId)
        {
            bool perfilRestrito = contextoAplicacao.IdPerfilUsuario != Perfis.ADMIN_DF && contextoAplicacao.IdPerfilUsuario != Perfis.EMFORPEF;

            var codafConferencia = await repositorioCodafConferencia.ObterPorIdDetalhadoAsync(codafConferenciaId);
            if (codafConferencia is null)
                return Erro.NaoEncontrado("Codaf não encontrado.");

            var codafConferenciaDto = mapper.Map<CodafConferenciaDetalhadoDto>(codafConferencia);

            if (perfilRestrito && codafConferenciaDto.CriadoLogin != contextoAplicacao.LoginUsuario)
                return Erro.Negocio("Você não tem permissão para visualizar este codaf.");

            if (codafConferenciaDto.Anexos != null)
            {
                foreach (var anexo in codafConferenciaDto.Anexos)
                {
                    anexo.UrlDownload = await servicoArmazenamento.ObterUrlPorChaveObjetoAsync(anexo.ArquivoCodigo.ToString());
                }
            }

            return codafConferenciaDto;
        }
    }
}
