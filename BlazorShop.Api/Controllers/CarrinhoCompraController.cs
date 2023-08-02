using BlazorShop.Api.Mappings;
using BlazorShop.Api.Repositories;
using BlazorShop.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace BlazorShop.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CarrinhoItemDto>> GetItem(int id)
        {
            try
            {
                var carrinhoItem = await carrinhoCompraRepo.GetItem(id);
                if (carrinhoItem == null)
                    return NotFound("Item não encontrado");

                var produto = await this.produtoRepo.GetItem(carrinhoItem.ProdutoId);
                if (produto == null)
                    return NotFound("Item não existe na fonte de dados");

                var carrinhoItemDto = carrinhoItem.ConverterCarrinhoItemParaDto(produto);
                return Ok(carrinhoItemDto);
            }
            catch (Exception ex)
            {
                logger.LogError($"## Erro ao obter item = {id} do carrinho");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Route("{usuarioId}/GetItens")]
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

        [HttpPost]
        public async Task<ActionResult<CarrinhoItemDto>> PostItem([FromBody] CarrinhoItemAdicionaDto carrinhoItemAdcionaDto)
        {
            try
            {
                var novoCarrinhoItem = await carrinhoCompraRepo.AdicionaItem(carrinhoItemAdcionaDto);
                if (novoCarrinhoItem == null)
                    return NoContent();

                var produto = await produtoRepo.GetItem(novoCarrinhoItem.ProdutoId);
                if (produto == null)
                    throw new Exception($"Produto não localizado (Id:({carrinhoItemAdcionaDto.ProdutoId})");

                var novoCarrinhoItemDto = novoCarrinhoItem.ConverterCarrinhoItemParaDto(produto);
                return CreatedAtAction(nameof(GetItem), new { id = novoCarrinhoItemDto.Id }, novoCarrinhoItemDto);
            }
            catch(Exception ex)
            {
                logger.LogError("## Erro criar um novo item no carrinho");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);

            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<CarrinhoItemDto>> DeleteItem(int id)
        {
            try
            {
                var carrinhoItem = await carrinhoCompraRepo.DeletaItem(id);
                if(carrinhoItem == null) 
                    return NoContent();

                var produto = await produtoRepo.GetItem(carrinhoItem.ProdutoId);
                if (produto is null)
                    return NotFound();

                var carrinhoItemDto = carrinhoItem.ConverterCarrinhoItemParaDto(produto);
                return Ok(carrinhoItemDto);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
