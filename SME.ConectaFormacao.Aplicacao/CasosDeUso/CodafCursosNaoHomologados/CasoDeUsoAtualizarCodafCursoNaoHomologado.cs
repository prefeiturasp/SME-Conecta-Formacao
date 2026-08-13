using AutoMapper;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf.Dependencias;
using SME.ConectaFormacao.Aplicacao.Dtos.CodafCursosNaoHomologados;
using SME.ConectaFormacao.Aplicacao.Interfaces.CodafCursosNaoHomologados;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafCursosNaoHomologados
{
    public class CasoDeUsoAtualizarCodafCursoNaoHomologado(
        CodafCursoNaoHomologadoDependencias dependencias,
        IMapper mapper,
        IContextoAplicacao contextoAplicacao) : ICasoDeUsoAtualizarCodafCursoNaoHomologado
    {
        public async Task<Resultado> ExecutarAsync(CodafCursoNaoHomologadoCadastroDto codafCursoNaoHomologadoCadastroDto, long id)
        {
            var perfilRestrito = !contextoAplicacao.EhAdministrador;

            var codafCursoNaoHomologadoExistente = await dependencias.RepositorioCodaf.ObterNaoExcluidosPorIdAsync(id);
            if (codafCursoNaoHomologadoExistente is null)
                return Erro.NaoEncontrado("Codaf não encontrado.");

            if (perfilRestrito && codafCursoNaoHomologadoExistente.CriadoLogin != contextoAplicacao.LoginUsuario)
                return Erro.Negocio("Você não tem permissão para editar este Codaf.");

            if (codafCursoNaoHomologadoExistente.Status == StatusCodafCursoNaoHomologado.Finalizado)
                return Erro.Negocio("Não é possível editar um Codaf com status 'Finalizado'.");

            codafCursoNaoHomologadoExistente.AtualizarInformacoes(codafCursoNaoHomologadoCadastroDto.Observacao);

            using var transacaoDb = dependencias.Transacao.Iniciar();

            try
            {
                await SalvarInscritosAsync(codafCursoNaoHomologadoCadastroDto, codafCursoNaoHomologadoExistente);
                await SalvarAnexosAsync(codafCursoNaoHomologadoCadastroDto, codafCursoNaoHomologadoExistente);
                codafCursoNaoHomologadoExistente.DefinirStatus();
                await dependencias.RepositorioCodaf.Atualizar(codafCursoNaoHomologadoExistente);
                transacaoDb.Commit();
                return Resultado.DeSucesso();
            }
            catch
            {
                transacaoDb.Rollback();
                return new Erro(TipoFalha.ErroInterno, "Erro ao salvar CODAF");
            }
        }

        private async Task SalvarAnexosAsync(CodafCursoNaoHomologadoCadastroDto codafCursoNaoHomologadoCadastroDto, CodafCursoNaoHomologado codafCursoNaoHomologadoExistente)
        {
            var anexos = dependencias.Mapper.Map<List<CodafCursoNaoHomologadoAnexo>>(codafCursoNaoHomologadoCadastroDto.Anexos);
            await dependencias.AnexoService.ProcessarAnexosAsync(codafCursoNaoHomologadoExistente.Id, anexos);
            codafCursoNaoHomologadoExistente.CodafAnexos = anexos;
        }

        public async Task SalvarInscritosAsync(CodafCursoNaoHomologadoCadastroDto codafCursoNaoHomologadoCadastroDto, CodafCursoNaoHomologado codafCursoNaoHomologado)
        {
            if (codafCursoNaoHomologado.DeclaracaoEmitida || codafCursoNaoHomologado.Status == StatusCodafCursoNaoHomologado.Finalizado)
                return;

            var inscritos = mapper.Map<List<CodafCursoNaoHomologadoInscricao>>(codafCursoNaoHomologadoCadastroDto.Inscritos);
            await dependencias.InscritosService.SalvarInscritosAsync(inscritos, codafCursoNaoHomologado.Id);
            codafCursoNaoHomologado.CodafInscricoes = inscritos;
        }
    }
}
