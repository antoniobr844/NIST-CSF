// Utils/SubcategoriaFormatter.cs
using NistXGH.Models;

namespace NistXGH.Utils
{
    public static class SubcategoriaFormatter
    {
        public static string FormatarSubcategoria(Subcategorias subcategoria)
        {
            if (subcategoria?.FuncaoNav == null)
                return $"ID: {subcategoria?.ID}";

            return $"{subcategoria.FuncaoNav.CODIGO}.{subcategoria.CATEGORIA}-{subcategoria.SUBCATEGORIA}";
        }
    }
}