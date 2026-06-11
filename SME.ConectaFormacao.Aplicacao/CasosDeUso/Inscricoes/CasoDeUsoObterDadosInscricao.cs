using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricoes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Servicos.Eol;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricoes
{
    public class CasoDeUsoObterDadosInscricao(IMediator mediator) : CasoDeUsoAbstrato(mediator), ICasoDeUsoObterDadosInscricao
    {
        public async Task<DadosInscricaoDto> Executar()
        {
            var usuarioLogado = await mediator.Send(new ObterUsuarioLogadoQuery());
            DadosInscricaoDto retorno;
            if (usuarioLogado.Tipo != TipoUsuario.Externo)
            {
                var cargosFuncoesEol = await mediator.Send(new ObterCargosFuncoesDresFuncionarioServicoEolQuery(usuarioLogado.Login));

                if (cargosFuncoesEol.Any())
                    await AtualizaCpfUsuario(usuarioLogado, cargosFuncoesEol);

                retorno = new DadosInscricaoDto
                {
                    UsuarioNome = usuarioLogado.Nome,
                    UsuarioCpf = (cargosFuncoesEol.Any() ? cargosFuncoesEol.First().Cpf : usuarioLogado.Login).AplicarMascara(@"000\.000\.000\-00"),
                    UsuarioEmail = usuarioLogado.EmailEducacional,
                    UsuarioRf = usuarioLogado.Login,
                    UsuarioCargos = ObterCargosBaseSobrepostoFuncaoAtividade(cargosFuncoesEol),
                    UsuarioTelefone = usuarioLogado.Telefone
                };
            }
            else
            {
                retorno = new DadosInscricaoDto
                {
                    UsuarioNome = usuarioLogado.Nome,
                    UsuarioCpf = usuarioLogado.Login.AplicarMascara(@"000\.000\.000\-00"),
                    UsuarioEmail = usuarioLogado.EmailEducacional,
                    UsuarioRf = usuarioLogado.Login,
                    UsuarioTelefone = usuarioLogado.Telefone
                };
            }

            return retorno;
        }

        private static List<DadosInscricaoCargoEol> ObterCargosBaseSobrepostoFuncaoAtividade(IEnumerable<CursistaCargoServicoEol> cargosFuncoesEol)
        {
            var usuarioCargos = new List<DadosInscricaoCargoEol>();
            foreach (var cargoFuncaoEol in cargosFuncoesEol)
            {
                var item = new DadosInscricaoCargoEol
                {
                    Codigo = cargoFuncaoEol.CdCargoBase.ToString(),
                    Descricao = cargoFuncaoEol.CargoBase,
                    DreCodigo = cargoFuncaoEol.CdDreCargoBase,
                    UeCodigo = cargoFuncaoEol.CdUeCargoBase,
                    TipoVinculo = cargoFuncaoEol.TipoVinculoCargoBase ?? 0,
                    DataInicio = cargoFuncaoEol.DataInicioCargoBase
                };

                if (cargoFuncaoEol.CdFuncaoAtividade.HasValue)
                {
                    item.Funcoes.Add(new DadosInscricaoCargoEol
                    {
                        Codigo = cargoFuncaoEol.CdFuncaoAtividade.ToString(),
                        Descricao = cargoFuncaoEol.FuncaoAtividade,
                        DreCodigo = cargoFuncaoEol.CdDreFuncaoAtividade,
                        UeCodigo = cargoFuncaoEol.CdUeFuncaoAtividade,
                        TipoVinculo = cargoFuncaoEol.TipoVinculoFuncaoAtividade ?? 0,
                        DataInicio = cargoFuncaoEol.DataInicioFuncaoAtividade
                    });
                }
                usuarioCargos.Add(item);

                if (cargoFuncaoEol.CdCargoSobreposto.HasValue)
                {
                    usuarioCargos.Add(new DadosInscricaoCargoEol
                    {
                        Codigo = cargoFuncaoEol.CdCargoSobreposto.ToString(),
                        Descricao = cargoFuncaoEol.CargoSobreposto,
                        DreCodigo = cargoFuncaoEol.CdDreCargoSobreposto,
                        UeCodigo = cargoFuncaoEol.CdUeCargoSobreposto,
                        TipoVinculo = cargoFuncaoEol.TipoVinculoCargoSobreposto ?? 0,
                        DataInicio = cargoFuncaoEol.DataInicioCargoSobreposto
                    });
                }
            }

            return usuarioCargos;
        }

        private async Task AtualizaCpfUsuario(Usuario usuarioLogado, IEnumerable<CursistaCargoServicoEol> cargosFuncoesEol)
        {
            var cpfEol = cargosFuncoesEol.First().Cpf;
            if (string.IsNullOrWhiteSpace(usuarioLogado.Cpf) && !string.IsNullOrWhiteSpace(cpfEol))
            {
                usuarioLogado.Cpf = cpfEol;
                await mediator.Send(new SalvarUsuarioCommand(usuarioLogado));
            }
        }
    }
}
