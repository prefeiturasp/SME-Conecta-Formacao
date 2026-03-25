using AutoMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SME.ConectaFormacao.Aplicacao.Comandos.Email.InscricaoEmEspera;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Dominio.Servicos.Interfaces;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Servicos.Eol;
using SME.ConectaFormacao.Infra.Servicos.Log;
using System.Diagnostics;

namespace SME.ConectaFormacao.Aplicacao.Comandos.Inscricoes.SalvarInscricao
{
    public class SalvarInscricaoCommandHandler(
        IMapper mapper, IMediator mediator, IRepositorioInscricao repositorioInscricao,
        ITransacao transacao, IUsuarioAcessibilidadeService usuarioAcessibilidadeService,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<SalvarInscricaoCommandHandler> logger,
        IServicoLogs servicoLogs) :
        IRequestHandler<SalvarInscricaoCommand, RetornoDTO>
    {
        private readonly ITransacao _transacao = transacao;

        public async Task<RetornoDTO> Handle(SalvarInscricaoCommand request, CancellationToken cancellationToken)
        {
            var sw = Stopwatch.StartNew();
            var login = request.InscricaoDto.UsuarioLogin;
            logger.LogInformation("[SalvarInscricao] Início. Login={Login}", login);

            var usuarioLogado = await mediator.Send(new ObterUsuarioPorLoginQuery(login), cancellationToken);
            logger.LogInformation("[SalvarInscricao] ObterUsuario: {Elapsed}ms. Login={Login}", sw.ElapsedMilliseconds, login);

            if(usuarioLogado == null)
                throw new NegocioException(MensagemNegocio.USUARIO_NAO_ENCONTRADO);

            /*var usuarioLogado = string.IsNullOrWhiteSpace(request.InscricaoDto.UsuarioLogin)
                ? await mediator.Send(new ObterUsuarioLogadoQuery(), cancellationToken)
                : await mediator.Send(new ObterUsuarioPorLoginQuery(request.InscricaoDto.UsuarioLogin), cancellationToken)
                ?? throw new NegocioException(MensagemNegocio.USUARIO_NAO_ENCONTRADO);*/

            if (usuarioLogado.Tipo.EhInterno() && string.IsNullOrWhiteSpace(request.InscricaoDto.CargoCodigo))
                throw new NegocioException(MensagemNegocio.INFORME_O_CARGO);

            var inscricao = mapper.Map<Inscricao>(request.InscricaoDto);
            inscricao.UsuarioId = usuarioLogado.Id;
            inscricao.Situacao = request.InscricaoDto.VagaRemanescente ? SituacaoInscricao.EmEspera : SituacaoInscricao.AguardandoAnalise;
            inscricao.Origem = OrigemInscricao.Manual;

            await MapearCargoFuncao(inscricao, cancellationToken);
            logger.LogInformation("[SalvarInscricao] MapearCargoFuncao: {Elapsed}ms. Login={Login}", sw.ElapsedMilliseconds, login);

            var propostaTurma = await mediator.Send(new ObterPropostaTurmaPorIdQuery(inscricao.PropostaTurmaId), cancellationToken) ??
                                throw new NegocioException(MensagemNegocio.TURMA_NAO_ENCONTRADA);
            logger.LogInformation("[SalvarInscricao] ObterPropostaTurma: {Elapsed}ms. Login={Login}", sw.ElapsedMilliseconds, login);

            await ValidarExisteInscricaoNaProposta(propostaTurma.PropostaId, inscricao.UsuarioId);
            logger.LogInformation("[SalvarInscricao] ValidarInscricaoExistente: {Elapsed}ms. Login={Login}", sw.ElapsedMilliseconds, login);

            if (usuarioLogado.Tipo == TipoUsuario.Interno)
            {
                if (!request.InscricaoDto.VagaRemanescente)
                {
                    await ValidarCargoFuncao(propostaTurma.PropostaId, inscricao.CargoId, inscricao.FuncaoId, cancellationToken);
                    logger.LogInformation("[SalvarInscricao] ValidarCargoFuncao: {Elapsed}ms. Login={Login}", sw.ElapsedMilliseconds, login);
                }

                await ValidarDreUsuarioInterno(usuarioLogado.Login, inscricao, cancellationToken);
                logger.LogInformation("[SalvarInscricao] ValidarDreInterno: {Elapsed}ms. Login={Login}", sw.ElapsedMilliseconds, login);
            }
            else
            {
                await ValidarDreUsuarioExterno(inscricao.PropostaTurmaId, usuarioLogado.CodigoEolUnidade, cancellationToken);
                logger.LogInformation("[SalvarInscricao] ValidarDreExterno: {Elapsed}ms. Login={Login}", sw.ElapsedMilliseconds, login);
            }

            var proposta = await mediator.Send(new ObterPropostaPorIdQuery(propostaTurma.PropostaId), cancellationToken) ??
                throw new NegocioException(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA);
            logger.LogInformation("[SalvarInscricao] ObterProposta: {Elapsed}ms. Login={Login}", sw.ElapsedMilliseconds, login);

            var formacaoHomologada = proposta.FormacaoHomologada == FormacaoHomologada.Sim;

            await PersistirInscricao(formacaoHomologada, inscricao, login);
            logger.LogInformation("[SalvarInscricao] PersistirInscricao: {Elapsed}ms. Login={Login}", sw.ElapsedMilliseconds, login);

            EnviarEmailInscricaoAsync(inscricao);

            var mensagem = DefinirMensagemInscricao(formacaoHomologada, proposta.IntegrarNoSGA, inscricao);
            logger.LogInformation("[SalvarInscricao] Concluído em {Elapsed}ms. Login={Login} InscricaoId={InscricaoId}", sw.ElapsedMilliseconds, login, inscricao.Id);
            return new RetornoDTO { Mensagem = mensagem, EntidadeId = inscricao.Id };
        }

        private async Task MapearCargoFuncao(Inscricao inscricao, CancellationToken cancellationToken)
        {
            var codigosFuncoesEol = !string.IsNullOrWhiteSpace(inscricao.FuncaoCodigo) ? [long.Parse(inscricao.FuncaoCodigo)] : Enumerable.Empty<long>();
            var codigosCargosEol = !string.IsNullOrWhiteSpace(inscricao.CargoCodigo) ? [long.Parse(inscricao.CargoCodigo)] : Enumerable.Empty<long>();
            if (codigosFuncoesEol.PossuiElementos() || codigosCargosEol.PossuiElementos())
            {
                var cargosFuncoes = await mediator.Send(new ObterCargoFuncaoPorCodigoEolQuery(codigosCargosEol, codigosFuncoesEol), cancellationToken);

                inscricao.CargoId = cargosFuncoes.FirstOrDefault(f => f.Tipo == CargoFuncaoTipo.Cargo)?.Id;
                inscricao.FuncaoId = cargosFuncoes.FirstOrDefault(f => f.Tipo == CargoFuncaoTipo.Funcao)?.Id;
            }
        }

        private async Task ValidarExisteInscricaoNaProposta(long propostaId, long usuarioId)
        {
            var possuiInscricaoNaProposta = await repositorioInscricao.UsuarioEstaInscritoNaProposta(propostaId, usuarioId);
            if (possuiInscricaoNaProposta)
                throw new NegocioException(MensagemNegocio.USUARIO_JA_INSCRITO_NA_PROPOSTA);
        }

        private async Task ValidarCargoFuncao(long propostaId, long? cargoId, long? funcaoId, CancellationToken cancellationToken)
        {
            var temErroCargo = false;
            var temErroFuncao = false;
            var cargosProposta = await mediator.Send(new ObterPropostaPublicosAlvosPorIdQuery(propostaId), cancellationToken);
            var funcaoAtividadeProposta = await mediator.Send(new ObterPropostaFuncoesEspecificasPorIdQuery(propostaId), cancellationToken);

            if (cargosProposta.PossuiElementos())
            {
                var cargoFuncaoOutros = await mediator.Send(new ObterCargoFuncaoOutrosQuery(), cancellationToken);
                var cargoEhOutros = cargosProposta.Any(t => t.CargoFuncaoId == cargoFuncaoOutros.Id);

                if (cargoId.HasValue && !cargoEhOutros && !cargosProposta.Any(a => a.CargoFuncaoId == cargoId))
                    temErroCargo = true;

            }

            if (funcaoAtividadeProposta.PossuiElementos())
            {
                if (funcaoId.HasValue && !funcaoAtividadeProposta.Any(a => a.CargoFuncaoId == funcaoId))
                    temErroFuncao = true;
            }

            if (temErroCargo && temErroFuncao)
                throw new NegocioException(MensagemNegocio.USUARIO_NAO_POSSUI_CARGO_PUBLI_ALVO_FORMACAO);

            if (!funcaoAtividadeProposta.PossuiElementos() && temErroCargo)
                throw new NegocioException(MensagemNegocio.USUARIO_NAO_POSSUI_CARGO_PUBLI_ALVO_FORMACAO);
        }

        private async Task ValidarDreUsuarioInterno(string registroFuncional, Inscricao inscricao, CancellationToken cancellationToken)
        {
            var dres = await mediator.Send(new ObterPropostaTurmaDresPorPropostaTurmaIdQuery(inscricao.PropostaTurmaId), cancellationToken);
            dres = dres.Where(t => !t.Dre.Todos);
            if (dres.PossuiElementos())
            {
                var dreUeAtribuicoes = await mediator.Send(new ObterDreUeAtribuicaoPorRegistroFuncionalCodigoCargoQuery(registroFuncional, inscricao.CargoCodigo), cancellationToken);
                if (dreUeAtribuicoes.PossuiElementos())
                {
                    var dreUeAtribuicao = dreUeAtribuicoes.FirstOrDefault(f => dres.Any(d => d.DreCodigo == f.DreCodigo)) ??
                                          dreUeAtribuicoes.First();
                    inscricao.CargoDreCodigo = dreUeAtribuicao.DreCodigo;
                    inscricao.CargoUeCodigo = dreUeAtribuicao.UeCodigo;
                }

                if ((!string.IsNullOrWhiteSpace(inscricao.CargoDreCodigo) && !dres.Any(a => a.Dre.Codigo == inscricao.CargoDreCodigo)) ||
                    (!string.IsNullOrWhiteSpace(inscricao.FuncaoDreCodigo) && !dres.Any(a => a.Dre.Codigo == inscricao.FuncaoDreCodigo)))
                    throw new NegocioException(MensagemNegocio.USUARIO_SEM_LOTACAO_NA_DRE_DA_TURMA);
            }
        }

        private async Task ValidarDreUsuarioExterno(long propostaTurmaId, string? codigoEolUnidade, CancellationToken cancellationToken)
        {
            var dres = await mediator.Send(new ObterPropostaTurmaDresPorPropostaTurmaIdQuery(propostaTurmaId), cancellationToken);
            dres = dres.Where(t => !t.Dre.Todos);
            if (dres.PossuiElementos())
            {
                var unidade = await mediator.Send(new ObterUnidadePorCodigoEOLQuery(codigoEolUnidade), cancellationToken);
                var codigosUndAdmReferencia = ObterCodigoUnidadeAdmReferenciaEscola(unidade)
                                              ?? ObterCodigosUnidadeAdmEReferencia(unidade);

                if (!dres.Any(t => codigosUndAdmReferencia.Contains(t.Dre.Codigo)))
                    throw new NegocioException(MensagemNegocio.USUARIO_SEM_LOTACAO_NA_DRE_DA_TURMA);
            }
        }

        private static string[]? ObterCodigoUnidadeAdmReferenciaEscola(UnidadeEol unidade)
            => unidade.Tipo == UnidadeEolTipo.Escola
               ? [unidade.CodigoReferencia]
               : null;

        private static string[] ObterCodigosUnidadeAdmEReferencia(UnidadeEol unidade)
            => [unidade.Codigo, unidade.CodigoReferencia];

        private async Task PersistirInscricao(bool formacaoHomologada, Inscricao inscricao, string login)
        {
            var sw = Stopwatch.StartNew();
            var transacaoDb = _transacao.Iniciar();
            try
            {
                await SalvarAcessibilidadeAsync(inscricao, inscricao.UsuarioAcessibilidade);
                logger.LogInformation("[SalvarInscricao][DB] SalvarAcessibilidade: {Elapsed}ms. Login={Login}", sw.ElapsedMilliseconds, login);

                await repositorioInscricao.Inserir(inscricao);
                logger.LogInformation("[SalvarInscricao][DB] InserirInscricao: {Elapsed}ms. Login={Login}", sw.ElapsedMilliseconds, login);

                if (!formacaoHomologada)
                {
                    bool confirmada = await repositorioInscricao.ConfirmarInscricaoVaga(inscricao);
                    logger.LogInformation("[SalvarInscricao][DB] ConfirmarVaga: {Elapsed}ms. Confirmada={Confirmada} Login={Login}", sw.ElapsedMilliseconds, confirmada, login);

                    if (!confirmada)
                        throw new NegocioException(MensagemNegocio.INSCRICAO_NAO_CONFIRMADA_POR_FALTA_DE_VAGA);

                    inscricao.Situacao = SituacaoInscricao.Confirmada;
                    await repositorioInscricao.Atualizar(inscricao);
                    logger.LogInformation("[SalvarInscricao][DB] AtualizarSituacao: {Elapsed}ms. Login={Login}", sw.ElapsedMilliseconds, login);
                }

                transacaoDb.Commit();
                logger.LogInformation("[SalvarInscricao][DB] Commit: {Elapsed}ms. Login={Login}", sw.ElapsedMilliseconds, login);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[SalvarInscricao][DB] Erro após {Elapsed}ms. Login={Login} InscricaoId={InscricaoId}", sw.ElapsedMilliseconds, login, inscricao.Id);
                await servicoLogs.Enviar(ex, $"[SalvarInscricao][DB] Erro após {sw.ElapsedMilliseconds}ms. Login={login} InscricaoId={inscricao.Id}", LogContexto.Geral, LogNivel.Critico);
                transacaoDb.Rollback();
                throw;
            }
            finally
            {
                transacaoDb.Dispose();
            }
        }

        private async Task SalvarAcessibilidadeAsync(Inscricao inscricao, UsuarioAcessibilidade? usuarioAcessibilidade)
        {
            usuarioAcessibilidade ??= new();
            usuarioAcessibilidade.UsuarioId = inscricao.UsuarioId;
            inscricao.UsuarioAcessibilidadeId =
                await usuarioAcessibilidadeService.SalvarAcessibilidadeDaInscricaoAsync(usuarioAcessibilidade);
        }

        private void EnviarEmailInscricaoAsync(Inscricao inscricao)
        {
            var inscricaoId = inscricao.Id;
            var situacao = inscricao.Situacao;

            _ = Task.Run(async () =>
            {
                using var scope = serviceScopeFactory.CreateScope();
                var mediatorScoped = scope.ServiceProvider.GetRequiredService<IMediator>();

                if (situacao == SituacaoInscricao.EmEspera)
                    await mediatorScoped.Send(new EnviarEmailInscricaoEmEsperaCommand(inscricaoId));
                else if (situacao == SituacaoInscricao.Confirmada)
                    await mediatorScoped.Send(new EnviarEmailConfirmacaoInscricaoCommand(inscricaoId));
            });
        }

        private static string DefinirMensagemInscricao(bool formacaoHomologada, bool integrarNoSGA, Inscricao inscricao)
        {
            if (inscricao.Situacao == SituacaoInscricao.EmEspera)
                return MensagemNegocio.INSCRICAO_EM_ESPERA;
            if (!formacaoHomologada)
                return MensagemNegocio.INSCRICAO_CONFIRMADA;
            if (integrarNoSGA)
                return MensagemNegocio.INSCRICAO_CONFIRMADA_NA_DATA_INICIO_DA_SUA_TURMA;
            return MensagemNegocio.INSCRICAO_EM_ANALISE;
        }
    }
}
