using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data.SqlClient;
using Library_REST_API;

namespace Library_REST_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        string sqlConnectionString = "Data Source=meister;Initial Catalog=Producao;" +
            "Integrated Security=True;Connect Timeout=30;Encrypt=False;" +
            "TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        // GET: api/produtos
        [HttpGet]
        public ActionResult Get()
        {
            List<object> produtos = new List<object>();
            using (SqlConnection con = new SqlConnection(sqlConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("stp_GetProdutos", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        produtos.Add(new
                        {
                            ID_Produto = reader.GetInt32(0),
                            Codigo_Peca = reader.GetString(1),
                            Data_Producao = reader.GetDateTime(2),
                            Hora_Producao = reader.GetTimeSpan(3),
                            Tempo_Producao = reader.GetInt32(4)
                        });
                    }
                    con.Close();
                }
            }
            return Ok(produtos);
        }

        
        [HttpPost]
        public async Task<IActionResult> InserirProduto([FromBody] Produto producao)
        {
            using (SqlConnection conn = new SqlConnection(sqlConnectionString))
            {
                await conn.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("EXEC InserirProduto @Codigo_Peca, @Data_Producao, @Hora_Producao, @Tempo_Producao", conn))
                {
                    cmd.Parameters.AddWithValue("@Codigo_Peca", producao.Codigo_Peca);
                    cmd.Parameters.AddWithValue("@Data_Producao", producao.Data_Producao);
                    cmd.Parameters.AddWithValue("@Hora_Producao", producao.Hora_Producao);
                    cmd.Parameters.AddWithValue("@Tempo_Producao", producao.Tempo_Producao);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            return Ok("Produção inserida com sucesso");
        }

        // PUT: api/produtos/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> AtualizarProduto(int id, [FromBody] Produto produto)
        {
            using (SqlConnection conn = new SqlConnection(sqlConnectionString))
            {
                await conn.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("stp_UpdateProduto", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ID_Produto", id);
                    cmd.Parameters.AddWithValue("@Codigo_Peca", produto.Codigo_Peca);
                    cmd.Parameters.AddWithValue("@Data_Producao", produto.Data_Producao);
                    cmd.Parameters.AddWithValue("@Hora_Producao", produto.Hora_Producao);
                    cmd.Parameters.AddWithValue("@Tempo_Producao", produto.Tempo_Producao);

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();
                    if (rowsAffected == 0)
                    {
                        return NotFound("Produto não encontrado!");
                    }
                }
            }
            return Ok("Produto atualizado com sucesso!");
        }

        // DELETE: api/produtos/{id}
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            using (SqlConnection con = new SqlConnection(sqlConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("stp_DeleteProduto", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ID_Produto", id);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
            return Ok("Produto eliminado com sucesso!");
        }
    }

    // ---------------------- TESTES CONTROLLER ----------------------

    [Route("api/[controller]")]
    [ApiController]
    public class TestesController : ControllerBase
    {
        private readonly string sqlConnectionString = "Data Source=meister;Initial Catalog=Producao;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;";

        // GET: api/testes
        [HttpGet]
        public ActionResult Get()
        {
            List<object> testes = new List<object>();
            using (SqlConnection con = new SqlConnection(sqlConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("stp_GetTestes", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        testes.Add(new
                        {
                            ID_Teste = reader.GetInt32(0),
                            ID_Produto = reader.GetInt32(1),
                            Codigo_Resultado = reader.GetInt32(2),
                            Data_Teste = reader.GetDateTime(3)
                        });
                    }
                    con.Close();
                }
            }
            return Ok(testes);
        }

        // POST: api/testes
        [HttpPost]
        public ActionResult Post([FromBody] Teste teste)
        {
            using (SqlConnection con = new SqlConnection(sqlConnectionString))
            {
                con.Open();

                // Verifica se o produto existe
                using (SqlCommand checkCmd = new SqlCommand("SELECT COUNT(1) FROM Produto WHERE ID_Produto = @ID_Produto", con))
                {
                    checkCmd.Parameters.AddWithValue("@ID_Produto", teste.ID_Produto);
                    int produtoExists = (int)checkCmd.ExecuteScalar();

                    if (produtoExists == 0)
                    {
                        return BadRequest("Produto não encontrado!");
                    }
                }

                // Insere o teste
                using (SqlCommand cmd = new SqlCommand("InserirTeste", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ID_Produto", teste.ID_Produto);
                    cmd.Parameters.AddWithValue("@Codigo_Resultado", teste.Codigo_Resultado);
                    cmd.Parameters.AddWithValue("@Data_Teste", teste.Data_Teste);

                    cmd.ExecuteNonQuery();
                }
            }
            return Ok("Teste inserido com sucesso!");
        }

        // PUT: api/testes/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> AtualizarTeste(int id, [FromBody] Teste teste)
        {
            using (SqlConnection con = new SqlConnection(sqlConnectionString))
            {
                await con.OpenAsync();

                // Verifica se o teste existe
                using (SqlCommand checkCmd = new SqlCommand("SELECT COUNT(1) FROM Teste WHERE ID_Teste = @ID_Teste", con))
                {
                    checkCmd.Parameters.AddWithValue("@ID_Teste", id);
                    int exists = (int)await checkCmd.ExecuteScalarAsync();

                    if (exists == 0)
                    {
                        return NotFound("Teste não encontrado!");
                    }
                }

                // Atualiza o teste
                using (SqlCommand cmd = new SqlCommand("stp_UpdateTeste", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ID_Teste", id);
                    cmd.Parameters.AddWithValue("@ID_Produto", teste.ID_Produto);
                    cmd.Parameters.AddWithValue("@Codigo_Resultado", teste.Codigo_Resultado);
                    cmd.Parameters.AddWithValue("@Data_Teste", teste.Data_Teste);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
            return Ok("Teste atualizado com sucesso!");
        }

        // DELETE: api/testes/{id}
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            using (SqlConnection con = new SqlConnection(sqlConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("stp_DeleteTeste", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ID_Teste", id);
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        return NotFound("Teste não encontrado!");
                    }
                }
            }
            return Ok("Teste eliminado com sucesso!");
        }
    }
}
