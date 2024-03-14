using MediatR;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Infra.Servicos.Eol;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Inscricao.Mocks;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Inscricao.ServicosFakes
{
    public class ObterCargosFuncoesDresFuncionarioServicoEolQueryHandlerFaker : IRequestHandler<ObterCargosFuncoesDresFuncionarioServicoEolQuery, IEnumerable<CursistaCargoServicoEol>>
    {
        public Task<IEnumerable<CursistaCargoServicoEol>> Handle(ObterCargosFuncoesDresFuncionarioServicoEolQuery request, CancellationToken cancellationToken)
        {
            var retorno = new List<CursistaCargoServicoEol>();

            foreach (var codigoCargo in AoObterDadosUsuarioInscricaoMock.CodigoCargos)
            {
                retorno.Add(new CursistaCargoServicoEol
                {
                    RF = long.Parse(AoObterDadosUsuarioInscricaoMock.Usuario.Login),
                    Cpf = string.Empty,
                    CdCargoBase = codigoCargo,
                    CdDreCargoBase = codigoCargo.ToString(),
                    CdUeCargoBase = codigoCargo.ToString(),
                });
            }

            foreach (var codigoFuncao in AoObterDadosUsuarioInscricaoMock.CodigoFuncoes)
            {
                retorno.Add(new CursistaCargoServicoEol
                {
                    RF = long.Parse(AoObterDadosUsuarioInscricaoMock.Usuario.Login),
                    Cpf = string.Empty,
                    CdFuncaoAtividade = codigoFuncao,
                    CdDreFuncaoAtividade = codigoFuncao.ToString(),
                    CdUeFuncaoAtividade = codigoFuncao.ToString(),
                    CdCargoBase = codigoFuncao,
                    CdDreCargoBase = codigoFuncao.ToString(),
                    CdUeCargoBase = codigoFuncao.ToString(),
                });
            }

            return Task.FromResult(retorno.AsEnumerable());
        }
    }
}
