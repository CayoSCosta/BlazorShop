using BlazorShop.Api.Mappings;
using BlazorShop.Api.Repositories;
using BlazorShop.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace BlazorShop.Api.Controllers
{
    public class CarrinhoCompraController : Controller
    {
        private readonly ICarrinhoCompraRepository carrinhoCompraRepo;
        private readonly IProdutoRepository produtoRepo;

        private ILogger<CarrinhoCompraController> logger;

        public CarrinhoCompraController(ICarrinhoCompraRepository carrinhoCompraRepo, IProdutoRepository produtoRepo, ILogger<CarrinhoCompraController> logger)
        {
            this.carrinhoCompraRepo = carrinhoCompraRepo;
            this.produtoRepo = produtoRepo;
            this.logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<IEnumerable<CarrinhoItemDto>>> GetItens(string usuarioId)
        {
            try
            {
                var carrinhoItens = await carrinhoCompraRepo.GetItens(usuarioId);
                if (carrinhoItens == null)
                    return NoContent();

                var produtos = await this.produtoRepo.GetItens();
                if (produtos == null)
                    throw new Exception("Não existe produtos...");

                var carrinhoItensDto = carrinhoItens.ConverterCarrinhoItensParaDto(produtos);
                return Ok(carrinhoItensDto);
            }
            catch (Exception ex)
            {
                logger.LogError("## Erro ao obter itens do carrinho");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
