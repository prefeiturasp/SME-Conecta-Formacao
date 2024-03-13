using MediatR;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Inscricao.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Inscricao.ServicosFakes
{
    public class ObterNomeProfissionalPorRegistroFuncionalQueryHandlerFaker : IRequestHandler<ObterNomeProfissionalPorRegistroFuncionalQuery, string>
    {
        public Task<string> Handle(ObterNomeProfissionalPorRegistroFuncionalQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(ObterNomeCursistaInscricaoMock.Usuario?.Nome);
        }
    }
}
