using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.DTOs.SubRolPermiso
{
    public class UpsertMasivoSubRolPermisoDto
    {
        public List<UpsertSubRolPermisoDto> Permisos { get; set; } = [];
    }
}
