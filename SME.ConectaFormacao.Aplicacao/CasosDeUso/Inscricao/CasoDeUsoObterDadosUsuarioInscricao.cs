using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricao
{
    public class CasoDeUsoObterDadosUsuarioInscricao : CasoDeUsoAbstrato, ICasoDeUsoObterDadosUsuarioInscricao
    {
        public CasoDeUsoObterDadosUsuarioInscricao(IMediator mediator) : base(mediator)
        {
        }

        public async Task<DadosUsuarioInscricaoDTO> Executar()
        {
            var usuarioLogado = await mediator.Send(ObterUsuarioLogadoQuery.Instancia);

            // TODO: Buscar cargos EOL somente para usuários internos
            var cargosFuncoesEol = await mediator.Send(new ObterCargosFuncoesDresFuncionarioServicoEolQuery(usuarioLogado.Login));

            var codigosCargosEol = new List<long>();
            codigosCargosEol.AddRange(cargosFuncoesEol.Where(w => w.CdCargoBase.HasValue).Select(t => t.CdCargoBase.GetValueOrDefault()));
            codigosCargosEol.AddRange(cargosFuncoesEol.Where(w => w.CdCargoSobreposto.HasValue).Select(t => t.CdCargoSobreposto.GetValueOrDefault()));

            var codigosFuncoesEol = cargosFuncoesEol.Where(w => w.CdFuncaoAtividade.HasValue).Select(t => t.CdFuncaoAtividade.GetValueOrDefault());

            var cargosFuncoes = await mediator.Send(new ObterCargoFuncaoPorCodigoEolQuery(codigosCargosEol, codigosFuncoesEol));

            var retorno = new DadosUsuarioInscricaoDTO()
            {
                Nome = usuarioLogado.Nome,
                Cpf = cargosFuncoesEol.FirstOrDefault().Cpf.AplicarMascara(@"\(00\) 00000\-0000"),
                Email = usuarioLogado.Email,
                Rf = usuarioLogado.Login,
                Cargos = cargosFuncoes.Where(w => w.Tipo == Dominio.Enumerados.CargoFuncaoTipo.Cargo).Select(t => new Dtos.RetornoListagemDTO { Id = t.Id, Descricao = t.Nome }),
                Funcoes = cargosFuncoes.Where(w => w.Tipo == Dominio.Enumerados.CargoFuncaoTipo.Funcao).Select(t => new Dtos.RetornoListagemDTO { Id = t.Id, Descricao = t.Nome }),
            };

            return retorno;
        }
    }
}
