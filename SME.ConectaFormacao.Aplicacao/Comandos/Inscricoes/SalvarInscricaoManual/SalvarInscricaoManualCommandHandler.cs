using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Comandos.Inscricoes.SalvarInscricaoManual
{
    public class SalvarInscricaoManualCommandHandler(
        IMapper mapper, IMediator mediator, 
        IRepositorioInscricao repositorioInscricao, ITransacao transacao) : IRequestHandler<SalvarInscricaoManualCommand, RetornoDTO>
    {
        private readonly ITransacao _transacao = transacao;

        public async Task<RetornoDTO> Handle(SalvarInscricaoManualCommand request, CancellationToken cancellationToken)
        {
            var usuario = await ObterUsuarioPorLogin(request.InscricaoManualDTO, cancellationToken) ??
                throw new NegocioException(MensagemNegocio.USUARIO_NAO_ENCONTRADO);

            if (!request.EhTransferencia)
            {
                if (usuario.Tipo.EhInterno() && string.IsNullOrWhiteSpace(request.InscricaoManualDTO.CargoCodigo))
                    throw new NegocioException(MensagemNegocio.INFORME_O_CARGO);
            }

            var inscricao = mapper.Map<Inscricao>(request.InscricaoManualDTO);
            inscricao.UsuarioId = usuario.Id;
            inscricao.Situacao = SituacaoInscricao.AguardandoAnalise;
            inscricao.Origem = OrigemInscricao.Manual;

            await MapearCargoFuncao(inscricao, cancellationToken);

            var propostaTurma = await mediator.Send(new ObterPropostaTurmaPorIdQuery(inscricao.PropostaTurmaId), cancellationToken) ??
                    throw new NegocioException(MensagemNegocio.TURMA_NAO_ENCONTRADA);

            var proposta = await mediator.Send(new ObterPropostaPorIdQuery(propostaTurma.PropostaId), cancellationToken) ??
                    throw new NegocioException(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA);

            if (usuario.Tipo.EhInterno())
            {
                if (proposta.FuncoesEspecificas != null && proposta.FuncoesEspecificas.Any())
                    await ValidarCargoFuncao(propostaTurma.PropostaId, inscricao.CargoId, inscricao.FuncaoId, cancellationToken);

                var possuiErros = await ValidarSeDreUsuarioInternoPossuiErros(usuario.Login, inscricao, cancellationToken);
                if (!request.InscricaoManualDTO.PodeContinuar && possuiErros)
                    throw new NegocioException(request.EhTransferencia ? MensagemNegocio.USUARIO_SEM_LOTACAO_NA_DRE_DA_TURMA_TRANSFERENCIA : MensagemNegocio.USUARIO_SEM_LOTACAO_NA_DRE_DA_TURMA_INSCRICAO_MANUAL);
            }
            else
            {
                var possuiErros = await ValidarSeDreUsuarioExternoPossuiErros(inscricao.PropostaTurmaId, usuario.CodigoEolUnidade, cancellationToken);
                if (!request.InscricaoManualDTO.PodeContinuar && possuiErros)
                    throw new NegocioException(request.EhTransferencia ? MensagemNegocio.USUARIO_SEM_LOTACAO_NA_DRE_DA_TURMA_TRANSFERENCIA : MensagemNegocio.USUARIO_SEM_LOTACAO_NA_DRE_DA_TURMA_INSCRICAO_MANUAL);
            }

            if (!request.EhTransferencia)
            {
                await ValidarExisteInscricaoNaProposta(propostaTurma.PropostaId, inscricao.UsuarioId);
                ValidaPeriodoDeInscricao(proposta);
            }

            return await PersistirInscricao(proposta.FormacaoHomologada == FormacaoHomologada.Sim, inscricao);

        }

        private static void ValidaPeriodoDeInscricao(Proposta proposta)
        {
            var dataAtual = DateTimeExtension.HorarioBrasilia().Date;
            if (proposta.DataInscricaoInicio is null ||
                !(dataAtual >= proposta.DataInscricaoInicio.GetValueOrDefault().Date && 
                  dataAtual <= proposta.DataInscricaoFim.GetValueOrDefault().Date))
                throw new NegocioException(MensagemNegocio.INSCRICAO_FORA_DO_PERIODO_INSCRICAO);
        }

        private async Task<Usuario> ObterUsuarioPorLogin(InscricaoManualDTO inscricaoManualDTO, CancellationToken cancellationToken)
        {
            var login = inscricaoManualDTO.ProfissionalRede ? inscricaoManualDTO.RegistroFuncional : inscricaoManualDTO.Cpf;

            var usuario = await mediator.Send(new ObterUsuarioPorLoginQuery(login.SomenteNumeros()), cancellationToken);
            if (usuario.NaoEhNulo())
                return usuario;

            if (inscricaoManualDTO.ProfissionalRede)
            {
                var dadosUsuario = await mediator.Send(new ObterMeusDadosServicoAcessosPorLoginQuery(login), cancellationToken);
                if (dadosUsuario.EhNulo())
                    return default!;

                usuario = mapper.Map<Usuario>(dadosUsuario);
                usuario.Cpf = inscricaoManualDTO.Cpf.SomenteNumeros();
                usuario.Tipo = TipoUsuario.Interno;

                await mediator.Send(new SalvarUsuarioCommand(usuario), cancellationToken);
            }

            return usuario;
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

        private async Task ValidarCargoFuncao(long propostaId, long? cargoId, long? funcaoId, CancellationToken cancellationToken)
        {
            var temErroCargo = false;
            var temErroFuncao = false;
            var cargosProposta = await mediator.Send(new ObterPropostaPublicosAlvosPorIdQuery(propostaId), cancellationToken);
            var funcaoAtividadeProposta = await mediator.Send(new ObterPropostaFuncoesEspecificasPorIdQuery(propostaId), cancellationToken);

            if (cargosProposta.PossuiElementos())
            {
                var cargoFuncaoOutros = await mediator.Send(ObterCargoFuncaoOutrosQuery.Instancia(), cancellationToken);
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

        private async Task<bool> ValidarSeDreUsuarioInternoPossuiErros(string registroFuncional, Inscricao inscricao, CancellationToken cancellationToken)
        {
            var dres = await mediator.Send(new ObterPropostaTurmaDresPorPropostaTurmaIdQuery(inscricao.PropostaTurmaId), cancellationToken);
            var existeTodos = dres.Any(t => t.Dre.Todos);
            dres = dres.Where(t => !t.Dre.Todos);
            if (dres.PossuiElementos())
            {
                var dreUeAtribuicoes = await mediator.Send(new ObterDreUeAtribuicaoPorRegistroFuncionalCodigoCargoQuery(registroFuncional, inscricao.CargoCodigo), cancellationToken);
                if (dreUeAtribuicoes.Any())
                {
                    var dreUeAtribuicao = dreUeAtribuicoes.FirstOrDefault(f => dres.Any(d => d.DreCodigo == f.DreCodigo)) ?? 
                                          dreUeAtribuicoes.First();
                    inscricao.CargoDreCodigo = dreUeAtribuicao.DreCodigo;
                    inscricao.CargoUeCodigo = dreUeAtribuicao.UeCodigo;
                }

                if ((!string.IsNullOrWhiteSpace(inscricao.CargoDreCodigo) && 
                     !dres.Any(a => a.Dre.Codigo == inscricao.CargoDreCodigo)) ||
                    (!string.IsNullOrWhiteSpace(inscricao.FuncaoDreCodigo) && 
                     !dres.Any(a => a.Dre.Codigo == inscricao.FuncaoDreCodigo)))
                    return true;

                return false;
            }

            return !existeTodos;
        }

        private async Task<bool> ValidarSeDreUsuarioExternoPossuiErros(long propostaTurmaId, string? codigoEolUnidade, CancellationToken cancellationToken)
        {
            var dres = await mediator.Send(new ObterPropostaTurmaDresPorPropostaTurmaIdQuery(propostaTurmaId), cancellationToken);
            dres = dres.Where(t => !t.Dre.Todos);
            if (dres.PossuiElementos())
            {
                var unidade = await mediator.Send(new ObterUnidadePorCodigoEOLQuery(codigoEolUnidade), cancellationToken);

                var codigo = unidade.Tipo == Infra.Servicos.Eol.UnidadeEolTipo.Escola ? unidade.CodigoReferencia : unidade.Codigo;
                if (!dres.Any(t => t.Dre.Codigo == codigo))
                    return true;

                return false;
            }
            return true;
        }

        private async Task ValidarExisteInscricaoNaProposta(long propostaId, long usuarioId)
        {
            var possuiInscricaoNaProposta = await repositorioInscricao.UsuarioEstaInscritoNaProposta(propostaId, usuarioId);
            if (possuiInscricaoNaProposta)
                throw new NegocioException(MensagemNegocio.USUARIO_JA_INSCRITO_NA_PROPOSTA);
        }

        private async Task<RetornoDTO> PersistirInscricao(bool formacaoHomologada, Inscricao inscricao)
        {
            var transacao = _transacao.Iniciar();
            try
            {
                await repositorioInscricao.Inserir(inscricao);

                if (!formacaoHomologada)
                {
                    bool confirmada = await repositorioInscricao.ConfirmarInscricaoVaga(inscricao);
                    if (!confirmada)
                        throw new NegocioException(MensagemNegocio.INSCRICAO_NAO_CONFIRMADA_POR_FALTA_DE_VAGA);

                    inscricao.Situacao = SituacaoInscricao.Confirmada;
                    await repositorioInscricao.Atualizar(inscricao);
                }

                transacao.Commit();

                return RetornoDTO.RetornarSucesso(MensagemNegocio.INSCRICAO_MANUAL_REALIZADA_COM_SUCESSO, inscricao.Id);
            }
            catch
            {
                transacao.Rollback();
                throw;
            }
            finally
            {
                transacao.Dispose();
            }
        }
    }
}
