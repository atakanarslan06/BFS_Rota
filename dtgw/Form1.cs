using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
            for (int i = 0; i < rakim.GetLength(1); i++)// for döngüsü ile sütunlarına ekledik 
            {
                dataGridView1.Columns.Add($"{i}", $"{i + 1}");
            }

            for (int i = 0; i < rakim.GetLength(0); i++) // satırlarına ekledik
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
        public class CellWithCost
        {
            public DataGridViewCell Cell { get; set; } 
            public int Cost { get; set; }
        }
        private List<DataGridViewCell> YolBulAStar(int?[,] rakim, DataGridViewCell startCell, DataGridViewCell destinationCell, TreeView treeView)
        {
            List<CellWithCost> sıra = new List<CellWithCost>();
            Dictionary<DataGridViewCell, int> maliyet = new Dictionary<DataGridViewCell, int>();
            Dictionary<DataGridViewCell, DataGridViewCell> rota = new Dictionary<DataGridViewCell, DataGridViewCell>();
            HashSet<DataGridViewCell> gezilen = new HashSet<DataGridViewCell>();

            maliyet[startCell] = 0;
            sıra.Add(new CellWithCost { Cell = startCell, Cost = 0 });
            gezilen.Add(startCell);

            while (sıra.Count > 0)
            {
                sıra = sıra.OrderBy(c => c.Cost + TahminEdiciMaliyet(c.Cell, destinationCell)).ToList();
                CellWithCost current = sıra[0];
                sıra.RemoveAt(0);

                if (current.Cell == destinationCell)
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

                    treeView.Nodes.Clear();
                    TreeNode rootNode = new TreeNode("Gezilen Hücreler");
                    treeView.Nodes.Add(rootNode);

                    foreach (DataGridViewCell cell in path)
                    {
                        string cellValue = cell.Value != null ? cell.Value.ToString() : "Null";
                        TreeNode cellNode = new TreeNode($"Cell: {cell.RowIndex}, {cell.ColumnIndex}, Value: {cellValue}");
                        rootNode.Nodes.Add(cellNode);
                    }

                    return path;
                }

                int currentRow = current.Cell.RowIndex;
                int currentCol = current.Cell.ColumnIndex;

                int[] dr = { -1, 1, 0, 0, 1, -1, -1, 1 };
                int[] dc = { 0, 0, -1, 1, 1, 1, -1, -1 };

                for (int i = 0; i < 8; i++)
                {
                    int newRow = currentRow + dr[i];
                    int newCol = currentCol + dc[i];

                    if (IsValidCell(newRow, newCol) && !gezilen.Contains(dataGridView1.Rows[newRow].Cells[newCol]))
                    {
                        DataGridViewCell neighborCell = dataGridView1.Rows[newRow].Cells[newCol];
                        if (neighborCell.Value != null && int.TryParse(neighborCell.Value.ToString(), out int neighborCellValue))
                        {
                            int totalCost = current.Cost + neighborCellValue;
                            if (!maliyet.ContainsKey(neighborCell) || totalCost < maliyet[neighborCell])
                            {
                                maliyet[neighborCell] = totalCost;
                                sıra.Add(new CellWithCost { Cell = neighborCell, Cost = totalCost });
                                gezilen.Add(neighborCell);
                                rota[neighborCell] = current.Cell;
                            }
                        }
                    }
                }
            }

            return new List<DataGridViewCell>();
        }
        private int TahminEdiciMaliyet(DataGridViewCell hedefCell, DataGridViewCell destinationCell)
        {
            int hedefSatir = hedefCell.RowIndex;
            int hedefSutun = hedefCell.ColumnIndex;
            int hedefSatirIndex = destinationCell.RowIndex;
            int hedefSutunIndex = destinationCell.ColumnIndex;

            return Math.Abs(hedefSatirIndex - hedefSatir) + Math.Abs(hedefSutunIndex - hedefSutun);
        }
        public bool IsValidCell(int row, int col) //satrr ve sütun indekslerinin dtgw kontrolünün sınırları içinde geçerli bir hücre konumu olup olmadığını kontrol eden IsValidCell fonksiyonu tanımladık.
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

                    List<DataGridViewCell> yol = YolBulAStar(rakim, baslangicNoktasiCell, varisNoktasiCell, treeView1); //BFS algoritması yol adında liste oluşturduk.

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