namespace Library_REST_API
{
    public class Produto
    {
        public int ID_Produto { get; set; }
        public string Codigo_Peca { get; set; }
        public DateTime Data_Producao { get; set; }
        public TimeSpan Hora_Producao { get; set; }
        public int Tempo_Producao { get; set; }
    }


}
