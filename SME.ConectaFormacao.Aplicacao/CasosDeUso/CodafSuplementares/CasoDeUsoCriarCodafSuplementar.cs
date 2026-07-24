using FluentValidation;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf.Dependencias;
using SME.ConectaFormacao.Aplicacao.Dtos.CodafSuplementares;
using SME.ConectaFormacao.Aplicacao.Interfaces.CodafSuplementares;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Entidades;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafSuplementares
{
    [ExcludeFromCodeCoverage]
    public class CasoDeUsoCriarCodafSuplementar(
        CodafSuplementarDependencias dependencias,
        IValidator<CodafSuplementarCadastroDto> validator) : ICasoDeUsoCriarCodafSuplementar
    {
        public async Task<Resultado<CodafSuplementarDetalhadoDto>> ExecutarAsync(CodafSuplementarCadastroDto codafSuplementarCadastroDto)
        {
            var validationResult = await validator.ValidateAsync(codafSuplementarCadastroDto);
            if (!validationResult.IsValid)
                return validationResult.ToErroValidacao();

            var codafOriginal = await dependencias.RepositorioLista.ObterNaoExcluidosPorIdAsync(codafSuplementarCadastroDto.CodafId);

            if (codafOriginal is null)
                return Erro.Validacao("Codaf não encontrado para a turma informada");

            var codafSuplementarExistente = await dependencias.RepositorioCodaf.ObterPorExpressaoAsync(c => c.CodafId == codafOriginal.Id && !c.Excluido);

            if (codafSuplementarExistente is not null)
                return Erro.Validacao("Já existe um codaf suplementar para a turma informada");

            var codafSuplementar = new CodafSuplementar(codafSuplementarCadastroDto.CodafId,
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
                var id = await dependencias.RepositorioCodaf.Inserir(codafSuplementar);
                codafSuplementar.Id = id;

                await SalvarInscritosAsync(codafSuplementarCadastroDto, codafSuplementar);
                await SalvarRetificacoesAsync(codafSuplementarCadastroDto, codafSuplementar.Id);
                var anexos = dependencias.Mapper.Map<List<CodafSuplementarAnexo>>(codafSuplementarCadastroDto.Anexos);
                await dependencias.AnexoService.ProcessarAnexosAsync(codafSuplementar.Id, anexos);

                codafSuplementar.CodafAnexos = anexos;
                codafSuplementar.DefinirStatus();

                transacaoDb.Commit();
                var codafSuplementarDetalhadoDto = new CodafSuplementarDetalhadoDto
                {
                    AlteradoEm = codafSuplementar.AlteradoEm,
                    AlteradoLogin = codafSuplementar.AlteradoLogin,
                    AlteradoPor = codafSuplementar.AlteradoPor,
                    CodigoCursoEol = codafSuplementar.CodigoCursoEol,
                    CodigoNivel = codafSuplementar.CodigoNivel,
                    CriadoEm = codafSuplementar.CriadoEm,
                    CriadoLogin = codafSuplementar.CriadoLogin,
                    CriadoPor = codafSuplementar.CriadoPor,
                    DataPublicacao = codafSuplementar.DataPublicacao,
                    DataPublicacaoDom = codafSuplementar.DataPublicacaoDom,
                    Id = codafSuplementar.Id,
                    CodigoFormacao = codafOriginal.PropostaId,
                    PropostaId = codafOriginal.PropostaId
                };

                return codafSuplementarDetalhadoDto;
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

        private async Task SalvarRetificacoesAsync(CodafSuplementarCadastroDto dto, long codafSuplementarId)
        {
            if (dto.Retificacoes is not null && dto.Retificacoes.Any())
            {
                var retificacoes = dependencias.Mapper.Map<List<CodafSuplementarRetificacao>>(dto.Retificacoes);
                foreach (var retificacao in retificacoes)
                {
                    retificacao.CodafSuplementarId = codafSuplementarId;
                    await dependencias.RepositorioRetificacao.Inserir(retificacao);
                }
            }
        }
    }
}
