using BlazorShop.Models.DTOs;
using System.Net;
using System.Net.Http.Json;

namespace BlazorShop.Web.Services
{
    #nullable disable

    public class ProdutoService : IProdutoService
    {
        public HttpClient _httpClient;
        public ProdutoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<ProdutoDto>> GetItens()
        {
            try
            {
                var produtosDto = await _httpClient.GetFromJsonAsync<IEnumerable<ProdutoDto>>("api/produtos");

                return produtosDto;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao acessar produtos : api/produtos \n ERRO: {ex.InnerException.Message}");
                throw;
            }
        }

        public async Task<ProdutoDto> GetItem(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/produtos/{id}");

                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == HttpStatusCode.NoContent)
                        return default;
                    
                    return await response.Content.ReadFromJsonAsync<ProdutoDto>();
                }
                else
                {
                    var message = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Erro ao obter produto pelo id = {id} - Message: {message}");
                    throw new Exception($"Status Code: {response.StatusCode} - {message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<CategoriaDto>> GetCategorias()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/produtos/GetCategorias");
                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == HttpStatusCode.NoContent)
                        return Enumerable.Empty<CategoriaDto>();

                    return await response.Content.ReadFromJsonAsync<IEnumerable<CategoriaDto>>();
                }
                else
                {
                    var message = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Status Code: {response.StatusCode} - {message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<ProdutoDto>> GetItensPorCategoria(int categoriaId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/produtos/{categoriaId}/GetItensPorCategoria");
                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == HttpStatusCode.NoContent)
                        return Enumerable.Empty<ProdutoDto>();

                    return await response.Content.ReadFromJsonAsync<IEnumerable<ProdutoDto>>();
                }
                else
                {
                    var message = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Status Code: {response.StatusCode} - {message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }
    }
}
