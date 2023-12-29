using MediatR;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Infra.Servicos.Eol.Dto;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Inscricao.Mocks;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Inscricao.ServicosFakes
{
    public class ObterCargosFuncoesDresFuncionarioServicoEolQueryHandlerFaker : IRequestHandler<ObterCargosFuncoesDresFuncionarioServicoEolQuery, IEnumerable<CargoFuncionarioConectaDTO>>
    {
        public Task<IEnumerable<CargoFuncionarioConectaDTO>> Handle(ObterCargosFuncoesDresFuncionarioServicoEolQuery request, CancellationToken cancellationToken)
        {
            var retorno = new List<CargoFuncionarioConectaDTO>();

            foreach (var codigoCargo in AoObterDadosUsuarioInscricaoMock.CodigoCargos)
            {
                retorno.Add(new CargoFuncionarioConectaDTO
                {
                    RF = long.Parse(AoObterDadosUsuarioInscricaoMock.Usuario.Login),
                    Cpf = string.Empty,
                    CdCargoBase = codigoCargo
                });
            }

            foreach (var codigoFuncao in AoObterDadosUsuarioInscricaoMock.CodigoFuncoes)
            {
                retorno.Add(new CargoFuncionarioConectaDTO
                {
                    RF = long.Parse(AoObterDadosUsuarioInscricaoMock.Usuario.Login),
                    Cpf = string.Empty,
                    CdFuncaoAtividade = codigoFuncao
                });
            }

            return Task.FromResult(retorno.AsEnumerable());
        }
    }
}
