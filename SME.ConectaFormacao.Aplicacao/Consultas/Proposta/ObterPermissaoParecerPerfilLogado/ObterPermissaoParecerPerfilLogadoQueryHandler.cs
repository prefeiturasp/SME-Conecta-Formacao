using MediatR;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPermissaoParecerPerfilLogadoQueryHandler : IRequestHandler<ObterPermissaoParecerPerfilLogadoQuery, IEnumerable<PermissaoTela>>
    {
        private readonly IMediator _mediator;

        public ObterPermissaoParecerPerfilLogadoQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<IEnumerable<PermissaoTela>> Handle(ObterPermissaoParecerPerfilLogadoQuery request, CancellationToken cancellationToken)
        {
            var perfilLogado = await _mediator.Send(new ObterGrupoUsuarioLogadoQuery(), cancellationToken);
            var ehAreaPromotora = (await _mediator.Send(new ObterPerfilAreaPromotoraQuery(perfilLogado), cancellationToken)).NaoEhNulo();

            var funcoes = new List<(bool ehperfil, Func<IEnumerable<PermissaoTela>> funcaoPermissao)>()
            {
                (perfilLogado.EhParecerista(), ObterPermissaoParecerista),
                (perfilLogado.EhPerfilAdminDF(), ObterPermissaoAdminDF),
                (ehAreaPromotora, ObterPermissaoAreaPromotora)
            };

            foreach(var funcao in funcoes)
            {
                if (funcao.ehperfil)
                    return funcao.funcaoPermissao();
            }

            return Enumerable.Empty<PermissaoTela>();
        }

        private IEnumerable<PermissaoTela> ObterPermissaoParecerista()
        {
            return new List<PermissaoTela>() { PermissaoTela.CONSULTA, PermissaoTela.INCLUSAO, PermissaoTela.EXCLUSAO, PermissaoTela.ALTERACAO };
        }

        private IEnumerable<PermissaoTela> ObterPermissaoAreaPromotora()
        {
            return new List<PermissaoTela>() { PermissaoTela.CONSULTA };
        }

        private IEnumerable<PermissaoTela> ObterPermissaoAdminDF()
        {
            return new List<PermissaoTela>() { PermissaoTela.CONSULTA, PermissaoTela.EXCLUSAO, PermissaoTela.ALTERACAO };
        }
    }
}
