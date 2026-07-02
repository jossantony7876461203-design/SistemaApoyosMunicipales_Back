using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.DTOs.RolPermiso
{
    public class UpsertPermisoRolDto
    {
        public Guid PermisoId { get; set; }
        public bool Asignado { get; set; }  
    }
}
