using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;


namespace dtgw
{
    public partial class Form1 : Form
    {
        private readonly int?[,] rakim = new int?[20, 20]; // rakim adında 20x20 matris oluşturduk
        public Form1()
        {
            InitializeComponent();
        }


        public void Form1_Load(object sender, EventArgs e)
        {
            Random rnd = new Random(); //Matris içine random sayı atadık
            for (int i = 0; i < rakim.GetLength(0); i++)
            {
                for (int j = 0; j < rakim.GetLength(1); j++)
                {
                    rakim[i, j] = null;
                }
            }
            for (int i = 0; i < rakim.GetLength(1); i++)// for döngüsü ile columnlarına ekledik 
            {
                dataGridView1.Columns.Add($"{i}", $"{i + 1}");
            }

            for (int i = 0; i < rakim.GetLength(0); i++) // rowlarına ekledik
            {
                DataGridViewRow row = new DataGridViewRow();

                for (int j = 0; j < rakim.GetLength(1); j++)
                {
                    rakim[i, j] = rnd.Next(0, 1900); // rastgele atanan sayılar 0 ile 1900 arasında 

                    if (rakim[i, j] >= 0 && rakim[i, j] < 200) // Hücre değeri 2 ile 150 arasında ise arka plan rengi mavi 
                    {
                        row.Cells.Add(new DataGridViewTextBoxCell
                        {
                            Value = rakim[i, j],
                            Style = { BackColor = Color.LightBlue }
                        }); ; ;
                    }
                    else if (rakim[i, j] >= 200 && rakim[i, j] < 500) //Hücre değeri 200 ile 500 arasında ise arka plan rengi açık yeşil 
                    {
                        row.Cells.Add(new DataGridViewTextBoxCell
                        {
                            Value = rakim[i, j],
                            Style = { BackColor = Color.LightGreen }
                        });
                    }
                    else if (rakim[i, j] >= 500 && rakim[i, j] < 1000) //Hücre değeri 500 ile 100 arasında ise arka plan rengi sarı
                    {
                        row.Cells.Add(new DataGridViewTextBoxCell
                        {
                            Value = rakim[i, j],
                            Style = { BackColor = Color.Yellow }
                        });
                    }
                    else if (rakim[i, j] >= 1000 && rakim[i, j] < 1500) //Hücre değeri 1000 ile 1500 arasında ise arka plan rengi turuncu
                    {
                        row.Cells.Add(new DataGridViewTextBoxCell
                        {
                            Value = rakim[i, j],
                            Style = { BackColor = Color.Orange }
                        });

                    }
                    else if (rakim[i, j] >= 1500 && rakim[i, j] < 2000) //Hücre değeri 1500 ile 2000 arasında ise arka plan rengi kahverengi
                    {
                        row.Cells.Add(new DataGridViewTextBoxCell
                        {
                            Value = rakim[i, j],
                            Style = { BackColor = Color.Brown }
                        });
                    }
                    else
                    {
                        row.Cells.Add(new DataGridViewTextBoxCell
                        {
                            Value = rakim[i, j]
                        });
                    }
                }
                dataGridView1.Rows.Add(row);

            }
            DataGridViewCellStyle cellStyle = new DataGridViewCellStyle(); //Hücre stili için cellStyle adında değişken tanımladık

            dataGridView1.DefaultCellStyle.BackColor = Color.Cyan;  //Hücrenin varsayılan arka plan rengi
            dataGridView1.DefaultCellStyle.ForeColor = Color.Black; //Hücre ön renk
            dataGridView1.CellClick += DataGridView1_CellClick; //Hücrelere tıklamak için tanımlandı

        }
        private List<DataGridViewCell> YolBulBFS(int?[,] rakim, DataGridViewCell startCell, DataGridViewCell destinationCell) //YolBulBFS adında liste döndüren bir fonksiyon tanımladık.
        {
            Queue<DataGridViewCell> sıra = new Queue<DataGridViewCell>(); //
            Dictionary<DataGridViewCell, DataGridViewCell> rota = new Dictionary<DataGridViewCell, DataGridViewCell>(); //
            HashSet<DataGridViewCell> gezilen = new HashSet<DataGridViewCell>(); //

            sıra.Enqueue(startCell);
            gezilen.Add(startCell);

            while (sıra.Count > 0)
            {
                DataGridViewCell currentCell = sıra.Dequeue();
                if (currentCell == destinationCell)
                {
                    List<DataGridViewCell> path = new List<DataGridViewCell>();
                    DataGridViewCell node = destinationCell;

                    while (node != startCell)
                    {
                        path.Add(node);
                        node = rota[node];
                    }

                    path.Add(startCell);
                    path.Reverse();

                    return path;
                }

                int currentRow = currentCell.RowIndex;
                int currentCol = currentCell.ColumnIndex;

                int[] dr = { -1, 1, 0, 0, 1, -1, -1, 1 };
                int[] dc = { 0, 0, -1, 1, 1, 1, -1, -1 };

                DataGridViewCell nextCell = null;
                int minCellValue = int.MaxValue; // Başlangıçta büyük bir değer veriyoruz

                for (int i = 0; i < 8; i++)
                {
                    int newRow = currentRow + dr[i];
                    int newCol = currentCol + dc[i];

                    if (IsValidCell(newRow, newCol) && !gezilen.Contains(dataGridView1.Rows[newRow].Cells[newCol]))
                    {
                        DataGridViewCell neighborCell = dataGridView1.Rows[newRow].Cells[newCol];
                        if (neighborCell.Value != null && int.TryParse(neighborCell.Value.ToString(), out int neighborCellValue)) // Hücreler int değer aldığında
                        {
                            if (neighborCellValue < minCellValue)
                            {
                                minCellValue = neighborCellValue;
                                nextCell = neighborCell;
                            }
                        }
                    }
                }

                if (nextCell != null)
                {
                    sıra.Enqueue(nextCell);
                    gezilen.Add(nextCell);
                    rota[nextCell] = currentCell;
                }
            }

            return new List<DataGridViewCell>();
        }
        private bool IsValidCell(int row, int col)
        {
            return row >= 0 && row < dataGridView1.Rows.Count && col >= 0 && col < dataGridView1.Columns.Count;
        }

        private DataGridViewCell baslangicNoktasiCell = null; //Baslangıc hücresi varsayılan olarak null
        private DataGridViewCell varisNoktasiCell = null; //Varis hücresi varsayılan olarak null

        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)//Hücrelere tıklandığında dönecek fonksiyon
        {
            // Seçilen hücrenin değerini alın
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0) // Geçerli bir hücreye tıklanırsa
            {
                DataGridViewCell currentCell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
                currentCell.Style.BackColor = Color.Black;
                currentCell.Style.ForeColor = Color.White;//seçili hücre tanımlaması

                if (baslangicNoktasiCell == null) // İlk hücre seçildiğinde mesaj olarak başlangıç noktanız mesajı döndürülecek
                {
                    
                    baslangicNoktasiCell = currentCell;
                    string message = "Başlangıç noktanız: " + baslangicNoktasiCell.Value.ToString();
                    MessageBox.Show(message, "Başlangıç Noktası", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (varisNoktasiCell == null && currentCell != baslangicNoktasiCell) //ikinci hücre seçildiğinde mesaj olarak varıs noktanız mesajı döndürülecek 
                {
                    varisNoktasiCell = currentCell;
                    string message = "Varış noktanız: " + varisNoktasiCell.Value.ToString();
                    MessageBox.Show(message, "Varış Noktası", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    List<DataGridViewCell> yol = YolBulBFS(rakim, baslangicNoktasiCell, varisNoktasiCell); //BFS algoritması yol adında liste oluşturduk.

                    if (yol.Count > 0) //Yol uzunluğu 0 dan büyük olduğu durumda foreach ile hücreleri gezip gezilen hücreleri gri ile boyadık
                    {
                        MessageBox.Show("En kısa yol bulundu!"); //En kısa yol bulundu mesajı döndürdük
                        foreach (DataGridViewCell cell in yol)
                        {
                            cell.Style.BackColor = Color.Gray; //Gezilen hücrelerin gri ile boyanması
                        }
                    }
                    else
                    {
                        MessageBox.Show("Yol Bulunamadı!"); //Yol bulunamadı mesajı döndürdük
                    }
                    dataGridView1.Refresh();

                    // İşlem tamamlandıktan sonra seçili hücreleri sıfırla
                    baslangicNoktasiCell = null;
                    varisNoktasiCell = null;
                }
                
            }
            
        }
    }
}