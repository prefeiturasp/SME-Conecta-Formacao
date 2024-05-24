using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Notificacao;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterNotificacaoPaginadaQuery : IRequest<PaginacaoResultadoDTO<NotificacaoPaginadoDTO>>
    {
        public ObterNotificacaoPaginadaQuery(string login, NotificacaoFiltroDTO filtro, int numeroPagina, int numeroRegistros, int quantidadeRegistrosIgnorados)
        {
            Login = login;
            Filtro = filtro;
            NumeroPagina = numeroPagina;
            NumeroRegistros = numeroRegistros;
            QuantidadeRegistrosIgnorados = quantidadeRegistrosIgnorados;
        }

        public string Login { get;  }
        public NotificacaoFiltroDTO Filtro { get;  }
        public int NumeroPagina { get;  }
        public int NumeroRegistros { get;  }
        public int QuantidadeRegistrosIgnorados { get;  }
    }

    public class ObterNotificacaoPaginadaQueryValidator : AbstractValidator<ObterNotificacaoPaginadaQuery>
    {
        public ObterNotificacaoPaginadaQueryValidator()
        {
            RuleFor(f => f.Login)
                .NotEmpty()
                .WithMessage("Informe o login para obter as notificações paginadas");
        }
    }
}
