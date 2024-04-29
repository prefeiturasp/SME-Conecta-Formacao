using MediatR;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using System.Net;
using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.ServicosFakes
{
    public class ObterNomeProfissionalPorRegistroFuncionalQueryHandlerFake : IRequestHandler<ObterNomeCpfProfissionalPorRegistroFuncionalQuery, RetornoUsuarioCpfNomeDTO>
    {
        public async Task<RetornoUsuarioCpfNomeDTO> Handle(ObterNomeCpfProfissionalPorRegistroFuncionalQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.RegistroFuncional))
                throw new NegocioException(mensagem: MensagemNegocio.PROFISSIONAL_NAO_LOCALIZADO, statusCode: HttpStatusCode.NoContent);
            return new RetornoUsuarioCpfNomeDTO() { Nome = "Nome do Profissional", Cpf = "99999999999" };
        }
    }
}