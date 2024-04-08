using MediatR;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Inscricao.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Inscricao.ServicosFakes
{
    public class ObterNomeProfissionalPorRegistroFuncionalQueryHandlerFaker : IRequestHandler<ObterNomeCpfProfissionalPorRegistroFuncionalQuery, RetornoUsuarioDTO>
    {
        public Task<RetornoUsuarioDTO> Handle(ObterNomeCpfProfissionalPorRegistroFuncionalQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new RetornoUsuarioDTO() { Nome = ObterNomeCursistaInscricaoMock.Usuario?.Nome, Cpf = ObterNomeCursistaInscricaoMock.Usuario?.Cpf});
        }
    }
}
