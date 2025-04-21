using Microsoft.AspNetCore.Mvc;

namespace SIG_PSPEP.Policies

{
    public static class AreaAcessoHelper
    {
        public static bool UsuarioTemAcessoArea(Controller controller, string? nomeAreaUsuario)
        {
            // Tenta obter o atributo [Area("X")] do controller
            var areaAttr = controller.GetType()
                .GetCustomAttributes(typeof(AreaAttribute), true)
                .FirstOrDefault() as AreaAttribute;

            // Extrai o nome da área do atributo e do usuário
            var areaAtual = areaAttr?.RouteValue?.ToUpperInvariant();
            var nomeUsuarioArea = nomeAreaUsuario?.ToUpperInvariant();

            // Se qualquer um for nulo ou vazio, acesso negado
            if (string.IsNullOrEmpty(areaAtual) || string.IsNullOrEmpty(nomeUsuarioArea))
                return false;

            // Permite acesso se o nome da área bater ou se for ADMIN
            return nomeUsuarioArea == areaAtual || nomeUsuarioArea == "ADMIN";
        }
    }
}
