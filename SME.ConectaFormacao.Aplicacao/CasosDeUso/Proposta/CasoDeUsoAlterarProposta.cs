using MediatR;
using SME.ConectaFormacao.Aplicacao.Comandos.Propostas.SalvarPropostaGrupoPeriodo;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoAlterarProposta(IMediator mediator, IRepositorioProposta repositorioProposta)
        : CasoDeUsoAbstrato(mediator), ICasoDeUsoAlterarProposta
    {
        public async Task<RetornoDTO> Executar(long id, PropostaDTO propostaDTO)
        {
            if (!await PodeEditar(id))
                throw new NegocioException(string.Format(MensagemNegocio.USUARIO_SEM_PERMISSAO_PARA_EDITAR_PROPOSTA, id));

            RetornoDTO retornoDto;
            if (propostaDTO.Situacao.EhParaSalvarRascunho() || propostaDTO.EhProximoPasso)
                retornoDto = await mediator.Send(new AlterarPropostaRascunhoCommand(id, propostaDTO));
            else
                retornoDto = await mediator.Send(new AlterarPropostaCommand(id, propostaDTO));

            var resultadoSalvarGrupoPeriodo = await mediator.Send(new SalvarPropostaGrupoPeriodoCommand(retornoDto.EntidadeId, propostaDTO));

            if (!resultadoSalvarGrupoPeriodo.Sucesso)
                throw new NegocioException(resultadoSalvarGrupoPeriodo.MensagensErro);
            return retornoDto;
        }

        private async Task<bool> PodeEditar(long id)
        {
            var perfilUsuarioLogado = await mediator.Send(new ObterGrupoUsuarioLogadoQuery());

            // Admin pode editar
            if (perfilUsuarioLogado.EhPerfilAdminDF())
                return true;

            var ehAreaPromotora =
                (await mediator.Send(new ObterPerfilAreaPromotoraQuery(perfilUsuarioLogado)))
                .NaoEhNulo();

            // Se não for área promotora, não pode editar
            if (!ehAreaPromotora)
                return false;

            var proposta = await repositorioProposta.ObterPorId(id)
                ?? throw new NegocioException(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA);

            var codigoCriador = proposta.CriadoLogin;

            Usuario usuarioLogado = await mediator.Send(new ObterUsuarioLogadoQuery());

            if (usuarioLogado == null)
                return true;

            var codigoUsuarioLogado = usuarioLogado.Login;


            return string.Equals(
                codigoCriador,
                codigoUsuarioLogado,
                StringComparison.OrdinalIgnoreCase);
        }
    }
}
