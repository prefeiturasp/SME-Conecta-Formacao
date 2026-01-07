using AutoMapper;
using FluentValidation;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Aplicacao.Interfaces.Codaf;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Servicos.Interfaces;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf
{
    public class CasoDeUsoCriarCodafListaPresenca(
        IRepositorioCodafListaPresenca repositorioCodafListaPresenca,
        IRepositorioCodafInscritosListaPresenca repositorioCodafInscritosListaPresenca,
        IRepositorioCodafRetificacaoListaPresenca repositorioCodafRetificacaoListaPresenca,
        IValidadorCodafListaPresencaService validadorCodafListaPresencaService,
        IContextoAplicacao contextoAplicacao,
        IMapper mapper,
        ITransacao transacao,
        IValidator<CodafListaPresencaCadastroDto> validator,
        IGerenciadorAnexosCodafService gerenciadorAnexosCodafService) :
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
                codafListaPresencaCadastroDto.DataPublicacao,
                codafListaPresencaCadastroDto.DataPublicacaoDom,
                codafListaPresencaCadastroDto.NumeroComunicado,
                codafListaPresencaCadastroDto.PaginaComunicadoDom,
                codafListaPresencaCadastroDto.CodigoCursoEol,
                codafListaPresencaCadastroDto.CodigoNivel,
                codafListaPresencaCadastroDto.Observacao,
                contextoAplicacao.IdPerfilUsuario);
            codafListaPresenca.Iniciar();

            using var transacaoDb = transacao.Iniciar();
            try
            {
                var idListaPresenca = await repositorioCodafListaPresenca.Inserir(codafListaPresenca);
                codafListaPresenca.Id = idListaPresenca;
                await SalvarInscritosAsync(codafListaPresencaCadastroDto, idListaPresenca);
                await SalvarRetificacoesAsync(codafListaPresencaCadastroDto, idListaPresenca);
                var anexos = mapper.Map<IEnumerable<CodafAnexo>>(codafListaPresencaCadastroDto.Anexos);
                await gerenciadorAnexosCodafService.ProcessarAnexosAsync(idListaPresenca, anexos);
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

            var erroVinculo = await validadorCodafListaPresencaService.ValidarVinculoPropostaTurmaAsync(
                codafListaPresencaCadastroDto.PropostaId,
                codafListaPresencaCadastroDto.PropostaTurmaId);

            if (erroVinculo is not null)
                return erroVinculo;

            var erroUnicidadeTurma = await validadorCodafListaPresencaService.ValidarUnicidadeTurmaListaDePresencaAsync(
                codafListaPresencaCadastroDto.PropostaTurmaId, 0);

            if (erroUnicidadeTurma is not null)
                return erroUnicidadeTurma;

            return null;
        }

        private async Task SalvarInscritosAsync(CodafListaPresencaCadastroDto codafListaPresencaCadastroDto, long codafListaPresencaId)
        {
            if (codafListaPresencaCadastroDto.Inscritos is not null && codafListaPresencaCadastroDto.Inscritos.Any())
            {
                var inscritosListaPresenca = mapper.Map<IEnumerable<CodafInscricaoListaPresenca>>(codafListaPresencaCadastroDto.Inscritos);
                foreach (var inscrito in inscritosListaPresenca)
                {
                    inscrito.CodafListaPresencaId = codafListaPresencaId;
                }
                await repositorioCodafInscritosListaPresenca.InserirVariosAsync(inscritosListaPresenca);
            }
        }

        private async Task SalvarRetificacoesAsync(CodafListaPresencaCadastroDto codafListaPresencaCadastroDto, long codafListaPresencaId)
        {
            if (codafListaPresencaCadastroDto.Retificacoes is null || !codafListaPresencaCadastroDto.Retificacoes.Any())
                return;

            var retificacoes = mapper.Map<IEnumerable<CodafRetificacaoListaPresenca>>(codafListaPresencaCadastroDto.Retificacoes);
            foreach (var retificacao in retificacoes)
            {
                retificacao.CodafListaPresencaId = codafListaPresencaId;
                await repositorioCodafRetificacaoListaPresenca.Inserir(retificacao);
            }
        }
    }
}