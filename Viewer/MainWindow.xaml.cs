using ProjectEuler.Solution;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ProjectEuler.Viewer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private BackgroundWorker worker;
        private List<Problem> problems;

        public MainWindow()
        {
            InitializeComponent();
            worker = new BackgroundWorker();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            textAnswer.IsReadOnly = true;
            textTimeCost.IsReadOnly = true;
            buttonCalc.IsEnabled = false;

            problems = new List<Problem>();
            for (int i = 0; ProblemSet.Get(i) != null; i++)
                problems.Add(ProblemSet.Get(i));
            problems.Reverse();
            viewProblem.ItemsSource = problems;

            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Problem p = e.Argument as Problem;

            p.Solve();
            e.Result = p;
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Problem p = e.Result as Problem;

            buttonCalc.IsEnabled = true;
            viewProblem.IsEnabled = true;
            SetContent(p);
        }

        private void SetContent(Problem p)
        {
            textQuestion.Navigate(p.QuestionUrl);
            if (p.Answer != null)
            {
                labelCorrect.Content = p.IsCorrect ? "Correct" : "Error";
                labelCorrect.Foreground = p.IsCorrect ? Brushes.Blue : Brushes.Red;
                textAnswer.Text = p.Answer;
                textTimeCost.Text = string.Format("{0}.{1:0000000}", p.Ticks / 10000000, p.Ticks % 10000000);
            }
            else
            {
                labelCorrect.Content = "Unknown";
                labelCorrect.Foreground = Brushes.Black;
                textAnswer.Clear();
                textTimeCost.Clear();
            }
        }

        private void viewProblem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Problem p = (Problem)viewProblem.SelectedItem;

            buttonCalc.IsEnabled = true;
            SetContent(p);
        }

        private void buttonCalc_Click(object sender, RoutedEventArgs e)
        {
            Problem p = (Problem)viewProblem.SelectedItem;

            buttonCalc.IsEnabled = false;
            viewProblem.IsEnabled = false;
            worker.RunWorkerAsync(p);
        }

        private void buttonExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}