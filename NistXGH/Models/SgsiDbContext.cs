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
    public DbSet<CenarioLog> CenarioLog { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Funcoes>(entity =>
        {
            entity.ToTable("FUNCOES", "SGSI");
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

        modelBuilder.Entity<CenarioAtual>(entity =>
        {
            entity.ToTable("CENARIO", "SGSI"); // NOME CORRETO DA TABELA
            entity.HasKey(e => e.ID);

            entity.Property(e => e.ID).HasColumnName("ID");

            entity.Property(e => e.SUBCATEGORIA).HasColumnName("SUBCATEGORIA");

            entity.Property(e => e.JUSTIFICATIVA).HasColumnName("JUSTIFICATIVA");

            entity.Property(e => e.PRIOR_ATUAL).HasColumnName("PRIOR_ATUAL").HasDefaultValue(1);

            entity.Property(e => e.STATUS_ATUAL).HasColumnName("STATUS_ATUAL");

            entity.Property(e => e.POLIT_ATUAL).HasColumnName("POLIT_ATUAL").HasMaxLength(20);
            entity.Property(e => e.PRAT_ATUAL).HasColumnName("PRAT_ATUAL").HasMaxLength(200);
            entity.Property(e => e.FUNC_RESP).HasColumnName("FUNC_RESP").HasMaxLength(50);

            entity.Property(e => e.REF_INFO).HasColumnName("REF_INFO").HasMaxLength(90);

            entity.Property(e => e.EVID_ATUAL).HasColumnName("EVID_ATUAL");

            entity.Property(e => e.NOTAS).HasColumnName("NOTAS");

            entity.Property(e => e.CONSIDERACOES).HasColumnName("CONSIDERACOES").HasMaxLength(200);

            entity.Property(e => e.DATA_REGISTRO).HasColumnName("DATA_REGISTRO");

            entity
                .HasOne(e => e.SubcategoriaNav)
                .WithMany()
                .HasForeignKey(e => e.SUBCATEGORIA)
                .HasConstraintName("FK_SUBCAT_ATUAL");

            entity
                .HasOne(e => e.Prioridade)
                .WithMany()
                .HasForeignKey(e => e.PRIOR_ATUAL)
                .HasConstraintName("ATUAL_PRIORIDADE_FK");

            entity
                .HasOne(e => e.Nivel)
                .WithMany()
                .HasForeignKey(e => e.STATUS_ATUAL)
                .HasConstraintName("ATUAL_STATUS_FK");
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

            entity
                .HasOne(e => e.Prioridade)
                .WithMany()
                .HasForeignKey(e => e.PRIORIDADE_ALVO)
                .HasPrincipalKey(p => p.ID)
                .HasConstraintName("FUTURO_PRIORIDADE");

            entity
                .HasOne(e => e.Nivel)
                .WithMany()
                .HasForeignKey(e => e.NIVEL_ALVO)
                .HasPrincipalKey(s => s.ID)
                .HasConstraintName("FUTURO_STATUS");

            entity
                .HasOne(e => e.SubcategoriaNav)
                .WithMany()
                .HasForeignKey(e => e.SUBCATEGORIA)
                .HasPrincipalKey(s => s.ID)
                .HasConstraintName("FK_SUBCAT_FUTURO");
        });

        modelBuilder.Entity<CenarioLog>(entity =>
        {
            entity.ToTable("CENARIO_LOG", "SGSI");
            entity.HasKey(e => e.ID);

            entity.Property(e => e.ID).HasColumnName("ID").ValueGeneratedOnAdd(); // Para identity no Oracle

            entity.Property(e => e.CENARIO_ID).HasColumnName("CENARIO_ID").IsRequired();

            entity
                .Property(e => e.CENARIO_TIPO)
                .HasColumnName("CENARIO_TIPO")
                .HasMaxLength(10)
                .IsRequired();

            entity.Property(e => e.SUBCATEGORIA_ID).HasColumnName("SUBCATEGORIA_ID").IsRequired();

            entity
                .Property(e => e.CAMPO_ALTERADO)
                .HasColumnName("CAMPO_ALTERADO")
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.VALOR_ANTIGO).HasColumnName("VALOR_ANTIGO");

            entity.Property(e => e.VALOR_NOVO).HasColumnName("VALOR_NOVO");

            entity.Property(e => e.USUARIO).HasColumnName("USUARIO").HasMaxLength(100);

            entity
                .Property(e => e.DATA_ALTERACAO)
                .HasColumnName("DATA_ALTERACAO")
                .HasDefaultValueSql("SYSDATE");

            entity.Property(e => e.IP_MAQUINA).HasColumnName("IP_MAQUINA").HasMaxLength(45);
        });
    }
}
