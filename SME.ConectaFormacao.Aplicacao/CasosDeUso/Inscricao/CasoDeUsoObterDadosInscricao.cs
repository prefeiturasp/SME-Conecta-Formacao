using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Servicos.Eol.Dto;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricao
{
    public class CasoDeUsoObterDadosInscricao : CasoDeUsoAbstrato, ICasoDeUsoObterDadosInscricao
    {
        public CasoDeUsoObterDadosInscricao(IMediator mediator) : base(mediator)
        {
        }

        public async Task<DadosInscricaoDTO> Executar()
        {
            var usuarioLogado = await mediator.Send(ObterUsuarioLogadoQuery.Instancia);

            // TODO: Buscar cargos EOL somente para usuários rede parceira
            var cargosFuncoesEol = await mediator.Send(new ObterCargosFuncoesDresFuncionarioServicoEolQuery(usuarioLogado.Login));

            var codigosCargosEol = new List<long>();
            codigosCargosEol.AddRange(cargosFuncoesEol.Where(w => w.CdCargoBase.HasValue).Select(t => t.CdCargoBase.GetValueOrDefault()));
            codigosCargosEol.AddRange(cargosFuncoesEol.Where(w => w.CdCargoSobreposto.HasValue).Select(t => t.CdCargoSobreposto.GetValueOrDefault()));

            var codigosFuncoesEol = cargosFuncoesEol.Where(w => w.CdFuncaoAtividade.HasValue).Select(t => t.CdFuncaoAtividade.GetValueOrDefault());

            var cargosFuncoes = await mediator.Send(new ObterCargoFuncaoPorCodigoEolQuery(codigosCargosEol, codigosFuncoesEol));

            await AtualizaCpfUsuario(usuarioLogado, cargosFuncoesEol);

            var retorno = new DadosInscricaoDTO()
            {
                UsuarioNome = usuarioLogado.Nome,
                UsuarioCpf = cargosFuncoesEol.FirstOrDefault().Cpf.AplicarMascara(@"\(00\) 00000\-0000"),
                UsuarioEmail = usuarioLogado.Email,
                UsuarioRf = usuarioLogado.Login,
                UsuarioCargos = cargosFuncoes.Where(w => w.Tipo == Dominio.Enumerados.CargoFuncaoTipo.Cargo).Select(t => new Dtos.RetornoListagemDTO { Id = t.Id, Descricao = t.Nome }),
                UsuarioFuncoes = cargosFuncoes.Where(w => w.Tipo == Dominio.Enumerados.CargoFuncaoTipo.Funcao).Select(t => new Dtos.RetornoListagemDTO { Id = t.Id, Descricao = t.Nome }),
            };

            return retorno;
        }

        private async Task AtualizaCpfUsuario(Dominio.Entidades.Usuario usuarioLogado, IEnumerable<CargoFuncionarioConectaDTO> cargosFuncoesEol)
        {
            var cpfEol = cargosFuncoesEol.FirstOrDefault().Cpf;
            if (usuarioLogado.Cpf.NaoEstaPreenchido() && cpfEol.EstaPreenchido())
            {
                usuarioLogado.Cpf = cpfEol;
                await mediator.Send(new SalvarUsuarioCommand(usuarioLogado));
            }
        }
    }
}
