using Microsoft.EntityFrameworkCore;
using WebApplication3.Models;

namespace WebApplication3.Data
{
    public class FileDBContext : DbContext
    {
        public FileDBContext(DbContextOptions<FileDBContext> options) 
            : base(options)
        {

        }


        public virtual DbSet<SpeechFile> SpeechFiles { get; set; }
    }
 }

