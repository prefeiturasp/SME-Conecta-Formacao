using AutoMapper;
using FluentValidation;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf.Dependencias;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Aplicacao.Interfaces.Codaf;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf
{
    public class CasoDeUsoCriarCodafListaPresenca(
        CodafListaPresencaDependencias dependencias,
        IValidator<CodafListaPresencaCadastroDto> validator,
        IMapper mapper,
        ITransacao transacao,
        IContextoAplicacao contextoAplicacao) :
        ICasoDeUsoCriarCodafListaPresenca
    {
        public async Task<Resultado<CodafListaPresencaDto>> ExecutarAsync(CodafListaPresencaCadastroDto codafListaPresencaCadastroDto)
        {
            var erroValidacao = await ValidarRegrasDeNegocio(codafListaPresencaCadastroDto);
            if (erroValidacao is not null)
                return erroValidacao.Value;

            var codafListaPresenca = new CodafListaPresenca(
                codafListaPresencaCadastroDto.PropostaId,
                codafListaPresencaCadastroDto.PropostaTurmaId,
                new(codafListaPresencaCadastroDto.DataPublicacao,
                codafListaPresencaCadastroDto.DataPublicacaoDom,
                codafListaPresencaCadastroDto.NumeroComunicado,
                codafListaPresencaCadastroDto.PaginaComunicadoDom,
                codafListaPresencaCadastroDto.CodigoCursoEol,
                codafListaPresencaCadastroDto.CodigoNivel,
                codafListaPresencaCadastroDto.Observacao),
                contextoAplicacao.IdPerfilUsuario);
            codafListaPresenca.Iniciar();

            using var transacaoDb = transacao.Iniciar();
            try
            {
                var idListaPresenca = await dependencias.RepositorioLista.Inserir(codafListaPresenca);
                codafListaPresenca.Id = idListaPresenca;
                await SalvarInscritosAsync(codafListaPresencaCadastroDto, idListaPresenca);
                await SalvarRetificacoesAsync(codafListaPresencaCadastroDto, idListaPresenca);
                var anexos = mapper.Map<IEnumerable<CodafAnexo>>(codafListaPresencaCadastroDto.Anexos);
                await dependencias.AnexosService.ProcessarAnexosAsync(idListaPresenca, anexos);
                await dependencias.MovimentacaoService.RegistrarMovimentacaoAsync(codafListaPresenca);
                transacaoDb.Commit();
                return mapper.Map<CodafListaPresencaDto>(codafListaPresenca);
            }
            catch
            {
                transacaoDb.Rollback();
                return new Erro(TipoFalha.ErroInterno, "Erro ao salvar a lista de presença.");
            }
        }
        private async Task<Erro?> ValidarRegrasDeNegocio(CodafListaPresencaCadastroDto codafListaPresencaCadastroDto)
        {
            var validationResult = await validator.ValidateAsync(codafListaPresencaCadastroDto);
            if (!validationResult.IsValid)
                return validationResult.ToErroValidacao();

            var erroVinculo = await dependencias.ValidadorDominio.ValidarVinculoPropostaTurmaAsync(
                codafListaPresencaCadastroDto.PropostaId,
                codafListaPresencaCadastroDto.PropostaTurmaId);

            if (erroVinculo is not null)
                return erroVinculo;

            var erroUnicidadeTurma = await dependencias.ValidadorDominio.ValidarUnicidadeTurmaListaDePresencaAsync(
                codafListaPresencaCadastroDto.PropostaTurmaId, 0);

            if (erroUnicidadeTurma is not null)
                return erroUnicidadeTurma;

            return null;
        }

        private async Task SalvarInscritosAsync(CodafListaPresencaCadastroDto codafListaPresencaCadastroDto, long codafListaPresencaId)
        {
            var inscritos = mapper.Map<List<CodafInscricaoListaPresenca>>(codafListaPresencaCadastroDto.Inscritos);
            await dependencias.InscritosService.SalvarInscritosAsync(inscritos, codafListaPresencaId);
        }

        private async Task SalvarRetificacoesAsync(CodafListaPresencaCadastroDto codafListaPresencaCadastroDto, long codafListaPresencaId)
        {
            if (codafListaPresencaCadastroDto.Retificacoes is null || !codafListaPresencaCadastroDto.Retificacoes.Any())
                return;

            var retificacoes = mapper.Map<IEnumerable<CodafRetificacaoListaPresenca>>(codafListaPresencaCadastroDto.Retificacoes);
            foreach (var retificacao in retificacoes)
            {
                retificacao.CodafListaPresencaId = codafListaPresencaId;
                await dependencias.RepositorioRetificacao.Inserir(retificacao);
            }
        }
    }
}