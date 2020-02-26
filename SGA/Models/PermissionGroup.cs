﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SGA.Models
{
    public class PermissionGroup : BaseModel
    {
        [Display(Name = "Nome do Grupo")]
        [Required(ErrorMessage = "É preciso informar o nome do grupo")]
        [StringLength(100, ErrorMessage = "O nome do grupo pode ter no máximo 100 caracteres.")]
        public string Name { get; set; }

        [Display(Name = "Domínio")]
        [Required(ErrorMessage = "É preciso informar o domínio")]
        [MinLength(1, ErrorMessage = "Nome deve ter no mínimo  caracter")]
        [MaxLength(255, ErrorMessage = "Nome deve ter no máximo 255 caracteres")]
        public string Domain { get; set; }

        [Display(Name = "Tipo de acesso")]
        [Required(ErrorMessage = "É preciso informar o tipo de acesso")]
        public EnumSGA.AccessType AccessType { get; set; }

        [Display(Name = "Ativo")]
        [Required(ErrorMessage = "É preciso informar o status")]
        public EnumSGA.Status Enable { get; set; }
    }

    internal class PermissionGroupIdComparer : IEqualityComparer<PermissionGroup>
    {
        public bool Equals(PermissionGroup x, PermissionGroup y)
        {
            if (x == null || y == null) {
                return false;
            }

            if (x == y) {
                return true;
            }

            if (x.Id == y.Id)
            {
                return true;
            }

            return false;
        }

        public int GetHashCode(PermissionGroup obj)
        {
            return obj.Name.GetHashCode();
        }
    }
}
