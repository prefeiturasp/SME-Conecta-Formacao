using MediatR;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Inscricoes.Mocks;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Inscricoes.ServicosFakes
{
    public class ObterNomeProfissionalPorRegistroFuncionalQueryHandlerFaker : IRequestHandler<ObterNomeCpfProfissionalPorRegistroFuncionalQuery, RetornoUsuarioCpfNomeDTO>
    {
        public Task<RetornoUsuarioCpfNomeDTO> Handle(ObterNomeCpfProfissionalPorRegistroFuncionalQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new RetornoUsuarioCpfNomeDTO() { Nome = ObterNomeCursistaInscricaoMock.Usuario?.Nome, Cpf = ObterNomeCursistaInscricaoMock.Usuario?.Cpf });
        }
    }
}
