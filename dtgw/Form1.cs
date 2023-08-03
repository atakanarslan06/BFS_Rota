using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using C5;


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
        private List<DataGridViewCell> YolBulBFS(int?[,] rakim, DataGridViewCell startCell, DataGridViewCell hedefCell, TreeView treeView) //YolBulBFS adında liste döndüren bir fonksiyon tanımladık.
        {
            var gDegeri = new Dictionary<DataGridViewCell, int>();
            gDegeri[startCell] = 0;
            var acikKume = new IntervalHeap<DataGridViewCell>(new AStarComparer());
            acikKume.Add(startCell);
            var geldigiYer = new Dictionary<DataGridViewCell, DataGridViewCell>();


            while (acikKume.Count > 0)
            {
                DataGridViewCell mevcutHucresi = acikKume.Dequeue();
                if (mevcutHucresi == hedefCell)
                {
                    List<DataGridViewCell> yol = new List<DataGridViewCell>();
                    DataGridViewCell dugum = hedefCell;

                    while (dugum != startCell)
                    {
                        yol.Add(dugum);
                        dugum = geldigiYer[dugum];
                    }
                    yol.Add(startCell);
                    yol.Reverse();

                    TreeNode rootNode = new TreeNode("Gezilen Hücreler");
                    treeView.Nodes.Clear();
                    treeView.Nodes.Add(rootNode);

                    foreach (DataGridViewCell cell in yol)
                    {
                        string cellValue = cell.Value != null ? cell.Value.ToString() : "Null";
                        TreeNode treeNode = new TreeNode($"Hücre: {cell.RowIndex}, {cell.ColumnIndex}, Deger: {cellValue}");
                        rootNode.Nodes.Add(treeNode);
                    }
                    return yol;
                }
                int mevcutGDegeri = gDegeri[mevcutHucresi];

                int[] dr = { -1, 1, 0, 0, 1, -1, -1, 1 };
                int[] dc = { 0, 0, -1, 1, 1, 1, -1, -1 };

                for (int i = 0; i < 8; i++)
                {
                    int yeniSatir = mevcutHucresi.RowIndex + dr[i];
                    int yeniSutun = mevcutHucresi.ColumnIndex + dc[i];

                    if (IsValidCell(yeniSatir, yeniSutun))
                    {
                        DataGridViewCell komsuHucresi = dataGridView1.Rows[yeniSatir].Cells[yeniSutun];
                        if (komsuHucresi.Value != null && int.TryParse(komsuHucresi.Value.ToString(), out int komsuHucresiDegeri))
                        {
                            int denemeGDegeri = mevcutGDegeri + komsuHucresiDegeri;

                            if (!gDegeri.TryGetValue(komsuHucresi, out int eskiGDegeri) || denemeGDegeri < eskiGDegeri)
                            {
                                gDegeri[komsuHucresi] = denemeGDegeri;
                                int fDegeri = denemeGDegeri + TahminEdiciMaliyet(komsuHucresi);
                                if (!acikKume.Contains(komsuHucresi))
                                {
                                    acikKume.Enqueue(komsuHucresi, fDegeri);
                                }
                                geldigiYer[komsuHucresi] = mevcutHucresi;
                            }
                        }
                    }
                }
           
         }

            return new List<DataGridViewCell>(); //fonksiyonun sonucunu döndürdük.
        }
        private bool IsValidCell(int row, int col) //satrr ve sütun indekslerinin dtgw kontrolünün sınırları içinde geçerli bir hücre konumu olup olmadığını kontrol eden IsValidCell fonksiyonu tanımladık.
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

                    List<DataGridViewCell> yol = YolBulBFS(rakim, baslangicNoktasiCell, varisNoktasiCell, treeView1); //BFS algoritması yol adında liste oluşturduk.

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