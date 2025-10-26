using Microsoft.EntityFrameworkCore;
using NistXGH.Controllers;
using NistXGH.Models;

public class SgsiDbContext : DbContext
{
    public SgsiDbContext(DbContextOptions<SgsiDbContext> options)
        : base(options) { }

    public DbSet<CenarioAtual> CenariosAtual { get; set; }
    public DbSet<CenarioFuturo> CenariosFuturo { get; set; }
    public DbSet<Funcoes> Funcoes { get; set; }
    public DbSet<Categorias> Categorias { get; set; }
    public DbSet<Subcategorias> Subcategorias { get; set; }
    public DbSet<PrioridadeTb> PrioridadeTb { get; set; }
    public DbSet<StatusTb> StatusTb { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Mapear nomes de tabelas/colunas conforme seu banco (ex: SGSI.FUNCOES)
        modelBuilder.Entity<Funcoes>(entity =>
        {
            entity.ToTable("FUNCOES", "SGSI"); // ajuste se for schema diferente: "SGSI.SGSI_FUNCOES" não é necessário em ToTable
            entity.HasKey(e => e.ID);
            entity.Property(e => e.ID).HasColumnName("ID");
            entity.Property(e => e.CODIGO).HasColumnName("CODIGO");
            entity.Property(e => e.NOME).HasColumnName("NOME");
            entity.Property(e => e.DESCRICAO).HasColumnName("DESCRICAO");
        });

        modelBuilder.Entity<Categorias>(entity =>
        {
            entity.ToTable("CATEGORIAS", "SGSI");
            entity.HasKey(e => e.ID);
            entity.Property(e => e.ID).HasColumnName("ID");
            entity.Property(e => e.CODIGO).HasColumnName("CODIGO");
            entity.Property(e => e.NOME).HasColumnName("NOME");
            entity.Property(e => e.DESCRICAO).HasColumnName("DESCRICAO");
            entity.Property(e => e.FUNCAO).HasColumnName("FUNCAO"); // ajuste se necessário

            entity
                .HasOne(c => c.FuncaoNav)
                .WithMany(f => f.Categorias)
                .HasForeignKey(c => c.FUNCAO)
                .HasConstraintName("FK_CAT_FUNCAO");
        });

        modelBuilder.Entity<Subcategorias>(entity =>
        {
            entity.ToTable("SUBCATEGORIAS", "SGSI");
            entity.HasKey(e => e.ID);
            entity.Property(e => e.ID).HasColumnName("ID");
            entity.Property(e => e.SUBCATEGORIA).HasColumnName("SUBCATEGORIA");
            entity.Property(e => e.DESCRICAO).HasColumnName("DESCRICAO");
            entity.Property(e => e.CATEGORIA).HasColumnName("CATEGORIA");

            entity
                .HasOne(s => s.CategoriaNav)
                .WithMany(c => c.Subcategorias)
                .HasForeignKey(s => s.CATEGORIA)
                .HasConstraintName("FK_SUBCAT_CAT");
        });
        modelBuilder.Entity<PrioridadeTb>(entity =>
        {
            entity.ToTable("PRIORIDADE_TB", "SGSI");
            entity.HasKey(e => e.ID);
            entity.Property(e => e.ID).HasColumnName("ID");
            entity.Property(e => e.NIVEL).HasColumnName("NIVEL").HasMaxLength(5);
        });

        modelBuilder.Entity<StatusTb>(entity =>
        {
            entity.ToTable("STATUS_TB", "SGSI");
            entity.HasKey(e => e.ID);
            entity.Property(e => e.ID).HasColumnName("ID");
            entity.Property(e => e.NIVEL).HasColumnName("NIVEL").HasMaxLength(50);
            entity.Property(e => e.STATUS).HasColumnName("STATUS").HasMaxLength(50);
        });



        modelBuilder.Entity<CenarioFuturo>(entity =>
        {
            entity.ToTable("CENARIO_FUTURO", "SGSI");
            entity.HasKey(e => e.ID);
            entity.Property(e => e.ID).HasColumnName("ID");
            entity.Property(e => e.SUBCATEGORIA).HasColumnName("SUBCATEGORIA");
            entity.Property(e => e.PRIORIDADE_ALVO).HasColumnName("PRIORIDADE_ALVO");
            entity.Property(e => e.NIVEL_ALVO).HasColumnName("NIVEL_ALVO");
            entity.Property(e => e.POLIT_ALVO).HasColumnName("POLIT_ALVO");
            entity.Property(e => e.PRAT_ALVO).HasColumnName("PRAT_ALVO");
            entity.Property(e => e.FUNC_ALVO).HasColumnName("FUNC_ALVO");
            entity.Property(e => e.REF_INFO_ALVO).HasColumnName("REF_INFO_ALVO");
            entity.Property(e => e.ARTEF_ALVO).HasColumnName("ARTEF_ALVO");
            entity.Property(e => e.DATA_REGISTRO).HasColumnName("DATA_REGISTRO");
            // 🔗 Relacionamentos (essencial!)
            entity
                .HasOne(e => e.Prioridade)
                .WithMany()
                .HasForeignKey(e => e.PRIORIDADE_ALVO)
                .HasPrincipalKey(p => p.ID)
                .HasConstraintName("FK_CENARIO_PRIORIDADE");

            entity
                .HasOne(e => e.Nivel)
                .WithMany()
                .HasForeignKey(e => e.NIVEL_ALVO)
                .HasPrincipalKey(s => s.ID)
                .HasConstraintName("FK_CENARIO_STATUS");

            entity
                .HasOne(e => e.SubcategoriaNav)
                .WithMany()
                .HasForeignKey(e => e.SUBCATEGORIA)
                .HasPrincipalKey(s => s.ID)
                .HasConstraintName("FK_SUBCAT_FUTURO");
        });
    }
}
