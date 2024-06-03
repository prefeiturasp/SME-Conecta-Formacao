using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.ImportacaoArquivo;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Aplicacao.Interfaces.ImportacaoArquivo;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Servicos.Utilitarios;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.ImportacaoInscricao
{
    public class CasoDeUsoImportacaoInscricaoCursistaValidarItem : CasoDeUsoAbstrato, ICasoDeUsoImportacaoInscricaoCursistaValidarItem
    {
        private readonly IMapper _mapper;

        public CasoDeUsoImportacaoInscricaoCursistaValidarItem(IMediator mediator, IMapper mapper) : base(mediator)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<bool> Executar(MensagemRabbit param)
        {
            var importacaoArquivoRegistro = param.ObterObjetoMensagem<ImportacaoArquivoRegistroDTO>() ??
                throw new NegocioException(MensagemNegocio.IMPORTACAO_ARQUIVO_REGISTRO_NAO_LOCALIZADA);

            try
            {
                var importacaoInscricaoCursista = importacaoArquivoRegistro.Conteudo.JsonParaObjeto<InscricaoCursistaImportacaoDTO>();

                var propostaTurma = await mediator.Send(new ObterPropostaTurmaPorNomeQuery(importacaoInscricaoCursista.Turma, importacaoArquivoRegistro.PropostaId)) ??
                    throw new NegocioException(MensagemNegocio.TURMA_NAO_ENCONTRADA);

                var usuario = await ObterUsuarioPorLogin(importacaoInscricaoCursista) ??
                    throw new NegocioException(MensagemNegocio.USUARIO_NAO_ENCONTRADO);

                await mediator.Send(new UsuarioEstaInscritoNaPropostaQuery(propostaTurma.PropostaId, usuario.Id));

                var tipoVinculo = int.Parse(importacaoInscricaoCursista.Vinculo);

                var inscricao = new Dominio.Entidades.Inscricao()
                {
                    PropostaTurmaId = propostaTurma.Id,
                    UsuarioId = usuario.Id,
                    Situacao = SituacaoInscricao.AguardandoAnalise,
                    Origem = OrigemInscricao.Manual,
                    TipoVinculo = tipoVinculo,
                };

                if (usuario.Tipo == TipoUsuario.Interno)
                    await MapearValidarCargoFuncao(inscricao, usuario.Login, propostaTurma.PropostaId, tipoVinculo);

                importacaoInscricaoCursista.Inscricao = inscricao;

                await mediator.Send(new AlterarImportacaoRegistroCommand(importacaoArquivoRegistro.Id, SituacaoImportacaoArquivoRegistro.Validado, importacaoInscricaoCursista.ObjetoParaJson()));
            }
            catch (NegocioException e)
            {
                await mediator.Send(new AlterarSituacaoImportacaoArquivoRegistroCommand(importacaoArquivoRegistro.Id, SituacaoImportacaoArquivoRegistro.Erro, e.Message));
            }

            await AlterarSituacaoArquivo(importacaoArquivoRegistro.ImportacaoArquivoId);

            return true;
        }

        private async Task MapearValidarCargoFuncao(Dominio.Entidades.Inscricao inscricao, string login, long propostaId, int tipoVinculo)
        {
            var temErroCargo = false;
            var temErroFuncao = false;
            var cargoFuncaoUsuarioEol = await mediator.Send(new ObterCargosFuncoesDresFuncionarioServicoEolQuery(login));

            cargoFuncaoUsuarioEol = cargoFuncaoUsuarioEol.Where(t =>
               t.TipoVinculoCargoSobreposto == tipoVinculo ||
               t.TipoVinculoCargoBase == tipoVinculo || 
               t.TipoVinculoFuncaoAtividade == tipoVinculo);

            var cargosProposta = await mediator.Send(new ObterPropostaPublicosAlvosPorIdQuery(propostaId));
            if (cargosProposta.PossuiElementos())
            {
                foreach (var cargoEol in cargoFuncaoUsuarioEol)
                {
                    long codigoCargo = 0;
                    string codigoDre, codigoUe;

                    if(cargoEol.CdCargoSobreposto.HasValue && cargoEol.TipoVinculoCargoSobreposto == tipoVinculo)
                    {
                        codigoCargo = cargoEol.CdCargoSobreposto.Value;
                        codigoDre = cargoEol.CdDreCargoSobreposto;
                        codigoUe = cargoEol.CdUeCargoSobreposto;
                    }
                    else if (cargoEol.CdCargoBase.HasValue && cargoEol.TipoVinculoCargoBase == tipoVinculo)
                    {
                        codigoCargo = cargoEol.CdCargoBase.Value;
                        codigoDre = cargoEol.CdDreCargoBase;
                        codigoUe = cargoEol.CdUeCargoBase;
                    }
                    else
                        continue;

                    var cargoFuncao = await mediator.Send(new ObterCargoFuncaoPorCodigoEolQuery(new long[] { codigoCargo }, new long[] { }));

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

                if (cargosProposta.PossuiElementos())
                {
                    var cargoFuncaoOutros = await mediator.Send(ObterCargoFuncaoOutrosQuery.Instancia());
                    var cargoEhOutros = cargosProposta.Any(t => t.CargoFuncaoId == cargoFuncaoOutros.Id);
                    if (!cargoEhOutros && (!inscricao.CargoId.HasValue || !cargosProposta.Any(a => a.CargoFuncaoId == inscricao.CargoId)))
                        temErroCargo = true;
                }
            }

            var funcaoAtividadeProposta = await mediator.Send(new ObterPropostaFuncoesEspecificasPorIdQuery(propostaId));
            if (funcaoAtividadeProposta.PossuiElementos())
            {
                foreach (var funcaoEol in cargoFuncaoUsuarioEol.Where(t => t.CdFuncaoAtividade.HasValue && t.TipoVinculoFuncaoAtividade == tipoVinculo))
                {
                    var codigoCargo = funcaoEol.CdCargoSobreposto.HasValue ? funcaoEol.CdCargoSobreposto.Value : funcaoEol.CdCargoBase.Value;
                    var codigoDre = funcaoEol.CdCargoSobreposto.HasValue ? funcaoEol.CdDreCargoSobreposto : funcaoEol.CdDreCargoBase;
                    var codigoUe = funcaoEol.CdCargoSobreposto.HasValue ? funcaoEol.CdUeCargoSobreposto : funcaoEol.CdUeCargoBase;

                    var cargoFuncao = await mediator.Send(new ObterCargoFuncaoPorCodigoEolQuery(
                        new long[] { codigoCargo }, new long[] { funcaoEol.CdFuncaoAtividade.Value }));

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

                if (funcaoAtividadeProposta.PossuiElementos())
                {
                    if (!inscricao.FuncaoId.HasValue || !funcaoAtividadeProposta.Any(a => a.CargoFuncaoId == inscricao.FuncaoId))
                        temErroFuncao = true;
                }
            }

            if (temErroCargo && temErroFuncao)
                throw new NegocioException(MensagemNegocio.CURSISTA_NAO_POSSUI_CARGO_PUBLI_ALVO_FORMACAO_INSCRICAO_MANUAL);

            if (!funcaoAtividadeProposta.PossuiElementos() && temErroCargo)
                throw new NegocioException(MensagemNegocio.CURSISTA_NAO_POSSUI_CARGO_PUBLI_ALVO_FORMACAO_INSCRICAO_MANUAL);
        }

        private async Task<Dominio.Entidades.Usuario> ObterUsuarioPorLogin(InscricaoCursistaImportacaoDTO inscricaoCursistaDTO)
        {
            var ehProfissionalRede = inscricaoCursistaDTO.ColaboradorRede.EhColaboradorRede();
            var login = ehProfissionalRede ? inscricaoCursistaDTO.RegistroFuncional : inscricaoCursistaDTO.Cpf;

            if (login.NaoEstaPreenchido())
                throw new NegocioException(MensagemNegocio.USUARIO_NAO_FOI_ENCONTRADO_COM_O_REGISTRO_FUNCIONAL_OU_CPF_INFORMADOS);

            if (ehProfissionalRede)
            {
                if (login.Length < 7)
                    throw new NegocioException(MensagemNegocio.RF_MENOR_QUE_7_DIGITOS);
            }
            else
            {
                if (!UtilValidacoes.CpfEhValido(login))
                    throw new NegocioException(MensagemNegocio.CPF_INVALIDO);
            }

            var usuario = await mediator.Send(new ObterUsuarioPorLoginQuery(login.SomenteNumeros()));
            if (usuario.NaoEhNulo())
                return usuario;

            if (ehProfissionalRede)
            {
                var dadosUsuario = await mediator.Send(new ObterMeusDadosServicoAcessosPorLoginQuery(login));
                if (dadosUsuario.EhNulo() || string.IsNullOrEmpty(dadosUsuario.Login))
                    return default;

                usuario = _mapper.Map<Dominio.Entidades.Usuario>(dadosUsuario);
                usuario.Cpf = inscricaoCursistaDTO.Cpf.SomenteNumeros();
                usuario.Tipo = TipoUsuario.Interno;

                await mediator.Send(new SalvarUsuarioCommand(usuario));
            }

            return usuario;
        }

        private async Task AlterarSituacaoArquivo(long importacaoArquivoId)
        {
            var possuiRegistroCarregamentoInicial = await mediator.Send(new PossuiRegistroPorArquivoSituacaoQuery(importacaoArquivoId, SituacaoImportacaoArquivoRegistro.CarregamentoInicial));
            var possuiRegistrosNaFila = await mediator.Send(new ObterTotalRegistroFilaQuery(RotasRabbit.RealizarImportacaoInscricaoCursistaValidarItem)) > 0;

            if (!possuiRegistroCarregamentoInicial || !possuiRegistrosNaFila)
                await mediator.Send(new AlterarSituacaoImportacaoArquivoCommand(importacaoArquivoId, SituacaoImportacaoArquivo.Validado));
        }
    }
}
