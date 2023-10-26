using MediatR;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.ServicosFakes
{
    public class ObterParametroSistemaPorTipoEAnoQueryFaker : IRequestHandler<ObterParametroSistemaPorTipoEAnoQuery, ParametroSistema>
    {
        public async Task<ParametroSistema> Handle(ObterParametroSistemaPorTipoEAnoQuery request, CancellationToken cancellationToken)
        {
            return new ParametroSistema
            {
                Ano = DateTime.Now.Year,
                Ativo = true,
                Descricao = "Descricao do Parametro",
                Nome = "Nome do Parametro",
                Tipo = TipoParametroSistema.ComunicadoAcaoFormativaDescricao,
                Valor = "Valor do Parametro"
            };
        }
    }
}