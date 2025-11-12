using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Carrito.Models;
using System.Collections.Generic;

namespace Carrito.Data
{
    public class LibreriaDBContext : DbContext
    {
        public LibreriaDBContext(DbContextOptions<LibreriaDBContext> options) 
            : base(options)
        {
        }
    }
}
