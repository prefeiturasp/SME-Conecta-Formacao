using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoObterInformacoesCadastrante : CasoDeUsoAbstrato, ICasoDeUsoObterInformacoesCadastrante
    {
        public CasoDeUsoObterInformacoesCadastrante(IMediator mediator) : base(mediator)
        {
        }

        public async Task<PropostaInformacoesCadastranteDTO> Executar()
        {
            var (grupoUsuarioLogadoId,dresCodigoDoUsuarioLogado) = await mediator.Send(ObterGrupoUsuarioEDresUsuarioLogadoQuery.Instancia());
            var areaPromotora = await mediator.Send(new ObterAreaPromotoraPorGrupoIdQuery(grupoUsuarioLogadoId,dresCodigoDoUsuarioLogado)) ??
                throw new NegocioException(MensagemNegocio.AREA_PROMOTORA_NAO_ENCONTRADA_GRUPO_USUARIO, System.Net.HttpStatusCode.NotFound);

            var informacoesCadastrante = new PropostaInformacoesCadastranteDTO
            {
                AreaPromotora = areaPromotora.Nome,
                AreaPromotoraEmails = areaPromotora.Email.Replace(";", ", "),
                AreaPromotoraTelefones = string.Join(", ", areaPromotora.Telefones.Select(t => t.Telefone.Length > 10 ? t.Telefone.AplicarMascara(@"\(00\) 00000\-0000") : t.Telefone.AplicarMascara(@"\(00\) 0000\-0000"))),
                AreaPromotoraTipo = areaPromotora.Tipo.Nome(),
            };

            informacoesCadastrante.UsuarioLogadoNome = await mediator.Send(ObterNomeUsuarioLogadoQuery.Instancia());
            informacoesCadastrante.UsuarioLogadoEmail = await mediator.Send(ObterEmailUsuarioLogadoQuery.Instancia());

            return informacoesCadastrante;
        }
    }
}
