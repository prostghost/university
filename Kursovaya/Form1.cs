using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OxyPlot;
using OxyPlot.WindowsForms;
using OxyPlot.Series;
using OxyPlot.Axes;

namespace Kursovaya
{
    public partial class Form1 : Form
    {
        string path = "output.txt"; // файл с записью исходных данных и результатов расчётов.
        string path2 = "values.txt"; //Файл для вывода значений
        string textOutput = "Значение функций:\n\n"; //Переменная для вывода всех значений функций за цикл работы программы
        public static int n, count, num_func; //кол-во узлов, и перменные для свитча метода и функции 
        public static double a, b, step, eps; //переменные с формы, задаваемые пользователем

        List<double> ChebyshevFunction = new List<double>(); //Лист для записи всех значений функций Чебышева за работу программы
        List<double> GaussFunction = new List<double>(); //Лист для записи всех значений функций Гаусса за работу программы

        List<int> lines_thickness = new List<int>() {0, 1, 2, 3} ; //Лист для заполнения комбобокса данными, отвечающие за толщину линий
        Dictionary<string, OxyColor> lines_color = new Dictionary<string, OxyColor>(); //Словарь для заполнения комбобокса данными, отвечающие за цвет линий
        Dictionary<string, LineStyle> lines_type = new Dictionary<string, LineStyle>(); //Словарь для заполнения комбобокса данными, отвечающие за тип линий

        Dictionary<RadioButton, int> check_RButton = new Dictionary<RadioButton, int>();

        public static int line_thickness = 1;                       //
        public static OxyColor line_color = OxyColors.Green;        //// Глобальные переменные, которые будут передавать значения из комбобокса графику
        public static LineStyle line_type = LineStyle.Solid;        //

        public Form1()
        {
            InitializeComponent();

            lines_type.Add("Нет линии", LineStyle.None); //Добавляем значения в словарь
            lines_type.Add("Сплошная", LineStyle.Solid);
            lines_type.Add("Пунктирная", LineStyle.Dash);
            lines_type.Add("Точечная", LineStyle.Dot);

            lines_color.Add("Черный", OxyColors.Black); //Добавляем значения в словарь
            lines_color.Add("Красный", OxyColors.Red);
            lines_color.Add("Синий", OxyColors.Blue);
            lines_color.Add("Зеленый", OxyColors.Green);
            lines_color.Add("Желтый", OxyColors.Yellow);

            check_RButton.Add(radioButton1, 0);
            check_RButton.Add(radioButton2, 1);
            check_RButton.Add(radioButton3, 2);

            foreach (int key in lines_thickness)
            {
                comboBox2.Items.Add(key); //Добавляем ключи из словаря в комбобокс
            }
            comboBox2.SelectedItem = lines_thickness[1];

            foreach (var key in lines_type.Keys)
            {
                comboBox3.Items.Add(key); //Добавляем ключи из словаря в комбобокс
            }
            comboBox3.SelectedItem = "Сплошная";

            foreach (var key in lines_color.Keys)
            {
                comboBox1.Items.Add(key);
            }
            comboBox1.SelectedItem = "Зеленый";

            Plotter pt = new Plotter(); //Класс с графиком
            plot1.Model = pt.PrintGraph(); //построение модели графика


        }

        public bool Input() //Ввод переменных пользователем
        {
            try //Проверка на разного рода ошибок через try catch
            {
                a = Convert.ToDouble(textBox1.Text); //Нижний предел
                b = Convert.ToDouble(textBox2.Text); //Верхний предел
                if (a >= b) { throw new Exception(); } //Предел a  не должен быть больше предела b, иначе -> блок catch
                n = Convert.ToInt32(numericUpDown1.Text); //Кол-во узлов 
                if (n < 2 || n > 5) { throw new Exception(); } //Программа работает исключительно с кол-вом узлов от 2 до 5, иначе -> блок catch
                eps = Convert.ToDouble(textBox3.Text); //Точность 
                if (eps > 0.1) { throw new Exception(); } //Проверка на адекватную точность 
                step = Convert.ToDouble(textBox4.Text); //Шаг для построения графика
                if (step > 1) { throw new Exception(); } //Проверка на шаг графика
                
                foreach (RadioButton btn in check_RButton.Keys)
                {
                    if (btn.Checked)
                    {
                        num_func = check_RButton[btn];
                    }
                }
            }
            catch
            {
                MessageBox.Show("Введены некорректные данные");
                return false; //Возвращает значение false и при нажатии кнопки программа дальше не работает 
            }
            return true; //Возвращает значение true, чтобы программа могла работать при нажатии кнопки
        }

        public double ChebyshevAndGauss() //Метод Чебышева и Гаусса
        {
            double step = Convert.ToDouble(b - a) / 2; //Шаг на интервале
            double function = 0; //Функция
            int k = n - 2; //Переменная для библиотеки 
            double difference; //Переменная для сравнения с точностью eps
            double previous_func = Math.Exp(-324); //Переменная для переприсваивания 
            double kk = Convert.ToDouble(b - a) / step; //Кол-во шагов в цикле
            double final = 0; //Конечная функция
            double middle = 0; //Промежуточная переменная
            Library lib = new Library(); //Класс с библиотекой значений
            do //Цикл для достижения наивысшей точности
            {
                List<double> array = new List<double>();
                kk *= 2; //Разбиение шага на 2 с целью повысить точность решения

                double step2 = Convert.ToDouble(b - a) / kk;
                for (int i = 0; i < kk; i++)
                {
                    array.Add(a + (step2 * i));
                }
                
                for (int j = 0; j < kk; j++) //Цикл для разбиения площади на множество частей, чтобы можно было высчитать каждую часть максимально точно
                {
                    for (int i = 0; i < n; i++) //Сумма функций по формуле
                    {
                        switch (count)
                        {
                            case 0:
                                middle += 2 / Convert.ToDouble(n) * Function.func((array[j] + (array[j] + step2)) / 2 + (((array[j] + step2) - array[j]) / 2 * lib.LibChebyshev[k][i])); //Метод Чебышева
                                break;
                            case 1:
                                middle += lib.coef_Gauss[k][i] * Function.func((array[j] + (array[j] + step2)) / 2 + (((array[j] + step2) - array[j]) / 2 * lib.LibGauss[k][i])); //Метод Гаусса
                                break;
                        }
                        
                    }
                    function += ((array[j] + step2) - array[j]) / 2 * middle;
                    
                    if (double.IsNaN(function)) { return Double.NaN; } //Если функция принимает "не число", то возвращает "не число"
                    middle = 0;
                }
                difference = Math.Abs(function - previous_func); //Разница функций
                previous_func = function;
                final = function; //Суммирование промежуточных значений функций для получения конечного значения функции
                function = 0;
            }
            while (difference > eps); //Сравнение полученной точности с требуемой
            return final; //Возвращает значение функции
        }

        public void GetAllValues() //Метод для записи всех полученных значений функций и последующей 
        {
            textOutput += $"Методом Чебышева: {ChebyshevFunction.Last()}\n" +
                $"Методом Гаусса: {GaussFunction.Last()}\n" +
                "------------------------\n";
            using (FileStream filestream = new FileStream(path2, FileMode.OpenOrCreate)) //Открытие или создание файла, если того не существует
            {
                byte[] array = System.Text.Encoding.Default.GetBytes(textOutput); //Заполнение массива байтов текстом
                filestream.Write(array, 0, array.Length); //Заполнение файла массивом байтов
            }
        } 

        private void btnSolve_Click(object sender, EventArgs e) 
        {
            if (!Input()) { return; } //Если метод Input() возвращает false, то программа не продолжает работу

            count = 0;
            ChebyshevFunction.Add(ChebyshevAndGauss()); //Запись значений функции Чебышева в Лист
            count = 1;
            GaussFunction.Add(ChebyshevAndGauss()); //Запись значений функции Гаусса в Лист

            if ((double.IsNaN(ChebyshevFunction.Last())) || (double.IsNaN(GaussFunction.Last()))) { MessageBox.Show("Введена некорректная функция для данного кол-ва узлов"); return; } //Проверка на "не число"

            string precision = $"{Convert.ToString(Math.Round(Math.Abs(ChebyshevFunction.Last() - GaussFunction.Last()) / (Math.Abs(ChebyshevFunction.Last() + GaussFunction.Last()) / 2) * 100, 6))}%";

            richTextBox1.AppendText(Convert.ToString($"{ChebyshevFunction.Last()}\n"));
            richTextBox2.AppendText(Convert.ToString($"{GaussFunction.Last()}\n")); //Вывод полученной функции на форму
            if ((ChebyshevFunction.Last() == 0) && (GaussFunction.Last() == 0))
            { precision = "Точность не может быть подсчитана из-за нулевых значений функций"; richTextBox3.AppendText("0\n"); }
            else { richTextBox3.AppendText(Convert.ToString($"{precision}\n")); }

            GetAllValues(); //Вызов метода

            string text = ""; //Локальная переменная для записи текста 
            File.WriteAllText(path, String.Empty); //Очистка файла перед записью 
            text += $"Нижний предел интегралла (a): {a}\n" +
                $"Верхний предел интегралла (b): {b}\n" +
                $"Количество узлов n: {n}\n" +
                $"Точность eps: {eps}\n" +
                $"Вычисление интегралла методом Чебышева: {ChebyshevFunction.Last()}\n" +
                $"Вычисление интегралла методом Гаусса: {GaussFunction.Last()}\n" +
                $"Сравнение точности расчёта по методам: {precision} ";
            using (FileStream filestream = new FileStream(path, FileMode.OpenOrCreate)) //Открытие или создание файла, если того не существует
            {
                byte[] array = System.Text.Encoding.Default.GetBytes(text);
                filestream.Write(array, 0, array.Length);
            }
           
        }

        private void cmbbxLineThickness_SelectedIndexChanged(object sender, EventArgs e) //Толщина линий
        {
            Plotter pt = new Plotter(); //Класс с графиком
            line_thickness = lines_thickness[Convert.ToInt32(comboBox2.Text)]; //Присваивание переменной значение толщины линии из комбобокса
            plot1.Model = pt.PrintGraph(); //построение модели графика
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmbbxLineColor_SelectedIndexChanged(object sender, EventArgs e) //Цвет линий
        {
            Plotter pt = new Plotter(); //Класс с графиком
            line_color = lines_color[comboBox1.Text]; //Присваивание переменной значение цвета линии из словаря
            plot1.Model = pt.PrintGraph(); //построение модели графика
        }

        private void cmbbxLineType_SelectedIndexChanged(object sender, EventArgs e) //Тип линий
        {
            Plotter pt = new Plotter(); //Класс с графиком
            line_type = lines_type[comboBox3.Text]; //Присваивание переменной значение типа линии из словаря
            plot1.Model = pt.PrintGraph(); //построение модели графика
        }

        private void btnDrawGraphic_Click(object sender, EventArgs e) //Постройка графика
        {
            if (!Input()) { return; } //Если метод Input() возвращает false, то программа не продолжает работу
            Plotter pt = new Plotter(); //Класс с графиком
            plot1.Model = pt.PrintGraph(); //построение модели графика
        }

    }

    public partial class Library //Класс с библиотекой
    {
        public double[][] LibChebyshev =  //Библиотека с полиномами Чебышева
        {
            new double[] {-0.577350, 0.577350},
            new double[] {-0.707107, 0, 0.707107},
            new double[] {-0.794654, -0.187592, 0.187592, 0.794654},
            new double[] {-0.832497, -0.374541, 0, 0.374541, 0.832497}
        };
        
        public double[][] LibGauss = //Библиотека с полиномами Гаусса
        {
            new double[] {-0.577350, 0.577350},
            new double[] {-0.774597, 0, 0.774597},
            new double[] {-0.861136, -0.339981, 0.339981, 0.861136},
            new double[] {-0.906180, -0.538469, 0, 0.538469, 0.906180}
        };

        public double[][] coef_Gauss = //Библиотека с коэффициентами Гаусса
        {
            new double[] {1, 1},
            new double[] {0.555555, 0.888888, 0.555555},
            new double[] {0.347855, 0.652145, 0.652145, 0.347855},
            new double[] {0.236927, 0.478629, 0.568889, 0.478629, 0.236927}
        };
    }

    public partial class Function //Класс с функцией
    {
        public static double func(double x)
        {
            try
            {
                switch (Form1.num_func)
                {
                    case 0:
                        return Math.Cos(x); //Возвращение значения функции
                        
                    case 1:
                        return Math.Sin(x); //Возвращение значения функции

                    case 2:
                        return Math.Exp(x); //Возвращение значения функции   
                }
            }
            catch
            {
                return double.NaN;
            }
            return double.NaN;
        }
    }

    public partial class Plotter //Класс с графиком
    {
        private PlotModel model; //переменная графика

        public Plotter()
        {
            model = new PlotModel { Title = "График функции" }; //легенда
        }

        public PlotModel PrintGraph() //Метод с постройкой графикой
        {
            double DownLimit = Form1.a; //Нижний предел
            double UpLimit = Form1.b; //Верхний предел
            double StepX = Form1.step; //Шаг построения
            model.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Minimum = DownLimit, Maximum = UpLimit, MajorGridlineStyle = LineStyle.Automatic, MinorGridlineStyle = LineStyle.Automatic }); //Создании оси Х
            var leftAxisY = new LinearAxis { Position = AxisPosition.Left, /*MajorGridlineColor = OxyColors.LightGray,*/ MajorGridlineStyle = LineStyle.Automatic, MinorGridlineStyle = LineStyle.Automatic };
            model.Axes.Add(leftAxisY); //Создание оси Y

            var lineSeries = new LineSeries //Параметры графика
            {
                Title = "Точечная диаграмма", //Заголовок графика
                StrokeThickness = Form1.line_thickness, //Толщина линии
                LineStyle = Form1.line_type, //Тип линии
                Color = Form1.line_color, //Цвет линии
                MarkerType = MarkerType.Circle, //Форма вершин
                MarkerSize = 2, //Размер вершин
                MarkerFill = OxyColors.BlueViolet, //Цвет вершин
                MarkerStrokeThickness = 1, //Толщина вершин
            };

            for (double i = DownLimit; i < UpLimit; i+= StepX) //Цикл построения графика по точкам
            {
                lineSeries.Points.Add(new DataPoint(i, Function.func(i)));
            }

            model.Series.Add(lineSeries); //Применение параметров графика
            return model;  //Возвращение модели
        }  
    }
}
