using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricoes;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Dtos;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricoes
{
    public class CasoDeUsoObterDadosInscricaoParaProposta(
        IRepositorioCargoFuncaoEol repositorioCargoFuncaoEol, 
        IRepositorioProposta repositorioProposta, 
        IMediator mediator,
        IContextoAplicacao contextoAplicacao) :
        CasoDeUsoAbstrato(mediator), ICasoDeUsoObterDadosInscricaoParaProposta
    {
        public async Task<DadosInscricaoPropostaDto> ExecutarAsync(long propostaId)
        {
            var usuarioLogado = await mediator.Send(new ObterUsuarioLogadoQuery()) ?? ObterUsuarioDoContexto();

            var dadosInscricao = new DadosInscricaoPropostaDto
            {
                UsuarioNome = usuarioLogado.Nome,
                UsuarioNomeSocial = usuarioLogado.NomeSocial,
                UsuarioCpf = usuarioLogado.Cpf?.AplicarMascara(@"000\.000\.000\-00") ?? string.Empty,
                UsuarioEmail = usuarioLogado.Email,
                UsuarioRf = usuarioLogado.Login,
                UsuarioTelefone = usuarioLogado.Telefone
            };

            if (usuarioLogado is not null && usuarioLogado.Tipo != TipoUsuario.Externo)
            {
                _ = await repositorioProposta.ObterPorId(propostaId) ??
                    throw new Exception("Proposta não encontrada.");

                var cargosFuncoesEol = await repositorioCargoFuncaoEol.ObterCargosFuncoesEolDoServidorAsync(usuarioLogado.Login);
                dadosInscricao.UsuarioCargos = ObterCargosBaseSobrepostoFuncaoAtividade(cargosFuncoesEol);
                dadosInscricao.VagaRemanescente = await InscricaoParaVagaRemanescente(propostaId, cargosFuncoesEol);
            }
            return dadosInscricao;
        }

        private static List<DadosInscricaoCargoEol> ObterCargosBaseSobrepostoFuncaoAtividade(IEnumerable<CargoFuncaoEolDto> cargosFuncoesEol) =>
            [.. cargosFuncoesEol.Select(c => new DadosInscricaoCargoEol
            {
                Codigo = c.Codigo.ToString(),
                Descricao = c.Nome ?? "",
                TipoVinculo = c.TipoVinculo ?? 0,
                DataInicio = c.DataPosse,
                DreCodigo = c.CodigoDre,
                UeCodigo = c.CodigoUe,
                Funcoes = [.. c.Funcoes.Select(f => new DadosInscricaoCargoEol
                {
                    Codigo = f.CodigoFuncao.ToString(),
                    DataInicio = f.DataPosse,
                    TipoVinculo = f.TipoVinculo ?? 0,
                    Descricao = f.Nome ?? "",
                    DreCodigo = f.CodigoDre,
                    UeCodigo = f.CodigoUe
                })]
            })];

        private async Task<bool> InscricaoParaVagaRemanescente(long propostaId, IEnumerable<CargoFuncaoEolDto> cargosFuncoesEol)
        {
            var codigosCargosFuncoesEol = cargosFuncoesEol.Select(c => c.CargoFuncaoId).Distinct();
            var publicoAlvo = await repositorioProposta.ObterPublicoAlvoPorId(propostaId);
            var cargosPublicoAlvo = publicoAlvo.Select(p => p.CargoFuncaoId).Distinct();

            if (codigosCargosFuncoesEol.Any(c => cargosPublicoAlvo.Contains(c)))
                return false;

            var vagasRemanescentes = await repositorioProposta.ObterVagasRemacenentesPorId(propostaId);
            return codigosCargosFuncoesEol.Any(c => vagasRemanescentes.Any(v => v.CargoFuncaoId == c));
        }

        public Usuario ObterUsuarioDoContexto() => new()
        {
            Login = contextoAplicacao.UsuarioLogado,
            Nome = contextoAplicacao.NomeUsuario,
            Tipo = TipoUsuario.Externo
        };
    }
}