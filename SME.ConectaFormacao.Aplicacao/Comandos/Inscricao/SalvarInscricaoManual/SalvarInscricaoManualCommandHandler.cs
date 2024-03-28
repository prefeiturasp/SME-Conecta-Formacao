using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarInscricaoManualCommandHandler : IRequestHandler<SalvarInscricaoManualCommand, RetornoDTO>
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IRepositorioInscricao _repositorioInscricao;
        private readonly ITransacao _transacao;

        public SalvarInscricaoManualCommandHandler(IMapper mapper, IMediator mediator, IRepositorioInscricao repositorioInscricao, ITransacao transacao)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _repositorioInscricao = repositorioInscricao ?? throw new ArgumentNullException(nameof(repositorioInscricao));
            _transacao = transacao ?? throw new ArgumentNullException(nameof(transacao));
        }

        public async Task<RetornoDTO> Handle(SalvarInscricaoManualCommand request, CancellationToken cancellationToken)
        {
            var usuario = await ObterUsuarioPorLogin(request.InscricaoManualDTO, cancellationToken) ??
                throw new NegocioException(MensagemNegocio.USUARIO_NAO_ENCONTRADO);

            var inscricao = new Inscricao()
            {
                PropostaTurmaId = request.InscricaoManualDTO.PropostaTurmaId,
                UsuarioId = usuario.Id,
                Situacao = SituacaoInscricao.EmAnalise
            };

            var propostaTurma = await _mediator.Send(new ObterPropostaTurmaPorIdQuery(inscricao.PropostaTurmaId), cancellationToken) ??
                    throw new NegocioException(MensagemNegocio.TURMA_NAO_ENCONTRADA);

            if (usuario.Tipo == TipoUsuario.Interno)
            {
                await MapearValidarCargoFuncao(inscricao, usuario.Login, propostaTurma.PropostaId, cancellationToken);

                var possuiErros = await ValidarSeDreDreUsuarioInternoPossuiErros(usuario.Login, inscricao, cancellationToken);
                if (!request.InscricaoManualDTO.PodeContinuar && possuiErros)
                    throw new NegocioException(MensagemNegocio.USUARIO_SEM_LOTACAO_NA_DRE_DA_TURMA_INSCRICAO_MANUAL);
            }
            else
            {
                var possuiErros =  await ValidarSeDreUsuarioExternoPossuiErros(inscricao.PropostaTurmaId, usuario.CodigoEolUnidade, cancellationToken);
                if (!request.InscricaoManualDTO.PodeContinuar && possuiErros)
                    throw new NegocioException(MensagemNegocio.USUARIO_SEM_LOTACAO_NA_DRE_DA_TURMA_INSCRICAO_MANUAL);
            }

            await ValidarExisteInscricaoNaProposta(propostaTurma.PropostaId, inscricao.UsuarioId);

            var proposta = await _mediator.Send(new ObterPropostaPorIdQuery(propostaTurma.PropostaId), cancellationToken) ??
                throw new NegocioException(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA);

            ValidaPeriodoDeInscricao(proposta);

            return await PersistirInscricao(proposta.FormacaoHomologada == FormacaoHomologada.Sim, inscricao, proposta.IntegrarNoSGA);
        }

        private void ValidaPeriodoDeInscricao(Proposta proposta)
        {
            var dataAtual = DateTimeExtension.HorarioBrasilia().Date;
            if (proposta.DataInscricaoInicio.EhNulo() ||
                !(dataAtual >= proposta.DataInscricaoInicio.GetValueOrDefault().Date && dataAtual <= proposta.DataInscricaoFim.GetValueOrDefault().Date))
                throw new NegocioException(MensagemNegocio.INSCRICAO_FORA_DO_PERIODO_INSCRICAO);
        }

        private async Task<Usuario> ObterUsuarioPorLogin(InscricaoManualDTO inscricaoManualDTO, CancellationToken cancellationToken)
        {
            var login = inscricaoManualDTO.ProfissionalRede ? inscricaoManualDTO.RegistroFuncional : inscricaoManualDTO.Cpf;

            var usuario = await _mediator.Send(new ObterUsuarioPorLoginQuery(login.SomenteNumeros()), cancellationToken);
            if (usuario.NaoEhNulo())
                return usuario;

            if (inscricaoManualDTO.ProfissionalRede)
            {
                var dadosUsuario = await _mediator.Send(new ObterMeusDadosServicoAcessosPorLoginQuery(login), cancellationToken);
                if (dadosUsuario.EhNulo())
                    return default;

                usuario = _mapper.Map<Usuario>(dadosUsuario);
                usuario.Cpf = inscricaoManualDTO.Cpf.SomenteNumeros();
                usuario.Tipo = TipoUsuario.Interno;

                await _mediator.Send(new SalvarUsuarioCommand(usuario), cancellationToken);
            }

            return usuario;
        }

        private async Task MapearValidarCargoFuncao(Inscricao inscricao, string login, long propostaId, CancellationToken cancellationToken)
        {
            var cargoFuncaoUsuarioEol = await _mediator.Send(new ObterCargosFuncoesDresFuncionarioServicoEolQuery(login), cancellationToken);

            var cargosProposta = await _mediator.Send(new ObterPropostaPublicosAlvosPorIdQuery(propostaId), cancellationToken);
            if (cargosProposta.PossuiElementos())
            {
                foreach (var cargoEol in cargoFuncaoUsuarioEol)
                {
                    var codigoCargo = cargoEol.CdCargoSobreposto.HasValue ? cargoEol.CdCargoSobreposto.Value : cargoEol.CdCargoBase.Value;
                    var codigoDre = cargoEol.CdCargoSobreposto.HasValue ? cargoEol.CdDreCargoSobreposto : cargoEol.CdDreCargoBase;
                    var codigoUe = cargoEol.CdCargoSobreposto.HasValue ? cargoEol.CdUeCargoSobreposto : cargoEol.CdUeCargoBase;

                    var cargoFuncao = await _mediator.Send(new ObterCargoFuncaoPorCodigoEolQuery(new long[] { codigoCargo }, new long[] { }), cancellationToken);

                    var cargoId = cargoFuncao.FirstOrDefault(t => t.Tipo == CargoFuncaoTipo.Cargo)?.Id;
                    if (cargosProposta.Any(a => a.CargoFuncaoId == cargoId))
                    {
                        inscricao.CargoCodigo = codigoCargo.ToString();
                        inscricao.CargoDreCodigo = codigoDre;
                        inscricao.CargoUeCodigo = codigoUe;
                        inscricao.CargoId = cargoId;
                        break;
                    }
                }

                if (!inscricao.CargoId.HasValue)
                    throw new NegocioException(MensagemNegocio.USUARIO_NAO_POSSUI_CARGO_PUBLI_ALVO_FORMACAO);
            }

            var funcaoAtividadeProposta = await _mediator.Send(new ObterPropostaFuncoesEspecificasPorIdQuery(propostaId), cancellationToken);
            if (funcaoAtividadeProposta.PossuiElementos())
            {
                foreach (var funcaoEol in cargoFuncaoUsuarioEol.Where(t => t.CdFuncaoAtividade.HasValue))
                {
                    var codigoCargo = funcaoEol.CdCargoSobreposto.HasValue ? funcaoEol.CdCargoSobreposto.Value : funcaoEol.CdCargoBase.Value;
                    var codigoDre = funcaoEol.CdCargoSobreposto.HasValue ? funcaoEol.CdDreCargoSobreposto : funcaoEol.CdDreCargoBase;
                    var codigoUe = funcaoEol.CdCargoSobreposto.HasValue ? funcaoEol.CdUeCargoSobreposto : funcaoEol.CdUeCargoBase;

                    var cargoFuncao = await _mediator.Send(new ObterCargoFuncaoPorCodigoEolQuery(new long[] { codigoCargo }, new long[] { funcaoEol.CdFuncaoAtividade.Value }), cancellationToken);

                    var cargoId = cargoFuncao.FirstOrDefault(t => t.Tipo == CargoFuncaoTipo.Cargo)?.Id;
                    var funcaoId = cargoFuncao.FirstOrDefault(t => t.Tipo == CargoFuncaoTipo.Funcao)?.Id;

                    if (funcaoAtividadeProposta.Any(a => a.CargoFuncaoId == funcaoId))
                    {
                        inscricao.CargoCodigo = codigoCargo.ToString();
                        inscricao.CargoDreCodigo = codigoDre;
                        inscricao.CargoUeCodigo = codigoUe;
                        inscricao.CargoId = cargoId;

                        inscricao.FuncaoCodigo = funcaoEol.CdFuncaoAtividade.Value.ToString();
                        inscricao.FuncaoDreCodigo = funcaoEol.CdDreFuncaoAtividade.ToString();
                        inscricao.FuncaoUeCodigo = funcaoEol.CdUeFuncaoAtividade.ToString();
                        inscricao.FuncaoId = funcaoId;

                        break;
                    }
                }

                if (!inscricao.FuncaoId.HasValue)
                    throw new NegocioException(MensagemNegocio.USUARIO_NAO_POSSUI_CARGO_PUBLI_ALVO_FORMACAO);
            }
        }

        private async Task<bool> ValidarSeDreDreUsuarioInternoPossuiErros(string registroFuncional, Inscricao inscricao, CancellationToken cancellationToken)
        {
            var dres = await _mediator.Send(new ObterPropostaTurmaDresPorPropostaTurmaIdQuery(inscricao.PropostaTurmaId), cancellationToken);
            dres = dres.Where(t => !t.Dre.Todos);
            if (dres.PossuiElementos())
            {
                var dreUeAtribuicoes = await _mediator.Send(new ObterDreUeAtribuicaoPorRegistroFuncionalCodigoCargoQuery(registroFuncional, inscricao.CargoCodigo), cancellationToken);
                if (dreUeAtribuicoes.PossuiElementos())
                {
                    var dreUeAtribuicao = dreUeAtribuicoes.FirstOrDefault(f => dres.Any(d => d.DreCodigo == f.DreCodigo));
                    if (dreUeAtribuicao.EhNulo())
                        dreUeAtribuicao = dreUeAtribuicoes.FirstOrDefault();

                    inscricao.CargoDreCodigo = dreUeAtribuicao.DreCodigo;
                    inscricao.CargoUeCodigo = dreUeAtribuicao.UeCodigo;
                }

                if ((inscricao.CargoDreCodigo.EstaPreenchido() && !dres.Any(a => a.Dre.Codigo == inscricao.CargoDreCodigo)) ||
                    (inscricao.FuncaoDreCodigo.EstaPreenchido() && !dres.Any(a => a.Dre.Codigo == inscricao.FuncaoDreCodigo)))
                    return true;

                return false;
            }

            return true;
        }

        private async Task<bool> ValidarSeDreUsuarioExternoPossuiErros(long propostaTurmaId, string codigoEolUnidade, CancellationToken cancellationToken)
        {
            var dres = await _mediator.Send(new ObterPropostaTurmaDresPorPropostaTurmaIdQuery(propostaTurmaId), cancellationToken);
            dres = dres.Where(t => !t.Dre.Todos);
            if (dres.PossuiElementos())
            {
                var unidade = await _mediator.Send(new ObterUnidadePorCodigoEOLQuery(codigoEolUnidade), cancellationToken);

                var codigo = unidade.Tipo == Infra.Servicos.Eol.UnidadeEolTipo.Escola ? unidade.CodigoReferencia : unidade.Codigo;
                if (!dres.Any(t => t.Dre.Codigo == codigo))
                    return true;
                    
                return false;
            }
            return true;
        }

        private async Task ValidarExisteInscricaoNaProposta(long propostaId, long usuarioId)
        {
            var possuiInscricaoNaProposta = await _repositorioInscricao.UsuarioEstaInscritoNaProposta(propostaId, usuarioId);
            if (possuiInscricaoNaProposta)
                throw new NegocioException(MensagemNegocio.USUARIO_JA_INSCRITO_NA_PROPOSTA);
        }

        private async Task<RetornoDTO> PersistirInscricao(bool formacaoHomologada, Inscricao inscricao, bool integrarNoSGA)
        {
            var transacao = _transacao.Iniciar();
            try
            {
                await _repositorioInscricao.Inserir(inscricao);

                if (!formacaoHomologada)
                {
                    bool confirmada = await _repositorioInscricao.ConfirmarInscricaoVaga(inscricao);
                    if (!confirmada)
                        throw new NegocioException(MensagemNegocio.INSCRICAO_NAO_CONFIRMADA_POR_FALTA_DE_VAGA);

                    inscricao.Situacao = SituacaoInscricao.Confirmada;
                    await _repositorioInscricao.Atualizar(inscricao);
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
