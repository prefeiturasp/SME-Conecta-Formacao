﻿
using SME.ConectaFormacao.Dominio.Constantes;

namespace SME.ConectaFormacao.Dominio.Extensoes;

public static class GuidExtension
{
    public static bool EhPerfilAdminDF(this Guid guid)
    {
        return guid == Perfis.ADMIN_DF;
    }

    public static bool EhPerfilParecerista(this Guid guid)
    {
        return guid == Perfis.PARECERISTA;
    }
}