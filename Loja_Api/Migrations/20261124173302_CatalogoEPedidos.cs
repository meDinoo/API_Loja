using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Loja_Api.Migrations
{
    /// <inheritdoc />
    public partial class CatalogoEPedidos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Pedido",
                columns: table => new
                {
                    ID_PEDIDO = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NomeContato = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmailContato = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TelefoneContato = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cep = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Endereco = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Numero = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Complemento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Bairro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cidade = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FormaPagamento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Subtotal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Frete = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pedido", x => x.ID_PEDIDO);
                });

            migrationBuilder.CreateTable(
                name: "Produto",
                columns: table => new
                {
                    ID_PRODUTO = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Artista = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Preco = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Imagem = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Categoria = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Destaque = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produto", x => x.ID_PRODUTO);
                });

            migrationBuilder.InsertData(
                table: "Produto",
                columns: new[] { "Nome", "Artista", "Descricao", "Preco", "Imagem", "Categoria", "Destaque" },
                values: new object[,]
                {
                    { "Clube da Esquina", "Milton Nascimento e Lô Borges", "Um dos discos mais importantes da música brasileira, com uma mistura marcante de MPB, rock e música mineira.", 99.90m, "https://musicofmatthew.com/wp-content/uploads/2025/10/ab67616d0000b2730f18b9691fefc38827ffbd7a.jpg", "MPB", true },
                    { "Kind of Blue", "Miles Davis", "Clássico do jazz e um dos álbuns mais influentes da história da música.", 129.90m, "https://quals.ua/image/cache/catalog/Covers/miles-davis-kind-of-blue-uhqr-45rpm-vinyl-2000x2000.jpeg", "Jazz", true },
                    { "The Dark Side of the Moon", "Pink Floyd", "Rock progressivo com uma experiência sonora marcante e atemporal.", 149.90m, "https://www.covercentury.com/covers/audio/p/Pink_Floyd_-_Dark_Side_Of_The_Moon-front.jpg", "Rock", false },
                    { "Acabou Chorare", "Novos Baianos", "Um disco brasileiro cheio de energia, mistura de samba, rock e alegria.", 109.90m, "https://static.wixstatic.com/media/a7f161_bdb61c505b934b04be71c294590eef68~mv2.jpg/v1/fit/w_500%2Ch_500%2Cq_90/file.jpg", "Brasileira", false },
                    { "A Night at the Opera", "Queen", "Rock clássico com grandes vocais e canções que marcaram gerações.", 139.90m, "https://i.scdn.co/image/ab67616d0000b2737110a2b3dc32dc1224b7670f", "Rock", false },
                    { "Getz/Gilberto", "Stan Getz e João Gilberto", "Um encontro marcante entre o jazz e a bossa nova.", 119.90m, "https://cdn.hmv.com/r/w-640/hmv/files/f2/f29366bf-88ec-4efa-b07f-0bd2d073968d.jpg", "Jazz", false },
                    { "Tropicália ou Panis et Circencis", "Vários artistas", "Álbum coletivo que representa a criatividade do movimento tropicalista.", 94.90m, "https://s2.glbimg.com/eZ7PeD0fmzx1Tlju2cP8mG6r9HA%3D/1200x/smart/filters%3Acover%28%29%3Astrip_icc%28%29/i.s3.glbimg.com/v1/AUTH_59edd422c0c84a879bd37670ae4f538a/internal_photos/bs/2018/N/5/vtbwGaT4S2GrGmRqSYEw/tropicalialp.jpg", "MPB", false },
                    { "The Rise and Fall of Ziggy Stardust", "David Bowie", "Um clássico do glam rock com uma identidade visual e sonora única.", 149.90m, "https://cdn11.bigcommerce.com/s-1xa2dhlu0a/images/stencil/500x659/products/91634/87471/LDB87376__00941.1657988485.jpg?c=1", "Rock", false },
                    { "Elis e Tom", "Elis Regina e Tom Jobim", "Grandes interpretações da música brasileira em um encontro inesquecível.", 124.90m, "https://cdn.awsli.com.br/2500x2500/769/769081/produto/99890549/21e5dc3ea5.jpg", "MPB", false },
                    { "Construção", "Chico Buarque", "Um dos discos mais importantes da carreira de Chico Buarque.", 104.90m, "https://www.jobim.org/chico/bitstream/handle/2010.2/1194/construcao.capa.jpg?sequence=2", "MPB", false },
                    { "Led Zeppelin IV", "Led Zeppelin", "Rock pesado, folk e músicas marcantes em um álbum essencial.", 144.90m, "https://media.rhino.com/sites/g/files/g2000012721/files/content/LZ%20IV%20Original%20Cover.jpg", "Rock", false },
                    { "Clube da Esquina 2", "Milton Nascimento", "Um encontro de vozes e harmonias que expandiu os caminhos da MPB.", 119.90m, "/imagens/discos/clube-da-esquina-2.svg", "MPB", true },
                    { "Transa", "Caetano Veloso", "Canções de exílio e liberdade em um clássico gravado em Londres.", 109.90m, "/imagens/discos/transa.svg", "MPB", false },
                    { "Da Lama ao Caos", "Nação Zumbi", "Maracatu e guitarras elétricas deram forma ao manguebeat.", 114.90m, "/imagens/discos/da-lama-ao-caos.svg", "Manguebeat", false },
                    { "Sobrevivendo no Inferno", "Racionais MC's", "Um marco do rap brasileiro, com relatos poderosos da vida na periferia.", 129.90m, "/imagens/discos/sobrevivendo-no-inferno.svg", "Rap", true }
                });

            migrationBuilder.CreateTable(
                name: "ItemPedido",
                columns: table => new
                {
                    ID_ITEM_PEDIDO = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PedidoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProdutoId = table.Column<int>(type: "int", nullable: false),
                    NomeProduto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PrecoUnitario = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Quantidade = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemPedido", x => x.ID_ITEM_PEDIDO);
                    table.ForeignKey(
                        name: "FK_ItemPedido_Pedido_PedidoId",
                        column: x => x.PedidoId,
                        principalTable: "Pedido",
                        principalColumn: "ID_PEDIDO",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemPedido_Produto_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produto",
                        principalColumn: "ID_PRODUTO",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItemPedido_PedidoId",
                table: "ItemPedido",
                column: "PedidoId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemPedido_ProdutoId",
                table: "ItemPedido",
                column: "ProdutoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemPedido");

            migrationBuilder.DropTable(
                name: "Pedido");

            migrationBuilder.DropTable(
                name: "Produto");
        }
    }
}
