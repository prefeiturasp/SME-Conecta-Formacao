using AutoMapper;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using SME.ConectaFormacao.Aplicacao.Interfaces.Usuario;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Servicos.Interfaces;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Usuarios
{
    public class CasoDeUsoSalvarUsuarioAcessibilidade(
        IUsuarioAcessibilidadeService usuarioAcessibilidadeService,
        IRepositorioUsuario repositorioUsuario,
        IMapper mapper) : ICasoDeUsoSalvarUsuarioAcessibilidade
    {
        public async Task<Resultado> ExecutarAsync(string login, UsuarioAcessibilidadeDto usuarioAcessibilidadeDto)
        {
            var usuarioId = usuarioAcessibilidadeDto.UsuarioId;

            if (usuarioId is null)
            {
                var usuario = await repositorioUsuario.ObterPorLogin(login);
                if (usuario is null)
                    return Erro.Validacao("Usuário não encontrado pelo login informado.");

                usuarioId = usuario.Id;
            }

            var usuarioAcessibilidade = mapper.Map<UsuarioAcessibilidade>(usuarioAcessibilidadeDto);
            usuarioAcessibilidade.UsuarioId = usuarioId.Value;
            await usuarioAcessibilidadeService.SalvarAcessibilidadeDaInscricaoAsync(usuarioAcessibilidade);

            return Resultado.DeSucesso();
        }
    }
}