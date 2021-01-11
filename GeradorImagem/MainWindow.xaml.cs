﻿using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows;

namespace GeradorImagem
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window
    {

        private String caminho = AppDomain.CurrentDomain.BaseDirectory;

        public MainWindow()
        {
            InitializeComponent();
        }



        public void Exportarimagem(string imagemBanco, string caminho, string nome, string pasta)
        {
            try
            {
                var bytes = Convert.FromBase64String(imagemBanco);
                string filePath = string.Format(@"{0}\FotosBH\{1}\{2}", caminho, pasta, nome + ".jpg");
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(filePath));
                using (var imageFile = new FileStream(filePath, FileMode.Create))
                {
                    imageFile.Write(bytes, 0, bytes.Length);
                    imageFile.Flush();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (DataSet dsResultado = new DataSet())
                {

                    dsResultado.ReadXml(caminho + @"\conexao.xml");
                    if (dsResultado.Tables.Count > 0)
                    {
                        string servidor = dsResultado.Tables["Banco"].Rows[0]["Servidor"].ToString();
                        string instancia = dsResultado.Tables["Banco"].Rows[0]["Instancia"].ToString();
                        string usuario = dsResultado.Tables["Banco"].Rows[0]["Usuario"].ToString();
                        string senha = dsResultado.Tables["Banco"].Rows[0]["Senha"].ToString();
                        string MyConString = string.Format(@"Data Source= {0} ;Initial Catalog={1};User Id={2};Password={3};", servidor, instancia, usuario, senha);
                        string caminho = dsResultado.Tables["Conexao"].Rows[0]["Caminho"].ToString();

                        SqlConnection connection = new SqlConnection(MyConString);
                        string queryString = string.Empty;
                        string pasta = string.Empty;
                        string tabela = string.Empty;
                        string codigo = string.Empty;
                        string imagem = string.Empty;

                        if (rbtEmpresa.IsChecked.Value)
                        {
                            tabela = dsResultado.Tables["Empresa"].Rows[0]["Nome"].ToString(); 
                            codigo = dsResultado.Tables["Empresa"].Rows[0]["ColunaCodigo"].ToString(); 
                            imagem = dsResultado.Tables["Empresa"].Rows[0]["ColunaImagem"].ToString();
                            queryString = string.Format("select {0}, {1} from {2}", codigo, imagem, tabela);
                            pasta = "empresa";
                        }
                        if (rbtColaborador.IsChecked.Value)
                        {
                            tabela = dsResultado.Tables["Colaborador"].Rows[0]["Nome"].ToString(); ;
                            codigo = dsResultado.Tables["Colaborador"].Rows[0]["ColunaCodigo"].ToString(); ;
                            imagem = dsResultado.Tables["Colaborador"].Rows[0]["ColunaImagem"].ToString(); ;
                            queryString = string.Format("select top(10) {0}, {1} from {2}", codigo, imagem, tabela);
                            pasta = "colaborador";
                        }
                        if (rbtVeiculo.IsChecked.Value)
                        {
                            tabela = dsResultado.Tables["Veiculo"].Rows[0]["Nome"].ToString(); ;
                            codigo = dsResultado.Tables["Veiculo"].Rows[0]["ColunaCodigo"].ToString(); ;
                            imagem = dsResultado.Tables["Veiculo"].Rows[0]["ColunaImagem"].ToString(); ;
                            queryString = string.Format("select top(10) {0}, {1} from {2}", codigo, imagem, tabela);
                            pasta = "veiculo";
                        }

                        SqlCommand cmd = new SqlCommand(queryString, connection);
                        connection.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        int totalRegistro = 0;
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                if (DBNull.Value != reader[1])
                                {
                                    Exportarimagem(reader.GetString(1), caminho, reader.GetInt32(0).ToString(), pasta);
                                    totalRegistro++;
                                }

                            }
                        }

                        MessageBox.Show("Total de Imgens geradas: " + totalRegistro, "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }

            catch (Exception ex)
            {

                MessageBox.Show("Erro ao gera imagens. Detalhe: " + ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }


        }
    }
}