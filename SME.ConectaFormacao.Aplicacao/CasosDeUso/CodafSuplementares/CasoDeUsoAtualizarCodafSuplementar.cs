using FluentValidation;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf.Dependencias;
using SME.ConectaFormacao.Aplicacao.Dtos.CodafSuplementares;
using SME.ConectaFormacao.Aplicacao.Interfaces.CodafSuplementares;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafSuplementares
{
    public class CasoDeUsoAtualizarCodafSuplementar(
        CodafSuplementarDependencias dependencias,
        IValidator<CodafSuplementarCadastroDto> validator) : ICasoDeUsoAtualizarCodafSuplementar
    {
        public async Task<Resultado> ExecutarAsync(CodafSuplementarCadastroDto codafSuplementarCadastroDto, long id)
        {
            var codafSuplementarExistente = await dependencias.RepositorioCodaf.ObterNaoExcluidosPorIdAsync(id);

            if (codafSuplementarExistente is null)
                return Erro.NaoEncontrado("Codaf Suplementar não encontrado");

            bool possuiCertificadoEmitido = codafSuplementarExistente.CertificadoEmitido;

            var validationResult = await validator.ValidateAsync(codafSuplementarCadastroDto);
            if (!validationResult.IsValid)
                return validationResult.ToErroValidacao();

            codafSuplementarExistente.AtualizarInformacoes(
                new(codafSuplementarCadastroDto.DataPublicacao,
                codafSuplementarCadastroDto.DataPublicacaoDom,
                codafSuplementarCadastroDto.NumeroComunicado,
                codafSuplementarCadastroDto.PaginaComunicadoDom,
                codafSuplementarCadastroDto.CodigoCursoEol,
                codafSuplementarCadastroDto.CodigoNivel,
                codafSuplementarCadastroDto.Observacao));

            using var transacaoDb = dependencias.Transacao.Iniciar();

            try
            {
                if (!possuiCertificadoEmitido || codafSuplementarExistente.Status != Dominio.Enumerados.StatusCodafSuplementar.Finalizado)
                    await SalvarInscritosAsync(codafSuplementarCadastroDto, codafSuplementarExistente);

                await SalvarRetificacoesAsync(codafSuplementarCadastroDto, codafSuplementarExistente.Id);
                var anexos = dependencias.Mapper.Map<List<CodafSuplementarAnexo>>(codafSuplementarCadastroDto.Anexos);
                await dependencias.AnexoService.ProcessarAnexosAsync(codafSuplementarExistente.Id, anexos);
                codafSuplementarExistente.CodafAnexos = anexos;
                codafSuplementarExistente.DefinirStatus();
                await dependencias.RepositorioCodaf.Atualizar(codafSuplementarExistente);
                transacaoDb.Commit();
                return Resultado.DeSucesso();
            }
            catch
            {
                transacaoDb.Rollback();
                return new Erro(TipoFalha.ErroInterno, "Erro ao salvar CODAF Suplementar");
            }
        }

        private async Task SalvarInscritosAsync(CodafSuplementarCadastroDto codafSuplementarCadastroDto, CodafSuplementar codafSuplementar)
        {
            var inscritos = dependencias.Mapper.Map<List<CodafSuplementarInscricao>>(codafSuplementarCadastroDto.Inscritos);
            await dependencias.InscritosService.SalvarInscritosAsync(inscritos, codafSuplementar.Id);
            codafSuplementar.CodafInscricoes = inscritos;
        }

        private async Task SalvarRetificacoesAsync(CodafSuplementarCadastroDto codafSuplementarCadastroDto, long codafSuplementarId)
        {
            var retificacoesExistentes = await dependencias.RepositorioRetificacao.ObterPorCodafSuplementarIdAsync(codafSuplementarId);
            var retificacoesEnviadas = codafSuplementarCadastroDto.Retificacoes ?? [];
            var retificacoesEnviadasIds = retificacoesEnviadas.Where(r => r.Id > 0).Select(r => r.Id).ToHashSet();
            var retificacoesParaRemover = retificacoesExistentes.Where(r => !retificacoesEnviadasIds.Contains(r.Id)).ToList();

            foreach (var item in retificacoesParaRemover)
            {
                await dependencias.RepositorioRetificacao.Remover(item);
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
                    var novaRetificacao = dependencias.Mapper.Map<CodafSuplementarRetificacao>(retificacaoDto);
                    novaRetificacao.CodafSuplementarId = codafSuplementarId;
                    await dependencias.RepositorioRetificacao.Inserir(novaRetificacao);
                }
            }
        }
    }
}
