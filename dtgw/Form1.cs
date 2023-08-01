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
        private List<DataGridViewCell> YolBulBFS(int?[,] rakim, DataGridViewCell startCell, DataGridViewCell destinationCell) //YolBulBFS adında liste döndüren bir fonksiyon tanımladık.
        {
            Queue<DataGridViewCell> sıra = new Queue<DataGridViewCell>(); //Hücreleri içeren kuyruk oluşturduk
            Dictionary<DataGridViewCell, DataGridViewCell> rota = new Dictionary<DataGridViewCell, DataGridViewCell>(); //Hücreler arası ilişkiyi tuttuk. Anahtar, bir hücreyi temsil ederken, değer, o hücreye ulaşmak için takip edilecek diğer hücreyi temsil eder.
            HashSet<DataGridViewCell> gezilen = new HashSet<DataGridViewCell>(); //Daha önce ziyaret edilen hücreleri takip etmek için kullanılacak.

            sıra.Enqueue(startCell); //Başlangıç hücresini sıraya ekler
            gezilen.Add(startCell); //Gezilen kümesinde olmayan yeni hücreleri ziyaret eder.

            while (sıra.Count > 0) //Sırada gezilecek hücreler olduğu sürece devam eder.
            {
                DataGridViewCell currentCell = sıra.Dequeue(); //Kuyruğun önünden bir hücre alınır ve currentCell değişkene atanır.
                if (currentCell == destinationCell) //currentCell adlı hücrenin, hedef hücre (destinationCell) olduğunu kontrol eder.
                { //Yol Bulunduğunda
                    List<DataGridViewCell> path = new List<DataGridViewCell>(); //path adında yeni bir liste oluşturduk. Bu liste, başlangıç hücresinden hedef hücreye olan yolun hücrelerini tutacak
                    DataGridViewCell node = destinationCell; //node(düğüm) adlı geçici hücre değişkenine hedef hücreyi atar.

                    while (node != startCell) //node hücresi başlangıç hücresine ulaşana kadar devam eder.
                    {
                        path.Add(node); //geçici node hücresini path listesine ekler.
                        node = rota[node]; //node hücresini rotada takip edilen bir sonraki hücreye yönlendirir.
                    }

                    path.Add(startCell); //başlangıç hücresini path listesine ekler
                    path.Reverse(); //path listesindeki hücreleri tersine çevirir. Çünkü yol başlangıçtan hedefe doğru oluşturulmuştu ve sonucun başlangıçtan hedefe sıralı olması beklenir

                    return path; //elde edilen yolun listesini fonksiyon çağrısının sonucu olarak döndürür.
                }

                int currentRow = currentCell.RowIndex; //Seçili satırın seçili indexi
                int currentCol = currentCell.ColumnIndex; //Seçili sütun seçili indexi

                int[] dr = { -1, 1, 0, 0, 1, -1, -1, 1 }; // Satır hücresinin eksenlerini belirledik.
                int[] dc = { 0, 0, -1, 1, 1, 1, -1, -1 }; // Sütun hücresinin eksenlerini belirledik.

                DataGridViewCell nextCell = null; //nextCell adında varsayılan değeri null olan bir değişken tanımladık. Bu değişken en uygun komşu hücrenin referansını tutacak.
                int minCellValue = int.MaxValue; //Başlangıçta en büyük int değeri olan MaxValue değeri ile atanan minCellValue adında değişken tanımladık. 

                for (int i = 0; i < 8; i++) //For döngüsü ile bir hücrenin 8 yöndeki komşularını gezinmeyi sağlar.
                {
                    int newRow = currentRow + dr[i]; //newRow şu anki gezilen hücrenin satır indeksine dr[i] değerini ekleyerek yeni bir satır indeksi oluşturur.
                    int newCol = currentCol + dc[i]; //newCol şu anki gezilen hücrenin sütun indeksine dc[i] değerini ekleyerek yeni bir sütun indeksi oluşturur.

                    if (IsValidCell(newRow, newCol) && !gezilen.Contains(dataGridView1.Rows[newRow].Cells[newCol])) //Bu if koşulu, yeni oluşturulan satır ve sütun indekslerinin geçerli bir hücre konumunu temsil ettiğini ve daha önce ziyaret edilmemiş (gezilen kümesinde olmadığını) kontrol eder.
                    {
                        DataGridViewCell neighborCell = dataGridView1.Rows[newRow].Cells[newCol]; //Yeni konumdaki komşu hücrenin referansını alır.
                        if (neighborCell.Value != null && int.TryParse(neighborCell.Value.ToString(), out int neighborCellValue)) // Bu if koşulu, komşu hücrenin değerinin null olmadığını ve int türüne dönüştürülebileceğini kontrol eder.
                        {
                            if (neighborCellValue < minCellValue) //komşu hücrenin değeri daha önce bulunan en küçük değerden daha küçükse devreye girer
                            {
                                minCellValue = neighborCellValue; //Eğer komşu hücrenin değeri, daha önce bulunan en küçük değerden daha küçükse, bu değeri minCellValue olarak günceller.
                                nextCell = neighborCell; //En küçük değeri taşıyan komşu hücreyi nextCell değişkenine atar. Böylece nextCell, şu anki gezilen hücreye en küçük değeri olan komşu hücrenin referansını tutar.
                            }
                        }
                    }
                }

                if (nextCell != null) //nextCell değişkeninin null olup olmadığını kontrol eder
                {
                    sıra.Enqueue(nextCell); //nextCell adlı en uygun komşu hücreyi sıra adlı kuyruğa ekler
                    gezilen.Add(nextCell); //gezilen adlı küme içine nextCell hücresini ekler.
                    rota[nextCell] = currentCell; //rota adlı sözlük içine bir çift (key-value) ekler. nextCell, anahtar olarak kullanılırken, currentCell ise değer olarak kullanılır. Bu işlem, nextCell hücresine ulaşmak için currentCell hücresinin kullanılacağını belirtir. Yani, nextCell hücresine nasıl ulaşıldığı bu sözlük üzerinde tutulur.
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