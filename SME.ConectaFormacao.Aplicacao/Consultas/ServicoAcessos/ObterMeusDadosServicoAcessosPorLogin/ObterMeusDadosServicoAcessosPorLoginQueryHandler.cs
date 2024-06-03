﻿using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Acessos.Interfaces;
using System.Text.RegularExpressions;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterMeusDadosServicoAcessosPorLoginQueryHandler : IRequestHandler<ObterMeusDadosServicoAcessosPorLoginQuery, DadosUsuarioDTO>
    {
        private readonly IMapper _mapper;
        private readonly IServicoAcessos _servicoAcessos;
        private readonly IRepositorioUsuario _repositorioUsuario;
        private readonly IMediator _mediator;

        public ObterMeusDadosServicoAcessosPorLoginQueryHandler(IMapper mapper, IServicoAcessos servicoAcessos, IRepositorioUsuario repositorioUsuario,
        IMediator mediator)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _servicoAcessos = servicoAcessos ?? throw new ArgumentNullException(nameof(servicoAcessos));
            _repositorioUsuario = repositorioUsuario ?? throw new ArgumentNullException(nameof(repositorioUsuario));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<DadosUsuarioDTO> Handle(ObterMeusDadosServicoAcessosPorLoginQuery request, CancellationToken cancellationToken)
        {
            var acessoDadosUsuario = await _servicoAcessos.ObterMeusDados(request.Login);
            var usuario = await _repositorioUsuario.ObterPorLogin(request.Login);
            if (usuario.Tipo == TipoUsuario.Externo)
            {
                var unidade = !string.IsNullOrEmpty(usuario.CodigoEolUnidade) ? await _mediator.Send(new ObterUnidadePorCodigoEOLQuery(usuario.CodigoEolUnidade), cancellationToken) : null;
                acessoDadosUsuario.Tipo = (int)TipoUsuario.Externo;
                acessoDadosUsuario.NomeUnidade = unidade?.NomeUnidade!;
            }
            acessoDadosUsuario.EmailEducacional = await _repositorioUsuario.ObterEmailEducacionalPorLogin(request.Login);

            var pattern = @"@edu\.sme\.prefeitura\.sp\.gov\.br$";
            if (Regex.IsMatch(acessoDadosUsuario.Email, pattern, RegexOptions.IgnoreCase) && acessoDadosUsuario.EmailEducacional.NaoEstaPreenchido())
                acessoDadosUsuario.EmailEducacional = acessoDadosUsuario.Email;

            if (acessoDadosUsuario.EmailEducacional.NaoEstaPreenchido())
                acessoDadosUsuario.EmailEducacional = await _mediator.Send(new GerarEmailEducacionalCommand(usuario), cancellationToken);

            return _mapper.Map<DadosUsuarioDTO>(acessoDadosUsuario);
        }
    }
}
