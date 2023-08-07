using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;



namespace dtgw
{
    public partial class Form1 : Form
    {
        //readonly-> Bir kez başlatıldıktan sonra, değeri değiştirilemez ve başka bir yerde değişkenin değeri değiştirilemez.
        private readonly int?[,] rakim = new int?[20, 20]; // rakim adında 20x20 matris oluşturduk
        public Form1()
        {
            InitializeComponent();
        }


        public void Form1_Load(object sender, EventArgs e)
        {
            Random rnd = new Random(); //Sayılara random değer verecek rnd adında parametre tanımladık.
            for (int i = 0; i < rakim.GetLength(0); i++)
            { //GetLength(0) matrisin satır sayısını, GetLength(1) ise matrisin sütun sayısını verir
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

        public class CellWithCost //Hücreyi ve o hücreye ulaşmak için ödenecek maliyeti temsil eder.
        {
            public DataGridViewCell Cell { get; set; } 
            public int Cost { get; set; }
        }

        private List<DataGridViewCell> YolBulAStar(int?[,] rakim, DataGridViewCell startCell, DataGridViewCell destinationCell, TreeView treeView)
        {
            //liste, hücrelerin maliyetleriyle birlikte sıralanmış bir sıra içinde tutulacak. 
            List<CellWithCost> sıra = new List<CellWithCost>();

            //her hücre için toplam maliyetin saklanacacağını belirttik.
            Dictionary<DataGridViewCell, int> maliyet = new Dictionary<DataGridViewCell, int>();

            //her hücre için en kısa yolda bir önceki hücrenin saklanacağını belirttik.
            Dictionary<DataGridViewCell, DataGridViewCell> rota = new Dictionary<DataGridViewCell, DataGridViewCell>();

            //ziyaret edilen hücreleri takip etmek için kullanılacak hashsetimizi tanımladık.
            HashSet<DataGridViewCell> gezilen = new HashSet<DataGridViewCell>();

            maliyet[startCell] = 0;
            sıra.Add(new CellWithCost { Cell = startCell, Cost = 0 });
            gezilen.Add(startCell);

            // Bu döngü, sıra listesindeki hücrelerin maliyetlerine göre sıralanmasını ve hedef hücreye olan tahmini maliyetlerine göre en uygun hücrenin seçilmesini sağlar.
            while (sıra.Count > 0)
            {
                sıra = sıra.OrderBy(c => c.Cost + TahminEdiciMaliyet(c.Cell, destinationCell)).ToList();
                CellWithCost current = sıra[0];
                sıra.RemoveAt(0);

                if (current.Cell == destinationCell)
                {
                    //path adında yeni bir liste oluşturduk. Bu liste, başlangıç hücresinden hedef hücreye olan yolun hücrelerini tutacak
                    List<DataGridViewCell> path = new List<DataGridViewCell>();

                    //node(düğüm) adlı geçici hücre değişkenine hedef hücreyi atar.
                    DataGridViewCell node = destinationCell;

                    //node hücresi başlangıç hücresine ulaşana kadar devam eder.
                    while (node != startCell)
                    {
                        path.Add(node); //geçici node hücresini path listesine ekler.
                        node = rota[node]; //node hücresini rotada takip edilen bir sonraki hücreye yönlendirir.
                    }

                    path.Add(startCell); //başlangıç hücresini path listesine ekler
                    //path listesindeki hücreleri tersine çevirir. Çünkü yol başlangıçtan hedefe doğru oluşturulmuştu ve sonucun başlangıçtan hedefe sıralı olması beklenir
                    path.Reverse();

                    //"Gezilen Hücreler" metni ile yeni bir TreeNode oluşturduk.
                    treeView.Nodes.Clear();

                    //Yeniden başlamak ve sadece mevcut yolu görüntülemek için temizleriz.
                    TreeNode rootNode = new TreeNode("Gezilen Hücreler");

                    //rootNode, TreeView kontrolüne eklenir. Bu, oluşturacağımız ağacın kökü olur.
                    treeView.Nodes.Add(rootNode);

                    foreach (DataGridViewCell cell in path)
                    {
                        string cellValue = cell.Value != null ? cell.Value.ToString() : "Null";
                        TreeNode cellNode = new TreeNode($"Cell: {cell.RowIndex}, {cell.ColumnIndex}, Value: {cellValue}");
                        rootNode.Nodes.Add(cellNode);
                    }

                    return path;//elde edilen yolun listesini fonksiyon çağrısının sonucu olarak döndürür.
                }

                int currentRow = current.Cell.RowIndex; //Seçili satırın seçili indexi
                int currentCol = current.Cell.ColumnIndex; //Seçili sütun seçili indexi

                int[] dr = { -1, 1, 0, 0, 1, -1, -1, 1 };  // Satır hücresinin eksenlerini belirledik.
                int[] dc = { 0, 0, -1, 1, 1, 1, -1, -1 }; // Sütun hücresinin eksenlerini belirledik.

                for (int i = 0; i < 8; i++)//hücrenin 8 komsusunu dolaşıyoruz
                {
                    int newRow = currentRow + dr[i];
                    //dr ve dc adlı diziler, mevcut hücrenin komşu hücrelere göre satır ve sütun değişimlerini belirlemek için kullanılır
                    int newCol = currentCol + dc[i];

                    //hesaplanan komşu hücrenin geçerli bir hücre olup olmadığını kontrol ediyoruz
                    if (IsValidCell(newRow, newCol) && !gezilen.Contains(dataGridView1.Rows[newRow].Cells[newCol]))
                    {
                        DataGridViewCell neighborCell = dataGridView1.Rows[newRow].Cells[newCol];//komsu hücre tanımladık

                        if (neighborCell.Value != null && int.TryParse(neighborCell.Value.ToString(), out int neighborCellValue))
                        {
                            int totalCost = current.Cost + neighborCellValue;//mevcut hücreye gelene kadar olan maliyete, komşu hücrenin maliyetini ekliyoruz.

                            /* 
                             ----Aşağıdaki İf Özeti----
                              Komşu hücre daha önce ziyaret edilmemişse veya toplam maliyet
                              maliyet sözlüğünde komşu hücre için saklanan maliyetten daha düşükse
                              bu durumda yeni bir yol bulduk demektir. Yeni maliyeti maliyet sözlüğünde güncelliyoruz,
                              komşu hücreyi sıra listesine ve gezilen kümesine ekliyoruz ve aynı zamanda rota sözlüğünde
                              komşu hücrenin önceki hücre olarak current.Cell'i (mevcut hücreyi) kaydediyoruz.
                             */
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
        private int TahminEdiciMaliyet(DataGridViewCell hedefCell, DataGridViewCell destinationCell) //Hücrenin hedef hücreye olan tahmini maliyeti hesaplar
        {
            // Hedef hücrenin satır ve sütun indekslerini alır.
            int hedefSatir = hedefCell.RowIndex;  
            int hedefSutun = hedefCell.ColumnIndex;

            // Hedef hücreye olan tahmini satır ve sütun indekslerini alır.
            int hedefSatirIndex = destinationCell.RowIndex;
            int hedefSutunIndex = destinationCell.ColumnIndex;

            //hedef hücrenin satır indeksi ile hedef hücreye olan tahmini satır indeksi arasındaki farkı ve hedef hücrenin sütun indeksi ile hedef hücreye olan tahmini sütun indeksi arasındaki farkı alırız.
            return Math.Abs(hedefSatirIndex - hedefSatir) + Math.Abs(hedefSutunIndex - hedefSutun);
            //return satirfarkı + sutunfarkı
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