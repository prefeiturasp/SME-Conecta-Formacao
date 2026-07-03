using AutoMapper;
using FluentValidation;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf.Dependencias;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Aplicacao.Interfaces.Codaf;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf
{
    public class CasoDeUsoAtualizarCodafListaPresenca(
        CodafListaPresencaDependencias dependencias,
        IValidator<CodafListaPresencaEdicaoDto> validator,
        IMapper mapper,
        ITransacao transacao,
        IContextoAplicacao contextoAplicacao) :
        ICasoDeUsoAtualizarCodafListaPresenca
    {
        public async Task<Resultado> ExecutarAsync(CodafListaPresencaEdicaoDto codafListaPresencaEdicaoDto, long id)
        {
            bool perfilRestrito = contextoAplicacao.IdPerfilUsuario != Perfis.ADMIN_DF && contextoAplicacao.IdPerfilUsuario != Perfis.EMFORPEF;

            var codafListaPresencaExistente = await dependencias.RepositorioLista.ObterNaoExcluidosPorIdAsync(id);
            if (codafListaPresencaExistente is null)
                return Erro.NaoEncontrado("Lista de presença não encontrada.");

            if (perfilRestrito && codafListaPresencaExistente.CriadoLogin != contextoAplicacao.LoginUsuario)
                return Erro.Negocio("Você não tem permissão para editar esta lista de presença.");

            if (codafListaPresencaExistente.EstaFinalizado())
                return Erro.Negocio("Não é possível editar uma lista de presença com situação 'Finalizado'.");

            var erroValidacao = await ValidarRegrasDeNegocio(codafListaPresencaEdicaoDto, id);
            if (erroValidacao is not null)
                return erroValidacao;

            codafListaPresencaExistente.AtualizarInformacoes(
                new(codafListaPresencaEdicaoDto.DataPublicacao,
                codafListaPresencaEdicaoDto.DataPublicacaoDom,
                codafListaPresencaEdicaoDto.NumeroComunicado,
                codafListaPresencaEdicaoDto.PaginaComunicadoDom,
                codafListaPresencaEdicaoDto.CodigoCursoEol,
                codafListaPresencaEdicaoDto.CodigoNivel,
                codafListaPresencaEdicaoDto.Observacao),
                contextoAplicacao.IdPerfilUsuario);

            var transacaoDb = transacao.Iniciar();
            if (transacaoDb == null)
                return Resultado.DeFalha(TipoFalha.ErroInterno, $"Erro ao atualizar a lista de presença.");

            try
            {
                await dependencias.RepositorioLista.Atualizar(codafListaPresencaExistente);
                await SalvarInscritosAsync(codafListaPresencaEdicaoDto, codafListaPresencaExistente);
                await SalvarRetificacoesAsync(codafListaPresencaEdicaoDto, codafListaPresencaExistente.Id);

                var anexos = Enumerable.Empty<CodafAnexo>();
                if (codafListaPresencaEdicaoDto.Anexos != null && codafListaPresencaEdicaoDto.Anexos.Any())
                    anexos = mapper.Map<IEnumerable<CodafAnexo>>(codafListaPresencaEdicaoDto.Anexos);

                await dependencias.AnexosService.ProcessarAnexosAsync(codafListaPresencaExistente.Id, anexos);
                await dependencias.MovimentacaoService.RegistrarMovimentacaoAsync(codafListaPresencaExistente);
                transacaoDb.Commit();
                return Resultado.DeSucesso();
            }
            catch
            {
                transacaoDb.Rollback();
                return Resultado.DeFalha(TipoFalha.ErroInterno, $"Erro ao atualizar a lista de presença.");
            }
            finally
            {
                transacaoDb?.Dispose();
            }
        }

        private async Task<Erro?> ValidarRegrasDeNegocio(CodafListaPresencaEdicaoDto codafListaPresencaEdicaoDto, long id)
        {
            var validationResult = await validator.ValidateAsync(codafListaPresencaEdicaoDto);
            if (validationResult == null || !validationResult.IsValid)
                return validationResult?.ToErroValidacao() ?? Erro.Validacao("Erro ao validar os dados de entrada.");

            var erroVinculo = await dependencias.ValidadorDominio.ValidarVinculoPropostaTurmaAsync(
                codafListaPresencaEdicaoDto.PropostaId,
                codafListaPresencaEdicaoDto.PropostaTurmaId);

            if (erroVinculo is not null)
                return erroVinculo;

            var erroUnicidadeTurma = await dependencias.ValidadorDominio.ValidarUnicidadeTurmaListaDePresencaAsync(
                codafListaPresencaEdicaoDto.PropostaTurmaId, id);

            if (erroUnicidadeTurma is not null)
                return erroUnicidadeTurma;

            return null;
        }

        private async Task SalvarInscritosAsync(CodafListaPresencaEdicaoDto codafListaPresencaEdicaoDto, CodafListaPresenca codafListaPresenca)
        {
            if (codafListaPresencaEdicaoDto.Inscritos == null || !codafListaPresencaEdicaoDto.Inscritos.Any())
                return;

            var inscritos = mapper.Map<List<CodafInscricaoListaPresenca>>(codafListaPresencaEdicaoDto.Inscritos);
            await dependencias.InscritosService.SalvarInscritosAsync(inscritos, codafListaPresenca.Id);
        }

        private async Task SalvarRetificacoesAsync(CodafListaPresencaEdicaoDto codafListaPresencaEdicaoDto, long codafListaPresencaId)
        {
            var retificacoesExistentes = await dependencias.RepositorioRetificacao.ObterPorListaPresencaIdAsync(codafListaPresencaId) ?? [];
            var retificacoesEnviadas = codafListaPresencaEdicaoDto.Retificacoes ?? [];
            var retificacoesEnviadasIds = retificacoesEnviadas.Where(r => r.Id > 0).Select(r => r.Id).ToHashSet();

            foreach (var retificacaoExistente in retificacoesExistentes)
            {
                if (!retificacoesEnviadasIds.Contains(retificacaoExistente.Id))
                {
                    await dependencias.RepositorioRetificacao.Remover(retificacaoExistente);
                }
            }

            foreach (var retificacaoDto in retificacoesEnviadas)
            {
                if (retificacaoDto.Id > 0)
                {
                    var retificacaoExistente = retificacoesExistentes.FirstOrDefault(r => r.Id == retificacaoDto.Id);
                    if (retificacaoExistente != null)
                    {
                        retificacaoExistente.AtualizarInformacoes(
                            retificacaoDto.DataRetificacao,
                            retificacaoDto.PaginaRetificacaoDom);
                        await dependencias.RepositorioRetificacao.Atualizar(retificacaoExistente);
                    }
                }
                else
                {
                    var novaRetificacao = mapper.Map<CodafRetificacaoListaPresenca>(retificacaoDto);
                    novaRetificacao.CodafListaPresencaId = codafListaPresencaId;
                    await dependencias.RepositorioRetificacao.Inserir(novaRetificacao);
                }
            }
        }
    }
}