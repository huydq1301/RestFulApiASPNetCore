using RestApi.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RestApi.Models;

public partial class RoleModel
{
	[Key]
	public int RoleId { get; set; }

    public string? Name { get; set; }

}
