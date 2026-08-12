using AutoMapper;
using SME.ConectaFormacao.Aplicacao.Dtos.CodafCursosNaoHomologados;
using SME.ConectaFormacao.Aplicacao.Interfaces.CodafCursosNaoHomologados;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Armazenamento.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafCursosNaoHomologados
{
    public class CasoDeUsoObterCodafCursoNaoHomologadoPorId(
   IRepositorioCodafCursoNaoHomologado repositorioCodafCursoNaoHomologado,
   IServicoArmazenamento servicoArmazenamento,
   IMapper mapper,
   IContextoAplicacao contextoAplicacao) : ICasoDeUsoObterCodafCursoNaoHomologadoPorId
    {
        public async Task<Resultado<CodafCursoNaoHomologadoDetalhadoDto>> ExecutarAsync(long codafCursoNaoHomologadoId)
        {
            bool perfilRestrito = contextoAplicacao.IdPerfilUsuario != Perfis.ADMIN_DF && contextoAplicacao.IdPerfilUsuario != Perfis.EMFORPEF;

            var codafCursoNaoHomologado = await repositorioCodafCursoNaoHomologado.ObterPorIdDetalhadoAsync(codafCursoNaoHomologadoId);

            if (codafCursoNaoHomologado == null)
                return Erro.NaoEncontrado("Codaf não encontrado.");

            var codafCursoNaoHomologadoDto = mapper.Map<CodafCursoNaoHomologadoDetalhadoDto>(codafCursoNaoHomologado);

            if (perfilRestrito && codafCursoNaoHomologadoDto.CriadoLogin != contextoAplicacao.LoginUsuario)
                return Erro.Negocio("Você não tem permissão para visualizar este codaf.");

            if (codafCursoNaoHomologadoDto.Anexos != null)
            {
                foreach (var anexo in codafCursoNaoHomologadoDto.Anexos)
                {
                    anexo.UrlDownload = await servicoArmazenamento.ObterUrlPorChaveObjetoAsync(anexo.ArquivoCodigo.ToString());
                }
            }

            return codafCursoNaoHomologadoDto;
        }       
    }
}
